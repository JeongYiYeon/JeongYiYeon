using Coffee.UIExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class SkillThumbnail : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private AtlasImage imgSkill = null;
    [SerializeField]
    private TMP_Text labelSkillTitle = null;
    [SerializeField]
    private TMP_Text labelSkillDesc = null;

    private SkillData skillData = null;

    private bool isTooltip = false;

    private void OnDisable()
    {
        if (isTooltip == true)
        {
            SkillToolTip.Instance.DeActive();
        }
    }

    public void SetSkillData(SkillData skillData)
    {
        this.skillData = skillData;
    }

    public void SetToolTip(bool isTooltip)
    {
        this.isTooltip = isTooltip;
    }

    public void InitSkillThumbnail()
    {
        if(skillData == null)
        {
            return;
        }

        AtlasManager.Instance.SetSprite(imgSkill, AtlasManager.Instance.Atlas[skillData.AtlasSkill], skillData.SkillImgPath);

        labelSkillTitle.text = skillData.Title;
        labelSkillDesc.text = skillData.Desc;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(skillData == null)
        {
            return;
        }

        if (isTooltip == true)
        {
            SkillToolTip.Instance.InitSkillToolTip(skillData.Title, skillData.Desc);

            Vector3 toolTipPos = CameraManager.Instance.GetCamera(CameraManager.ECamera.UI).
                WorldToScreenPoint(SkillToolTip.Instance.transform.position);

            SkillToolTip.Instance.SetPosition(
            CameraManager.Instance.GetCamera(CameraManager.ECamera.UI).ScreenToWorldPoint(
                new Vector3(
                    eventData.position.x,
                    eventData.position.y,
                    toolTipPos.z)));


            Vector3 toolTipViewPos = CameraManager.Instance.GetCamera(CameraManager.ECamera.UI).
                WorldToViewportPoint(SkillToolTip.Instance.transform.position);

            SkillToolTip.Instance.SetRotation(toolTipViewPos.x < 0.5f);
            SkillToolTip.Instance.SetAttachToolTipSize();
            SkillToolTip.Instance.Active();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isTooltip == true)
        {
            SkillToolTip.Instance.DeActive();
        }
    }

}
