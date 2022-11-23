using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using System.Data;
using System.Linq;
using System.Text;
using System.Diagnostics;

public class AstcDecodeEditor : EditorWindow
{
    [MenuItem("MENU/ASTC -> PNG 변환")]
    public static void ShowWindow()
    {
        AstcDecodeEditor astcDecodeEditor = (AstcDecodeEditor)EditorWindow.GetWindow(typeof(AstcDecodeEditor));
        astcDecodeEditor.minSize = new Vector2(800, 700);
    }

    private string path;
    private string _astcFolderPath;
    private bool isExistAstc = false;
    private bool isCompleteCovert = false;

    private StringBuilder output = new StringBuilder();
    private List<FileInfo> _astcFileList = new List<FileInfo>();
    private DirectoryInfo _info = null;

    private string _lastPath = "";

    private void OnEnable()
    {
        SetAstcFolder();
    }

    private void SetAstcFolder()
    {
        if (string.IsNullOrEmpty(path))
        {
            path = EditorUtility.OpenFolderPanel("astcend 경로", "", "");
        }

        if (path.Length != 0)
        {
            if (string.IsNullOrEmpty(_astcFolderPath))
            {
                _astcFolderPath = EditorUtility.OpenFolderPanel("astc -> png 변환할 폴더", _lastPath, "");
            }
        }
    }

    private void CheckAstc()
    {
        _info = new DirectoryInfo(_astcFolderPath);

        foreach (var _file in _info.GetFiles())
        {
            if (_file.Extension == ".astc")
            {
                if (isExistAstc == false)
                {
                    isExistAstc = true;
                }

                EditorGUILayout.TextField(_file.Name);
                EditorGUILayout.Space(10);
            }
        }
    }

    void SetTextureOption(string file)
    {
        UnityEngine.Object _tmpTexture = AssetDatabase.LoadAssetAtPath(file, typeof(Texture2D));

        string _assetPath = AssetDatabase.GetAssetPath(_tmpTexture);

        TextureImporter textureImporter =  AssetImporter.GetAtPath(_assetPath) as TextureImporter;

        textureImporter.isReadable = true;

        AssetDatabase.ImportAsset(_assetPath);
        AssetDatabase.Refresh();

        //Texture2D _savePng = FlipTexture((Texture2D)_tmpTexture);
        Texture2D _savePng = (Texture2D)_tmpTexture;
              
        if(_savePng != null)
        {
            _assetPath = AssetDatabase.GetAssetPath(_savePng);

            textureImporter.textureType = TextureImporterType.Default;
            textureImporter.textureShape = TextureImporterShape.Texture2D;
            textureImporter.sRGBTexture = true;
            textureImporter.alphaSource = TextureImporterAlphaSource.FromInput;
            textureImporter.wrapMode = TextureWrapMode.Clamp;
            textureImporter.filterMode = FilterMode.Bilinear;
            textureImporter.anisoLevel = 1;
            textureImporter.npotScale = TextureImporterNPOTScale.None;
            textureImporter.mipmapEnabled = false;
            textureImporter.maxTextureSize = 2048;
            textureImporter.compressionQuality = 100;
            textureImporter.textureCompression = TextureImporterCompression.CompressedHQ;
            textureImporter.isReadable = false;

            TextureImporterPlatformSettings _settings = new TextureImporterPlatformSettings();

            _settings.overridden = true;
            _settings.resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
            _settings.format = TextureImporterFormat.ASTC_8x8;
            _settings.compressionQuality = 100;

            for (int i = 0; i < 3; i++)
            {
                if (i == 0)
                {
                    _settings.name = "iPhone";
                }

                else if (i == 1)
                {
                    _settings.name = "Android";
                    _settings.androidETC2FallbackOverride = AndroidETC2FallbackOverride.UseBuildSettings;
                }

                textureImporter.SetPlatformTextureSettings(_settings);
            }

            AssetDatabase.ImportAsset(_assetPath);
            AssetDatabase.Refresh();

        }
    }

    private Texture2D FlipTexture(Texture2D tex)
    {      
        Texture2D _tex = new Texture2D(tex.width, tex.height);

        for (int x = 0; x < tex.width; x++)        
        {
            for (int y = 0; y < tex.height; y++)
            {
                _tex.SetPixel(x, tex.height - y - 1, tex.GetPixel(x, y));
            }
        }

        _tex.Apply();

        byte[] textureByte = _tex.EncodeToPNG();

        File.WriteAllBytes(_astcFolderPath + string.Format(@"\{0}.png", tex.name), textureByte);

        return _tex;
    }

