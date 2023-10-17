using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilPlayEffect : MonoBehaviour
{
    [SerializeField]
    private UtilState state = null;

    private ParticleSystem[] effects = null;
    private float duration = 0;

    private bool isPlaying = false;

    private void InitEffects()
    {
        if (state != null)
        {
            effects = state.transform.Find(state.ActiveStateName()).GetComponentsInChildren<ParticleSystem>(true);
        }
        else
        {
            effects = this.GetComponentsInChildren<ParticleSystem>(true);
        }

        GetDuration();
    }

    private void GetDuration()
    {
        if (effects == null || effects.Length == 0)
        {
            return;
        }

        for (int i = 0; i < effects.Length; i++)
        {
            if(effects[i].main.duration > duration)
            {
                duration = effects[i].main.duration;
            }
        }
    }

    public void PlayEffect(float speed, string stateName = "", bool isSyncRotation = true, bool isForce = false, Action successCb = null)
    {
        if (isSyncRotation == true)
        {
            this.transform.localRotation = Quaternion.Euler(0, -this.transform.parent.eulerAngles.y, 0);
        }

        if (string.IsNullOrEmpty(stateName) == false)
        {
            if (state != null)
            {
                if (isForce == false)
                {
                    if (isPlaying == true)
                    {
                        if (state.ActiveStateName() == stateName)
                        {
                            return;
                        }
                    }

                    if (state.ActiveStateName() != stateName)
                    {
                        state.ActiveState(stateName);
                    }
                }
                else
                {
                    state.ActiveState(stateName);
                }
            }
        }

        InitEffects();

        StartCoroutine(IEPlayEffect(speed, successCb));
    }

    private IEnumerator IEPlayEffect(float speed, Action successCb = null)
    {
        if (effects == null || effects.Length == 0)
        {
            state.ResetState();

            if (successCb != null)
            {
                successCb.Invoke();
            }

            isPlaying = false;

            yield break;
        }

        isPlaying = true;

        if (speed > 0f)
        {
            for (int i = 0; i < effects.Length; i++)
            {
                ParticleSystem.MainModule main = effects[i].main;

                main.simulationSpeed = speed;
            }
        }
        else
        {
            speed = 1f;
        }

        for (int i = 0; i < effects.Length; i++)
        {
            effects[i].Play();
        }

        yield return new WaitForSeconds(duration / speed);

        state.ResetState();

        if (successCb!= null) 
        {
            successCb.Invoke();
        }

        isPlaying = false;
    }
}
