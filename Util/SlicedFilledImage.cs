using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_2017_4 || UNITY_2018_2_OR_NEWER
using UnityEngine.U2D;
#endif
using Sprites = UnityEngine.Sprites;
using Coffee.UIExtensions;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
[CustomEditor(typeof(AtlasImage), true)]
[CanEditMultipleObjects]
public class SlicedFilledImageEditor : ImageEditor
{
    [MenuItem("CONTEXT/Image/AtlasImage To SlicedFilledImage", true)]
    static bool _AtlasImageToSlicedFilledImage(MenuCommand command)
    {
        return CanConvertTo<SlicedFilledImage>(command.context);
    }

    [MenuItem("CONTEXT/Image/AtlasImage To SlicedFilledImage", false)]
    static void AtlasImageToSlicedFilledImage(MenuCommand command)
    {
        ConvertTo<SlicedFilledImage>(command.context);
    }

    /// <summary>
    /// Verify whether it can be converted to the specified component.
    /// </summary>
    protected static bool CanConvertTo<T>(Object context)
        where T : MonoBehaviour
    {
        return context && context.GetType() != typeof(T);
    }

    /// <summary>
    /// Convert to the specified component.
    /// </summary>
    protected static void ConvertTo<T>(Object context) where T : MonoBehaviour
    {
        var target = context as MonoBehaviour;
        var so = new SerializedObject(target);
        so.Update();

        bool oldEnable = target.enabled;
        target.enabled = false;

        // Find MonoScript of the specified component.
        foreach (var script in Resources.FindObjectsOfTypeAll<MonoScript>())
        {
            if (script.GetClass() != typeof(T))
                continue;

            // Set 'm_Script' to convert.
            so.FindProperty("m_Script").objectReferenceValue = script;
            so.ApplyModifiedProperties();
            break;
        }

        (so.targetObject as MonoBehaviour).enabled = oldEnable;
    }
}
#endif

public class SlicedFilledImage : AtlasImage
{
    private static class SetPropertyUtility
    {
        public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
        {
            if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
                return false;

            currentValue = newValue;
            return true;
        }

        public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
        {
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
                return false;

            currentValue = newValue;
            return true;
        }
    }

    private static readonly Vector3[] s_Vertices = new Vector3[4];
    private static readonly Vector2[] s_UVs = new Vector2[4];
    private static readonly Vector2[] s_SlicedVertices = new Vector2[4];
    private static readonly Vector2[] s_SlicedUVs = new Vector2[4];

#pragma warning disable 1692
#pragma warning disable IDE1006 // Suppress 'Naming rule violation' warnings
#pragma warning disable 0649

    [SerializeField]
    private bool m_PreserveBorder = false;
    public bool preserveBorder
    {
        get { return m_PreserveBorder; }
        set
        {
            if (SetPropertyUtility.SetStruct(ref m_PreserveBorder, value))
                SetVerticesDirty();
        }
    }

#pragma warning restore 0649

    public override Texture mainTexture
    {
        get
        {
            if (sprite != null)
                return sprite.texture;

            return material != null && material.mainTexture != null ? material.mainTexture : s_WhiteTexture;
        }
    }

    public override Material material
    {
        get
        {
            if (m_Material != null)
                return m_Material;

            if (sprite && sprite.associatedAlphaSplitTexture != null)
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
#endif
                    return Image.defaultETC1GraphicMaterial;
            }

            return defaultMaterial;
        }
        set { base.material = value; }
    }

    //public float alphaHitTestMinimumThreshold { get; set; }
#pragma warning restore IDE1006
#pragma warning restore 1692

    protected SlicedFilledImage()
    {
        useLegacyMeshGeneration = false;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        //TrackImage();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        //if (m_Tracked)
        //    UnTrackImage();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        pixelsPerUnitMultiplier = Mathf.Max(0.01f, pixelsPerUnitMultiplier);
    }
