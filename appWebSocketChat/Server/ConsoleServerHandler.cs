using appWebSocketChat.Common;
using Fleck;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace appWebSocketChat.Server
{
    /// <summary>
    /// Specific server implementation for console application.
    /// </summary>
    internal class ConsoleServerHandler
    {
        /// <summary>
        /// Initializes a new <see cref="ConsoleServerHandler"/> instance with the indicated parameters.
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> instance to log messages.</param>
        /// <param name="port">The <see cref="WebSocketServer"/> port number.</param>
        public ConsoleServerHandler(ILogger<Program> logger, int port)
        {
            _logger = logger;

            InitializeServer(port);
        }

        #region fields 

        ILogger<Program> _logger;
        IServer _server;

        #endregion

        #region properties 

        /// <summary>
        /// Gets the list of connected clients.
        /// </summary>
        public IList<ClientConnection> ConnectedClients { get { return _server.ConnectedClients; } }

        /// <summary>
        /// Gets the <see cref="WebSocketServer"/> port number.
        /// </summary>
        public int Port { get { return _server.Port; } }

        #endregion

        #region methods 

        /// <summary>
        /// Start listening to client connections.
        /// </summary>
        public void Start()
        {
            _server.Start();
            _logger.LogWarning($"Type '{Commands.QUIT}' to close application");

            string _line;

            do
            {
                _line = Console.ReadLine();


                // Request to quit application.
                if (_line.ToLower().Equals(Commands.QUIT))
                    continue;

                // Request to known connected clients.
                else if (_line.ToLower().Equals(Commands.CLIENTS))
                    _logger.LogInformation($"Connected clients: {ConnectedClients?.Count ?? 0}");

                else if (!string.IsNullOrEmpty(_line))
                    _server.SendToConnectedClients(_line);
            }
            while (!_line.ToLower().Equals(Commands.QUIT));


            // Disconnect all clients.
            _server.ConnectedClients.ToList().ForEach(c =>
            {
                if (c.Connection.IsAvailable)
                    c.Connection.Close();
            });
        }

        #endregion

        #region private functions 

        /// <summary>
        /// Server initialization.
        /// </summary>
        /// <param name="port">The <see cref="WebSocketServer"/> port number.</param>
        private void InitializeServer(int port)
        {
            _server = new Server(port);

            _server.OnClientConnection += (s, e) =>
            {
                _logger.LogInformation($"Client connected: {e.Value.Nickname}");
            };

            _server.OnClientDisconnection += (s, e) =>
            {
                _logger.LogWarning($"Client disconnected: {e.Value.Nickname}");
            };

            _server.OnClientConnectionError += (s, e) =>
            {
                _logger.LogWarning($"Client disconnected: {e.Value.Nickname}");
                _logger.LogError(e.Value2, "Client EXCEPTION");
            };

            _server.OnMessageReceived += (s, e) =>
            {
                // Request to change nickname.
                if (e.Value2.ToLower().StartsWith(Commands.NICKNAME))
                {
                    string[] _chunks = e.Value2.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    if (_chunks[0].Equals(Commands.NICKNAME) && _chunks.Length > 1)
                    {
                        _server.SetNicknameToConnectedClient(_chunks[1], e.Value.Id);

                        return;
                    }
                }


                _logger.LogInformation($"{e.Value.Nickname} says: {e.Value2}");

                _server.SendToConnectedClients($"{e.Value.Nickname} says: {e.Value2}", new List<Guid> { e.Value.Id });
            };

            _server.OnClientConnectionNicknameChanged += (s, e) =>
            {
                _logger.LogInformation($"Client {e.Value2} changed the nickname to {e.Value.Nickname}");

                _server.SendToConnectedClients($"Client {e.Value2} changed the nickname to {e.Value.Nickname}");
            };
        }

        #endregion
    }
}
