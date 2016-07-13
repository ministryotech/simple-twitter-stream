using System;
using System.Collections.Generic;
using System.Web;

namespace Ministry.SimpleTwitterStream.Models
{
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


        #region | Properties |

        /// <summary>
        /// Gets the master handle.
        /// </summary>
        /// <value>
        /// The master handle.
        /// </value>
        public string MasterHandle { get; private set; }

        /// <summary>
        /// Gets the secondary handles.
        /// </summary>
        /// <value>
        /// The secondary handles.
        /// </value>
        public IReadOnlyCollection<string> SecondaryHandles { get; private set; }

        #endregion

    }
}