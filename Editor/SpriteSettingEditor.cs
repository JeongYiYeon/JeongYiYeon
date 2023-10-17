using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine.U2D;
using System.IO;
using System;
using UnityEditor.UI;
using System.Text;
using UnityEditorInternal;
using System.Linq;
using UnityEngine.Rendering.UI;

public class AtlasMaker : EditorWindow
{
    private UnityEngine.Object obj = null;
    private SpriteAtlas atlas = null;
    private Sprite[] sprites = null;
    private SpriteAtlasAsset atlasAsset = null;

    private Vector2 scrollPos = Vector2.zero;

    private int preSpriteSelect = -1;
    private int spriteSelect = -1;

    public void Init(UnityEngine.Object obj)
    {
        this.obj = obj;
        atlas= obj as SpriteAtlas;
        atlasAsset = SpriteAtlasAsset.Load(AssetDatabase.GetAssetPath(obj.GetInstanceID()));
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        obj = EditorGUILayout.ObjectField("선택한 아틀라스", obj, typeof(SpriteAtlas), true);

        EditorGUILayout.EndHorizontal();

        Init(obj);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        EditorGUILayout.Space(128);

        RefreshAtlasPreview();

        EditorGUILayout.EndScrollView();

        if (IsSelectTexture() == true)
        {
            if (GUILayout.Button("선택한 스프라이트 추가 하기",
                        new[]
                        {
                        GUILayout.Width (256),
                        GUILayout.Height (64)
                        }) == true)
            {
                AddSpriteInAtlas();
            }
        }

        if (GUI.changed)
        {
            RemoveSpriteInAtlas();
        }       
    }

    private bool IsSelectTexture()
    {
        if (Selection.objects != null && Selection.objects.Length > 0)
        {
            foreach (var obj in Selection.objects)
            {
                UnityEngine.Object o = obj as Texture2D;
                if (o != null)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void AddSpriteInAtlas()
    {
        if (atlasAsset != null)
        {
            atlas.GetSprites(sprites);

            List<UnityEngine.Object> objList = new List<UnityEngine.Object>();

            List<Sprite> spriteList = sprites.ToList();

            for (int i = 0; i < Selection.objects.Length; i++)
            {
                if (spriteList.Find(x => x.name.Replace("(Clone)", "") == Selection.objects[i].name) == null)
                {
                    UnityEngine.Object o = Selection.objects[i] as Texture2D;

                    if (o != null)
                    {
                        objList.Add(Selection.objects[i]);
                    }
                    else
                    {
                        Debug.LogError($"{Selection.objects[i].name}은 텍스쳐가 아니에요");
                    }
                }
            }

            if (objList.Count > 0)
            {
                UnityEngine.Object[] addObject = objList.ToArray();

                if (addObject.Length > 0)
                {
                    atlasAsset.Add(addObject);

                    string[] assetLabels = new string[1] { $"Atlas_{atlas.name}" };

                    foreach (UnityEngine.Object obj in addObject)
                    {
                        string[] tmpAssetLabels = AssetDatabase.GetLabels(obj);

                        if (tmpAssetLabels.Contains(assetLabels[0]) == false)
                        {
                            AssetDatabase.SetLabels(obj, assetLabels);
                        }
                    }
                }

                SpriteAtlasAsset.Save(atlasAsset, AssetDatabase.GetAssetPath(obj.GetInstanceID()));

                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

                preSpriteSelect = -1;
                spriteSelect = -1;
            }
        }
    }

    private void RemoveSpriteInAtlas()
    {
        if (atlasAsset != null)
        {
            if (sprites != null && sprites.Length > 0)
            {
                if (preSpriteSelect != -1 && spriteSelect != -1)
                {
                    if (preSpriteSelect == spriteSelect)
                    {
                        string spriteName = sprites[spriteSelect].name.Replace("(Clone)", "");
                        string[] findSpritePath = AssetDatabase.FindAssets($"{spriteName}", new string[] { "Assets/BundleResources", "Assets/Resources" });

                        if (findSpritePath != null && findSpritePath.Length > 0)
                        {
                            UnityEngine.Object[] tmpObj = new UnityEngine.Object[] { AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(findSpritePath[0])) };

                            if (tmpObj.Length > 0)
                            {
                                atlasAsset.Remove(tmpObj);
                                string[] tmpAssetLabels = AssetDatabase.GetLabels(tmpObj[0]);
                                if (tmpAssetLabels.Length > 0)
                                {
                                    AssetDatabase.SetLabels(tmpObj[0], null);
                                }
                            }

                            SpriteAtlasAsset.Save(atlasAsset, AssetDatabase.GetAssetPath(obj.GetInstanceID()));

                            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

                            preSpriteSelect = -1;
                            spriteSelect = -1;
                        }
                    }
                    else
                    {
                        preSpriteSelect = spriteSelect;
                    }
                }
                else
                {
                    preSpriteSelect = spriteSelect;
                }
            }
        }
    }

    private void RefreshAtlasPreview()
    {
        Array.Resize(ref sprites, atlas.spriteCount);

        atlas.GetSprites(sprites);

        GUIContent[] guiContents = sprites
            .Select(s => new GUIContent(s.name.Replace("(Clone)", ""), AssetPreview.GetAssetPreview(s)))
            .ToArray();

        spriteSelect = GUI.SelectionGrid(new Rect(0, 0, 800, 800), spriteSelect, guiContents, 3,
            new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                active = new GUIStyleState()
                {                    
                    textColor = Color.black
                }
            });
    }
}

[CanEditMultipleObjects]
[InitializeOnLoad]
public class SpriteSettingEditor
{
    [MenuItem("Assets/아틀라스v2 설정", false)]
    public static void AddAtlasSprite()
    {
        AtlasMaker editor = EditorWindow.GetWindow<AtlasMaker>();
        editor.titleContent = new GUIContent("아틀라스 설정");

        editor.Init(Selection.activeObject);

        editor.Show();
    }

