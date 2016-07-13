using System;

namespace Ministry.SimpleTwitterStream
{
    /// <summary>
    /// Wrapper for Times
    /// </summary>
    public interface ITimeProvider
    {
        /// <summary>
        /// Gets the current time.
        /// </summary>
        /// <value>
        /// The current time.
        /// </value>
        DateTime Now { get; }
    }

    /// <summary>
    /// Wrapper for Times
    /// </summary>
    public class TimeProvider : ITimeProvider
    {
        /// <summary>
        /// Gets the current time.
        /// </summary>
        /// <value>
        /// The current time.
        /// </value>
        public DateTime Now { get { return DateTime.Now; } }
    }
}