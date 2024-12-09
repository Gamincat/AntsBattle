#if UNITY_ANDROID
// ReSharper disable InconsistentNaming

using System;
using UnityEngine;

namespace GamincatKit.Native
{
    public class NativeUtilsPlugin_Android : NativeUtilsPlugin
    {
        private static AndroidJavaObject jCurrentActivity =>
            new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");

        private static int apiLevel => new AndroidJavaClass("android.os.Build$VERSION").GetStatic<int>("SDK_INT");


        public string GetDataDirectory()
        {
            var filesDir = jCurrentActivity.Call<AndroidJavaObject>("getFilesDir");
            return filesDir.Call<string>("getCanonicalPath");
        }

        public string GetCacheDirectory()
        {
            var filesDir = jCurrentActivity.Call<AndroidJavaObject>("getCacheDir");
            return filesDir.Call<string>("getCanonicalPath");
        }

        public string GetVersion()
        {
            var packageManager = jCurrentActivity.Call<AndroidJavaObject>("getPackageManager");
            var packageInfo = packageManager.Call<AndroidJavaObject>("getPackageInfo",
                jCurrentActivity.Call<string>("getPackageName"), 0);
            return packageInfo.Get<string>("versionName");
        }

        public string GetBuildNumber()
        {
            var packageManager = jCurrentActivity.Call<AndroidJavaObject>("getPackageManager");
            var packageInfo = packageManager.Call<AndroidJavaObject>("getPackageInfo",
                jCurrentActivity.Call<string>("getPackageName"), 0);
            var versionCode = packageInfo.Get<int>("versionCode");
            return versionCode.ToString();
        }

        public float GetWidth()
        {
            var metrics = getMetrics();
            var width = metrics.Get<int>("widthPixels");
            var scaledDensity = metrics.Get<float>("scaledDensity");
            return width / scaledDensity;
        }

        public float GetHeight()
        {
            var metrics = getMetrics();
            var height = metrics.Get<int>("heightPixels");
            var scaledDensity = metrics.Get<float>("scaledDensity");
            return height / scaledDensity;
        }

        public float GetScale()
        {
            var metrics = getMetrics();
            return metrics.Get<float>("scaledDensity");
        }

        public float GetSafeAreaTop()
        {
            return 0;
        }

        public float GetSafeAreaBottom()
        {
            return 0;
        }

        public void ShowWebView(string url)
        {
            Application.OpenURL(url);
        }

        public string GetLanguage()
        {
            var locale = new AndroidJavaClass("java.util.Locale");
            var def = locale.CallStatic<AndroidJavaObject>("getDefault");
            return def.Call<string>("getLanguage");
        }

        public void OpenReview(string appId)
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id=" + appId);
        }

        public void OpenReviewDialog(string appId, string title, string message, string okButton = "OK",
            string cancelButton = "Cancel", Action<bool> callback = null)
        {
            var activity =
                new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            var alertDialog = new AndroidJavaObject("android.app.AlertDialog$Builder", activity);
            alertDialog.Call<AndroidJavaObject>("setTitle", title);
            alertDialog.Call<AndroidJavaObject>("setMessage", message);
            alertDialog.Call<AndroidJavaObject>("setPositiveButton", okButton, new AndroidOnClickListener(which =>
            {
                Application.OpenURL("https://play.google.com/store/apps/details?id=" + appId);
                if (callback != null) callback.Invoke(true);
            }));
            alertDialog.Call<AndroidJavaObject>("setNegativeButton", cancelButton, new AndroidOnClickListener(which =>
            {
                if (callback != null) callback.Invoke(false);
            }));
            alertDialog.Call<AndroidJavaObject>("show");
        }

        public void Alert(string title, string message)
        {
            var activity =
                new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            var alertDialog = new AndroidJavaObject("android.app.AlertDialog$Builder", activity);
            alertDialog.Call<AndroidJavaObject>("setTitle", title);
            alertDialog.Call<AndroidJavaObject>("setMessage", message);
            alertDialog.Call<AndroidJavaObject>("setPositiveButton", "OK", null);
            alertDialog.Call<AndroidJavaObject>("show");
        }