    [MenuItem("Assets/아틀라스v2 설정", true)]
    public static bool IsSpriteAtlas()
    {
        if (Selection.activeObject != null)
        {
            var selectionObj = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());

            if (selectionObj.Contains("spriteatlasv2"))
            {
                return true;
            }
        }
        return false;
    }

    [MenuItem("Assets/선택한 스프라이트 아틀라스v2 생성")]
    public static void CreateAtlasV2ForSelectedSprites()
    {

        var selectionObj = Selection.objects;

        SpriteAtlasAsset atlas = new SpriteAtlasAsset();
        string path = AssetDatabase.GetAssetPath(selectionObj[0].GetInstanceID());
        path = Path.GetDirectoryName(path);

        InternalEditorUtility.SaveToSerializedFileAndForget(new UnityEngine.Object[] { atlas }, $"{path}/New Sprite Atlas.spriteatlasv2", true);
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

        atlas = SpriteAtlasAsset.Load($"{path}/New Sprite Atlas.spriteatlasv2");
        atlas.Add(Selection.objects);        

        SpriteAtlasPackingSettings setting = atlas.GetPackingSettings();
        setting.enableTightPacking = false;

        atlas.SetIncludeInBuild(false);
        atlas.SetPackingSettings(setting);

#if UNITY_ANDROID
        TextureImporterPlatformSettings settings = atlas.GetPlatformSettings("Android");
#elif UNITY_IOS
        TextureImporterPlatformSettings settings = atlas.GetPlatformSettings("iPhone");
#endif

        settings.overridden = true;
        settings.maxTextureSize = 2048;
        settings.format = TextureImporterFormat.ASTC_4x4;

        atlas.SetPlatformSettings(settings);

        SpriteAtlasAsset.Save(atlas, $"{path}/New Sprite Atlas.spriteatlasv2");

        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    }


    //[MenuItem("Assets/선택한 스프라이트 아틀라스 생성")]
    //public static void CreateAtlasForSelectedSprites()
    //{
    //    var selectionObj = Selection.objects;

    //    SpriteAtlas atlas = new SpriteAtlas();
    //    string path = AssetDatabase.GetAssetPath(selectionObj[0].GetInstanceID());
    //    path = Path.GetDirectoryName(path);

    //    AssetDatabase.CreateAsset(atlas, $"{path}/newAtlas.spriteatlas");

    //    foreach (var obj in Selection.objects)
    //    {
    //        UnityEngine.Object o = obj as Texture2D;
    //        if (o != null)
    //        {
    //            SpriteAtlasExtensions.Add(atlas, new UnityEngine.Object[] { o });
    //        }
    //    }

    //    SpriteAtlasPackingSettings setting = SpriteAtlasExtensions.GetPackingSettings(atlas);
    //    setting.enableTightPacking = false;

    //    SpriteAtlasExtensions.SetIncludeInBuild(atlas, false);
    //    SpriteAtlasExtensions.SetPackingSettings(atlas, setting);

