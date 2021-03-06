﻿// Copyright (c) 2016 Minotech Ltd.
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

namespace Ministry.SimpleTwitterStream
{
    /// <summary>
    /// An interface to Twitter configuration information.
    /// </summary>
    public interface ITwitterConfig
    {
        /// <summary>
        /// Gets the master handle.
        /// </summary>
        /// <value>
        /// The master handle.
        /// </value>
        string MasterHandle { get; }

        /// <summary>
        /// Gets the secondary handles.
        /// </summary>
        /// <value>
        /// The secondary handles.
        /// </value>
        string[] SecondaryHandles { get; }

        /// <summary>
        /// Gets the number of tweets to display.
        /// </summary>
        /// <value>
        /// The number of tweets.
        /// </value>
        int TweetCount { get; }

        /// <summary>
        /// Gets the twitter timeout.
        /// </summary>
        /// <value>
        /// The twitter timeout.
        /// </value>
        int TwitterTimeout { get; }

        /// <summary>
        /// Gets the consumer key.
        /// </summary>
        /// <value>
        /// The consumer key.
        /// </value>
        string ConsumerKey { get; }

        /// <summary>
        /// Gets the consumer secret.
        /// </summary>
        /// <value>
        /// The consumer secret.
        /// </value>
        string ConsumerSecret { get; }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        string AccessToken { get; }

        /// <summary>
        /// Gets the access token secret.
        /// </summary>
        /// <value>
        /// The access token secret.
        /// </value>
        string AccessTokenSecret { get; }
    }
}