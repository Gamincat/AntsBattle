using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UniRx;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelValue : SaveManager
{
    [SerializeField] public int defaultValue = 0;
    [SerializeField] private string constID = "";
    [SerializeField] private bool isUpOnly = false;

#if UNITY_EDITOR
    [SerializeField, ReadOnly] private int debugViewValue;
#endif


    private bool IsConstIDActive => constID.Length > 0;

    public string Path
    {
        get
        {
            //このオブジェクトの階層を遡っていき、gameObject.nameを階層のように扱う文字列を返す
            var path = "";
            var currentTransform = transform;
            while (currentTransform != null)
            {
                path = currentTransform.name + "/" + path;
                currentTransform = currentTransform.parent;
            }

            path = path.TrimEnd('/');
            return path;
        }
    }

    private ReactiveProperty<int> Level => GetIntProperty(IsConstIDActive ? constID : gameObject.name, defaultValue);

    [ContextMenu("SetConstIDToPath")]
    public void SetConstIDToPath()
    {
#if UNITY_EDITOR
//ダーティとしてマークする(変更があった事を記録する)
        EditorUtility.SetDirty(gameObject);
        constID = Path;
//保存する
        AssetDatabase.SaveAssets();
#endif
    }

    public void ValueMove(int value)
    {
        Value += value;
    }

    public void Increment()
    {
        Value++;
    }

    public void Decrement()
    {
        Value--;
    }

    #region UniRxOverRides

    public IDisposable Subscribe(Action<int> onNext, GameObject tokenObject)
    {
        var disposable = Level.Subscribe(onNext);
        disposable.AddTo(tokenObject);
        return disposable;
    }


    public int Value
    {
        get
        {
#if UNITY_EDITOR
            debugViewValue = Level.Value;
#endif

            return Level.Value;
        }
        set
        {
            var res = isUpOnly ? Mathf.Max(value, Level.Value) : value;
#if UNITY_EDITOR
            debugViewValue = res;
#endif
            Level.Value = res;
        }
    }

    #endregion
}