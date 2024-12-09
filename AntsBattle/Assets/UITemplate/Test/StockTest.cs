using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockTest : MonoBehaviour
{
    public Animator[] Elements;
    public int index = 0;
    public string getTrigger = "Get";
    public string releaseTrigger = "Reset";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AnimatorTrigger()
    {
        if (index <= Elements.Length - 1)
        {
            Elements[index].SetTrigger(getTrigger);
            index++;
        }
        else
        {
            for(int i = 0; i < Elements.Length; i++)
            {
                Elements[i].SetTrigger(releaseTrigger);
            }
            index = 0;
        }
    }
}