    //    TextureImporterPlatformSettings settings = SpriteAtlasExtensions.GetPlatformSettings(atlas, "Android");

    //    settings.overridden = true;
    //    settings.maxTextureSize = 2048;
    //    settings.format = TextureImporterFormat.ASTC_4x4;

    //    SpriteAtlasExtensions.SetPlatformSettings(atlas, settings);

    //    SpriteAtlasUtility.PackAtlases(new[] { atlas }, EditorUserBuildSettings.activeBuildTarget);

    //    AssetDatabase.SaveAssets();

    //    AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    //}


    [MenuItem("Assets/전체 스프라이트 단일 셋팅")]
    public static void SetSingleAllSpriteOption()
    {
        var selectionObj = Selection.activeObject;

        foreach (var guid in AssetDatabase.FindAssets("t:Texture", new string[] { AssetDatabase.GetAssetPath(selectionObj.GetInstanceID())}))
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

            if(textureImporter != null)
            {
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.spriteImportMode = SpriteImportMode.Single;
                textureImporter.alphaIsTransparency = true;
                textureImporter.mipmapEnabled = false;
                textureImporter.wrapMode = TextureWrapMode.Clamp;
                //textureImporter.filterMode = FilterMode.Bilinear;
                textureImporter.filterMode = FilterMode.Point;

                textureImporter.maxTextureSize = 2048;

#if UNITY_ANDROID
                TextureImporterPlatformSettings settings = textureImporter.GetPlatformTextureSettings("Android");
#elif UNITY_IOS
                TextureImporterPlatformSettings settings = textureImporter.GetPlatformTextureSettings("iPhone");
#endif
                settings.overridden = true;
                settings.maxTextureSize = 2048;
                settings.resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
                settings.format = TextureImporterFormat.ASTC_4x4;

                textureImporter.SetPlatformTextureSettings(settings);

                textureImporter.SaveAndReimport();

                AssetDatabase.Refresh();
            }
        }     
    }

    [MenuItem("CONTEXT/TextureImporter/단일 스프라이트 셋팅")]
    public static void SetSingleSpriteOption(MenuCommand command)
    {
        var textureImporter = (TextureImporter)command.context;
        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.spriteImportMode = SpriteImportMode.Single;
        textureImporter.alphaIsTransparency = true;
        textureImporter.mipmapEnabled = false;
        textureImporter.wrapMode = TextureWrapMode.Clamp;
        textureImporter.filterMode = FilterMode.Bilinear;

        textureImporter.maxTextureSize = 2048;

#if UNITY_ANDROID
        TextureImporterPlatformSettings settings = textureImporter.GetPlatformTextureSettings("Android");
#elif UNITY_IOS
        TextureImporterPlatformSettings settings = textureImporter.GetPlatformTextureSettings("iPhone");
#endif
        settings.overridden = true;
        settings.maxTextureSize = 2048;
        settings.resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
        settings.format = TextureImporterFormat.ASTC_4x4;

        textureImporter.SetPlatformTextureSettings(settings);

        textureImporter.SaveAndReimport();

        AssetDatabase.Refresh();
    }

    [MenuItem("CONTEXT/TextureImporter/멀티 스프라이트 셋팅")]
    public static void SetMultiSpriteOption(MenuCommand command)
    {
        var textureImporter = (TextureImporter)command.context;
        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.spriteImportMode = SpriteImportMode.Multiple;
        textureImporter.alphaIsTransparency = true;
        textureImporter.mipmapEnabled = false;
        textureImporter.wrapMode = TextureWrapMode.Clamp;
        textureImporter.filterMode = FilterMode.Bilinear;

        textureImporter.maxTextureSize = 2048;

#if UNITY_ANDROID
        TextureImporterPlatformSettings settings = textureImporter.GetPlatformTextureSettings("Android");
#elif UNITY_IOS
        TextureImporterPlatformSettings settings = textureImporter.GetPlatformTextureSettings("iPhone");
#endif

        settings.overridden = true;
        settings.maxTextureSize = 2048;
        settings.resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
        settings.format = TextureImporterFormat.ASTC_4x4;

        textureImporter.SetPlatformTextureSettings(settings);

        textureImporter.SaveAndReimport();

        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/전체 스프라이트 레이블 초기화")]
    public static void SetResetLabels()
    {
        var selectionObj = Selection.activeObject;

        foreach (var guid in AssetDatabase.FindAssets("t:Texture", new string[] { AssetDatabase.GetAssetPath(selectionObj.GetInstanceID()) }))
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);

            AssetDatabase.SetLabels(AssetDatabase.LoadAssetAtPath(path, typeof(Texture)), null);
            AssetDatabase.Refresh();
        }
    }

    [MenuItem("Assets/전체 스프라이트 안드로이드 셋팅")]
    public static void SetAllSpriteOptionAndroid()
    {
        foreach (var guid in AssetDatabase.FindAssets("t:Texture", new string[] { "Assets" }))
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

            if (textureImporter != null)
            {
                if (textureImporter.spriteImportMode == SpriteImportMode.Single)
                {
                    textureImporter.textureType = TextureImporterType.Sprite;
                    textureImporter.alphaIsTransparency = true;
                    textureImporter.mipmapEnabled = false;
                    textureImporter.wrapMode = TextureWrapMode.Clamp;
                    textureImporter.filterMode = FilterMode.Point;

                    textureImporter.maxTextureSize = 2048;

                    TextureImporterPlatformSettings settings = textureImporter.GetPlatformTextureSettings("Android");

                    settings.overridden = true;
                    settings.maxTextureSize = 2048;
                    settings.resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
                    settings.format = TextureImporterFormat.ASTC_4x4;

                    textureImporter.SetPlatformTextureSettings(settings);

                    textureImporter.SaveAndReimport();

                    AssetDatabase.Refresh();
                }
                else if (textureImporter.spriteImportMode == SpriteImportMode.Multiple)
                {
                    textureImporter.textureType = TextureImporterType.Sprite;
                    textureImporter.alphaIsTransparency = true;
                    textureImporter.mipmapEnabled = false;
                    textureImporter.wrapMode = TextureWrapMode.Clamp;
                    textureImporter.filterMode = FilterMode.Point;

                    textureImporter.maxTextureSize = 2048;

                    TextureImporterPlatformSettings settings = textureImporter.GetPlatformTextureSettings("Android");

                    settings.overridden = true;
                    settings.maxTextureSize = 2048;
                    settings.resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
                    settings.format = TextureImporterFormat.ASTC_4x4;

                    textureImporter.SetPlatformTextureSettings(settings);

                    textureImporter.SaveAndReimport();

                    AssetDatabase.Refresh();
                }
                     
            }
        }
    }

    [MenuItem("Assets/전체 스프라이트 iOS 셋팅")]
    public static void SetAllSpriteOptioniOS()
    {

        foreach (var guid in AssetDatabase.FindAssets("t:Texture", new string[] { "Assets" }))
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

            if (textureImporter != null)
            {
                if (textureImporter.spriteImportMode == SpriteImportMode.Single)
                {
                    textureImporter.textureType = TextureImporterType.Sprite;
                    textureImporter.alphaIsTransparency = true;
                    textureImporter.mipmapEnabled = false;
                    textureImporter.wrapMode = TextureWrapMode.Clamp;
                    textureImporter.filterMode = FilterMode.Point;

                    textureImporter.maxTextureSize = 2048;

                    TextureImporterPlatformSettings settings = textureImporter.GetPlatformTextureSettings("iPhone");

                    settings.overridden = true;
                    settings.maxTextureSize = 2048;
                    settings.resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
                    settings.format = TextureImporterFormat.ASTC_4x4;

                    textureImporter.SetPlatformTextureSettings(settings);

                    textureImporter.SaveAndReimport();

                    AssetDatabase.Refresh();
                }
                else if (textureImporter.spriteImportMode == SpriteImportMode.Multiple)
                {
                    textureImporter.textureType = TextureImporterType.Sprite;
                    textureImporter.alphaIsTransparency = true;
                    textureImporter.mipmapEnabled = false;
                    textureImporter.wrapMode = TextureWrapMode.Clamp;
                    textureImporter.filterMode = FilterMode.Point;

                    textureImporter.maxTextureSize = 2048;

                    TextureImporterPlatformSettings settings = textureImporter.GetPlatformTextureSettings("iPhone");

                    settings.overridden = true;
                    settings.maxTextureSize = 2048;
                    settings.resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
                    settings.format = TextureImporterFormat.ASTC_4x4;

                    textureImporter.SetPlatformTextureSettings(settings);

                    textureImporter.SaveAndReimport();

                    AssetDatabase.Refresh();
                }

            }
        }
    }
}
