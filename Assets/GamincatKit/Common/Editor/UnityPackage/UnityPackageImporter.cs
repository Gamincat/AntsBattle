using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GamincatKit.Common;
using UnityEditor;
using UnityEngine;
using UnityEditor.PackageManager;

namespace GamincatKit.UnityPackage
{
    public static class UnityPackageImporter
    {
        private const string UnityPackageRoot = "../GamincatKit/UnityPackages";

        [MenuItem("GamincatKit/UnityPackage/Import GDPR")]
        private static async void ImportGDPRAsync()
        {
            await ImportWithUnityPackages("com.unity.localization", "GDPR");
        }

        [MenuItem("GamincatKit/UnityPackage/Adjust")]
        private static void ImportAdjustPackage()
        {
            Application.OpenURL("https://github.com/adjust/unity_sdk/releases");
        }

        [MenuItem("GamincatKit/UnityPackage/AppLovinMAX")]
        private static void ImportAppLovinMAXPackage()
        {
            Application.OpenURL("https://dash.applovin.com/documentation/mediation/unity/getting-started/integration");
        }

        [MenuItem("GamincatKit/UnityPackage/facebook")]
        private static void ImportFacebookPackage()
        {
            Application.OpenURL("https://github.com/facebook/facebook-sdk-for-unity/releases");
        }

        [MenuItem("GamincatKit/UnityPackage/ReviewDialog")]
        private static void ImportReviewDialog()
        {
            ImportPackage("ReviewDialog");
        }

        private static void ImportPackage(string folderName)
        {
            var packagePath = SelectUnityPackagePath(folderName);
            Log.Info($"Import UnityPackage : {packagePath}");
            AssetDatabase.ImportPackage(packagePath, false);
        }

        private static string SelectUnityPackagePath(string folderName)
        {
            var directory = $"{UnityPackageRoot}/{folderName}/";
            var unityPackagePath = Directory.EnumerateFiles(directory, "*.unitypackage").First();
            return unityPackagePath;
        }

        private static async Task ImportWithUnityPackages(string requirePackageDomain, string unityPackageName)
        {
            var isPackageInstalled = await IsPackageInstalled(requirePackageDomain);
            if (isPackageInstalled)
            {
                ImportPackage(unityPackageName);
                return;
            }

            var displayDialog = EditorUtility.DisplayDialog("Package Required",
                "unity localization is required. Do you want to add?",
                "OK", "NO");

            if (!displayDialog)
            {
                return;
            }

            EditorApplication.LockReloadAssemblies();
            Client.Add(requirePackageDomain);
            ImportPackage(unityPackageName);
            EditorApplication.UnlockReloadAssemblies();
        }

        private static async Task<bool> IsPackageInstalled(string packageName)
        {
            var request = Client.List();
            while (!request.IsCompleted)
            {
                await Task.Delay(100);
            }

            return request.Result.Any(p => p.name == packageName);
        }
    }
}