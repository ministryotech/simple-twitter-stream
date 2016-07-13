using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ministry.SimpleTwitterStream
{
    /// <summary>
    /// A factory for a Twitter stream
    /// </summary>
    public interface ITwitterApiGateway : ITwitterGateway
    { }

    /// <summary>
    /// A factory for a Twitter stream
    /// </summary>
    public class TwitterApiGateway : ITwitterApiGateway
    {
        private readonly ITwitterConfig _twitterConfig;
        private readonly ITimeProvider _timeProvider;
        private readonly SingleUserAuthorizer _auth;
        private TwitterContext _context;

        #region | Construction |

        /// <summary>
        /// Initializes a new instance of the <see cref="TweetListBuilder" /> class.
        /// </summary>
        /// <param name="twitterConfig">The configuration reader.</param>
        public TwitterApiGateway(ITwitterConfig twitterConfig, ITimeProvider timeProvider)
        {
            _twitterConfig = twitterConfig;
            _timeProvider = timeProvider;
            _auth = GetAuthorizer();
            _context = new TwitterContext(_auth);
        }

        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether the Twitter rate limit has been hit.
        /// </summary>
        /// <value>
        /// <c>true</c> if the Twitter rate limit has been hit; otherwise, <c>false</c>.
        /// </value>
        public bool TwitterRateLimitHit { get; set; }

        /// <summary>
        /// Gets or sets the twitter rate limit resets on.
        /// </summary>
        /// <value>
        /// The twitter rate limit resets on.
        /// </value>
        public DateTime TwitterRateLimitResetsOn { get; set; }

        /// <summary>
        /// Gets the tweets for a handle.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="tweetCount">The tweet count.</param>
        /// <returns></returns>
        public IList<Status> GetTweetsForHandle(string handle, int tweetCount = 20)
        {
            var tweetsTask = (from tweet in _context.Status
                                where tweet.Type == StatusType.User &&
                                tweet.ScreenName == handle
                                select tweet).Take(tweetCount).ToListAsync();
            tweetsTask.Wait(_twitterConfig.TwitterTimeout);

            var result =  tweetsTask.Result;

            TwitterRateLimitHit = _context.RateLimitRemaining < 2;

            if (TwitterRateLimitResetsOn <= _timeProvider.Now)
            {
                TwitterRateLimitResetsOn = _timeProvider.Now.AddMinutes(15);
            }

            return result;
        }

        #region | Private Methods |

        /// <summary>
        /// Gets the authorizer.
        /// </summary>
        /// <returns>A Twitter authorizer</returns>
        private SingleUserAuthorizer GetAuthorizer()
        {
            return new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = _twitterConfig.ConsumerKey,
                    ConsumerSecret = _twitterConfig.ConsumerSecret,
                    AccessToken = _twitterConfig.AccessToken,
                    AccessTokenSecret = _twitterConfig.AccessTokenSecret
                }
            };
        }

        #endregion
    }
}
