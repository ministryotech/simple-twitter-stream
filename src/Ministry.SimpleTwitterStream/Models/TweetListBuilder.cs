// Copyright (c) 2016 Minotech Ltd.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files
// (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ministry.SimpleTwitterStream.Models
{
    #region | Interface |

    /// <summary>
    /// A factory for a Twitter stream
    /// </summary>
    public interface ITweetListBuilder
    {
        /// <summary>
        /// Gets or sets the tweet count.
        /// </summary>
        /// <value>
        /// The tweet count.
        /// </value>
        int TweetCount { get; set; }

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
        /// <param name="handle">The handle.</param>
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

    #endregion

    /// <summary>
    /// A factory for a Twitter stream
    /// </summary>
    public class TweetListBuilder : ITweetListBuilder
    {
        private readonly ITwitterLocalCacheGateway localCacheGateway;
        private readonly ITwitterConfig twitterConfig;
        private readonly ITweetBuilder tweetBuilder;
        private readonly ITwitterApiGateway twitterApiGateway;

        #region | Construction |

        /// <summary>
        /// Initializes a new instance of the <see cref="TweetListBuilder" /> class.
        /// </summary>
        /// <param name="twitterConfig">The configuration reader.</param>
        /// <param name="localCacheGateway">State of the application.</param>
        /// <param name="twitterApiGateway">The twitter API gateway.</param>
        /// <param name="tweetBuilder">The tweet builder.</param>
        public TweetListBuilder(ITwitterConfig twitterConfig, ITwitterLocalCacheGateway localCacheGateway, ITwitterApiGateway twitterApiGateway, ITweetBuilder tweetBuilder)
        {
            this.localCacheGateway = localCacheGateway;
            this.tweetBuilder = tweetBuilder;
            this.twitterApiGateway = twitterApiGateway;
            this.twitterConfig = twitterConfig;

            TweetCount = twitterConfig.TweetCount > 0 ? twitterConfig.TweetCount : 5;
            this.twitterApiGateway.TwitterRateLimitHit = this.localCacheGateway.TwitterRateLimitHit;
            this.twitterApiGateway.TwitterRateLimitResetsOn = this.localCacheGateway.TwitterRateLimitResetsOn;
        }

        #endregion

        /// <summary>
        /// Gets or sets the tweet count.
        /// </summary>
        /// <value>
        /// The tweet count.
        /// </value>
        public int TweetCount { get; set; }

        /// <summary>
        /// Builds the twitter stream using twitter config data.
        /// </summary>
        /// <returns>
        /// The tweets.
        /// </returns>
        public ITweetList Build() 
            => twitterConfig.SecondaryHandles.Any() 
                ? BuildForHandles(twitterConfig.MasterHandle, twitterConfig.SecondaryHandles)
                : BuildForHandle(twitterConfig.MasterHandle);

        /// <summary>
        /// Builds the twitter stream for a handle.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <returns>
        /// The tweets.
        /// </returns>
        public ITweetList BuildForHandle(string handle)
        {
            var result = new TweetList(handle);
            var tweets = new List<Status>();

            LoadTweetsForHandle(tweets, handle);

            result.AddRange(tweets.OrderByDescending(tweet => tweet.CreatedAt).Take(TweetCount).Select(tweet => tweetBuilder.Build(tweet)));
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

            result.AddRange(tweets.OrderByDescending(tweet => tweet.CreatedAt).Take(TweetCount).Select(tweet => tweetBuilder.Build(tweet)));
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
                if (twitterApiGateway.TwitterRateLimitResetsOn < DateTime.Now) twitterApiGateway.TwitterRateLimitHit = false;

                if (!twitterApiGateway.TwitterRateLimitHit)
                {
                    var newTweets = GetTweetsForHandleFromGateway(twitterApiGateway, handle, includeRetweets);

                    localCacheGateway.TwitterRateLimitHit = twitterApiGateway.TwitterRateLimitHit;
                    localCacheGateway.TwitterRateLimitResetsOn = twitterApiGateway.TwitterRateLimitResetsOn;
                    localCacheGateway.SaveTweetsForHandle(handle, newTweets);

                    tweets.AddRange(newTweets);
                }
                else
                {
                    var newTweets = GetTweetsForHandleFromGateway(localCacheGateway, handle, includeRetweets);
                    if (newTweets != null) tweets.AddRange(newTweets);
                }
            }
            catch (Exception ex)
            {
                // Try loading from local storage
                try
                {
                    var newTweets = GetTweetsForHandleFromGateway(localCacheGateway, handle, includeRetweets);
                    if (newTweets != null) tweets.AddRange(newTweets);
                }
                catch
                {
                    throw ex;
                }
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
            => includeRetweets 
                ? gateway.GetTweetsForHandle(handle, TweetCount) 
                : gateway.GetTweetsForHandle(handle, TweetCount).Where(tweet => !tweet.Text.StartsWith("RT")).ToList();

        #endregion
    }
}
