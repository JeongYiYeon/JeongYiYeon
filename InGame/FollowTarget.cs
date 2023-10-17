using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField]
    private RectTransform followTarget = null;

    private Coroutine CorMoveFollowTarget = null;

    private Action eventCallbackHandler = null;
    private Action<Vector2> moveVec2EventHandler = null;
    private Action<Vector3> moveVec3EventHandler = null;

    public void SetFollowTarget(RectTransform followTarget)
    {
        this.followTarget = followTarget;
    }

    public void AddEventCb(Action cb)
    {
        eventCallbackHandler += cb;
    }

    public void AddVec3Event(Action<Vector3> vec)
    {
        moveVec3EventHandler += vec;
    }

    public void AddVec2Event(Action<Vector2> vec)
    {
        moveVec2EventHandler += vec;
    }

    private void ResetEventCb()
    {
        eventCallbackHandler = null;
    }

    private void ResetVec3Event()
    {
        moveVec3EventHandler = null;
    }
    private void ResetVec2Event()
    {
        moveVec2EventHandler = null;
    }

    public void MoveFollowTarget()
    {
        CorMoveFollowTarget = StartCoroutine(IEMoveFollowTarget());
    }

    public void StopFollowTartget()
    {
        if(CorMoveFollowTarget != null) 
        {
            StopCoroutine(CorMoveFollowTarget);
            CorMoveFollowTarget = null;
            ResetVec3Event();
            ResetVec2Event();
        }
    }

    public IEnumerator IEMoveFollowTarget()
    {
        while(true)
        {
            if (followTarget != null)
            {
                Vector3 characterPos = this.transform.position;
                Vector3 movePos = followTarget.transform.position;

                if (Vector3.Distance(characterPos, movePos) > 0.1f)
                {
                    this.transform.position =
                        Vector3.Lerp(characterPos, movePos, Time.deltaTime * GameManager.Instance.GameSpeed);

                    if(eventCallbackHandler != null)
                    {
                        eventCallbackHandler.Invoke();
                    }

                    if (moveVec3EventHandler != null)
                    {
                        moveVec3EventHandler.Invoke(movePos - characterPos);
                    }

                    if (moveVec2EventHandler != null) 
                    {
                        moveVec2EventHandler.Invoke((Vector2)(movePos - characterPos));
                    }
                }
                else
                {
                    this.transform.position = followTarget.transform.position;

                    if (eventCallbackHandler != null)
                    {
                        ResetEventCb();
                    }

                    if (moveVec3EventHandler != null)
                    {
                        moveVec3EventHandler.Invoke(Vector3.zero);
                        ResetVec3Event();
                    }
                    if (moveVec2EventHandler != null)
                    {
                        moveVec2EventHandler.Invoke(Vector2.zero);
                        ResetVec2Event();
                    }

                    yield break;
                }

                yield return null;
            }
            else
            {
                if (eventCallbackHandler != null)
                {
                    ResetEventCb();
                }
                if (moveVec3EventHandler != null)
                {
                    moveVec3EventHandler.Invoke(Vector3.zero);
                    ResetVec3Event();
                }
                if (moveVec2EventHandler != null)
                {
                    moveVec2EventHandler.Invoke(Vector2.zero);
                    ResetVec2Event();
                }
                yield break;
            }

        }
    }
}
