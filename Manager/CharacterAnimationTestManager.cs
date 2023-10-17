using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;
using TMPro;
using UnityEngine.AddressableAssets;
using System.Linq;

public class CharacterAnimationTestManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown characterDropdown = null;
    [SerializeField]
    private TMP_Dropdown bgDropdown = null;

    [SerializeField]
    private BackgroundController backGroundController = null;

    [SerializeField]
    private Animator animator = null;
    [SerializeField]
    private SpriteRendererAnimation characterAnimation = null;
    [SerializeField]
    private BaseCharacter character = null;
    [SerializeField]
    private UtilPlayEffect effect;

    public List<string> characterNameList = new List<string>();

    public List<string> bgNameList = new List<string>();

    private void OnEnable()
    {
        InitCharacterList();
        InitAtlas();
        SetCharacterDropDown();
        InitBgList();
        SetBgDropDown();
    }

    private void InitAtlas()
    {
        Addressables.LoadResourceLocationsAsync("Labels_Atlas", typeof(SpriteAtlas)).Completed += (handle) => 
        {
            if (handle.Result.Count > 0)
            {
                Dictionary<string, SpriteAtlas> uiAtlasDic = new Dictionary<string, SpriteAtlas>();

                for (int i = 0; i < handle.Result.Count; i++) 
                {
                    Addressables.LoadAssetAsync<SpriteAtlas>(handle.Result[i].PrimaryKey).Completed += (_handle) =>
                    {
                        if(_handle.Result != null)
                        {
                            if (uiAtlasDic.ContainsKey(_handle.Result.name) == false)
                            {
                                uiAtlasDic.Add(_handle.Result.name, _handle.Result);
                            }
                        }
                    };
                }

                AtlasManager.Instance.SetAtlas(uiAtlasDic);
            }
        };       
    }

    private void InitCharacterList()
    {
        string path = Environment.CurrentDirectory + "\\Assets\\BundleResources\\Atlas\\Character";

        DirectoryInfo di = new DirectoryInfo(path);

        foreach (DirectoryInfo dir in di.GetDirectories())
        {
            characterNameList.Add(dir.Name);
        }
    }
    private void InitBgList()
    {
        string path = Environment.CurrentDirectory + "\\Assets\\BundleResources\\Atlas\\BackGround";

        DirectoryInfo di = new DirectoryInfo(path);

        foreach (FileInfo fi in di.GetFiles())
        {
            if(fi.Name.Contains(".meta"))
            {
                continue;
            }

            bgNameList.Add(fi.Name.Replace(".spriteatlasv2",""));
        }
    }

    private void SetBgDropDown()
    {
        bgDropdown.AddOptions(bgNameList);
    }

    private void SetCharacterDropDown()
    {
        characterDropdown.AddOptions(characterNameList);
    }

    public void OnDropdownChanged(TMP_Dropdown dropdown)
    {
        characterAnimation.SetAtlas(characterNameList[dropdown.value]);

        if(characterNameList[dropdown.value].Contains("Enemy"))
        {
            Addressables.LoadResourceLocationsAsync("Labels_Animation", typeof(RuntimeAnimatorController)).Completed += (handle) =>
            {
                if (handle.Result.Count > 0)
                {
                    for (int i = 0; i < handle.Result.Count; i++)
                    {
                        if (handle.Result[i].PrimaryKey.Contains("EnemyAnimator"))
                        {
                            Addressables.LoadAssetAsync<RuntimeAnimatorController>(handle.Result[i].PrimaryKey).Completed += (handle) =>
                            {
                                animator.runtimeAnimatorController = handle.Result;

                                animator.SetTrigger(BaseCharacter.AnimationType.Idle.ToString());
                            };
                        }
                    }
                }
            };
        }
        else
        {
            Addressables.LoadResourceLocationsAsync("Labels_Animation", typeof(RuntimeAnimatorController)).Completed += (handle) =>
            {
                if (handle.Result.Count > 0)
                {
                    for (int i = 0; i < handle.Result.Count; i++)
                    {
                        if (handle.Result[i].PrimaryKey.Contains("BaseAnimator"))
                        {
                            Addressables.LoadAssetAsync<RuntimeAnimatorController>(handle.Result[i].PrimaryKey).Completed += (handle) =>
                            {
                                animator.runtimeAnimatorController = handle.Result;

                                animator.SetTrigger(BaseCharacter.AnimationType.Idle.ToString());
                            };
                        }
                    }
                }
            };
        }
    }

    public void OnDropdownBgChanged(TMP_Dropdown dropdown)
    {
        backGroundController.ChangeAtlas(AtlasManager.Instance.Atlas[bgNameList[dropdown.value]]);
    }


    public void OnClickIdle()
    {
        animator.SetTrigger(BaseCharacter.AnimationType.Idle.ToString());

    }
    public void OnClickRun()
    {
        animator.SetTrigger(BaseCharacter.AnimationType.Run.ToString());
        effect.PlayEffect(1f, BaseCharacter.AnimationType.Run.ToString());
    }

    public void OnClickAtk()
    {
        animator.SetTrigger(BaseCharacter.AnimationType.Attack.ToString());

    }
    public void OnClickHit()
    {
        animator.SetTrigger(BaseCharacter.AnimationType.Hit.ToString());
        effect.PlayEffect(1f, BaseCharacter.AnimationType.Hit.ToString());        

    }

    public void OnClickTest()
    {
        Test();
    }

    private void Test()
    {
        Debug.LogError(character.CharacterAnimation.Sprite.textureRect.width + "::" + character.CharacterAnimation.Sprite.textureRect.height);
        character.GetComponent<CircleCollider2D>().radius = character.CharacterAnimation.Sprite.textureRect.width / 2f;
        character.GetComponent<CircleCollider2D>().offset = new Vector2(0, -character.GetComponent<CircleCollider2D>().radius);
    }
}
