using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HeroPositionDataItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private UtilEnumSelect characterType = null;

    private CharacterData characterData = null;

    private RaycastHit2D[] slotHits = new RaycastHit2D[5];
    private RaycastHit2D[] slotItemHits = new RaycastHit2D[5];

    private bool isHave = false;

    public CharacterData Data => characterData;

    public BaseCharacter.CHARACTER_TYPE CharacterType => characterType.CharacterType;
    public bool IsHave => isHave;

    public void SetCharacterData(CharacterData characterData)
    {
        this.characterData = characterData;
    }

    public void SetCharacterType(BaseCharacter.CHARACTER_TYPE type)
    {
        characterType.CharacterType = type;
    }

    public void SetHave(bool isHave)
    {
        this.isHave = isHave;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        int layerMask = 1 << LayerMask.NameToLayer(ConstHelper.LAYER_UI);

        int hits = Physics2D.RaycastNonAlloc(eventData.pointerCurrentRaycast.worldPosition, Vector2.zero, slotItemHits, layerMask);

        if (hits > 0)
        {
            for (int i = 0; i < hits; i++)
            {
                if (slotItemHits[i].collider != null)
                {
                    if (slotItemHits[i].collider.tag == ConstHelper.LAYER_SLOTITEM)
                    {
                        if(slotItemHits[i].transform.GetComponent<HeroPositionDataItem>() == null)
                        {
                            return;
                        }

                        if (slotItemHits[i].transform.GetComponent<HeroPositionDataItem>().CharacterType == BaseCharacter.CHARACTER_TYPE.HERO)
                        {
                            Debug.LogError("영웅 캐릭 못옮김");
                            return;
                        }


                        if (slotItemHits[i].transform.GetComponent<HeroPositionDataItem>().IsHave == false)
                        {
                            Debug.LogError("보유하지 않은 캐릭 못옮김");
                            return;
                        }

                        HeroPosition.Instance.CurClickCharacterData =
                            slotItemHits[i].transform.GetComponent<HeroPositionDataItem>().characterData;
                        Debug.LogError(HeroPosition.Instance.CurClickCharacterData.Name);


                        HeroPosition.Instance.OnBeginDrag(eventData);
                    }
                }
            }
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        HeroPosition.Instance.OnDrag(eventData);

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        int layerMask = 1 << LayerMask.NameToLayer(ConstHelper.LAYER_UI);
        int hits = Physics2D.RaycastNonAlloc(eventData.pointerCurrentRaycast.worldPosition, Vector2.zero, slotHits, 0f, layerMask);

        if (hits > 0)
        {
            for (int i = 0; i < hits; i++)
            {
                if (slotHits[i].collider != null)
                {
                    if (slotHits[i].collider.tag == ConstHelper.LAYER_SLOT)
                    {
                        HeroPositionDataItem item = slotHits[i].transform.GetComponent<HeroPositionDataItem>();

                        if (item != null)
                        {
                            if (item.CharacterType == BaseCharacter.CHARACTER_TYPE.HERO)
                            {
                                break;
                            }

                            else
                            {
                                HeroPosition.Instance.OnEndDrag(item, eventData);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            HeroPosition.Instance.ResetClick();
        }
        Array.Clear(slotHits, 0, slotHits.Length);
    }
}
