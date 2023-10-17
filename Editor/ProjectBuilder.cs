using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEditor.Build.Reporting;
#if UNITY_IOS
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.Build;
#endif
using UnityEngine;



public static class ProjectBuilder
{
    public enum ServerType
    {
        Dev,
        QA,
        Prod,
    }

    public enum StoreType
    {
        Android,
        Apple
    }

    public class BuildParam
    {
        public ServerType serverType;
        public StoreType storeType;
        public string bundleIdentifier;
        public string bundleVersion;
        public int bundleVersionCode;
        public bool enableaabBuild;
        public bool enableDebugLog;
        public bool enableCheat;
        public bool enableAdmobTest;
    }

    public static BuildParam GetBuildParamCommandLine()
    {
        BuildParam param = new BuildParam();

        param.serverType = (ServerType)Enum.Parse(typeof(ServerType), CommandLineReader.GetCustomArgument("server_type"));
        param.storeType = (StoreType)Enum.Parse(typeof(StoreType), CommandLineReader.GetCustomArgument("store_type"));
        param.bundleIdentifier = CommandLineReader.GetCustomArgument("bundle_identifier");
        param.bundleVersion = CommandLineReader.GetCustomArgument("bundle_version");
        param.bundleVersionCode = int.Parse(CommandLineReader.GetCustomArgument("bundle_version_code"));
        param.enableaabBuild = (CommandLineReader.GetCustomArgument("enable_aab_build") == "true");
        param.enableDebugLog = (CommandLineReader.GetCustomArgument("enable_debug_log") == "true");
        param.enableCheat = (CommandLineReader.GetCustomArgument("enable_cheat") == "true");
        param.enableAdmobTest = (CommandLineReader.GetCustomArgument("enable_AdmobTest") == "true");
        return param;
    }

    public static void PerformAndroidBuild()
    {
        string outputDir = Directory.GetCurrentDirectory().Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        BuildAndroid(outputDir, GetBuildParamCommandLine());
    }

#if UNITY_IOS

    public static void PerformIOSBuild()
    {
        string outputDir = Directory.GetCurrentDirectory().Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        BuildIOS(outputDir, GetBuildParamCommandLine());
    }
#endif
    public static void PerformAndroidAssetBundleBuild()
    {
        string outputDir = Directory.GetCurrentDirectory().Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        ////int version = int.Parse(CommandLineReader.GetCustomArgument("bundle_version"));
        //bool forceRebuildAssetBundle = (CommandLineReader.GetCustomArgument("force_rebuild_assetbundle") == "true");

        //AssetBundleBuilder builder = new AssetBundleBuilder();
        //builder.Build(outputDir, BuildTarget.Android, forceRebuildAssetBundle);
    }

    public static void PerformIOSAssetBundleBuild()
    {
        string outputDir = Directory.GetCurrentDirectory().Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        ////int version = int.Parse(CommandLineReader.GetCustomArgument("bundle_version"));
        //bool forceRebuildAssetBundle = (CommandLineReader.GetCustomArgument("force_rebuild_assetbundle") == "true");

        //AssetBundleBuilder builder = new AssetBundleBuilder();
        //builder.Build(outputDir, BuildTarget.iOS, forceRebuildAssetBundle);
    }

    private static int StartProcessWaitingForExit(string fileName, string arguments)
    {
        UnityEngine.Debug.Log("filename=" + fileName + ", arguments=" + arguments);

        int exitCode = 0;
        using (Process process = Process.Start(new ProcessStartInfo(fileName, arguments)))
        {
            process.WaitForExit();

            exitCode = process.ExitCode;
        }

        return exitCode;
    }

    public static void BuildAndroid(string outputDir, BuildParam param, BuildOptions options = BuildOptions.None)
    {
        if(string.IsNullOrEmpty(outputDir))
        {
            return;
        }

        string originalDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
        const int NONE = 0;
        const int RELEASE = 1;
        const int ALIGNED = 2;
        const int SIGNED = 3;
        int progressState = NONE;

        Func<string> GetResultState = () =>
        {
            switch (progressState)
            {
                case NONE:
                    return "NONE";
                case RELEASE:
                    return "RELEASE";
                case ALIGNED:
                    return "RELEASE";
                case SIGNED:
                    return "SIGNED";
            }

            return string.Empty;
        };

        try
        {
            EditorUtility.DisplayProgressBar("Preparing to build", "준비 중...", 0.1f);
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
            EditorUserBuildSettings.buildAppBundle = param.enableaabBuild;

            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android,
                MakeDefineSymbols(param.enableDebugLog, param.enableCheat, param.enableAdmobTest));
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, param.bundleIdentifier);

