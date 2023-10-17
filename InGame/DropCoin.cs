using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using DG.Tweening;

public class DropCoin : MonoBehaviour
{
    private IObjectPool<DropCoin> pool = null;

    private Vector3 startPos = Vector3.zero;
    private RectTransform endTf = null;

    private void OnDisable()
    {
        startPos = Vector3.zero;
        endTf = null;
    }    

    public void SetDropCoin(IObjectPool<DropCoin> pool)
    {
        this.pool = pool;
    }

    public void SetPos(Vector3 startPos, RectTransform endTf)
    {
        this.startPos = startPos;
        this.endTf = endTf;
    }

    public void TweenDropCoin()
    {
        this.transform.position = startPos;
        this.GetComponent<RectTransform>().pivot = endTf.pivot;

        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() => { this.gameObject.SetActive(true);});
        seq.Append(this.transform.DOJump(transform.TransformPoint(startPos), 0.3f, 1, 0.5f / GameManager.Instance.GameSpeed).SetEase(Ease.InQuad));
        seq.Append(this.transform.DOJump(endTf.position, 0.3f, 1, 0.7f / GameManager.Instance.GameSpeed));
        seq.AppendCallback(() => { this.gameObject.SetActive(false); });
        seq.Append(endTf.DOScale(0.95f, 0.025f));
        seq.Append(endTf.DOScale(1.5f, 0.05f));
        seq.Append(endTf.DOScale(1f, 0.1f));
        seq.AppendCallback(() => { pool.Release(this); });
        seq.Play();
    }
}
