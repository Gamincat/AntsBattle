using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTrigger : MonoBehaviour
{
    public Animator anim;
    public string triggerName;


    private void OnEnable()
    {
        if (anim)
            anim.SetTrigger(triggerName);
    }
}
