using System;
using GamincatKit;
using UnityEngine;

namespace GamincatKit.Common
{
    public static class PlayerPrefsSerializer
    {
        private static bool _hasChange;

        public static T GetData<T>(string key, T defaultValue = default)
        {
            var jsonString = PlayerPrefs.GetString(key);
            if (string.IsNullOrEmpty(jsonString))
            {
                return defaultValue;
            }

            T result;
            try
            {
                result = JsonUtility.FromJson<T>(jsonString);
            }
            catch (Exception e)
            {
                Log.Error($"PlayerPrefsSerializer GetData {e.Message}");
                result = defaultValue;
            }

            return result;
        }

        public static bool SetData<T>(string key, T data)
        {
            Exception exception = null;

            try
            {
                var jsonString = JsonUtility.ToJson(data);
                PlayerPrefs.SetString(key, jsonString);
            }
            catch (Exception e)
            {
                exception = e;
            }

            _hasChange = true;
            return exception == null;
        }

        private static readonly object LockObject = new object();

        public static void Save()
        {
            lock (LockObject)
            {
                if (_hasChange)
                {
                    PlayerPrefs.Save();
                    _hasChange = false;
                }
            }
        }
    }
}