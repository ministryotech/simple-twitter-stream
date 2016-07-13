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