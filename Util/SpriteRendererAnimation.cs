using Coffee.UIExtensions;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;


public class SpriteRendererAnimation : MonoBehaviour
{
    [SerializeField]
    private AtlasImage image = null;
    [SerializeField]
    private SpriteAtlas spriteAtlas = null;

    [SerializeField]
    private string atlasName = "";

    private Dictionary<string, Sprite> atlasDics = new Dictionary<string, Sprite>();

    public SpriteAtlas Atlas=> spriteAtlas;
    
    public Sprite Sprite { get { return image.sprite; } }

    private void OnEnable()
    {
        if (!string.IsNullOrEmpty(atlasName))
        {
            if (spriteAtlas == null)
            {
                spriteAtlas = AtlasManager.Instance.Atlas[atlasName];
                image.spriteAtlas = spriteAtlas;
                atlasName = spriteAtlas.name;
            }
        }
    }

    public void Clear()
    {
        if (atlasDics != null)
        {
            atlasDics.Clear();
        }

        image.spriteAtlas = null;
        image.sprite = null;

        if (spriteAtlas != null)
        {
            spriteAtlas = null;
        }
    }

    public void SetAtlas(string atlasName)
    {
        Clear();

        spriteAtlas = AtlasManager.Instance.Atlas[atlasName];
        image.spriteAtlas = spriteAtlas;
        spriteAtlas.name = atlasName;
    }

    public void ChangeSpriteAtlas(string atlasName)
    {
        SetAtlas(atlasName);

        if (spriteAtlas == null)
        {
            return;
        }
    }

    public void ChangeSprite(string spriteName)
    {
        if (spriteAtlas != null)
        {
            if (atlasDics != null)
            {
                if (atlasDics.ContainsKey(spriteName))
                {
                    image.sprite = atlasDics[spriteName];
                }
                else
                {
                    image.sprite = spriteAtlas.GetSprite(spriteName);

                    atlasDics.Add(spriteName, image.sprite);
                }
            }
        }
    }

    public bool IsFlip 
    {
        get
        {
            if (image != null)
            {
                return image.transform.localRotation.y > 0;
            }

            else
            {
                return false;
            }
        }
    }

    public void FlipX(bool value)
    {
        if (image != null)
        {
            if (value == true)
            {
                image.transform.localRotation = Quaternion.Euler(0, 180f, 0);
            }
            else
            {
                image.transform.localRotation = Quaternion.identity;
            }

        }
    }

    public void ChangeColor(string colorHexCode)
    {
        Color color = Color.white;
        ColorUtility.TryParseHtmlString($"#{colorHexCode}", out color);

        image.color = color;
    }
}
