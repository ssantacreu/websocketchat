using appWebSocketChat.Common.CustomEventArgs;
using Fleck;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace appWebSocketChat.Client
{
    /// <summary>
    /// The application client.
    /// </summary>
    internal interface IClient
    {
        /// <summary>
        /// Gets the <see cref="WebSocketServer"/> port number.
        /// </summary>
        int Port { get; }

        /// <summary>
        /// Gets the WebSocket state of the <see cref="ClientWebSocket"/> instance.
        /// </summary>
        WebSocketState State { get; }

        /// <summary>
        /// Tries to connect to the server.
        /// </summary>
        /// <returns></returns>
        Task Connect();

        /// <summary>
        /// Sends a message to the server.
        /// </summary>
        /// <param name="message">Message to be sent to the server.</param>
        Task Send(string message);

        /// <summary>
        /// Tries to receive a message from the server.
        /// </summary>
        /// <returns>Message received from the server.</returns>
        Task<string> Receive();

        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        /// <param name="statusDescription">A description of the close status.</param>
        Task Disconnect(string statusDescription);

        /// <summary>
        /// Occurs when the server connection is stablished.
        /// </summary>
        event EventHandler<GenericEventArgs<int>> OnConnectionStablished;

        /// <summary>
        /// Occurs when the indicated message was sent to server.
        /// </summary>
        event EventHandler<GenericEventArgs<string>> OnMessageSent;

        /// <summary>
        /// Occurs when a message was received from the server.
        /// </summary>
        event EventHandler<GenericEventArgs<string>> OnMessageReceived;

        /// <summary>
        /// Occurs when a close message was received from the server.
        /// </summary>
        event EventHandler OnCloseMessageReceived;

        /// <summary>
        /// Occurs when server connection is closed.
        /// </summary>
        event EventHandler OnConnectionClosed;
    }
}
