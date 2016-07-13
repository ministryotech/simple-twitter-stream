using LinqToTwitter;
using Ministry.SimpleTwitterStream;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ministry.SimpleTwitterStream.Models
{
    /// <summary>
    /// A factory for a Twitter stream
    /// </summary>
    public interface ITweetListBuilder
    {
        /// <summary>
        /// Builds the twitter stream using twitter config data.
        /// </summary>
        /// <returns>
        /// The tweets.
        /// </returns>
        ITweetList Build();

        /// <summary>
        /// Builds the twitter stream for a handle.
        /// </summary>
        /// <param name="masterHandle">The master handle.</param>
        /// <param name="otherHandles">The other handles.</param>
        /// <returns>
        /// The tweets.
        /// </returns>
        ITweetList BuildForHandle(string handle);

        /// <summary>
        /// Builds the twitter stream for a specific group of handles.
        /// </summary>
        /// <param name="masterHandle">The master handle.</param>
        /// <param name="otherHandles">The other handles.</param>
        /// <returns>
        /// The tweets.
        /// </returns>
        ITweetList BuildForHandles(string masterHandle, string[] otherHandles);
    }

    /// <summary>
    /// A factory for a Twitter stream
    /// </summary>
    public class TweetListBuilder : ITweetListBuilder
    {
        private readonly ITwitterLocalCacheGateway _localCacheGateway;
        private readonly ITwitterConfig _twitterConfig;
        private readonly ITweetBuilder _tweetBuilder;
        private readonly ITwitterApiGateway _twitterApiGateway;

        #region | Construction |

        /// <summary>
        /// Initializes a new instance of the <see cref="TweetListBuilder" /> class.
        /// </summary>
        /// <param name="twitterConfig">The configuration reader.</param>
        /// <param name="localCacheGateway">State of the application.</param>
        /// <param name="timeProvider">The time provider.</param>
        /// <param name="tweetBuilder">The tweet builder.</param>
        public TweetListBuilder(ITwitterConfig twitterConfig, ITwitterLocalCacheGateway localCacheGateway, ITwitterApiGateway twitterApiGateway, ITweetBuilder tweetBuilder)
        {
            _localCacheGateway = localCacheGateway;
            _tweetBuilder = tweetBuilder;
            _twitterApiGateway = twitterApiGateway;
            _twitterConfig = twitterConfig;

            TweetCount = twitterConfig.TweetCount > 0 ? twitterConfig.TweetCount : 5;
            _twitterApiGateway.TwitterRateLimitHit = _localCacheGateway.TwitterRateLimitHit;
            _twitterApiGateway.TwitterRateLimitResetsOn = _localCacheGateway.TwitterRateLimitResetsOn;
        }

        #endregion

        #region | Properties |

        /// <summary>
        /// Gets or sets the tweet count.
        /// </summary>
        /// <value>
        /// The tweet count.
        /// </value>
        public int TweetCount { get; set; }

        #endregion

        /// <summary>
        /// Builds the twitter stream using twitter config data.
        /// </summary>
        /// <returns>
        /// The tweets.
        /// </returns>
        public ITweetList Build()
        {
            return _twitterConfig.SecondaryHandles.Any() ? BuildForHandles(_twitterConfig.MasterHandle, _twitterConfig.SecondaryHandles)
                                                        : BuildForHandle(_twitterConfig.MasterHandle);
        }

        /// <summary>
        /// Builds the twitter stream for a handle.
        /// </summary>
        /// <param name="masterHandle">The master handle.</param>
        /// <param name="otherHandles">The other handles.</param>
        /// <returns>
        /// The tweets.
        /// </returns>
        public ITweetList BuildForHandle(string handle)
        {
            var result = new TweetList(handle);
            var tweets = new List<Status>();

            LoadTweetsForHandle(tweets, handle);

            foreach (var tweet in tweets.OrderByDescending(tweet => tweet.CreatedAt).Take(TweetCount))
            {
                result.Add(_tweetBuilder.Build(tweet));
            }

            return result;
        }

        /// <summary>
        /// Builds the twitter stream for a group of handles.
        /// </summary>
        /// <param name="masterHandle">The master handle.</param>
        /// <param name="otherHandles">The other handles.</param>
        /// <returns>
        /// The tweets.
        /// </returns>
        public ITweetList BuildForHandles(string masterHandle, string[] otherHandles)
        {
            var result = new TweetList(masterHandle, otherHandles);
            var tweets = new List<Status>();

            LoadTweetsForHandle(tweets, masterHandle);

            foreach (var handle in otherHandles)
            {
                LoadTweetsForHandle(tweets, handle, false);
            }

            foreach (var tweet in tweets.OrderByDescending(tweet => tweet.CreatedAt).Take(TweetCount))
            {
                result.Add(_tweetBuilder.Build(tweet));
            }

            return result;
        }

        #region | Private Methods |

        /// <summary>
        /// Loads the tweets for handle.
        /// </summary>
        /// <param name="tweets">The tweets.</param>
        /// <param name="handle">The handle.</param>
        /// <param name="includeRetweets">if set to <c>false</c> excludes retweets.</param>
        private void LoadTweetsForHandle(List<Status> tweets, string handle, bool includeRetweets = true)
        {
            try
            {
                if (_twitterApiGateway.TwitterRateLimitResetsOn < DateTime.Now) _twitterApiGateway.TwitterRateLimitHit = false;

                if (!_twitterApiGateway.TwitterRateLimitHit)
                {
                    var newTweets = GetTweetsForHandleFromGateway(_twitterApiGateway, handle, includeRetweets);

                    _localCacheGateway.TwitterRateLimitHit = _twitterApiGateway.TwitterRateLimitHit;
                    _localCacheGateway.TwitterRateLimitResetsOn = _twitterApiGateway.TwitterRateLimitResetsOn;
                    _localCacheGateway.SaveTweetsForHandle(handle, newTweets);

                    tweets.AddRange(newTweets);
                }
                else
                {
                    var newTweets = GetTweetsForHandleFromGateway(_localCacheGateway, handle, includeRetweets);
                    if (newTweets != null) tweets.AddRange(newTweets);
                }
            }
            catch
            {
                // Try loading from local storage
                try
                {
                    var newTweets = GetTweetsForHandleFromGateway(_localCacheGateway, handle, includeRetweets);
                    if (newTweets != null) tweets.AddRange(newTweets);
                }
                catch { }
            }
        }

        /// <summary>
        /// Loads the tweets for handle from a specified gateway.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        /// <param name="handle">The handle.</param>
        /// <param name="includeRetweets">if set to <c>true</c> [include retweets].</param>
        /// <returns>The tweet statuses</returns>
        private IList<Status> GetTweetsForHandleFromGateway(ITwitterGateway gateway, string handle, bool includeRetweets)
        {
            if (includeRetweets)
            {
                return gateway.GetTweetsForHandle(handle, TweetCount);
            }
            else
            {
                return gateway.GetTweetsForHandle(handle, TweetCount).Where(tweet => !tweet.Text.StartsWith("RT")).ToList();
            }
        }

        #endregion
    }
}
