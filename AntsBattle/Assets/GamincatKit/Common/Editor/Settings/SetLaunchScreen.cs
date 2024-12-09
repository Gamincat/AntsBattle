using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;

namespace GamincatKit.Settings
{
    public static class SetLaunchScreen
    {
        private const string SplashResourceDirectory = "Assets/GamincatKit/Common/Editor/SplashScreen";

        [MenuItem("GamincatKit/Settings/SetLaunchScreen/White")]
        public static void SetWhiteLaunchScreen()
        {
            SetLaunchScreenByColor("White");
        }

        [MenuItem("GamincatKit/Settings/SetLaunchScreen/Black")]
        public static void SetBlackLaunchScreen()
        {
            SetLaunchScreenByColor("Black");
        }

        private static void SetLaunchScreenByColor(string colorDir)
        {
            CopyAndroidSplashResources(colorDir);
            CopyIOSSplashResources(colorDir);
            AssetDatabase.Refresh();

            var projectSettings = File.ReadAllText("ProjectSettings/ProjectSettings.asset");
            var guid = AssetDatabase.AssetPathToGUID("Assets/Editor/Android-SplashScreen.png");
            
            projectSettings = Regex.Replace(projectSettings, "m_ShowUnitySplashScreen:.+", "m_ShowUnitySplashScreen: 0");

            var androidSplashScreenSetting = $"androidSplashScreen: {{fileID: 2800000, guid: {guid}, type: 3}}";
            projectSettings = Regex.Replace(projectSettings, "androidSplashScreen:.+", androidSplashScreenSetting);
            projectSettings = Regex.Replace(projectSettings, "AndroidSplashScreenScale:.+",
                "AndroidSplashScreenScale: 2");

            var storyBoardIOS = $"Assets/GamincatKit/Editor/SplashScreen/{colorDir}/iOS/LaunchScreen-iPhone.storyboard";
            var storyBordIPad = $"Assets/GamincatKit/Editor/SplashScreen/{colorDir}/iOS/LaunchScreen-iPad.storyboard";

            projectSettings = Regex.Replace(projectSettings, "iOSLaunchScreenCustomStoryboardPath:.+",
                $"iOSLaunchScreenCustomStoryboardPath: {storyBoardIOS}");
            projectSettings = Regex.Replace(projectSettings, "iOSLaunchScreeniPadCustomStoryboardPath:.+",
                $"iOSLaunchScreeniPadCustomStoryboardPath: {storyBordIPad}");
            
            projectSettings = Regex.Replace(projectSettings, "iOSLaunchScreenType:.+", "iOSLaunchScreenType: 5");
            projectSettings = Regex.Replace(projectSettings, "iOSLaunchScreeniPadType:.+", "iOSLaunchScreeniPadType: 5");
            
            //not work but to use set dirty flag
            PlayerSettings.iOS.SetiPhoneLaunchScreenType(iOSLaunchScreenType.CustomStoryboard);
            PlayerSettings.iOS.SetiPadLaunchScreenType(iOSLaunchScreenType.CustomStoryboard);
            PlayerSettings.SplashScreen.show = false;

            File.WriteAllText("ProjectSettings/ProjectSettings.asset", projectSettings);
            AssetDatabase.Refresh();

        }

        private static void CopyAndroidSplashResources(string colorDir)
        {
            var androidImagePath = Path.Combine(Path.Combine(SplashResourceDirectory, colorDir),
                "Android/Android-SplashScreen.png");

            const string texturesDir = "Assets/Editor";
            if (!Directory.Exists(texturesDir)) Directory.CreateDirectory(texturesDir);

            var toAndroidImagePath = Path.Combine(texturesDir, "Android-SplashScreen.png");
            if (File.Exists(toAndroidImagePath)) File.Delete(toAndroidImagePath);

            File.Copy(androidImagePath, toAndroidImagePath);
        }

        private static void CopyIOSSplashResources(string colorDir)
        {
            var iosLogoPath = Path.Combine(Path.Combine(SplashResourceDirectory, colorDir),
                "iOS/iOS-SplashScreen-Logo.png");

            const string streamingAssetsDir = "Assets/StreamingAssets";
            if (!Directory.Exists(streamingAssetsDir)) Directory.CreateDirectory(streamingAssetsDir);

            var toIosLogoPath = Path.Combine(streamingAssetsDir, "iOS-SplashScreen-Logo.png");
            if (File.Exists(toIosLogoPath)) File.Delete(toIosLogoPath);

            File.Copy(iosLogoPath, toIosLogoPath);
        }

        private class ImportProcess : AssetPostprocessor
        {
            private void OnPreprocessTexture()
            {
                var texImporter = assetImporter as TextureImporter;
                if (texImporter.assetPath.EndsWith("/Android-SplashScreen.png"))
                    texImporter.textureType = TextureImporterType.Sprite;
            }
        }
    }
}