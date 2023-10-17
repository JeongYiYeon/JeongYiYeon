using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageProgress : MonoBehaviour
{
    public enum EStageFlag
    {
        None,
        Flag,
        Boss
    }

    [SerializeField]
    private TMP_Text labelStageName = null;

    [SerializeField]
    private HorizontalLayoutGroup horizontalLayout = null;

    [SerializeField]
    private Flag[] flags = null;
    [SerializeField]
    private Transform bossFlag = null;

    [SerializeField]
    private UtilState state = null;

    [SerializeField]
    private GameObject currentWaveArrow = null;

    private bool isFlag = false;

    private Sequence arrowTweenSeq = null;

    public void SetStageName(string stageName)
    {
        labelStageName.text = stageName;
    }

    public void SetFlags(Canvas uiCanvas)
    {
        StartCoroutine(IESetFlags(uiCanvas));
    }

    public void SetState(EStageFlag flag)
    {
        if (flag == EStageFlag.None)
        {
            state.ResetState();
        }
        else
        {
            state.ActiveState(flag.ToString());
        }
    }

    private IEnumerator IESetFlags(Canvas uiCanvas)
    {
        isFlag = false;

        yield return new WaitUntil(() => uiCanvas.GetComponent<UtilResolutionCanvas>().IsSync);

        for (int i = 0; i < flags.Length; i++)
        {
            flags[i].ClearDeActive();
            flags[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < GameManager.Instance.WaveCnt; i++)
        {
            if (i != GameManager.Instance.WaveCnt - 1)
            {
                flags[i].gameObject.SetActive(true);
                isFlag = true;
            }
        }

        bossFlag.gameObject.SetActive(!UserData.Instance.user.Stage.IsEnterBoss);

        //플레그 재정렬
        horizontalLayout.SetLayoutHorizontal();

        yield return new WaitForFixedUpdate();

        if (isFlag == true)
        {
            if (arrowTweenSeq != null)
            {
                arrowTweenSeq.Kill();
                arrowTweenSeq = null;
            }

            currentWaveArrow.transform.position =
                        new Vector3(flags[0].transform.position.x,
                        flags[0].transform.position.y,
                        currentWaveArrow.transform.position.z);

            TweenArrow();
        }
        else
        {
            currentWaveArrow.transform.position = new Vector3(0, bossFlag.transform.position.y, currentWaveArrow.transform.position.z);
            TweenArrow();
        }
    }

    public IEnumerator IEWaveProgressBar()
    {
        if (isFlag == true)
        {
            if (arrowTweenSeq != null)
            {
                arrowTweenSeq.Kill();
                arrowTweenSeq = null;

                if (GameManager.Instance.WaveClearCnt > 0)
                {
                    currentWaveArrow.transform.position = new Vector3(
                        flags[GameManager.Instance.WaveClearCnt - 1].transform.position.x,
                        flags[GameManager.Instance.WaveClearCnt - 1].transform.position.y,
                        currentWaveArrow.transform.position.z);
                    flags[GameManager.Instance.WaveClearCnt - 1].ClearActive();
                }
                else
                {
                    currentWaveArrow.transform.position =
                        new Vector3(flags[0].transform.position.x,
                        flags[0].transform.position.y,
                        currentWaveArrow.transform.position.z);
                }
            }

            Vector3 tmpPos = Vector3.zero;

            if (GameManager.Instance.WaveClearCnt < GameManager.Instance.WaveCnt - 1)
            {
                tmpPos = flags[GameManager.Instance.WaveClearCnt].transform.position;
            }
            else
            {
                tmpPos = bossFlag.position;
            }

            yield return TweenMoveArrow(tmpPos).WaitForCompletion();

            TweenArrow();
        }
        else
        {
            currentWaveArrow.transform.position = new Vector3(0, 0, currentWaveArrow.transform.position.z);
            TweenArrow();
        }
    }

    private Sequence TweenMoveArrow(Vector3 targetPos)
    {
        if (arrowTweenSeq == null)
        {
            arrowTweenSeq = DOTween.Sequence();
        }

        arrowTweenSeq.Append(currentWaveArrow.transform.DOMove(targetPos, 0.5f / GameManager.Instance.GameSpeed));
        arrowTweenSeq.Play();

        return arrowTweenSeq;
    }

    private void TweenArrow()
    {
        if (arrowTweenSeq != null)
        {
            arrowTweenSeq.Kill();
            arrowTweenSeq = null;
            arrowTweenSeq = DOTween.Sequence();
        }
        else
        {
            arrowTweenSeq = DOTween.Sequence();
        }

        arrowTweenSeq.SetLoops(-1, LoopType.Yoyo);        
        arrowTweenSeq.Append(currentWaveArrow.transform.DOLocalMoveY(currentWaveArrow.transform.localPosition.y + 30f, 0.5f / GameManager.Instance.GameSpeed));
        arrowTweenSeq.Play();
    }
}
