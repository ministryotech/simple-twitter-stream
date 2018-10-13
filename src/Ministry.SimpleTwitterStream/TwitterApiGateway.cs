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
using Ministry.SimpleTwitterStream.Models;

namespace Ministry.SimpleTwitterStream
{
    #region | Interface |

    /// <summary>
    /// A factory for a Twitter stream
    /// </summary>
    public interface ITwitterApiGateway : ITwitterGateway
    { }

    #endregion

    /// <summary>
    /// A factory for a Twitter stream
    /// </summary>
    public class TwitterApiGateway : ITwitterApiGateway
    {
        private readonly ITwitterConfig twitterConfig;
        private readonly IDateTimeAccessor dateTimeAccessor;
        private readonly TwitterContext context;

        #region | Construction |

        /// <summary>
        /// Initializes a new instance of the <see cref="TweetListBuilder" /> class.
        /// </summary>
        /// <param name="twitterConfig">The configuration reader.</param>
        /// <param name="dateTimeAccessor">The date time accessor.</param>
        public TwitterApiGateway(ITwitterConfig twitterConfig, IDateTimeAccessor dateTimeAccessor)
        {
            this.twitterConfig = twitterConfig;
            this.dateTimeAccessor = dateTimeAccessor;

            context = new TwitterContext(GetAuthorizer());
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
            var tweetsTask = (from tweet in context.Status
                                where tweet.Type == StatusType.User &&
                                tweet.ScreenName == handle
                                select tweet).Take(tweetCount).ToListAsync();
            tweetsTask.Wait(twitterConfig.TwitterTimeout);
            
            TwitterRateLimitHit = context.RateLimitRemaining < 2;

            if (TwitterRateLimitResetsOn <= dateTimeAccessor.Now)
                TwitterRateLimitResetsOn = dateTimeAccessor.Now.AddMinutes(15);

            return tweetsTask.Result;
        }

        #region | Private Methods |

        /// <summary>
        /// Gets the authorizer.
        /// </summary>
        /// <returns>A Twitter authorizer</returns>
        private SingleUserAuthorizer GetAuthorizer() 
            => new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = twitterConfig.ConsumerKey,
                    ConsumerSecret = twitterConfig.ConsumerSecret,
                    AccessToken = twitterConfig.AccessToken,
                    AccessTokenSecret = twitterConfig.AccessTokenSecret
                }
            };

        #endregion
    }
}
