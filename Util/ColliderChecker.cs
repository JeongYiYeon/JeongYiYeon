using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderChecker : MonoBehaviour
{
    [SerializeField]
    private GameObject targetGo;

    private Collider2D collider;

    private bool isCollider = false;

    public bool IsCollider => isCollider;

    private void OnEnable()
    {
        collider = GetComponent<Collider2D>();
    }

    private void OnDisable()
    {
        isCollider = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == targetGo && 
            collision.bounds.Contains(collider.bounds.min) &&
            collision.bounds.Contains(collider.bounds.max))
        {
            isCollider = true;
        }
        else
        {
            isCollider = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject == targetGo &&
            collision.bounds.Contains(collider.bounds.min) &&
            collision.bounds.Contains(collider.bounds.max))
        {
            isCollider = true;
        }
        else
        {
            isCollider = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isCollider = false;
    }
}
