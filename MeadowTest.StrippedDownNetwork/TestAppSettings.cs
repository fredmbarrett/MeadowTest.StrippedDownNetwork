using Meadow;
using System;
using System.Collections.Generic;
using System.Text;

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

        private Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();

        public TestAppSettings() { }

        public TestAppSettings(Dictionary<string, string> settings)
        {
            Settings = settings;
            foreach (var s in Settings)
            {
                Resolver.Log.Trace($"{s.Key} = {s.Value} [{s.Value.ToAsciiHex()}]");
            }

            DeviceName = ParseStringSetting("TestApp.DeviceName", "MeadowTest");

            WifiSsid = ParseStringSetting("TestApp.WifiSsid", "");
            WifiPassword = ParseStringSetting("TestApp.WifiPassword", "");
            WifiWakeUpDelaySeconds = ParseIntSetting("TestApp.WifiWakeUpDelaySeconds", 15);
            WifiMaxRetryCount = ParseIntSetting("TestApp.WifiMaxRetryCount", 3);
            WifiTimeoutSeconds = ParseIntSetting("TestApp.WifiTimeoutSeconds", 30);

            CellApnName = ParseStringSetting("TestApp.CellApnName", "");
            CellWakeUpDelaySeconds = ParseIntSetting("TestApp.CellWakeUpDelaySeconds", 15);
            CellTimeoutSeconds = ParseIntSetting("TestApp.CellTimeoutSeconds", 60);
            CellRetryDelaySeconds = ParseIntSetting("TestApp.CellRetryDelaySeconds", 900);
            CellMaxRetryCount = ParseIntSetting("TestApp.CellMaxRetryCount", 3);
        }

        private string ParseStringSetting(string hive, string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(hive)) { return defaultValue; }
            return Settings[hive] ?? defaultValue;
        }

        private string ParseStringSettingAlt(string hive, string defaultValue)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hive)) { return defaultValue; }
                string result = Settings[hive] ?? defaultValue;
                return result.Replace("\"", "");
            }
            catch
            {
                return defaultValue;
            }
        }

        private int ParseIntSetting(string passedValue, int? defaultValue = 0)
        {
            string value = ParseStringSetting(passedValue, defaultValue.ToString());
            return value.ToInt(defaultValue);
        }
    }
}

public static class SettingExtensions
{
    /// <summary>
    /// Converts the string to an INT32 if a valid int value.
    /// Returns either a default value or zero if string is missing not valid int.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static int ToInt(this string value, int? defaultValue)
    {
        if ((string.IsNullOrEmpty(value)) || (!Int32.TryParse(value, out int result)))
        {
            result = defaultValue ?? 0;
        }

        return result;
    }

    /// <summary>
    /// Converts string to ASCII hex for display
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToAsciiHex(this string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        byte[] asciiBytes = Encoding.ASCII.GetBytes(value);
        return BitConverter.ToString(asciiBytes);
    }
}