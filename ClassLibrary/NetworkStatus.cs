using Newtonsoft.Json;

namespace ClassLibrary
{
    public class NetworkStatus
    {
        private bool _isAvailable;

        /// <summary>
        /// Instantiate a new instance with the given availability.
        /// </summary>
        /// <param name="isAvailable"></param>
        public NetworkStatus(bool isAvailable)
        {
            this._isAvailable = isAvailable;
        }

        /// <summary>
        /// Gets a Boolean value indicating the current state of Internet connectivity.
        /// </summary>
        public bool IsAvailable
        {
            get { return _isAvailable; }
        }
    }

    //********************************************************************************************
    // delegate NetworkStatusChangedHandler
    //********************************************************************************************

    /// <summary>
    /// Define the method signature for network status changes.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void NetworkStatusChangedHandler(object sender, NetworkStatus e);
}

