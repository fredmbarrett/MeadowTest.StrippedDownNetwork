using Meadow;
using Meadow.Gateway.WiFi;
using Meadow.Hardware;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MeadowTest.StrippedDownNetwork
{
    internal class NetworkController : MeadowBase
    {
        #region Singleton Declaration

        private static readonly Lazy<NetworkController> instance =
            new Lazy<NetworkController>(() => new NetworkController());

        public static NetworkController Instance => instance.Value;

        private NetworkController()
        {
            Resolver.Device.PlatformOS.NtpClient.TimeChanged += (time) =>
            {
                Log.Info($"Network time changed to {time}");
                IsTimeSet = true;
            };
        }

        #endregion Singleton Declaration

        public IPAddress? IpAddress { get; private set; }
        public IPAddress? SubnetMask { get; private set; }
        public IPAddress? DefaultGateway { get; private set; }

        public string SSID { get; private set; }
        public bool IsConnected { get; private set; }
        public bool IsTimeSet { get; private set; }

        IWiFiNetworkAdapter _wifi;

        /// <summary>
        /// Initialize the WiFiController functions
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task InitializeWifiNetwork()
        {
            // TODO: Get from Device Network menu
            SSID = AppSettings.WifiSsid;
            if (string.IsNullOrEmpty(SSID))
                throw new ArgumentNullException(nameof(SSID));

            var passwd = AppSettings.WifiPassword;
            if (string.IsNullOrEmpty(passwd))
                throw new ArgumentNullException(nameof(passwd));

            var sleepTime = TimeSpan.FromSeconds(AppSettings.WifiWakeUpDelaySeconds);
            var timeout = TimeSpan.FromSeconds(AppSettings.WifiTimeoutSeconds);
            var retries = AppSettings.WifiMaxRetryCount;

            Log.Debug($"Initializing NetworkController for network {SSID}...");
            _wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            _wifi.NetworkConnected += OnWifiNetworkConnected;
            _wifi.NetworkError += OnWifiNetworkError;

            await ScanForAccessPoints(_wifi);

            try
            {
                Log.Debug($"...wifi connecting to {SSID}...");
                await _wifi.Connect(SSID, passwd, timeout);

                // Give the wifi network a little more time to come up
                //await Task.Delay(sleepTime);
            }
            catch (Exception ex)
            {
                Log.Error($"Error initializing WiFi: {ex.Message} with {retries} left");
                retries -= 1;

                if (retries == 0)
                    throw new Exception("Cannot connect to network");
            }
        }

        private void OnWifiNetworkError(INetworkAdapter sender, NetworkErrorEventArgs args)
        {
            Log.Error($"Wifi network error: {args.ErrorCode}");
            throw new NetworkException($"Error connecting to Wifi: {args.ErrorCode}");
        }

        public async Task InitializeCellNetwork()
        {
            var cell = Device.NetworkAdapters.Primary<ICellNetworkAdapter>();
            var sleepTime = TimeSpan.FromSeconds(AppSettings.CellWakeUpDelaySeconds);

            while (!cell.IsConnected)
            {
                await Task.Delay(1000);
            }

            // Give the cell network a little more time to come up
            Thread.Sleep(sleepTime);

            IpAddress = cell.IpAddress;
            SubnetMask = cell.SubnetMask;
            DefaultGateway = cell.Gateway;
            IsConnected = cell.IsConnected;

            Log.Info($"Cellular network is up, ip is {DefaultGateway.MapToIPv4()}");
        }

        private void OnWifiNetworkConnected(INetworkAdapter sender, NetworkConnectionEventArgs args)
        {
            if (args != null)
            {
                IpAddress = args.IpAddress;
                SubnetMask = args.Subnet;
                DefaultGateway = args.Gateway;
                IsConnected = true;
            }

            Log.Info($"Wifi network {SSID} is up, ip is {DefaultGateway.MapToIPv4()}");
        }

        async Task ScanForAccessPoints(IWiFiNetworkAdapter wifi)
        {
            Resolver.Log.Info("Getting list of access points");
            var networks = await wifi.Scan(TimeSpan.FromSeconds(60));

            if (networks.Count > 0)
            {
                Resolver.Log.Info("|-------------------------------------------------------------|---------|");
                Resolver.Log.Info("|         Network Name             | RSSI |       BSSID       | Channel |");
                Resolver.Log.Info("|-------------------------------------------------------------|---------|");

                foreach (WifiNetwork accessPoint in networks)
                {
                    Resolver.Log.Info($"| {accessPoint.Ssid,-32} | {accessPoint.SignalDbStrength,4} | {accessPoint.Bssid,17} |   {accessPoint.ChannelCenterFrequency,3}   |");
                }
            }
            else
            {
                Resolver.Log.Info($"No access points detected");
            }
        }

        public async Task WaitForNtpTimeUpdate()
        {
            Log.Info($"Waiting for Network Time event...");
            if (IsTimeSet == false)
            {
                var ntpTimeout = TimeSpan.FromSeconds(AppSettings.NtpTimeoutSeconds);
                var ntpRetries = AppSettings.NtpMaxRetryCount;
                do
                {
                    await Task.Delay(ntpTimeout);
                    ntpRetries--;
                } while (!NetworkController.Instance.IsTimeSet && ntpRetries > 0);

                if (IsTimeSet == false)
                    throw new Exception("NTP time update failed"); 
            }
        }
    }
}
