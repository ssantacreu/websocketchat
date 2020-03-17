using System;

namespace appWebSocketChat.Common.CustomExceptions
{
    /// <summary>
    /// The exception that is thrown when the <see cref="Server.IServer"/> is not found.
    /// </summary>
    internal class WebSocketServerNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new <see cref="WebSocketServerNotFoundException"/> intance with the indicated parameters.
        /// </summary>
        /// <param name="port">Port number used to connect.</param>
        public WebSocketServerNotFoundException(int port) => Port = port;


        #region properties 

        /// <summary>
        /// Gets the port number used to connect.
        /// </summary>
        public int Port { get; set; }

        #endregion
    }
}