            PlayerSettings.bundleVersion = param.bundleVersion;
            PlayerSettings.Android.bundleVersionCode = param.bundleVersionCode;

            //* signing
            string keystoreFilePath = "Keystore/cinamongames.keystore";
            string keystorePasswordFilePath = "Keystore/keystorePw.pwd";
            string keyAliasFilePath = "Keystore/key.alias";
            string keyAliasPasswordFilePath = "Keystore/key.pwd";

            PlayerSettings.Android.keystoreName = Directory.GetCurrentDirectory() + "/" + keystoreFilePath;
            PlayerSettings.Android.keystorePass = File.ReadAllText(Directory.GetCurrentDirectory() + "/" + keystorePasswordFilePath);
            PlayerSettings.Android.keyaliasName = File.ReadAllText(Directory.GetCurrentDirectory() + "/" + keyAliasFilePath);
            PlayerSettings.Android.keyaliasPass = File.ReadAllText(Directory.GetCurrentDirectory() + "/" + keyAliasPasswordFilePath);
            //*/
            AssetDatabase.Refresh();

            if ((options & BuildOptions.CompressWithLz4HC) != BuildOptions.CompressWithLz4HC)
            {
                options |= BuildOptions.CompressWithLz4HC;
            }

            // refresh and build
            AssetDatabase.Refresh();

            string buildPath = outputDir + "/android";
            string outputPath = buildPath + "/release";

            if (EditorUserBuildSettings.buildAppBundle == false)
            {              
                string releaseFilePath = outputPath + "/APK/release.apk";

                BuildReport br = BuildPipeline.BuildPlayer(GetEnabledScenes(), releaseFilePath, BuildTarget.Android, options);
                UnityEngine.Debug.Log("build result=" + br.summary.result);

                if (br.summary.result == BuildResult.Succeeded)
                {
                    progressState = RELEASE;

                    EditorUtility.DisplayProgressBar("Build", "Align 적용 중...", 0.1f);
                    string alignedFilePath = string.Format("{0}/release_aligned.apk", outputPath);
                    if (File.Exists(alignedFilePath))
                        File.Delete(alignedFilePath);
                    progressState = ALIGNED;

                    string sdkRoot = "C:/Program Files/Unity/Hub/Editor/2021.3.28f1/Editor/Data/PlaybackEngines/AndroidPlayer/SDK/build-tools/30.0.3";
                    int exitCode = StartProcessWaitingForExit(sdkRoot + "/zipalign", string.Format("-v -p 4 {0} {1}", releaseFilePath, alignedFilePath));
                    UnityEngine.Debug.Log("align exitCode=" + exitCode);

                    // apksigner
                    string APKSIGNER_PATH = sdkRoot + "/apksigner";
                    exitCode = StartProcessWaitingForExit(APKSIGNER_PATH,
                    string.Format("sign -v --in {0} --ks {1} --ks-pass file:{2} --key-pass file:{3} --ks-key-alias {4}",
                        alignedFilePath,
                        Directory.GetCurrentDirectory() + "/" + keystoreFilePath,
                        Directory.GetCurrentDirectory() + "/" + keystorePasswordFilePath,
                        Directory.GetCurrentDirectory() + "/" + keyAliasPasswordFilePath,
                        PlayerSettings.Android.keyaliasName));
                    UnityEngine.Debug.Log("build apks exitCode=" + exitCode);
                    progressState = SIGNED;
                }
                else if (br.summary.result == BuildResult.Cancelled)
                {
                    RestoreProject();
                }
            }
            else
            {
                BuildPlayerOptions buildPlayerOptions = default(BuildPlayerOptions);
                buildPlayerOptions.scenes = GetEnabledScenes();
                buildPlayerOptions.locationPathName = outputPath + "/AAB/release.aab";
                buildPlayerOptions.target = BuildTarget.Android;
                buildPlayerOptions.options = options;

                BuildReport br = BuildPipeline.BuildPlayer(buildPlayerOptions);

                if (br.summary.result == BuildResult.Cancelled)
                {
                    RestoreProject();
                }
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log("Exception=" + e.ToString());
        }
        finally
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, originalDefineSymbols);
            PlayerSettings.Android.keystoreName = "";
            PlayerSettings.Android.keyaliasName = "";
            PlayerSettings.Android.keyaliasPass = "";
            PlayerSettings.Android.keystorePass = "";
            EditorUtility.ClearProgressBar();
        }
    }
