using LinqToTwitter;
using System.Collections.Generic;

namespace Ministry.SimpleTwitterStream
{
    /// <summary>
    /// A factory for a Twitter stream
    /// </summary>
    public interface ITwitterLocalCacheGateway : ITwitterGateway
    {
        /// <summary>
        /// Saves the tweets for a specific handle.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="tweets">The tweets.</param>
        void SaveTweetsForHandle(string handle, IList<Status> tweets);
    }
}