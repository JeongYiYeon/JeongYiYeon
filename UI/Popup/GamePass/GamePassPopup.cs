using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePassPopup : BasePopup
{
    [SerializeField]
    private GamePass gamePass = null;

    private void OnEnable()
    {
        SetType(EPopupType.GamePass);
    }

    public override void Init()
    {
        base.Init();       
    }

    public override void Active()
    {
        base.Active();

        gamePass.SetSeason(0);
        gamePass.SetGPLevel(0);
        gamePass.SetCurrentExp(0);
        gamePass.SetNextExp(0);

        gamePass.InitGamePassData();
    }

    public void SetGamePassSeason(int season)
    {
        gamePass.SetSeason(season);
    }

    public void SetGamePassCurrentExp(int exp)
    {
        gamePass.SetCurrentExp(exp);
    }
    public void SetGamePassLevel(int level)
    {
        gamePass.SetGPLevel(0);
    }
}
