using System.Diagnostics;
using System.IO;
using GamincatKit.Common;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace GamincatKit.Build
{
    public class ExecutePreprocessShellScript : IPreprocessBuildWithReport
    {
        private const string ShellScriptFileName = "./jenkins-shell/preprocess_build.sh";

        public int callbackOrder => -1;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (!File.Exists(ShellScriptFileName))
            {
                Log.Verbose("Skip ExecutePreprocessShellScript preprocess_build.sh not exist");
                return;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = "/bin/zsh",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                Arguments = ShellScriptFileName
            };

            var process = Process.Start(startInfo);
            process.Start();
            process.WaitForExit();
            Log.Info(process.StandardOutput.ReadToEnd());
            process.Close();
        }
    }
}