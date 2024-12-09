using System;
using UnityEditor;

namespace GamincatKit.Settings
{
    public static class InitSettings
    {
        [MenuItem("GamincatKit/Settings/InitPlayerSettings")]
        public static void SetPlayerSettings()
        {
            PlayerSettings.companyName = "Gamincat";

            PlayerSettings.iOS.appleDeveloperTeamID = "MV944KT443";
            PlayerSettings.iOS.targetOSVersionString = "11.0";
            PlayerSettings.iOS.appleEnableAutomaticSigning = true;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);

            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;

            string[] sdkVersionEnums = { "AndroidApiLevel33", "33" };
            var sdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            foreach (var verStr in sdkVersionEnums)
                if (Enum.TryParse(verStr, out sdkVersion))
                    break;

            PlayerSettings.Android.targetSdkVersion = sdkVersion;
            PlayerSettings.Android.minifyWithR8 = false;
            PlayerSettings.Android.minifyDebug = false;
            PlayerSettings.Android.minifyRelease = false;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        }
    }
}