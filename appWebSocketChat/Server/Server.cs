using appWebSocketChat.Common.CustomEventArgs;
using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;

namespace appWebSocketChat.Server
{
    /// <summary>
    /// The application server implementation.
    /// </summary>
    internal class Server : IServer
    {
        /// <summary>
        /// Initializes a new <see cref="Server"/> instance with the indicated parameters.
        /// </summary>
        /// <param name="port">The <see cref="WebSocketServer"/> port number.</param>
        public Server(int port) => Port = port;

        /// <summary>
        /// dtcor.
        /// </summary>
        ~Server()
        {
            try
            {
                if (_server != null)
                    _server.Dispose();
            }
            catch { }
        }


        #region fields 

        IWebSocketServer _server;

        #endregion

        #region properties 

        /// <summary>
        /// Gets the list of connected clients.
        /// </summary>
        public IList<ClientConnection> ConnectedClients { get; private set; } = new List<ClientConnection>();

        /// <summary>
        /// Gets the <see cref="WebSocketServer"/> port number.
        /// </summary>
        public int Port { get; private set; }

        #endregion

        #region methods 

        /// <summary>
        /// Start listening to client connections.
        /// </summary>
        public void Start()
        {
            _server = new WebSocketServer($"ws://0.0.0.0:{Port}");

            _server.Start(socketConnection =>
            {
                // New client connection.
                socketConnection.OnOpen = () =>
                {
                    ClientConnection _client = new ClientConnection(socketConnection);      
                    
                    ConnectedClients.Add(_client);
                    RaiseOnClientConnection(new GenericEventArgs<ClientConnection>(_client));
                };

                // Connected client closes.
                socketConnection.OnClose = () =>
                {
                    ClientConnection _client = ConnectedClients.Single(x => x.Id.Equals(socketConnection.ConnectionInfo.Id));
                    
                    ConnectedClients.Remove(_client);
                    RaiseOnClientDisconnection(new GenericEventArgs<ClientConnection>(_client));
                };

                // Unexpected error on connected client.
                socketConnection.OnError = ex =>
                {
                    ClientConnection _client = ConnectedClients.Single(x => x.Id.Equals(socketConnection.ConnectionInfo.Id));
                    
                    ConnectedClients.Remove(_client);
                    RaiseOnClientConnectionError(new GenericEventArgs<ClientConnection, Exception>(_client, ex));
                };

                // Message received.
                socketConnection.OnMessage = message =>
                {
                    ClientConnection _client = ConnectedClients.Single(x => x.Id.Equals(socketConnection.ConnectionInfo.Id));

                    RaiseOnMessageReceived(new GenericEventArgs<ClientConnection, string>(_client, message));
                };
            });
        }

        /// <summary>
        /// Sets a nickname to the indicated connected client.
        /// </summary>
        /// <param name="nickname">Nickname for the connected client.</param>
        /// <param name="clientId">Connected client identifier.</param>
        public void SetNicknameToConnectedClient(string nickname, Guid clientId)
        {
            if (ConnectedClients?.Count <= 0)
                return;

            try
            {
                ClientConnection _client = ConnectedClients.First(c => c.Id.Equals(clientId));

                string _oldNickname = _client.Nickname;

                _client.Nickname = nickname;

                RaiseOnClientConnectionNicknameChanged(new GenericEventArgs<ClientConnection, string>(_client, _oldNickname));
            }
            catch { }
        }

        /// <summary>
        /// Sends a message to all connected clients.
        /// </summary>
        /// <param name="message">Message to send.</param>
        /// <param name="excludedConnectedClients">Exclusion list of connected clients.</param>
        public void SendToConnectedClients(string message, IList<Guid> excludedConnectedClients = null)
        {
            if (ConnectedClients?.Count <= 0)
                return;

            bool _exclude = excludedConnectedClients?.Count > 0;

            ConnectedClients.ToList().ForEach(c =>
            {
                if (!_exclude || !excludedConnectedClients.ToList().Contains(c.Id))
                    c.Connection.Send(message);
            });
        }

        #endregion

        #region events 

        /// <summary>
        /// Occurs when a new client connection is stablished.
        /// </summary>
        public event EventHandler<GenericEventArgs<ClientConnection>> OnClientConnection;

        /// <summary>
        /// Raises the new client connection stablished event.
        /// </summary>
        /// <param name="e">Custom <see cref="EventArgs"/> for the event.</param>
        protected virtual void RaiseOnClientConnection(GenericEventArgs<ClientConnection> e)
        {
            OnClientConnection?.Invoke(this, e);
        }

        /// <summary>
        /// Occurs when a client is disconnected.
        /// </summary>
        public event EventHandler<GenericEventArgs<ClientConnection>> OnClientDisconnection;

        /// <summary>
        /// Raises the client disconnection event.
        /// </summary>
        /// <param name="">Custom <see cref="EventArgs"/> for the event.</param>
        protected virtual void RaiseOnClientDisconnection(GenericEventArgs<ClientConnection> e)
        {
            OnClientDisconnection?.Invoke(this, e);
        }

        /// <summary>
        /// Occurs when a client connection has an error.
        /// </summary>
        public event EventHandler<GenericEventArgs<ClientConnection, Exception>> OnClientConnectionError;

        /// <summary>
        /// Raises the client connection error event.
        /// </summary>
        /// <param name="e">Custom <see cref="EventArgs"/> for the event.</param>
        protected virtual void RaiseOnClientConnectionError(GenericEventArgs<ClientConnection, Exception> e)
        {
            OnClientConnectionError?.Invoke(this, e);
        }

        /// <summary>
        /// Occurs when the server receives a message from one client connection.
        /// </summary>
        public event EventHandler<GenericEventArgs<ClientConnection, string>> OnMessageReceived;

        /// <summary>
        /// Raises the message received from one client connection event.
        /// </summary>
        /// <param name="e">Custom <see cref="EventArgs"/> for the event.</param>
        protected virtual void RaiseOnMessageReceived(GenericEventArgs<ClientConnection, string> e)
        {
            OnMessageReceived?.Invoke(this, e);
        }

        /// <summary>
        /// Occurs when a client connection changes the nickname.
        /// </summary>
        public event EventHandler<GenericEventArgs<ClientConnection, string>> OnClientConnectionNicknameChanged;

        /// <summary>
        /// Raises the client connection nickname changed event.
        /// </summary>
        /// <param name="e">Custom <see cref="EventArgs"/> for the event.</param>
        protected virtual void RaiseOnClientConnectionNicknameChanged(GenericEventArgs<ClientConnection, string> e)
        {
            OnClientConnectionNicknameChanged?.Invoke(this, e);
        }

        #endregion
    }
}