#endif

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (sprite == null)
        {
            base.OnPopulateMesh(vh);
            return;
        }

        if (fillMethod == FillMethod.Horizontal || fillMethod == FillMethod.Vertical)
        {
            GenerateSlicedFilledSprite(vh);
        }
        else
        {
            base.OnPopulateMesh(vh);
        }
    }

    /// <summary>
    /// Update the renderer's material.
    /// </summary>
    protected override void UpdateMaterial()
    {
        base.UpdateMaterial();

        // Check if this sprite has an associated alpha texture (generated when splitting RGBA = RGB + A as two textures without alpha)
        if (sprite == null)
        {
            canvasRenderer.SetAlphaTexture(null);
            return;
        }

        Texture2D alphaTex = sprite.associatedAlphaSplitTexture;
        if (alphaTex != null)
            canvasRenderer.SetAlphaTexture(alphaTex);
    }

    private void GenerateSlicedFilledSprite(VertexHelper vh)
    {
        vh.Clear();

        if (fillAmount < 0.001f)
            return;

        Rect rect = GetPixelAdjustedRect();
        Vector4 outer = Sprites.DataUtility.GetOuterUV(sprite);
        Vector4 padding = Sprites.DataUtility.GetPadding(sprite);

        if (!hasBorder)
        {
            Vector2 size = sprite.rect.size;

            int spriteW = Mathf.RoundToInt(size.x);
            int spriteH = Mathf.RoundToInt(size.y);

            // Image's dimensions used for drawing. X = left, Y = bottom, Z = right, W = top.
            Vector4 vertices = new Vector4(
                rect.x + rect.width * (padding.x / spriteW),
                rect.y + rect.height * (padding.y / spriteH),
                rect.x + rect.width * ((spriteW - padding.z) / spriteW),
                rect.y + rect.height * ((spriteH - padding.w) / spriteH));

            GenerateFilledSprite(vh, vertices, outer, fillAmount);
            return;
        }

        Vector4 inner = Sprites.DataUtility.GetInnerUV(sprite);
        Vector4 border = GetAdjustedBorders(sprite.border / pixelsPerUnit, rect);

        padding = padding / pixelsPerUnit;

        s_SlicedVertices[0] = new Vector2(padding.x, padding.y);
        s_SlicedVertices[3] = new Vector2(rect.width - padding.z, rect.height - padding.w);

        s_SlicedVertices[1].x = border.x;
        s_SlicedVertices[1].y = border.y;

        s_SlicedVertices[2].x = rect.width - border.z;
        s_SlicedVertices[2].y = rect.height - border.w;

        if (m_PreserveBorder)
        {
            switch (fillMethod)
            {
                case FillMethod.Horizontal:

                    if (fillOrigin == 0)
                    {
                        s_SlicedVertices[0].x = padding.x;
                        s_SlicedVertices[3].x = rect.width * fillAmount - padding.z;

                        float width = s_SlicedVertices[3].x - s_SlicedVertices[0].x;
                        if (width < border.x + border.z)
                        {
                            s_SlicedVertices[1].x = border.x;
                            s_SlicedVertices[2].x = border.x;
                        }
                        else
                        {
                            s_SlicedVertices[1].x = border.x;
                            s_SlicedVertices[2].x = rect.width * fillAmount - border.z;
                        }
                    }
                    else
                    {
                        s_SlicedVertices[0].x = rect.width * (1f - fillAmount) + padding.x;
                        s_SlicedVertices[3].x = rect.width - padding.z;

                        float width = s_SlicedVertices[3].x - s_SlicedVertices[0].x;
                        if (width < border.x + border.z)
                        {
                            s_SlicedVertices[1].x = rect.width - border.z;
                            s_SlicedVertices[2].x = rect.width - border.z;
                        }
                        else
                        {
                            s_SlicedVertices[1].x = rect.width * (1f - fillAmount) + border.x;
                            s_SlicedVertices[2].x = rect.width - border.z;
                        }

                    }

                    break;

                case FillMethod.Vertical:
                    if (fillOrigin == 0)
                    {
                        s_SlicedVertices[0].y = padding.y;
                        s_SlicedVertices[3].y = rect.height * fillAmount - padding.w;

                        float height = s_SlicedVertices[3].y - s_SlicedVertices[0].y;
                        if (height < border.y + border.w)
                        {
                            s_SlicedVertices[1].y = border.y;
                            s_SlicedVertices[2].y = border.y;
                        }
                        else
                        {
                            s_SlicedVertices[1].y = border.y;
                            s_SlicedVertices[2].y = rect.height * fillAmount - border.w;
                        }
                    }
                    else
                    {
                        s_SlicedVertices[0].y = rect.height * (1f - fillAmount) + padding.y;
                        s_SlicedVertices[3].y = rect.height - padding.w;

                        float height = s_SlicedVertices[3].y - s_SlicedVertices[0].y;
                        if (height < border.y + border.w)
                        {
                            s_SlicedVertices[1].y = rect.height - border.w;
                            s_SlicedVertices[2].y = rect.height - border.w;
                        }
                        else
                        {
                            s_SlicedVertices[1].y = rect.height * (1f - fillAmount) + border.y;
                            s_SlicedVertices[2].y = rect.height - border.w;
                        }
                    }
                    break;
            }
        }

        for (int i = 0; i < 4; ++i)
        {
            s_SlicedVertices[i].x += rect.x;
            s_SlicedVertices[i].y += rect.y;
        }

        s_SlicedUVs[0] = new Vector2(outer.x, outer.y);
        s_SlicedUVs[1] = new Vector2(inner.x, inner.y);
        s_SlicedUVs[2] = new Vector2(inner.z, inner.w);
        s_SlicedUVs[3] = new Vector2(outer.z, outer.w);

        float rectStartPos = 0;
        float _1OverTotalSize = 0;

        if (fillMethod == FillMethod.Horizontal)
        {
            rectStartPos = rect.x + padding.x;

            float totalSize = rect.width - padding.x - padding.z;
            _1OverTotalSize = totalSize > 0f ? 1f / totalSize : 1f;
        }
        else if(fillMethod == FillMethod.Vertical)
        {
            rectStartPos = rect.y + padding.y;

            float totalSize = rect.height - padding.y - padding.w;
            _1OverTotalSize = totalSize > 0f ? 1f / totalSize : 1f;
        }

        for (int x = 0; x < 3; x++)
        {
            int x2 = x + 1;

            for (int y = 0; y < 3; y++)
            {
                if (!fillCenter && x == 1 && y == 1)
                    continue;

                int y2 = y + 1;

                float sliceStart, sliceEnd;
                switch (fillMethod)
                {
                    case FillMethod.Horizontal:
                        if (fillOrigin == 0)
                        {
                            sliceStart = (s_SlicedVertices[x].x - rectStartPos) * _1OverTotalSize;
                            sliceEnd = (s_SlicedVertices[x2].x - rectStartPos) * _1OverTotalSize;
                        }
                        else
                        {
                            sliceStart = 1f - (s_SlicedVertices[x2].x - rectStartPos) * _1OverTotalSize;
                            sliceEnd = 1f - (s_SlicedVertices[x].x - rectStartPos) * _1OverTotalSize;
                        }
                        break;

                    case FillMethod.Vertical:
                        if (fillOrigin == 0)
                        {
                            sliceStart = 1f - (s_SlicedVertices[y2].y - rectStartPos) * _1OverTotalSize;
                            sliceEnd = 1f - (s_SlicedVertices[y].y - rectStartPos) * _1OverTotalSize;
                        }
                        else
                        {
                            sliceStart = (s_SlicedVertices[y].y - rectStartPos) * _1OverTotalSize;
                            sliceEnd = (s_SlicedVertices[y2].y - rectStartPos) * _1OverTotalSize;
                        }
                        break;

                    default: // Just there to get rid of the "Use of unassigned local variable" compiler error
                        sliceStart = sliceEnd = 0f;
                        break;
                }

                if (sliceStart >= fillAmount)
                    continue;

                Vector4 vertices = new Vector4(s_SlicedVertices[x].x, s_SlicedVertices[y].y, s_SlicedVertices[x2].x, s_SlicedVertices[y2].y);
                Vector4 uvs = new Vector4(s_SlicedUVs[x].x, s_SlicedUVs[y].y, s_SlicedUVs[x2].x, s_SlicedUVs[y2].y);
                float _fillAmount = (fillAmount - sliceStart) / (sliceEnd - sliceStart);

                GenerateFilledSprite(vh, vertices, uvs, _fillAmount);
            }
        }
    }

    private Vector4 GetAdjustedBorders(Vector4 border, Rect adjustedRect)
    {
        Rect originalRect = rectTransform.rect;

        for (int axis = 0; axis <= 1; axis++)
        {
            float borderScaleRatio;

            // The adjusted rect (adjusted for pixel correctness) may be slightly larger than the original rect.
            // Adjust the border to match the adjustedRect to avoid small gaps between borders (case 833201).
            if (originalRect.size[axis] != 0)
            {
                borderScaleRatio = adjustedRect.size[axis] / originalRect.size[axis];
                border[axis] *= borderScaleRatio;
                border[axis + 2] *= borderScaleRatio;
            }

            // If the rect is smaller than the combined borders, then there's not room for the borders at their normal size.
            // In order to avoid artefacts with overlapping borders, we scale the borders down to fit.
            float combinedBorders = border[axis] + border[axis + 2];
            if (adjustedRect.size[axis] < combinedBorders && combinedBorders != 0)
            {
                borderScaleRatio = adjustedRect.size[axis] / combinedBorders;
                border[axis] *= borderScaleRatio;
                border[axis + 2] *= borderScaleRatio;
            }
        }

        return border;
    }

    private void GenerateFilledSprite(VertexHelper vh, Vector4 vertices, Vector4 uvs, float fillAmount)
    {
        if (fillAmount < 0.001f)
            return;

        float uvLeft = uvs.x;
        float uvBottom = uvs.y;
        float uvRight = uvs.z;
        float uvTop = uvs.w;

        if (fillAmount < 1f)
        {
            if (fillMethod == FillMethod.Horizontal)
            {
                if (fillOrigin == 0)
                {
                    vertices.z = vertices.x + (vertices.z - vertices.x) * fillAmount;
                    uvRight = uvLeft + (uvRight - uvLeft) * fillAmount;
                }
                else
                {
                    vertices.x = vertices.z - (vertices.z - vertices.x) * fillAmount;
                    uvLeft = uvRight - (uvRight - uvLeft) * fillAmount;

                }
            }
            else
            {
                if (fillOrigin == 0)
                {
                    vertices.y = vertices.w - (vertices.w - vertices.y) * fillAmount;
                    uvBottom = uvTop - (uvTop - uvBottom) * fillAmount;
                }
                else
                {
                    vertices.w = vertices.y + (vertices.w - vertices.y) * fillAmount;
                    uvTop = uvBottom + (uvTop - uvBottom) * fillAmount;
                }
            }
        }

        s_Vertices[0] = new Vector3(vertices.x, vertices.y);
        s_Vertices[1] = new Vector3(vertices.x, vertices.w);
        s_Vertices[2] = new Vector3(vertices.z, vertices.w);
        s_Vertices[3] = new Vector3(vertices.z, vertices.y);

        s_UVs[0] = new Vector2(uvLeft, uvBottom);
        s_UVs[1] = new Vector2(uvLeft, uvTop);
        s_UVs[2] = new Vector2(uvRight, uvTop);
        s_UVs[3] = new Vector2(uvRight, uvBottom);

        int startIndex = vh.currentVertCount;

        for (int i = 0; i < 4; i++)
            vh.AddVert(s_Vertices[i], color, s_UVs[i]);

        vh.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
        vh.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
    }

    public override int layoutPriority { get { return 0; } }
    public override float minWidth { get { return 0; } }
    public override float minHeight { get { return 0; } }
    public override float flexibleWidth { get { return -1; } }
    public override float flexibleHeight { get { return -1; } }

    public override float preferredWidth
    {
        get
        {
            if (sprite == null)
                return 0;

            return Sprites.DataUtility.GetMinSize(sprite).x / pixelsPerUnit;
        }
    }

    public override float preferredHeight
    {
        get
        {
            if (sprite == null)
                return 0;

            return Sprites.DataUtility.GetMinSize(sprite).y / pixelsPerUnit;
        }
    }

    public override void CalculateLayoutInputHorizontal() { }
    public override void CalculateLayoutInputVertical() { }

    public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        if (alphaHitTestMinimumThreshold <= 0)
            return true;

        if (alphaHitTestMinimumThreshold > 1)
            return false;

        if (sprite == null)
            return true;

        Vector2 local;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out local))
            return false;

        Rect rect = GetPixelAdjustedRect();

        // Convert to have lower left corner as reference point.
        local.x += rectTransform.pivot.x * rect.width;
        local.y += rectTransform.pivot.y * rect.height;

        Rect spriteRect = sprite.rect;
        Vector4 border = sprite.border;
        Vector4 adjustedBorder = GetAdjustedBorders(border / pixelsPerUnit, rect);

        for (int i = 0; i < 2; i++)
        {
            if (local[i] <= adjustedBorder[i])
                continue;

            if (rect.size[i] - local[i] <= adjustedBorder[i + 2])
            {
                local[i] -= (rect.size[i] - spriteRect.size[i]);
                continue;
            }

            float lerp = Mathf.InverseLerp(adjustedBorder[i], rect.size[i] - adjustedBorder[i + 2], local[i]);
            local[i] = Mathf.Lerp(border[i], spriteRect.size[i] - border[i + 2], lerp);
        }

        // Normalize local coordinates.
        Rect textureRect = sprite.textureRect;
        Vector2 normalized = new Vector2(local.x / textureRect.width, local.y / textureRect.height);

        // Convert to texture space.
        float x = Mathf.Lerp(textureRect.x, textureRect.xMax, normalized.x) / sprite.texture.width;
        float y = Mathf.Lerp(textureRect.y, textureRect.yMax, normalized.y) / sprite.texture.height;

        switch (fillMethod)
        {
            case FillMethod.Horizontal:
                if (fillOrigin == 0)
                {
                    if (1f - x > fillAmount)
                        return false;
                }
                else
                {
                    if (x > fillAmount)
                        return false;
                }
                break;

            case FillMethod.Vertical:
                if (fillOrigin == 0)
                {
                    if (1f - y > fillAmount)
                        return false;
                }
                else
                {
                    if (y > fillAmount)
                        return false;
                }
                break;
        }

        try
        {
            return sprite.texture.GetPixelBilinear(x, y).a >= alphaHitTestMinimumThreshold;
        }
        catch (UnityException e)
        {
            Debug.LogError("Using alphaHitTestMinimumThreshold greater than 0 on Image whose sprite texture cannot be read. " + e.Message + " Also make sure to disable sprite packing for this sprite.", this);
            return true;
        }
    }

    public override void OnBeforeSerialize() { }
    public override void OnAfterDeserialize()
    {
        fillAmount = Mathf.Clamp01(fillAmount);
    }

    //    // Whether this is being tracked for Atlas Binding
    //    private bool m_Tracked = false;

    //#if UNITY_2017_4 || UNITY_2018_2_OR_NEWER
    //    private static List<SlicedFilledImage> m_TrackedTexturelessImages = new List<SlicedFilledImage>();
    //    private static bool s_Initialized;
    //#endif

    //    private void TrackImage()
    //    {
    //        if (sprite != null && sprite.texture == null)
    //        {
    //#if UNITY_2017_4 || UNITY_2018_2_OR_NEWER
    //            if (!s_Initialized)
    //            {
    //                SpriteAtlasManager.atlasRegistered += RebuildImage;
    //                s_Initialized = true;
    //            }

    //            m_TrackedTexturelessImages.Add(this);
    //#endif
    //            m_Tracked = true;
    //        }
    //    }

    //    private void UnTrackImage()
    //    {
    //#if UNITY_2017_4 || UNITY_2018_2_OR_NEWER
    //        m_TrackedTexturelessImages.Remove(this);
    //#endif
    //        m_Tracked = false;
    //    }

    //#if UNITY_2017_4 || UNITY_2018_2_OR_NEWER
    //    private static void RebuildImage(SpriteAtlas spriteAtlas)
    //    {
    //        for (int i = m_TrackedTexturelessImages.Count - 1; i >= 0; i--)
    //        {
    //            SlicedFilledImage image = m_TrackedTexturelessImages[i];
    //            if (spriteAtlas.CanBindTo(image.activeSprite))
    //            {
    //                image.SetAllDirty();
    //                m_TrackedTexturelessImages.RemoveAt(i);
    //            }
    //        }
    //    }
    //#endif
}