using LinqToTwitter;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ministry.SimpleTwitterStream.Models
{
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
                    AvatarUrl = statusUpdate.User.ProfileImageUrl != null ? statusUpdate.User.ProfileImageUrl.Remove(0, 5) : string.Empty,
                    Markup = new HtmlString(GetMarkup(statusUpdate))
                };
            }
            else
            {
                var retweetedTweet = Build(statusUpdate.RetweetedStatus) as Tweet;
                retweetedTweet.Retweet = true;
                retweetedTweet.RetweetedBy = statusUpdate.User.Name;
                retweetedTweet.RetweetedByHandle = statusUpdate.User.ScreenNameResponse;
                retweetedTweet.DateRetweeted = statusUpdate.CreatedAt;

                return retweetedTweet;
            }
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
        /// <param name="input">The input.</param>
        /// <returns></returns>
        private string ProcessLinks(Status statusUpdate)
        {
            var links = new List<string>();

            GetSpecialItemRecursive("http://", statusUpdate.Text, ref links);
            GetSpecialItemRecursive("https://", statusUpdate.Text, ref links);

            string retVal = statusUpdate.Text;

            foreach (var link in links)
            {
                UrlEntity entity = null;

                if (statusUpdate.Entities != null && statusUpdate.Entities.UrlEntities != null)
                {
                    entity = (from urlEntity in statusUpdate.Entities.UrlEntities
                              where urlEntity.Url == link
                              select urlEntity).FirstOrDefault();
                }

                retVal = (entity != null) ? retVal.Replace(link, "<a href=\"" + entity.ExpandedUrl + "\" target=\"_blank\">" + entity.DisplayUrl + "</a>")
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

            foreach (var tag in tags)
            {
                input = input.Replace(tag, "<a href=\"https://twitter.com/hashtag/" + tag.Substring(1) + "?src=hash\" target=\"_blank\">" + tag + "</a>");
            }

            return input;
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

            foreach (var mention in mentions)
            {
                input = input.Replace(mention, "<a href=\"https://twitter.com/" + mention.Substring(1) + "\" target=\"_blank\">" + mention + "</a>");
            }

            return input;
        }

        /// <summary>
        /// Gets the mention.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        private void GetSpecialItemRecursive(string itemKey, string input, ref List<string> items)
        {
            var nextItemIndex = input.IndexOf(itemKey);

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
