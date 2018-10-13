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

using System;
using System.Web;

namespace Ministry.SimpleTwitterStream.Models
{
    /// <summary>
    /// A representation of a tweet
    /// </summary>
    public class Tweet
    {

        #region | Properties |

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>
        /// The author.
        /// </value>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the handle.
        /// </summary>
        /// <value>
        /// The handle.
        /// </value>
        public string Handle { get; set; }

        /// <summary>
        /// Gets or sets the avatar URL.
        /// </summary>
        /// <value>
        /// The avatar URL.
        /// </value>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Tweet"/> is a retweet.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a retweet; otherwise, <c>false</c>.
        /// </value>
        public bool Retweet { get; set; }

        /// <summary>
        /// Gets or sets the name of the user the tweet was retweeted by.
        /// </summary>
        /// <value>
        /// The user the tweet was retweeted by.
        /// </value>
        public string RetweetedBy { get; set; }

        /// <summary>
        /// Gets or sets the handle of the user the tweet was retweeted by.
        /// </summary>
        /// <value>
        /// The retweeted by handle.
        /// </value>
        public string RetweetedByHandle { get; set; }

        /// <summary>
        /// Gets or sets the date the tweet was created.
        /// </summary>
        /// <value>
        /// The date created.
        /// </value>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date the tweet was retweeted.
        /// </summary>
        /// <value>
        /// The date retweeted.
        /// </value>
        public DateTime? DateRetweeted { get; set; }

        /// <summary>
        /// Returns the tweet text as markup.
        /// </summary>
        /// <returns>An HTML formatted text.</returns>
        public HtmlString Markup { get; set; }


        #endregion

    }
}