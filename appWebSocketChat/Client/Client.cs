using appWebSocketChat.Common.CustomEventArgs;
using appWebSocketChat.Common.CustomExceptions;
using Fleck;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace appWebSocketChat.Client
{
    /// <summary>
    /// The application client implementation.
    /// </summary>
    internal class Client : IClient
    {
        /// <summary>
        /// Initializes a new <see cref="Client"/> instance with the indicated parameters.
        /// </summary>
        /// <param name="port">The <see cref="WebSocketServer"/> port number.</param>
        public Client(int port) => Port = port;

        /// <summary>
        /// dtcor.
        /// </summary>
        ~Client()
        {
            try
            {
                if (_client != null)
                    _client.Dispose();
            }
            catch { }
        }


        #region fields 

        ClientWebSocket _client;

        #endregion

        #region properties 

        /// <summary>
        /// Gets the <see cref="WebSocketServer"/> port number.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Gets the WebSocket state of the <see cref="ClientWebSocket"/> instance.
        /// </summary>
        public WebSocketState State
        {
            get
            {
                return
                    _client?.State ?? WebSocketState.None;
            }
        }

        #endregion

        #region methods 

        /// <summary>
        /// Tries to connect to the server.
        /// </summary>
        public async Task Connect()
        {
            _client = new ClientWebSocket();

            try
            {
                Common.Validator.ValidatePortNumber(Port);

                await _client.ConnectAsync(new Uri($"ws://localhost:{Port}"), CancellationToken.None);

                RaiseOnConnectionStablished(new GenericEventArgs<int>(Port));
            }
            catch (InvalidPortException)
            {
                throw;
            }
            catch (System.Net.WebSockets.WebSocketException)
            {
                throw new WebSocketServerNotFoundException(Port);
            }
        }

        /// <summary>
        /// Sends a message to the server.
        /// </summary>
        /// <param name="message">Message to be sent to the server.</param>
        public async Task Send(string message)
        {
            if (!_client.State.Equals(WebSocketState.Open))
                return;


            byte[] _encodedMessage = Encoding.UTF8.GetBytes(message);

            await _client.SendAsync(new ArraySegment<byte>(_encodedMessage, 0, _encodedMessage.Length), WebSocketMessageType.Text, true, CancellationToken.None);

            RaiseOnMessageSent(new GenericEventArgs<string>(message));
        }

        /// <summary>
        /// Tries to receive a message from the server.
        /// </summary>
        /// <returns>Message received from the server.</returns>
        public async Task<string> Receive()
        {
            if (!_client.State.Equals(WebSocketState.Open))
                return
                    string.Empty;


            string _message = string.Empty;


            byte[] _buffer = new byte[1024];

            WebSocketReceiveResult _result = await _client.ReceiveAsync(new ArraySegment<byte>(_buffer), CancellationToken.None);

            if (_result.MessageType.Equals(WebSocketMessageType.Close))
                RaiseOnCloseMessageReceived();
            else
            {
                _message = Encoding.UTF8.GetString(_buffer).TrimEnd('\0');
                RaiseOnMessageReceived(new GenericEventArgs<string>(_message));
            }

            return
                _message;
        }

        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        /// <param name="statusDescription">A description of the close status.</param>
        public async Task Disconnect(string statusDescription)
        {
            if (_client.State.Equals(WebSocketState.Connecting))
                _client.Abort();

            else if (_client.State.Equals(WebSocketState.Open))
                await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, statusDescription, CancellationToken.None);

            RaiseOnConnectionClosed();
        }

        #endregion

        #region events 

        /// <summary>
        /// Occurs when the server connection is stablished.
        /// </summary>
        public event EventHandler<GenericEventArgs<int>> OnConnectionStablished;

        /// <summary>
        /// Raises the server connection stablished event.
        /// </summary>
        /// <param name="e">Custom <see cref="EventArgs"/> for the event.</param>
        protected virtual void RaiseOnConnectionStablished(GenericEventArgs<int> e)
        {
            OnConnectionStablished?.Invoke(this, e);
        }

        /// <summary>
        /// Occurs when the indicated message was sent to server.
        /// </summary>
        public event EventHandler<GenericEventArgs<string>> OnMessageSent;

        /// <summary>
        /// Raises the message sent to server event.
        /// </summary>
        /// <param name="e">Custom <see cref="EventArgs"/> for the event.</param>
        protected virtual void RaiseOnMessageSent(GenericEventArgs<string> e)
        {
            OnMessageSent?.Invoke(this, e);
        }

        /// <summary>
        /// Occurs when a message was received from the server.
        /// </summary>
        public event EventHandler<GenericEventArgs<string>> OnMessageReceived;

        /// <summary>
        /// Raises the message received from the server event.
        /// </summary>
        /// <param name="e">Custom <see cref="EventArgs"/> for the event.</param>
        protected virtual void RaiseOnMessageReceived(GenericEventArgs<string> e)
        {
            OnMessageReceived?.Invoke(this, e);
        }

        /// <summary>
        /// Occurs when a close message was received from the server.
        /// </summary>
        public event EventHandler OnCloseMessageReceived;

        /// <summary>
        /// Raises the close message received from the server event.
        /// </summary>
        protected virtual void RaiseOnCloseMessageReceived()
        {
            OnCloseMessageReceived?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when server connection is closed.
        /// </summary>
        public event EventHandler OnConnectionClosed;

        /// <summary>
        /// Raises the server connection is closed event.
        /// </summary>
        protected virtual void RaiseOnConnectionClosed()
        {
            OnConnectionClosed?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
