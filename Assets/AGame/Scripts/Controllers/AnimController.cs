using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    [SerializeField] public Animator anim;

    private string currentAnim = "";

    public void ChangeAnim(string animName)
    {
        if (anim == null) return;

        if (currentAnim != animName)
        {
            if (!string.IsNullOrEmpty(animName))
            {
                if (!string.IsNullOrEmpty(currentAnim))
                {
                    anim.ResetTrigger(currentAnim);
                }
                currentAnim = animName;
                anim.SetTrigger(currentAnim);
            }
            else
            {
                currentAnim = "";
            }
        }
    }
}
