using Coffee.UIExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class AtlasManager : Singleton<AtlasManager>
{
    private Dictionary<string, SpriteAtlas> atlas = new Dictionary<string, SpriteAtlas>();

    public Dictionary<string, SpriteAtlas> Atlas => atlas;

    public void SetAtlas(Dictionary<string, SpriteAtlas> atlas)
    {
        this.atlas = atlas;
    }

    public void SetSprite(AtlasImage image, SpriteAtlas atlas, string spriteName)
    {
        if(image.spriteAtlas == null)
        {
            return;
        }
        else
        {
            if(image.spriteAtlas.name == atlas.name)
            {
                image.spriteName = spriteName;
            }
            else
            {
                image.spriteAtlas = atlas;
                image.sprite = atlas.GetSprite(spriteName);
            }
        }
    }
}
