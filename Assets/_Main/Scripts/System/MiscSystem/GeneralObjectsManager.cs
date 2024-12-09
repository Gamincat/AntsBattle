using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralObjectsManager : MonoSingleton<GeneralObjectsManager>
{
    protected void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 例外的なシーンなどで全般オブジェクトを非表示にしたい場合のための関数
    /// </summary>
    /// <param name="flag"></param>
    public void SetActive(bool flag)
    {
        gameObject.SetActive(flag);
    }
}