using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class DisplayFPS : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmp;
    public float span = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Display", span, span);
    }

    // Update is called once per frame
    void Display()
    {
        float fps = 1f / Time.deltaTime;
        tmp.text = "fps:" + fps.ToString("N2");
    }
}
