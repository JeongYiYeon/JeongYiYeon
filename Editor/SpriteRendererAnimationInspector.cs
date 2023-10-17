using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CanEditMultipleObjects]
[CustomEditor(typeof(SpriteRendererAnimation), true)]
public class SpriteRendererAnimationInspector : Editor
{
    private SpriteRendererAnimation spriteRenderer;
    private Animation animation;
    private Animator animator;
    private string[] animationNames = null;
    private int selectedIndex = 0;


    private void OnEnable()
    {
        spriteRenderer = target as SpriteRendererAnimation;
        animation = spriteRenderer.GetComponent<Animation>();
        animator = spriteRenderer.GetComponent<Animator>();

        animationNames = null;

        if (animation != null)
         {
            List<string> names = new List<string>();

            foreach (AnimationState state in animation)
            {
                names.Add(state.name);
            }

            if (names.Count > 0)
            {
                animationNames = names.ToArray();
            }
        }
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (targets.Length == 1 && animationNames != null && animationNames.Length > 0)
        {
            EditorGUILayout.Space();

            selectedIndex = EditorGUILayout.Popup("Animations", selectedIndex, animationNames);

            if (GUILayout.Button("PLAY"))
            {
                animation[animationNames[selectedIndex]].wrapMode = WrapMode.Loop;                
                animation.Play(animationNames[selectedIndex]);
            }
        }
    }
}
