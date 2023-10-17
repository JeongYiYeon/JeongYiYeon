using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ContextEditor : MonoBehaviour
{
    [MenuItem("Util/GUID 0000 제거")]
    public static void GUIDClear()
    {
        List<string> scenePaths = new List<string>();
        foreach (UnityEditor.EditorBuildSettingsScene S in UnityEditor.EditorBuildSettings.scenes)
        {
            if (S.enabled)
                scenePaths.Add(S.path);
        }
        string changeString = "    texture: {fileID: 2, type: 0}";
        foreach (string str in scenePaths)
        {
            string tempPath = Application.dataPath.Replace("Assets", "") + str;
            string[] allLines = System.IO.File.ReadAllLines(tempPath);
            for (int i = 0; i < allLines.Length; ++i)
            {
                string lineStr = allLines[i];
                if (lineStr.Contains("fileID:") && lineStr.Contains("guid: 00000000000000000000000000000000"))
                {
                    lineStr.Remove(0, lineStr.Length);
                    allLines[i] = changeString;
                }
            }
            System.IO.File.WriteAllLines(tempPath, allLines);
        }
    }


    [MenuItem("CONTEXT/Collider2D/사이즈 맞추기")]
    public static void AttachColliderSize(MenuCommand menuCommand)
    {
        var go = Selection.activeObject as GameObject;

        RectTransform rectTransform = go.GetComponent<RectTransform>();

        if(menuCommand.context.GetType() == typeof(BoxCollider2D))
        {
            BoxCollider2D collider2D = menuCommand.context as BoxCollider2D;

            collider2D.offset = GetOffset(rectTransform);

            collider2D.size = rectTransform.rect.size;
        }
        else if (menuCommand.context.GetType() == typeof(CircleCollider2D))
        {
            CircleCollider2D collider2D = menuCommand.context as CircleCollider2D;

            collider2D.offset = GetOffset(rectTransform);

            collider2D.radius = rectTransform.rect.size.x / 2f;
        }      
    }

    public static Vector2 GetOffset(RectTransform rectTransform)
    {
        float x = 0;
        float y = 0;

        if (rectTransform.pivot.x != 0.5f)
        {
            x = rectTransform.pivot.x == 0 ? rectTransform.rect.size.x / 2f : -rectTransform.rect.size.x / 2f;
        }

        if (rectTransform.pivot.y != 0.5f)
        {
            y = rectTransform.pivot.y == 0 ? rectTransform.rect.size.y / 2f : -rectTransform.rect.size.y / 2f;
        }

        return new Vector2(x, y);
    }
}
