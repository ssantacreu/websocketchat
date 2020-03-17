using appWebSocketChat.Common.CustomEventArgs;
using Fleck;
using System;
using System.Collections.Generic;

namespace appWebSocketChat.Server
{
    /// <summary>
    /// The application server.
    /// </summary>
    internal interface IServer
    {
        /// <summary>
        /// Gets the list of connected clients.
        /// </summary>
        IList<ClientConnection> ConnectedClients { get; }

        /// <summary>
        /// Gets the <see cref="WebSocketServer"/> port number.
        /// </summary>
        int Port { get; }

        /// <summary>
        /// Start listening to client connections.
        /// </summary>
        void Start();

        /// <summary>
        /// Sets a nickname to the indicated connected client.
        /// </summary>
        /// <param name="nickname">Nickname for the connected client.</param>
        /// <param name="clientId">Connected client identifier.</param>
        void SetNicknameToConnectedClient(string nickname, Guid clientId);

        /// <summary>
        /// Sends a message to all connected clients.
        /// </summary>
        /// <param name="message">Message to send.</param>
        /// <param name="excludedConnectedClients">Exclusion list of connected clients.</param>
        void SendToConnectedClients(string message, IList<Guid> excludedConnectedClients = null);

        /// <summary>
        /// Occurs when a new client connection is stablished.
        /// </summary>
        event EventHandler<GenericEventArgs<ClientConnection>> OnClientConnection;

        /// <summary>
        /// Occurs when a client is disconnected.
        /// </summary>
        event EventHandler<GenericEventArgs<ClientConnection>> OnClientDisconnection;

        /// <summary>
        /// Occurs when a client connection has an error.
        /// </summary>
        event EventHandler<GenericEventArgs<ClientConnection, Exception>> OnClientConnectionError;

        /// <summary>
        /// Occurs when the server receives a message from one client connection.
        /// </summary>
        event EventHandler<GenericEventArgs<ClientConnection, string>> OnMessageReceived;

        /// <summary>
        /// Occurs when a client connection changes the nickname.
        /// </summary>
        event EventHandler<GenericEventArgs<ClientConnection, string>> OnClientConnectionNicknameChanged;
    }
}
