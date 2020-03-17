
namespace appWebSocketChat.Common
{
    /// <summary>
    /// Common validations helper.
    /// </summary>
    internal static class Validator
    {
        /// <summary>
        /// Validates the indicated port number.
        /// </summary>
        /// <param name="port">Port number to validate.</param>
        public static void ValidatePortNumber(string port)
        {
            if (string.IsNullOrEmpty(port))
                throw new CustomExceptions.InvalidPortException("A port number has not been specified");

            if (!int.TryParse(port, out int intPort))
                throw new CustomExceptions.InvalidPortException("The indicated port is not a numerical value");

            ValidatePortNumber(intPort);
        }

        /// <summary>
        /// Validates the indicated port number.
        /// </summary>
        /// <param name="port">Port number to validate.</param>
        public static void ValidatePortNumber(int port)
        {
            if (port < 1 || port > 65535)
                throw new CustomExceptions.InvalidPortException("The indicated port is outside the valid numeric range (1 to 65535)");
        }
    }
}
