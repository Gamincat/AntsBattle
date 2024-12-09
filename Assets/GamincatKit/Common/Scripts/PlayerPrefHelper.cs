using UnityEngine;

namespace GamincatKit.Common
{
    public static class PlayerPrefHelper
    {
        public static bool GetBool(string key, bool defaultValue)
        {
            var value = PlayerPrefs.GetInt(key, defaultValue ? 1 : 0);
            return value == 1;
        }

        public static void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }
    }
}