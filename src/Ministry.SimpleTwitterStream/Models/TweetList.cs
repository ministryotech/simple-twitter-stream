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

using System.Collections.Generic;

namespace Ministry.SimpleTwitterStream.Models
{
    #region | Interface |

    /// <summary>
    /// A representation of a stream of tweets
    /// </summary>
    public interface ITweetList : IList<Tweet>
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
        IReadOnlyCollection<string> SecondaryHandles { get; }
    }

    #endregion

    /// <summary>
    /// A representation of a stream of tweets
    /// </summary>
    public class TweetList : List<Tweet>, ITweetList
    {
        #region | Construction |

        /// <summary>
        /// Initializes a new instance of the <see cref="TweetList"/> class.
        /// </summary>
        /// <param name="handle">The handle.</param>
        public TweetList(string handle)
        {
            MasterHandle = handle;
            SecondaryHandles = new List<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TweetList"/> class.
        /// </summary>
        /// <param name="masterHandle">The master handle.</param>
        /// <param name="secondaryHandles">The secondary handles.</param>
        public TweetList(string masterHandle, string[] secondaryHandles)
        {
            MasterHandle = masterHandle;
            SecondaryHandles = secondaryHandles;
        }

        #endregion

        /// <summary>
        /// Gets the master handle.
        /// </summary>
        /// <value>
        /// The master handle.
        /// </value>
        public string MasterHandle { get; }

        /// <summary>
        /// Gets the secondary handles.
        /// </summary>
        /// <value>
        /// The secondary handles.
        /// </value>
        public IReadOnlyCollection<string> SecondaryHandles { get; }
    }
}