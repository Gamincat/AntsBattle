#if UNITY_IOS

using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace GamincatKit
{
    public class NativeUtilsPostProcessBuild
    {
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
        {
            var projPath = Path.Combine(path, "Unity-iPhone.xcodeproj/project.pbxproj");
            var proj = new PBXProject();
            proj.ReadFromString(File.ReadAllText(projPath));
            var target = proj.GetUnityFrameworkTargetGuid();
            proj.AddFrameworkToProject(target, "SafariServices.framework", false);
            proj.AddFrameworkToProject(target, "StoreKit.framework", false);

            File.WriteAllText(projPath, proj.WriteToString());
        }
    }
}

#endif