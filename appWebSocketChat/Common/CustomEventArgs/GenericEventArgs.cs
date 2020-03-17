using System;

namespace appWebSocketChat.Common.CustomEventArgs
{
    /// <summary>
    /// Generic <see cref="EventArgs"/> class that will store a value of some type.
    /// </summary>
    /// <typeparam name="T">Type of the value to store.</typeparam>
    internal class GenericEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new <see cref="GenericEventArgs{T}"/> instance with the indicated parameters.
        /// </summary>
        /// <param name="value">Value to store.</param>
        public GenericEventArgs(T value) => Value = value;


        #region properties 

        /// <summary>
        /// Stored value of <see cref="T"/> type.
        /// </summary>
        public T Value { get; private set; }

        #endregion
    }

    /// <summary>
    /// Generic <see cref="EventArgs"/> class that will store two values of some type.
    /// </summary>
    /// <typeparam name="T">Type of the first value to store.</typeparam>
    /// <typeparam name="U">Type of the second value to store.</typeparam>
    internal class GenericEventArgs<T, U> : GenericEventArgs<T>
    {
        /// <summary>
        /// Initializes a new <see cref="GenericEventArgs{T, U}"/> instance with the indicated parameters.
        /// </summary>
        /// <param name="value">First value to store.</param>
        /// <param name="value2">Second value to store.</param>
        public GenericEventArgs(T value, U value2) : base(value) => Value2 = value2;


        #region properties 

        /// <summary>
        /// Stored value of <see cref="U"/> type.
        /// </summary>
        public U Value2 { get; private set; }

        #endregion
    }
}
