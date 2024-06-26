using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    [SerializeField] public Animator anim;

    private string currentAnim = "";

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
