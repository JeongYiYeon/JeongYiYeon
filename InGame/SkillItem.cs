using Coffee.UIExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillItem : MonoBehaviour
{
    public enum EHaveSkill
    {
        Skill,
        Empty
    }

    [SerializeField]
    private UtilState state = null;

    [SerializeField]
    private AtlasImage imgSkill = null;
    [SerializeField]
    private SlicedFilledImage imgCoolTime = null;
    [SerializeField]
    private TMP_Text labelCoolTime = null;

    private float activeTime = 0;
    private float remainCoolTime = 0;
    private SkillData skillData = null;

    private bool isCoolTime = false;
    private Vector3 targetPos = Vector3.zero;
    private GameObject targetGo = null;

    private Skill curSkill = null;
    private Coroutine corCheckCoolTime = null;
    private Coroutine corActiveSkill = null;

    private void OnEnable()
    {
        Messenger<Vector3, GameObject>.AddListener(ConstHelper.MessengerString.MSG_ACTIVESKILL, SetTarget);

        StartCoroutine(IEAutoSkill());
    }
    private void OnDisable()
    {
        Messenger<Vector3, GameObject>.RemoveListener(ConstHelper.MessengerString.MSG_ACTIVESKILL, SetTarget);
    }

    public void SetSkillData(SkillData skillData)
    {
        this.skillData = skillData;
    }

    public void InitSkllItem()
    {
        ResetSkillCor();
        AtlasManager.Instance.SetSprite(imgSkill, AtlasManager.Instance.Atlas[skillData.AtlasSkill], skillData.SkillImgPath);
        imgCoolTime.fillAmount = 0;
        remainCoolTime = 0;
        activeTime = 0;
        isCoolTime = false;
        targetPos = Vector3.zero;
        targetGo = null;
        imgCoolTime.gameObject.SetActive(false);
        labelCoolTime.text = remainCoolTime.ToString();
    }

    public void SetState(EHaveSkill state)
    {
        this.state.ActiveState(state.ToString());
    }

    private void ResetSkillCor()
    {
        if (corCheckCoolTime != null)
        {
            StopCoroutine(corCheckCoolTime);
            corCheckCoolTime = null;
        }

        if (corActiveSkill != null)
        {
            StopCoroutine(corActiveSkill);
            corActiveSkill = null;

            if(curSkill != null)
            {
                SkillManager.Instance.SkillPool.Release(curSkill);
                curSkill = null;
            }
        }
    }

    private IEnumerator IEActiveSkill()
    {
        SkillManager.Instance.SkillPool.Get(out Skill go);

        if (go != null)
        {
            curSkill = go;
            go.SetSkill(skillData);
            go.transform.position = targetPos;
            go.ActiveSkill(targetGo);
        }

        while (true)
        {
            activeTime += Time.deltaTime * GameManager.Instance.GameSpeed;

            //if (activeTime >= skillData.Time)
            if (activeTime >= 3f)
            {
                activeTime = 0f;

                targetPos = Vector3.zero;
                targetGo = null;
                break;
            }

            yield return null;
        }

        if (go != null)
        {
            SkillManager.Instance.SkillPool.Release(go);
            curSkill = null;
        }
    }

    private IEnumerator IECheckCoolTime()
    {
        isCoolTime = true;
        imgCoolTime.fillAmount = 1f;
        imgCoolTime.gameObject.SetActive(true);

        while (true)
        {
            remainCoolTime += Time.deltaTime * GameManager.Instance.GameSpeed;

            if (remainCoolTime >= skillData.CoolTime)
            {
                remainCoolTime = 0f;
                imgCoolTime.fillAmount = 0f;
                targetPos = Vector3.zero;
                targetGo = null;
                imgCoolTime.gameObject.SetActive(false);

                isCoolTime = false;
                break;
            }

            imgCoolTime.fillAmount = Mathf.Lerp(imgCoolTime.fillAmount, 1f - (float)(remainCoolTime / skillData.CoolTime), remainCoolTime);
            labelCoolTime.text = $"{(int)(skillData.CoolTime - remainCoolTime)}";

            yield return null;
        }
    }

    private void SetTarget(Vector3 pos, GameObject targetGo)
    {
        targetPos = pos;
        this.targetGo = targetGo;
    }

    private IEnumerator IEAutoSkill()
    {
        while(true)
        {
            yield return new WaitUntil(() => GameManager.Instance.IsAuto);

            if (skillData == null)
            {
                continue;
            }

            if (targetPos == Vector3.zero)
            {
                continue;
            }

            if (targetGo == null)
            {
                continue;
            }

            if (isCoolTime == true)
            {
                continue;
            }

            ResetSkillCor();
            corCheckCoolTime = StartCoroutine(IECheckCoolTime());
            corActiveSkill = StartCoroutine(IEActiveSkill());
        }
    }

    public void OnClickActiveSkill()
    {
        if(skillData == null)
        {
            return;
        }

        if (targetPos == Vector3.zero)
        {
            return;
        }

        if (targetGo == null)
        {
            return;
        }

        if (isCoolTime == true)
        {
            return;
        }

        if (GameManager.Instance.IsAuto == true)
        {
            return;
        }

        ResetSkillCor();
        corCheckCoolTime = StartCoroutine(IECheckCoolTime());
        corActiveSkill = StartCoroutine(IEActiveSkill());
    }

}
