using System;

namespace appWebSocketChat.Common.CustomExceptions
{
    /// <summary>
    /// The exception that is thrown when the indicated port is invalid.
    /// </summary>
    internal class InvalidPortException : Exception
    {
        /// <summary>
        /// Initializes a new <see cref="InvalidPortException"/> intance.
        /// </summary>
        /// <param name="message">Message that describes the current exception.</param>
        public InvalidPortException(string message) { Message = message; }


        #region properties 

        /// <summary>
        /// Gets the message that describes the current exception.
        /// </summary>
        public new string Message { get; private set; }

        #endregion
    }
}
