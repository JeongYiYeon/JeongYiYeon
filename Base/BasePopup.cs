using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Events;

public class BasePopup : MonoBehaviour
{
    /// <summary>
    /// 각 팝업의 타입을 선언해서 매칭되는 팝업 생성
    /// </summary>
    public enum EPopupType
    {
        None,
        Alram,                  // 기본 팝업창 (확인, 취소)
        PrivacyPolicy,          // 개인정보 및 약관
        AdBuff,                 // 광고 버프
        Setting,                // 셋팅
        Mail,                   // 메일
        Attendance,             // 출첵
        GamePass,               // 게임패스
        ShowRewardItems,        // 득템 확인창
        ItemInfo,               // 아이템 정보
        HeroList,               // 영웅 목록
        HeroUpgrade,            // 영웅 성장
        ItemUpgrade,            // 아이템 업그레이드
        ConnectReward,          // 접속 보상
        Inventory,              // 인벤토리
        AllSell,                // 일괄판매
        ShowDefeat,             // 패배창
    }

    public enum EButtonType
    {
        None = -1,
        One,
        Two,
    }

    private protected string title;
    private protected string desc;
    private protected string confirmBtTitle;
    private protected string cancelBtTitle;

    private protected EPopupType type = EPopupType.None;
    private protected EButtonType btType = EButtonType.One;

    private protected bool isDestroy = false;

    //뒤로가기 기본적으론 허용
    private protected bool isBackKeyOn = true;

    private bool isAnimationPlay = false;

    private Action onCancelCallback;
    private Action onConfirmCallback;

    private void OnDisable()
    {
        isAnimationPlay = false;

        if (onCancelCallback != null)
        {
            onCancelCallback = null;
        }

        if (onConfirmCallback != null)
        {
            onConfirmCallback = null;
        }

        if (isDestroy == true)
        {
            Destroy(this.gameObject);
        }

    }

    private void Awake()
    {
        Init();
    }

#if UNITY_EDITOR || UNITY_ANDROID
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (isBackKeyOn == true)
            {
                OnClickCancel();
            }
        }
    }
#endif

    public virtual void Init()
    {

    }


    public virtual void Active()
    {
        if (this.gameObject.activeSelf)
        {
            return;
        }

        this.gameObject.transform.SetAsLastSibling();
        this.gameObject.SetActive(true);
    }

    public virtual void DeActive()
    {
        if (this.gameObject.activeSelf && isAnimationPlay == false)
        {
            StartCoroutine(IEDeActive(() => { this.gameObject.SetActive(false); }));
        }
    }

    public virtual void ResetLabel()
    {
       
    }

    public virtual void Reset()
    {

    }

    private IEnumerator IEDeActive(Action closeCb = null)
    {
        if (isAnimationPlay == false)
        {
            isAnimationPlay = true;
            float time = 0;
            float checkTime = 0;

            Animator animator = this.GetComponent<Animator>();

            animator.SetBool("isHide", true);

            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Hide"));

            time = animator.GetCurrentAnimatorStateInfo(0).length;

            while (true)
            {
                checkTime += Time.deltaTime;

                if (time < checkTime && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f )
                {
                    animator.SetBool("isHide", false);

                    if (closeCb != null)
                    {
                        closeCb.Invoke();
                    }

                    isAnimationPlay = false;

                    this.gameObject.SetActive(false);

                    yield break;
                }

                yield return null;
            }
        }
        else
        {
            yield break;
        }
    }

    #region Set    
    private protected void SetType(EPopupType type)
    {
        this.type = type;
    }
    public virtual void SetButtonType(EButtonType type)
    {
        btType = type;
    }

    public void SetTitle(string title)
    {
        this.title = title;
    }
    public void SetDesc(string desc)
    {
        this.desc = desc;
    }
    public void SetConfirmBtLabel(string desc)
    {
        confirmBtTitle = desc;
    }
    public void SetCancelBtLabel(string desc)
    {
        cancelBtTitle = desc;
    }

    /// <summary>
    /// 팝업을 꺼지면 객체를 삭제 (기본 false)
    /// </summary>
    /// <param name="isDestroy"></param>
    public void SetDestory(bool isDestroy)
    {
        this.isDestroy = isDestroy;
    }

    /// <summary>
    /// 팝업 백키 허용 (기본 true)
    /// </summary>
    /// <param name="isBackKeyOn"></param>
    public void SetBackKeyOn(bool isBackKeyOn)
    {
        this.isBackKeyOn = isBackKeyOn;
    }

    public virtual void SetCancelCallback(Action cb)
    {
        if (cb != null)
        {
            onCancelCallback = cb;
        }
    }

    public virtual void SetConfirmCallBack(Action cb)
    {
        if (cb != null)
        {
            onConfirmCallback = cb;
        }
    }
    #endregion

    #region Get

    #endregion

    #region OnClick

    //백판 종료용
    public void OnClickBg()
    {
        if(isBackKeyOn == true)
        {
            OnClickCancel();
        }
    }

    public virtual void OnClickCancel()
    {
        if (onCancelCallback != null)
        {
            LoadingManager.Instance.ActiveLoading();

            StartCoroutine(IEDeActive(() => 
            {
                onCancelCallback.Invoke();
                LoadingManager.Instance.Reset(LoadingManager.EState.Loading);
            }));
        }
        else
        {
            DeActive();
        }
    }
    public virtual void OnClickConfirm()
    {
        if (onConfirmCallback != null)
        {
            LoadingManager.Instance.ActiveLoading();

            StartCoroutine(IEDeActive(() => 
            {
                onConfirmCallback.Invoke();
                LoadingManager.Instance.Reset(LoadingManager.EState.Loading);
            }));
        }
        else
        {
            DeActive();
        }
    }
    #endregion
}
