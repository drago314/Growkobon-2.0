using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelTransitioner : MonoBehaviour
{
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private TMPro.TextMeshProUGUI levelText;

    public void StartLevelTransition(string sceneName)
    {
        levelText.text = sceneName;
        transitionAnimator.SetTrigger("Start");
    }
 
    public void EndLevelTransition()
    {
        transitionAnimator.SetTrigger("End");
    }
}