#if UNITY_IOS

    public static void BuildIOS(string outputDir, BuildParam param, BuildOptions options = BuildOptions.AcceptExternalModificationsToPlayer)
    {
        string projectPath = outputDir;

        if ((options & BuildOptions.AcceptExternalModificationsToPlayer) == BuildOptions.AcceptExternalModificationsToPlayer)
        {
            switch (BuildPipeline.BuildCanBeAppended(BuildTarget.iOS, projectPath))
            {
                case CanAppendBuild.Yes:
                    break;
                case CanAppendBuild.Unsupported:
                    {
                        UnityEngine.Debug.LogError("The build target does not support build appending.");
                        options ^= BuildOptions.AcceptExternalModificationsToPlayer;
                    }
                    break;
                case CanAppendBuild.No:
                    {
                        UnityEngine.Debug.LogError("The build cannot be appended.");
                        options ^= BuildOptions.AcceptExternalModificationsToPlayer;
                    }
                    break;
                default:
                    {
                        UnityEngine.Debug.LogError("case 문이 없다?? 업데이트 되었나?");
                    }
                    break;
            }

        }

        if ((options & BuildOptions.CompressWithLz4HC) != BuildOptions.CompressWithLz4HC)
        {
            options |= BuildOptions.CompressWithLz4HC;
        }

        string originalDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);

        try
        {
            EditorUtility.DisplayProgressBar("Preparing to build", "준비 중...", 0.1f);
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);

            //* settings
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS,
                MakeDefineSymbols(param.enableDebugLog, param.enableCheat, param.enableAdmobTest));
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, param.bundleIdentifier);

            PlayerSettings.bundleVersion = param.bundleVersion;
            PlayerSettings.iOS.buildNumber = param.bundleVersionCode.ToString();
            PlayerSettings.iOS.targetOSVersionString = "12.0";
            //*/
            AssetDatabase.Refresh();
            // refresh and build
            AssetDatabase.Refresh();


            BuildReport br = BuildPipeline.BuildPlayer(GetEnabledScenes(), projectPath, BuildTarget.iOS, options);
            UnityEngine.Debug.Log("build result=" + br.summary.result);
            if (br.summary.result == BuildResult.Cancelled)
            {
                RestoreProject();
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e.ToString());
        }
        finally
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, originalDefineSymbols);
            EditorUtility.ClearProgressBar();
        }
    }
