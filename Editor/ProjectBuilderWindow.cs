using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;


public class ProjectBuilderWindow : EditorWindow
{
    private string outputDir;
    ProjectBuilder.BuildParam param;

    private string _bundleVersionCode = "1";


    [MenuItem("Tools/ProjectBuilder", priority = 10000)]
    public static void OpenProjectBuilderWindow()
    {
        ProjectBuilderWindow pWindow = (ProjectBuilderWindow)GetWindow(typeof(ProjectBuilderWindow), false);

        pWindow.minSize = new Vector2(600, 200);
        pWindow.Show();
    }

    private void OnEnable()
    {
        outputDir = PlayerPrefs.GetString("buildOutputPath");

        param = new ProjectBuilder.BuildParam();       

        param.bundleIdentifier = Application.identifier;
        param.bundleVersion = Application.version;

#if UNITY_ANDROID
        param.bundleVersionCode = PlayerSettings.Android.bundleVersionCode;
        param.enableaabBuild = false;
#elif UNITY_IOS
        param.bundleVersionCode = int.Parse(PlayerSettings.iOS.buildNumber);
#endif

    }


    private void OnGUI()
    {
        GUILayout.BeginVertical();
        {
            EditorGUILayout.LabelField("빌드 아웃풋 경로", outputDir);

            if (GUILayout.Button($"아웃풋 폴더 설정", GUILayout.Width(150f)))
            {
                string _path = "";

                _path = EditorUtility.OpenFolderPanel("아웃풋 폴더", "", "");

                outputDir = _path;

                PlayerPrefs.SetString("buildOutputPath", outputDir);
            }

            param.serverType = (ProjectBuilder.ServerType)EditorGUILayout.EnumPopup("서버", param.serverType);
            param.storeType = (ProjectBuilder.StoreType)EditorGUILayout.EnumPopup("OS", param.storeType);
            param.bundleIdentifier = EditorGUILayout.TextField("빌드 고유명", param.bundleIdentifier);
            param.bundleVersion = EditorGUILayout.TextField("빌드 버전", param.bundleVersion);
            param.bundleVersionCode = EditorGUILayout.IntField("빌드 버전 코드", param.bundleVersionCode);
            param.enableaabBuild = EditorGUILayout.Toggle("AAB 빌드", param.enableaabBuild);
            param.enableDebugLog = EditorGUILayout.Toggle("디버그 로그 표시", param.enableDebugLog);
            param.enableCheat = EditorGUILayout.Toggle("치트 기능", param.enableCheat);
            param.enableAdmobTest = EditorGUILayout.Toggle("애드몹 테스트", param.enableAdmobTest);
        }
        GUILayout.EndVertical();

        EditorGUILayout.Space(128);

        GUILayout.BeginHorizontal();
        {
#if UNITY_ANDROID
            if (GUILayout.Button("안드로이드 빌드", GUILayout.Height(40f)))
            {
                ProjectBuilder.BuildAndroid(outputDir, param);
            }
#elif UNITY_IOS
            if (GUILayout.Button("iOS 빌드", GUILayout.Height(40f)))
            {
                ProjectBuilder.BuildIOS(outputDir, param);
            }

#endif
        }
        GUILayout.EndHorizontal();

        if (GUI.changed)
        {
            Debug.LogError("설정 적용");
        }
    }
}