    public void OnGUI()
    {
        if(path.Length != 0)
        {
            if (_astcFolderPath.Length != 0)
            {
                _lastPath = _astcFolderPath;

                EditorGUILayout.BeginVertical();

                CheckAstc();

                EditorGUILayout.EndVertical();


                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("이미지 폴더 변경", GUILayout.Width(200), GUILayout.Height(70)))
                {
                    isExistAstc = false;
                    isCompleteCovert = false;

                    _astcFolderPath = EditorUtility.OpenFolderPanel("astc -> png 변환할 폴더", _lastPath, "");

                    if (_astcFolderPath.Length != 0)
                    {
                        _lastPath = _astcFolderPath;
                        CheckAstc();
                    }

                    else
                    {
                        Close();
                    }
                }

                EditorGUILayout.Space(10);

                if (isExistAstc == true)
                {
                    if (GUILayout.Button("PNG 파일로 변경", GUILayout.Width(200), GUILayout.Height(70)))
                    {
                        _astcFileList.Clear();

                        foreach (var _file in _info.GetFiles())
                        {
                            if (_file.Extension == ".astc")
                            {
                                _astcFileList.Add(_file);
                            }
                        }

                        for (int i = 0; i < _astcFileList.Count; i++)
                        {
                            string _astcencExe = Path.GetFullPath(Path.Combine(path, "astcenc-sse4.1.exe"));
                            string _astcPath = Path.GetFullPath(Path.Combine(_astcFolderPath, _astcFileList[i].Name));
                            string _pngPath = Path.GetFullPath(Path.Combine(_astcFolderPath, _astcFileList[i].Name.Replace(".astc", ".png")));

                            Process process = new Process();

                            process.StartInfo.UseShellExecute = false;
                            process.StartInfo.RedirectStandardInput = true;
                            process.StartInfo.RedirectStandardOutput = true;
                            process.StartInfo.RedirectStandardError = true;
                            process.StartInfo.WorkingDirectory = path;
                            process.StartInfo.CreateNoWindow = true;
                            process.StartInfo.FileName = _astcencExe;
                            process.StartInfo.Arguments = $"-dl " + _astcPath + " " + _pngPath + " -yflip";

                            process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
                            {
                                if (!string.IsNullOrEmpty(e.Data))
                                {

                                }
                            });

                            process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
                            {
                                if (!string.IsNullOrEmpty(e.Data))
                                {
                                }
                            });

                            process.Start();
                            process.BeginOutputReadLine();
                            process.BeginErrorReadLine();
                            process.WaitForExit();

                            if (process.ExitCode == 0)
                            {
                                output.AppendLine(_astcFileList[i].Name + " ---> " + _astcFileList[i].Name.Replace(".astc", ".png"));
                                AssetDatabase.Refresh();
                            }
                        }

                        isCompleteCovert = true;
                    }

                    EditorGUILayout.Space(10);

                    if (isCompleteCovert == true)
                    {
                        if (GUILayout.Button("PNG 텍스쳐 설정", GUILayout.Width(200), GUILayout.Height(70)))
                        {
                            string _resultFile = "convertFile.txt";

                            for (int i = 0; i < _astcFileList.Count; i++)
                            {
                                string _pngPath = Path.GetFullPath(Path.Combine(_astcFolderPath, _astcFileList[i].Name.Replace(".astc", ".png")));

                                SetTextureOption(_pngPath.Substring(_pngPath.IndexOf("Assets\\")).Replace("\\", "/"));
                            }

                            File.WriteAllText(_resultFile, output.ToString());

                            Process.Start("notepad.exe", _resultFile);
                            Process.Start(_astcFolderPath);

                            isCompleteCovert = false;
                        }
                    }
                }



                EditorGUILayout.EndHorizontal();

                if (isExistAstc == false)
                {
                    isCompleteCovert = false;

                    EditorGUILayout.BeginVertical();

                    EditorGUILayout.Space(10);

                    EditorGUILayout.TextField("astc 파일이 없습니다.", GUILayout.Width(200), GUILayout.Height(70));

                    EditorGUILayout.EndVertical();
                }


            }


        }
    }

}
