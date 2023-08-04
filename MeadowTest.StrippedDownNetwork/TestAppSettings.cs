using Meadow;
using System;
using System.Collections.Generic;

namespace MeadowTest.StrippedDownNetwork
{
    public class TestAppSettings
    {
        // device settings
        public string DeviceName { get; set; } = "MeadowTest";

        // network settings
        public string WifiSsid { get; set; }
        public string WifiPassword { get; set; }
        public int WifiWakeUpDelaySeconds { get; set; } = 15;
        public int WifiMaxRetryCount { get; set; } = 3;
        public int WifiTimeoutSeconds { get; set; } = 30;

        // cell settings
        public string CellApnName { get; set; } = "";
        public int CellWakeUpDelaySeconds { get; set; } = 15;
        public int CellTimeoutSeconds { get; set; } = 60;
        public int CellRetryDelaySeconds { get; set; } = 900;
        public int CellMaxRetryCount { get; set; } = 3;

        // ntp settings
        public int NtpTimeoutSeconds { get; set; } = 60;
        public int NtpMaxRetryCount { get; set; } = 5;

        public TestAppSettings() { }

        public TestAppSettings(Dictionary<string, string> settings)
        {
            foreach (var s in settings)
            {
                Resolver.Log.Trace($"{s.Key} = {s.Value}");
            }

            DeviceName = settings["TestApp.DeviceName"] ?? "MeadowTest";

            WifiSsid = settings["TestApp.WifiSsid"] ?? "";
            WifiPassword = settings["TestApp.WifiPassword"] ?? "";
            WifiWakeUpDelaySeconds = ParseIntSetting(settings["TestApp.WifiWakeUpDelaySeconds"], 15);
            WifiMaxRetryCount = ParseIntSetting(settings["TestApp.WifiMaxRetryCount"], 3);
            WifiTimeoutSeconds = ParseIntSetting(settings["TestApp.WifiTimeoutSeconds"], 30);

            CellApnName = settings["TestApp.CellApnName"] ?? "";
            CellWakeUpDelaySeconds = ParseIntSetting(settings["TestApp.CellWakeUpDelaySeconds"], 15);
            CellTimeoutSeconds = ParseIntSetting(settings["TestApp.CellTimeoutSeconds"], 60);
            CellRetryDelaySeconds = ParseIntSetting(settings["TestApp.CellRetryDelaySeconds"], 900);
            CellMaxRetryCount = ParseIntSetting(settings["TestApp.CellMaxRetryCount"], 3);
        }

        private int ParseIntSetting(string passedValue, int defaultValue)
        {
            Int32 result = -1;

            if ((string.IsNullOrEmpty(passedValue)) || (!Int32.TryParse(passedValue, out result)))
            {
                result = defaultValue;
            }

            return result;
        }
    }

}
