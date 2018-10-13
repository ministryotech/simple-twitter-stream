using LinqToTwitter;
using System;
using System.Collections.Generic;

namespace Ministry.SimpleTwitterStream
{
    /// <summary>
    /// A non-existent cache implementation to use if no cache is to be used.
    /// </summary>
    /// <seealso cref="ITwitterLocalCacheGateway" />
    public class NullLocalCacheGateway :ITwitterLocalCacheGateway
    {
        /// <summary>
        /// Gets or sets a value indicating whether the Twitter rate limit has been hit.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the Twitter rate limit has been hit; otherwise, <c>false</c>.
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
        /// Gets Tweets for a specific handle.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="tweetCount">The tweet count.</param>
        /// <returns></returns>
        public IList<Status> GetTweetsForHandle(string handle, int tweetCount = 20)
            => new List<Status>();

        /// <summary>
        /// Saves the tweets for a specific handle.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="tweets">The tweets.</param>
        public void SaveTweetsForHandle(string handle, IList<Status> tweets)
        { }
    }
}
