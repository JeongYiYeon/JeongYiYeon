using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class AlramLabelController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text labelTitle = null;
    [SerializeField]
    private TMP_Text labelDsec = null;

    public void Active()
    {
        this.gameObject.SetActive(true);
    }
    public void DeActive()
    {
        this.gameObject.SetActive(false);
    }

    public void SetTitleLabel(string text)
    {
        labelTitle.text = text;
    }
    public void SetDescLabel(string text)
    {
        labelDsec.text = text;
    }

    public void SetWarningLabel(float duration, out float tweenDuration)
    {
        Sequence sequence = DOTween.Sequence()
            .SetAutoKill(false)
            .OnStart(() =>
            {
                labelDsec.color = Color.white;
                labelDsec.transform.localScale = Vector3.one;
            });

        DOTweenTMPAnimator tmproAnimator = new DOTweenTMPAnimator(labelDsec);
        for (int i = 0; i < tmproAnimator.textInfo.characterCount; i++)
        {
            sequence.Append(tmproAnimator.DOColorChar(i, Color.red, 0.15f));
            sequence.Join(tmproAnimator.DOOffsetChar(i, tmproAnimator.GetCharOffset(i) + new Vector3(0, 10f, 0), 0.15f).SetEase(Ease.OutFlash, 2f));
            sequence.Join(tmproAnimator.DOFadeChar(i, 1, 0.15f));
            sequence.Join(tmproAnimator.DOScaleChar(i, 1, 0.15f).SetEase(Ease.OutBack));
        }

        sequence.Join(labelDsec.DOScale(1.3f, duration).SetEase(Ease.OutBounce).SetLoops(-1));

        sequence.Restart();

        tweenDuration = sequence.Duration();
    }
}
