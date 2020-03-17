using Fleck;
using System;

namespace appWebSocketChat.Server
{
    /// <summary>
    /// Represents a client connection.
    /// </summary>
    internal class ClientConnection
    {
        /// <summary>
        /// Initializes a new <see cref="ClientConnection"/> instance with the indicated parameters.
        /// </summary>
        /// <param name="connection">The client <see cref="IWebSocketConnection"/>.</param>
        public ClientConnection(IWebSocketConnection connection) => Connection = connection;


        #region properties 

        /// <summary>
        /// Gets or sets the client <see cref="IWebSocketConnection"/>.
        /// </summary>
        public IWebSocketConnection Connection { get; set; }


        private string _nickname;

        /// <summary>
        /// Gets or sets the nickname of the connected client.
        /// </summary>
        public string Nickname
        {
            get
            {
                if (!string.IsNullOrEmpty(_nickname))
                    return _nickname;

                return $"Client {Connection.ConnectionInfo.Id}";
            }

            set { _nickname = value; }
        }

        /// <summary>
        /// Gets the unique identifier of the connected client.
        /// </summary>
        public Guid Id => Connection.ConnectionInfo.Id;

        #endregion
    }
}