#endif
    public static void RestoreProject()
    {
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

#if UNITY_IOS
    public class BuildProcessors : IPostprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }

        public void OnPostprocessBuild(BuildReport report)
        {
            UnityEngine.Debug.Log($"OnPostprocessBuild() BuildReport target = {report.summary.platform} , path= {report.summary.outputPath}");

            if (report.summary.platform == BuildTarget.iOS)
            {
                var pathToBuiltProject = report.summary.outputPath;
                var projectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
                var project = new PBXProject();
                project.ReadFromString(File.ReadAllText(projectPath));

                string targetGuid = project.GetUnityMainTargetGuid();

                project.AddFrameworkToProject(targetGuid, "UnityFramework.framework", false);
                project.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");

                project.WriteToFile(projectPath);
            }
        }
    }

    [PostProcessBuildAttribute(1)]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        UnityEngine.Debug.Log("OnPostProcessBuild() target=" + target + " path=" + path);
        if (target != BuildTarget.iOS)
        {
            return;
        }

        // project
        string projectPath = PBXProject.GetPBXProjectPath(path);
        PBXProject project = new PBXProject();
        project.ReadFromFile(projectPath);
        string targetGuid = project.GetUnityMainTargetGuid();
        project.AddFrameworkToProject(targetGuid, "GameKit.framework", false);
        project.AddFrameworkToProject(targetGuid, "StoreKit.framework", false);
        project.AddFrameworkToProject(targetGuid, "CoreServices.framework", false);
        project.AddFrameworkToProject(targetGuid, "ImageIO.framework", false);
        project.AddFrameworkToProject(targetGuid, "ReplayKit.framework", false);
        project.AddFrameworkToProject(targetGuid, "AuthenticationServices.framework", false);
        project.AddFrameworkToProject(targetGuid, "AdServices.framework", false);
        project.AddFrameworkToProject(targetGuid, "AppTrackingTransparency.framework", true);   //ios 14 미만에서는 앱트래킹 프레임워크 X

        project.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
        project.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-ObjC");
        project.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-lstdc++");
        project.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-lz");
        File.WriteAllText(projectPath, project.WriteToString());

        var manager = new ProjectCapabilityManager(projectPath, "Entitlements.entitlements", null, targetGuid);
        manager.AddSignInWithApple();
        manager.AddGameCenter();
        manager.AddInAppPurchase();
        manager.AddPushNotifications(false);
        manager.WriteToFile();

        // plist
        string plistPath = path + "/Info.plist";
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);
        PlistElementDict rootDict = plist.root;

        rootDict.SetString("NSUserTrackingUsageDescription", "광고 트래킹");
        rootDict.SetString("GADApplicationIdentifier", "ca-app-pub-4325556403049642~2627953448");    // 구글 애드몹 광고ID
        rootDict.SetBoolean("GADIsAdManagerApp", true);    // 구글 애드몹

        // Background modes
        PlistElementArray backgroundModes = rootDict.CreateArray("UIBackgroundModes");
        AddUniqueString(backgroundModes, "fetch");
        AddUniqueString(backgroundModes, "remote-notification");
        // Required device capabilities
        PlistElementArray requiredDeviceCapabilities = GetOrCreateArray(rootDict, "UIRequiredDeviceCapabilities");
        AddUniqueString(requiredDeviceCapabilities, "gamekit");
        // Write to file
        File.WriteAllText(plistPath, plist.WriteToString());
    }

    private static void CopyFileAndAdd(PBXProject project, string targetGuid, string filePath, string projectPath)
    {
        UnityEngine.Debug.Log("filePath=" + filePath);
        if (File.Exists(filePath))
        {
            string fileName = Path.GetFileName(filePath);
            UnityEngine.Debug.Log("fileName=" + fileName);
            string copyFilePath = projectPath + "/" + fileName;
            UnityEngine.Debug.Log("copyFilePath=" + copyFilePath);
            File.Delete(copyFilePath);
            File.Copy(filePath, copyFilePath);
            project.AddFileToBuild(targetGuid, project.AddFile(copyFilePath, Path.GetFileName(fileName)));
        }
    }


    private static PlistElementArray GetOrCreateArray(PlistElementDict root, string key)
    {
        PlistElement element;
        if (root.values.TryGetValue(key, out element))
        {
            return element.AsArray();
        }

        return root.CreateArray(key);
    }


    private static void AddUniqueString(PlistElementArray elementArray, string value)
    {
        elementArray.values.RemoveAll(delegate (PlistElement element)
        {
            return element.AsString() == value;
        });

        elementArray.AddString(value);
    }
