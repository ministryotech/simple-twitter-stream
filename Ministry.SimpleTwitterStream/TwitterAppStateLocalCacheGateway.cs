using LinqToTwitter;
using Ministry.StrongTyped;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ministry.SimpleTwitterStream
{
    /// <summary>
    /// Wrapper for storing local Twitter info in Application State
    /// </summary>
    /// <remarks>
    /// This is a sample implementation of the ITwitterLocalCacheGateway - Not suitable for scaled implementations
    /// </remarks>
    public class TwitterAppStateLocalCacheGateway : ApplicationStateBase, ITwitterLocalCacheGateway
    {
        private const string _twitterRateLimitHitKey = "TwitterRateLimitHit";
        private const string _twitterRateLimitResetsOnKey = "TwitterRateLimitResetsOn";
        private const string _twitterTweetsPrefix = "TwitterTweetsFor";

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterAppStateLocalCacheGateway"/> class.
        /// </summary>
        public TwitterAppStateLocalCacheGateway()
            : base(new HttpContextWrapper(HttpContext.Current))
        { }

        /// <summary>
        /// Gets or sets a value indicating whether the Twitter rate limit has been hit.
        /// </summary>
        /// <value>
        /// <c>true</c> if the Twitter rate limit has been hit; otherwise, <c>false</c>.
        /// </value>
        public bool TwitterRateLimitHit
        {
            get { return GetValue<bool>(_twitterRateLimitHitKey); }
            set { SetValue(_twitterRateLimitHitKey, value); }
        }

        /// <summary>
        /// Gets or sets the twitter rate limit resets on.
        /// </summary>
        /// <value>
        /// The twitter rate limit resets on.
        /// </value>
        public DateTime TwitterRateLimitResetsOn
        {
            get { return GetValue<DateTime>(_twitterRateLimitResetsOnKey); }
            set { SetValue(_twitterRateLimitResetsOnKey, value); }
        }

        /// <summary>
        /// Gets Tweets for a specific handle.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="tweetCount">The tweet count.</param>
        /// <returns></returns>
        public IList<Status> GetTweetsForHandle(string handle, int tweetCount = 20)
        {
            return GetValue<IList<Status>>(_twitterTweetsPrefix + handle).Take(tweetCount).ToList();
        }

        /// <summary>
        /// Saves the tweets for a specific handle.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="tweets">The tweets.</param>
        public void SaveTweetsForHandle(string handle, IList<Status> tweets)
        {
            SetValue(_twitterTweetsPrefix + handle, tweets);
        }
    }
}