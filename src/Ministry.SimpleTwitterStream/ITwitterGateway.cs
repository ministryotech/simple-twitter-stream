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
        /// <returns></returns>
        IList<Status> GetTweetsForHandle(string handle, int tweetCount = 20);
    }
}
