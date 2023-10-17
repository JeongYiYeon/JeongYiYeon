using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilUIParticle : MonoBehaviour
{
    [SerializeField]
    private Canvas UICanvas = null;

    private ParticleSystemRenderer[] effects = null;

    private void Awake()
    {
        effects = this.GetComponentsInChildren<ParticleSystemRenderer>(true);
        SetParticleLayer();
    }

    private void SetParticleLayer()
    {
        if(effects == null || effects.Length == 0)
        {
            return;
        }

        for(int i = 0; i < effects.Length; i++)
        {
            //하나씩 높게
            effects[i].sortingOrder = UICanvas.sortingOrder + 1 + i;
        }
    }
}
