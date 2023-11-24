using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransitioner : MonoBehaviour
{
    [SerializeField] private Animator transitionAnimator;

    public void StartLevelTransition()
    {
        transitionAnimator.SetTrigger("Start");
    }
 
    public void EndLevelTransition()
    {
        transitionAnimator.SetTrigger("End");
    }
}
