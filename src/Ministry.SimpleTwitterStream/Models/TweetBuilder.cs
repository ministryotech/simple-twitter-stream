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
using LinqToTwitter;
using System.Collections.Generic;
using System.Linq;

namespace Ministry.SimpleTwitterStream.Models
{
    #region | Interface |

    /// <summary>
    /// A factory for a Twitter stream
    /// </summary>
    public interface ITweetBuilder
    {
        /// <summary>
        /// Builds the tweet object based on provided API data.
        /// </summary>
        /// <param name="statusUpdate">The status update.</param>
        /// <returns>A tweet instance.</returns>
        Tweet Build(Status statusUpdate);
    }

    #endregion

    /// <summary>
    /// A factory for a Twitter stream
    /// </summary>
    public class TweetBuilder : ITweetBuilder
    {
        /// <summary>
        /// Builds the tweet object based on provided API data.
        /// </summary>
        /// <param name="statusUpdate">The status update.</param>
        /// <returns>
        /// A tweet instance.
        /// </returns>
        public Tweet Build(Status statusUpdate)
        {
            if (!statusUpdate.Retweeted || statusUpdate.RetweetedStatus.User == null)
            {
                return new Tweet
                {
                    Author = statusUpdate.User.Name,
                    Handle = statusUpdate.User.ScreenNameResponse,
                    Text = statusUpdate.Text,
                    DateCreated = statusUpdate.CreatedAt,
                    AvatarUrl = statusUpdate.User.ProfileImageUrl?.Remove(0, 5) ?? string.Empty,
                    Markup = GetMarkup(statusUpdate)
                };
            }

            var retweetedTweet = Build(statusUpdate.RetweetedStatus);
            retweetedTweet.Retweet = true;
            retweetedTweet.RetweetedBy = statusUpdate.User.Name;
            retweetedTweet.RetweetedByHandle = statusUpdate.User.ScreenNameResponse;
            retweetedTweet.DateRetweeted = statusUpdate.CreatedAt;

            return retweetedTweet;
        }

        #region | Private Methods |

        /// <summary>
        /// Gets the markup.
        /// </summary>
        /// <returns></returns>
        private string GetMarkup(Status statusUpdate)
        {
            var retVal = ProcessLinks(statusUpdate);
            retVal = ProcessMentions(retVal);
            retVal = ProcessHashTags(retVal);

            return retVal;
        }

        /// <summary>
        /// Processes the links.
        /// </summary>
        /// <param name="statusUpdate">The status update.</param>
        /// <returns></returns>
        private string ProcessLinks(Status statusUpdate)
        {
            var links = new List<string>();

            GetSpecialItemRecursive("http://", statusUpdate.Text, ref links);
            GetSpecialItemRecursive("https://", statusUpdate.Text, ref links);

            var retVal = statusUpdate.Text;

            foreach (var link in links)
            {
                UrlEntity entity = null;

                if (statusUpdate.Entities?.UrlEntities != null)
                {
                    entity = (from urlEntity in statusUpdate.Entities.UrlEntities
                              where urlEntity.Url == link
                              select urlEntity).FirstOrDefault();
                }

                retVal = entity != null ? retVal.Replace(link, "<a href=\"" + entity.ExpandedUrl + "\" target=\"_blank\">" + entity.DisplayUrl + "</a>")
                                          : retVal.Replace(link, "<a href=\"" + link + "\" target=\"_blank\">(more)</a>");
            }

            return retVal;
        }

        /// <summary>
        /// Processes the hash tags.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        private string ProcessHashTags(string input)
        {
            var tags = new List<string>();
            GetSpecialItemRecursive("#", input, ref tags);

            return tags.Aggregate(input, (current, tag) => current.Replace(tag, "<a href=\"https://twitter.com/hashtag/" + tag.Substring(1) + "?src=hash\" target=\"_blank\">" + tag + "</a>"));
        }

        /// <summary>
        /// Processes the mention.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        private string ProcessMentions(string input)
        {
            var mentions = new List<string>();

            GetSpecialItemRecursive("@", input, ref mentions);
            return mentions.Aggregate(input, (current, mention) => current.Replace(mention, "<a href=\"https://twitter.com/" + mention.Substring(1) + "\" target=\"_blank\">" + mention + "</a>"));
        }

        /// <summary>
        /// Gets the mention.
        /// </summary>
        /// <param name="itemKey">The item key.</param>
        /// <param name="input">The input.</param>
        /// <param name="items">The items.</param>
        private void GetSpecialItemRecursive(string itemKey, string input, ref List<string> items)
        {
            var nextItemIndex = input.IndexOf(itemKey, StringComparison.Ordinal);

            if (nextItemIndex == -1) return;

            var afterItem = input.Substring(nextItemIndex);

            if (afterItem.Length == 1)
            {
                // This ends in the symbol
                return;
            }

            if (afterItem.StartsWith(itemKey + " "))
            {
                // Ignore any lonely @ or # symbols
                var ignoreRemainder = input.Substring(2);
                if (ignoreRemainder.Length > 0) GetSpecialItemRecursive(itemKey, input.Substring(2), ref items);
                return;
            }

            var spaceIndex = afterItem.IndexOf(' ');

            items.Add(spaceIndex == -1 ? afterItem : afterItem.Substring(0, spaceIndex));

            var remainder = spaceIndex == -1 ? string.Empty : afterItem.Substring(spaceIndex);
            if (remainder.Length > 0) GetSpecialItemRecursive(itemKey, remainder, ref items);
        }

        #endregion
    }
}
