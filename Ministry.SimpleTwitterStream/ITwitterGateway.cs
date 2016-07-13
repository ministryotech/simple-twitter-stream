using LinqToTwitter;
using System;
using System.Collections.Generic;

namespace Ministry.SimpleTwitterStream
{
    /// <summary>
    /// A factory for a Twitter stream
    /// </summary>
    public interface ITwitterGateway
    {
        /// <summary>
        /// Gets or sets a value indicating whether the Twitter rate limit has been hit.
        /// </summary>
        /// <value>
        /// <c>true</c> if the Twitter rate limit has been hit; otherwise, <c>false</c>.
        /// </value>
        bool TwitterRateLimitHit { get; set; }

        /// <summary>
        /// Gets or sets the twitter rate limit resets on.
        /// </summary>
        /// <value>
        /// The twitter rate limit resets on.
        /// </value>
        DateTime TwitterRateLimitResetsOn { get; set; }

        /// <summary>
        /// Gets Tweets for a specific handle.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="tweetCount">The tweet count.</param>
        /// <param name="includeRetweets">if set to <c>true</c> includes retweets.</param>
        /// <returns></returns>
        IList<Status> GetTweetsForHandle(string handle, int tweetCount = 20);
    }
}
