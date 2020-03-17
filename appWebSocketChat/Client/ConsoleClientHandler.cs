using appWebSocketChat.Common;
using Fleck;
using Microsoft.Extensions.Logging;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace appWebSocketChat.Client
{
    /// <summary>
    /// Specific client implementation for console application.
    /// </summary>
    internal class ConsoleClientHandler
    {
        /// <summary>
        /// Initializes a new <see cref="ConsoleClientHandler"/> instance with the indicated parameters.
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> instance to log messages.</param>
        /// <param name="port">The <see cref="WebSocketServer"/> port number.</param>
        public ConsoleClientHandler(ILogger<Program> logger, int port)
        {
            _logger = logger;

            InitializeClient(port);
        }

        #region fields 

        ILogger<Program> _logger;
        IClient _client;

        #endregion

        #region properties 

        /// <summary>
        /// Gets the <see cref="WebSocketServer"/> port number.
        /// </summary>
        public int Port { get { return _client.Port; } }

        #endregion

        #region methods 

        /// <summary>
        /// Tries to connect to the server.
        /// </summary>
        public async Task Start()
        {
            await _client.Connect();
            await Task.WhenAll(Receive(), Send());
        }

        #endregion

        #region private functions 

        /// <summary>
        /// Client initialization.
        /// </summary>
        /// <param name="port">The <see cref="WebSocketServer"/> port number.</param>
        private void InitializeClient(int port)
        {
            _client = new Client(port);

            _client.OnConnectionStablished += (s, e) =>
            {
                _logger.LogInformation($"Connection stablished at port { e.Value}");
            };

            _client.OnMessageSent += (s, e) =>
            {
                _logger.LogInformation($"Message sent to server: {e.Value}");
            };

            _client.OnMessageReceived += (s, e) =>
            {
                _logger.LogInformation($"Message received from server: {e.Value}");
            };

            _client.OnCloseMessageReceived += (s, e) =>
            {
                _logger.LogWarning("Close message from server received");
            };

            _client.OnConnectionClosed += (s, e) =>
            {
                _logger.LogWarning("Connection closed");
            };
        }

        /// <summary>
        /// Starts to listen to receive messages from the server.
        /// </summary>
        private async Task Receive()
        {
            while (_client.State.Equals(WebSocketState.Open))
            {
                await _client.Receive();
            }
        }

        /// <summary>
        /// Waits for user to type to send the message to the server.
        /// </summary>
        private async Task Send()
        {
            string _line;

            do
            {
                _logger.LogInformation("Write something to send to the server...");


                _line = Console.ReadLine();

                // Request to quit application.
                if (_line.ToLower().Equals(Commands.QUIT))
                    await _client.Disconnect("User quits connection");


                if (!string.IsNullOrEmpty(_line))
                    await _client.Send(_line);
            }
            while (_client.State.Equals(WebSocketState.Open));
        }

        #endregion
    }
}
