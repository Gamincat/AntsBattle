using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using GamincatKit.Common;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace GamincatKit.Options
{
    public static class AndroidKeystore
    {
        private const string KeystoreFile = "";
        private const string KeyaliasName = "";
        private const string KeystorePass = "";
        private const string KeyaliasPass = "";

        private static string ApplicationIdentifier =>
            PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);

        [MenuItem("GamincatKit/Options/AndroidKeystore/Create Keystore")]
        public static void CreateKeystore()
        {
            var keystoreFile = GetKeystoreFile();
            var keyaliasName = GetKeyaliasName();
            var keystorePass = GetKeyStorePass();
            var keyaliasPass = GetKeyAliasPass();

            if (File.Exists(keystoreFile))
            {
                Log.Error("Keystore file is already exist. : " + keystoreFile);
                return;
            }

            var keytoolArgFormat =
                "-genkey -v -keystore {0} -keyalg RSA -keysize 2048 -validity 10000 -alias {1} -storepass \"{2}\" -keypass \"{3}\" -dname \"CN=Gamincat, OU=Gamincat, O=Gamincat, L=Unknown, S=Unknown, C=JP\"";
            var keytoolArg = string.Format(keytoolArgFormat, keystoreFile, keyaliasName, keystorePass, keyaliasPass);

            var p = new Process();
            p.StartInfo.FileName = "/usr/bin/keytool";
            p.StartInfo.Arguments = keytoolArg;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.Start();
            p.WaitForExit();
            p.Close();
            Log.Info(string.Format("Created keytool : {0}", keystoreFile));
        }

        [MenuItem("GamincatKit/Options/AndroidKeystore/Set Keystore")]
        public static void SetKeystore()
        {
            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = GetKeystoreFile();
            PlayerSettings.Android.keyaliasName = GetKeyaliasName();
            PlayerSettings.Android.keystorePass = GetKeyStorePass();
            PlayerSettings.Android.keyaliasPass = GetKeyAliasPass();
        }


        private static string GetKeystoreFile()
        {
            if (!string.IsNullOrEmpty(KeystoreFile)) return KeystoreFile;
            var tokens = ApplicationIdentifier.Split('.');
            var basename = tokens[^1];
            return basename.ToLower() + ".keystore";
        }

        private static string GetKeyaliasName()
        {
            if (!string.IsNullOrEmpty(KeyaliasName)) return KeyaliasName;
            var tokens = ApplicationIdentifier.Split('.');
            var basename = tokens[^1];
            return basename.ToLower();
        }

        private static string GetKeyStorePass()
        {
            if (!string.IsNullOrEmpty(KeystorePass)) return KeystorePass;
            return CreatePassword();
        }

        private static string GetKeyAliasPass()
        {
            if (!string.IsNullOrEmpty(KeyaliasPass)) return KeyaliasPass;
            return CreatePassword();
        }

        private static string CreatePassword()
        {
            var baseStr = ApplicationIdentifier + "#MaGiCaNt$gAmE=";
            HashAlgorithm hashAlgorithm = SHA1.Create();
            var srcBytes = Encoding.UTF8.GetBytes(baseStr);
            var destBytes = hashAlgorithm.ComputeHash(srcBytes);
            return Convert.ToBase64String(destBytes);
        }


#if UNITY_ANDROID
        public class AndroidBuildPreProcessorKeystore : IPreprocessBuildWithReport
        {
            public int callbackOrder => 0;

            public void OnPreprocessBuild(BuildReport report)
            {
                SetKeystore();
            }
        }
#endif
    }
}