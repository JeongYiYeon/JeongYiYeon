using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowDefeatPopup : BasePopup
{
    private void OnEnable()
    {
        SetType(EPopupType.ShowDefeat);
    }
}