#endif


    private static string MakeDefineSymbols(bool enableDebugLog, bool enableCheat, bool enableAdmobTest)
    {
        List<string> scriptDefineSymbols = new List<string>();

        if (enableDebugLog == false)
        {
            scriptDefineSymbols.Add("BUILD_MODE");
        }

        if (enableCheat)
        {
            scriptDefineSymbols.Add("ENABLE_CHEAT");
        }

        if (enableAdmobTest)
        {
            scriptDefineSymbols.Add("ADMOB_TEST");
        }

        scriptDefineSymbols.Add("UNITY_POST_PROCESSING_STACK_V2");

        StringBuilder sb = new StringBuilder();
        foreach (string symbols in scriptDefineSymbols)
        {
            if (!string.IsNullOrEmpty(symbols))
            {
                if (sb.Length > 0)
                {
                    sb.Append(";" + symbols);
                }
                else
                {
                    sb.Append(symbols);
                }
            }
        }

        return sb.ToString();
    }


    private static string GetCurrentDirectory()
    {
        return Directory.GetCurrentDirectory().Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }


    private static void MoveFileAndDirectory(string path, string to)
    {
        DirectoryInfo di = new DirectoryInfo(path);
        foreach (DirectoryInfo info in di.GetDirectories())
        {
            info.MoveTo(to + "/" + info.Name);
        }

        foreach (FileInfo info in di.GetFiles())
        {
            info.MoveTo(to + "/" + info.Name);
        }
    }


    private static void DeleteFileAndDirectory(string path)
    {
        DirectoryInfo di = new DirectoryInfo(path);
        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete();
        }

        foreach (DirectoryInfo dir in di.GetDirectories())
        {
            dir.Delete(true);
        }
    }

    private static string[] GetEnabledScenes()
    {
        List<string> sceneList = new List<string>();

        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                sceneList.Add(scene.path);
            }
        }

        return sceneList.ToArray();
    }


    private static string GetAndroidBuildToolsPath()
    {
        string androidSdkRoot = EditorPrefs.GetString("AndroidSdkRoot");

        if (!string.IsNullOrEmpty(androidSdkRoot))
        {
            List<string> directoires = Directory.GetDirectories(androidSdkRoot + "/build-tools")
                .Select(x => x.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
                .ToList();

            if (directoires.Count > 0)
            {
                directoires.Sort(delegate (string a, string b)
                {
                    System.Version va = new System.Version(Path.GetFileName(a));
                    System.Version vb = new System.Version(Path.GetFileName(b));

                    return vb.CompareTo(va);
                });

                foreach (string dir in directoires)
                {
                    UnityEngine.Debug.Log(dir);
                }

                return directoires[0];
            }
        }

        return "";
    }




    #region 안드로이드 심볼 재압축
    public static void AndroidSymbolRecompression(string path)
    {
        if (string.IsNullOrEmpty(path))
            return;

        var targetDirectory = Path.GetDirectoryName(path);
        var intermediatePath = Path.Combine(targetDirectory, "TempShrink");
        var newZip = Path.Combine(targetDirectory, Path.GetFileNameWithoutExtension(path) + ".ReCompress.zip");

        var zipFileName = Path.GetFullPath(Path.Combine(EditorApplication.applicationContentsPath, "Tools", "7z.exe"));

        if (!File.Exists(zipFileName))
        {
            throw new Exception($"Failed to locate {zipFileName}");
        }

        Cleanup(intermediatePath);
        Cleanup(newZip);

        var result = RunProcess(targetDirectory, zipFileName, $"x -o\"{intermediatePath}\" \"{path}\"");
        if (result.ExitCode != 0)
        {
            throw new Exception(result.ToString());
        }

        EditorUtility.DisplayProgressBar("심볼 제작중", "심볼 재압축 진행중...", 0.5f);

        var files = Directory.GetFiles(intermediatePath, "*.*", SearchOption.AllDirectories);

        var symSo = ".sym.so";

        foreach (var file in files)
        {
            if (file.EndsWith(".dbg.so"))
            {
                Cleanup(file);
            }

            if (file.EndsWith(symSo))
            {
                var fileSO = file.Substring(0, file.Length - symSo.Length) + ".so";
                UnityEngine.Debug.Log($"Rename {file} --> {fileSO}");
                File.Move(file, fileSO);
            }
        }

        //2020.3에서 심볼 생성 하면 해당 파일들을 가져오지 않아서 복사해서 넣어줌
        string symbolPath = Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines/AndroidPlayer/Variations/il2cpp/Release/Symbols");
        string[] type = { "arm64-v8a" };
        string[] soFile = { "libmain.sym.so", "libunity.sym.so" };
        string[] copySoFile = { "libmain.so", "libunity.so" };

        for (int i = 0; i < type.Length; i++)
        {
            for (int j = 0; j < soFile.Length; j++)
            {
                string _originPath = Path.Combine(symbolPath, type[i], soFile[j]);
                string _copyPath = Path.Combine(intermediatePath, type[i], copySoFile[j]);

                File.Copy(_originPath, _copyPath);
            }
        }

        result = RunProcess(intermediatePath, zipFileName, $"a -tzip \"{newZip}\"");

        EditorUtility.ClearProgressBar();
        if (result.ExitCode != 0)
        {
            throw new Exception(result.ToString());
        }

        Cleanup(intermediatePath);

        UnityEngine.Debug.Log($"New small symbol package: {newZip}");
        EditorUtility.RevealInFinder(newZip);
    }

    private static (int ExitCode, string output, string error) RunProcess(string workingDirectory, string fileName, string args)
    {
        UnityEngine.Debug.Log($"Executing {fileName} {args} (Working Directory: {workingDirectory}");
        Process process = new Process();
        process.StartInfo.FileName = fileName;
        process.StartInfo.Arguments = args;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.WorkingDirectory = workingDirectory;
        process.StartInfo.CreateNoWindow = true;
        var output = new StringBuilder();
        process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                output.AppendLine(e.Data);
            }
        });

        var error = new StringBuilder();
        process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                error.AppendLine(e.Data);
            }
        });

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        UnityEngine.Debug.Log($"{fileName} exited with {process.ExitCode}");

        return (process.ExitCode, output.ToString(), error.ToString());
    }

    private static void Cleanup(string path)
    {
        if (Directory.Exists(path))
        {
            UnityEngine.Debug.Log($"Delete {path}");
            Directory.Delete(path, true);
        }
        if (File.Exists(path))
        {
            UnityEngine.Debug.Log($"Delete {path}");
            File.Delete(path);
        }
    }

    #endregion
}