        public bool IsHapticFeedbackSupported()
        {
            return true;
        }

        public void HapticFeedback(NativeUtils.FeedbackType type)
        {
            switch (type)
            {
                case NativeUtils.FeedbackType.ImpactLight:
                    hapticFeedbackImpact(1);
                    break;

                case NativeUtils.FeedbackType.ImpactMedium:
                    hapticFeedbackImpact(5);
                    break;

                case NativeUtils.FeedbackType.ImpactHeavy:
                    hapticFeedbackImpact(10);
                    break;

                case NativeUtils.FeedbackType.NotificationSuccess:
                    hapticFeedbackWave(new long[] { 0, 5, 100, 5 });
                    break;

                case NativeUtils.FeedbackType.NotificationWarning:
                    hapticFeedbackWave(new long[] { 0, 5, 200, 5 });
                    break;

                case NativeUtils.FeedbackType.NotificationError:
                    hapticFeedbackWave(new long[] { 0, 5, 50, 5, 50, 5 });
                    break;

                case NativeUtils.FeedbackType.Selection:
                    hapticFeedbackImpact(1);
                    break;
            }
        }

        private AndroidJavaObject getMetrics()
        {
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var windowManager = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")
                .Call<AndroidJavaObject>("getWindowManager");
            var display = windowManager.Call<AndroidJavaObject>("getDefaultDisplay");
            var metrics = new AndroidJavaObject("android.util.DisplayMetrics");

            if (apiLevel >= 17)
                display.Call("getRealMetrics", metrics);
            else
                display.Call("getMetrics", metrics);

            return metrics;
        }

        private void hapticFeedbackImpact(long time)
        {
            var jVibratorService =
                new AndroidJavaClass("android.content.Context").GetStatic<string>("VIBRATOR_SERVICE");
            var jVibrator = jCurrentActivity.Call<AndroidJavaObject>("getSystemService", jVibratorService);

            if (apiLevel >= 26)
            {
                var jEffectCls = new AndroidJavaClass("android.os.VibrationEffect");
                var jEffect = jEffectCls.CallStatic<AndroidJavaObject>("createOneShot", time,
                    jEffectCls.GetStatic<int>("DEFAULT_AMPLITUDE"));
                jVibrator.Call("vibrate", jEffect);
            }
            else
            {
                jVibrator.Call("vibrate", time);
            }
        }

        private void hapticFeedbackWave(long[] times)
        {
            var jVibratorService =
                new AndroidJavaClass("android.content.Context").GetStatic<string>("VIBRATOR_SERVICE");
            var jVibrator = jCurrentActivity.Call<AndroidJavaObject>("getSystemService", jVibratorService);

            if (apiLevel >= 26)
            {
                var jEffectCls = new AndroidJavaClass("android.os.VibrationEffect");
                var jTimes = AndroidJNI.ToLongArray(times);
                var jParams1 = new jvalue[2];
                jParams1[0].l = jTimes;
                jParams1[1].i = -1;
                var jmidCreateWaveForm = AndroidJNIHelper.GetMethodID(jEffectCls.GetRawClass(), "createWaveform",
                    "([JI)Landroid/os/VibrationEffect;", true);
                var jEffect = AndroidJNI.CallStaticObjectMethod(jEffectCls.GetRawClass(), jmidCreateWaveForm, jParams1);

                var jmidVibrate = AndroidJNIHelper.GetMethodID(jVibrator.GetRawClass(), "vibrate",
                    "(Landroid/os/VibrationEffect;)V");
                var jParams2 = new jvalue[2];
                jParams2[0].l = jEffect;
                AndroidJNI.CallVoidMethod(jVibrator.GetRawObject(), jmidVibrate, jParams2);
            }
            else
            {
                jVibrator.Call("vibrate", times);
            }
        }

        private class AndroidOnClickListener : AndroidJavaProxy
        {
            private readonly Action<int> action;

            public AndroidOnClickListener(Action<int> action) : base("android.content.DialogInterface$OnClickListener")
            {
                this.action = action;
            }

            public void onClick(AndroidJavaObject dialog, int which)
            {
                action.Invoke(which);
            }
        }
    }
}

#endif