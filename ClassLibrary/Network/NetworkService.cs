using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ClassLibrary.Network
{
    public class NetworkService
    {
        public async Task<Response> CheckConnection()
        {
            var client = new HttpClient(); //Testar se tem ligação à net

            try
            {
                using (client.GetAsync("http://client3.google.com/generate_204"))
                {
                    return new Response
                    {
                        Success = true,
                        //Message = "Correu Tudo Bem",
                    };
                }
            }
            catch
            {
                return new Response
                {
                    Success = false,
                    Message = "Configure a sua ligação à internet",
                };
            }
        }

        private static bool _isAvailable;
        private static NetworkStatusChangedHandler _handler;

        static NetworkService()
        {
            _isAvailable = IsNetworkAvailable();
        }

        public static event NetworkStatusChangedHandler AvailabilityChanged
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                if (_handler == null)
                {
                    NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(DoNetworkAvailabilityChanged);

                    NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(DoNetworkAddressChanged);
                }

                _handler = (NetworkStatusChangedHandler)Delegate.Combine(_handler, value);
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                _handler = (NetworkStatusChangedHandler)Delegate.Remove(_handler, value);

                if (_handler == null)
                {
                    NetworkChange.NetworkAvailabilityChanged -= new NetworkAvailabilityChangedEventHandler(DoNetworkAvailabilityChanged);

                    NetworkChange.NetworkAddressChanged -= new NetworkAddressChangedEventHandler(DoNetworkAddressChanged);
                }
            }
        }

        public static bool IsAvailable
        {
            get { return _isAvailable; }
        }

        public static bool IsNetworkAvailable()
        {
            // only recognizes changes related to Internet adapters
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                // however, this will include all adapters
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface face in interfaces)
                {
                    // filter so we see only Internet adapters
                    if (face.OperationalStatus == OperationalStatus.Up)
                    {
                        if ((face.NetworkInterfaceType != NetworkInterfaceType.Tunnel) &&
                            (face.NetworkInterfaceType != NetworkInterfaceType.Loopback))
                        {
                            IPv4InterfaceStatistics statistics = face.GetIPv4Statistics();

                            // all testing seems to prove that once an interface comes online
                            // it has already accrued statistics for both received and sent...

                            if ((statistics.BytesReceived > 0) &&
                                (statistics.BytesSent > 0))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }


        private static void DoNetworkAddressChanged(object sender, EventArgs e)
        {
            SignalAvailabilityChange(sender);
        }


        private static void DoNetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            SignalAvailabilityChange(sender);
        }


        private static void SignalAvailabilityChange(object sender)
        {
            bool change = IsNetworkAvailable();

            if (change != _isAvailable)
            {
                _isAvailable = change;

                if (_handler != null)
                {
                    _handler(sender, new NetworkStatus(_isAvailable));
                }
            }
        }

    }
}
