using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableParam : MonoBehaviour
{
    public int MaxLevel => divideCount;

    [SerializeField] private int divideCount;
    [SerializeField] private AnimationCurve levelCurve;


    public float GetLevelParam(int level)
    {
        var weight = Mathf.Clamp(level / Mathf.Max(1, divideCount), 0f, 1f);

        return levelCurve.Evaluate(weight);
    }
}
