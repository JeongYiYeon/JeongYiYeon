using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    [SerializeField]
    private GameObject clearGo = null;

    public void ClearActive()
    {
        clearGo.SetActive(true);
    }

    public void ClearDeActive()
    {
        clearGo.SetActive(false);
    }
}
