using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public static class ConstRandom
{
    private static readonly Dictionary<string, ReactiveProperty<Random.State>> RandomStatesSaved =
        new Dictionary<string, ReactiveProperty<Random.State>>();

    private static readonly Dictionary<string, ReactiveProperty<Random.State>> RandomStatesNotSaved =
        new Dictionary<string, ReactiveProperty<Random.State>>();


    /// <summary>
    /// セーブするランダムをリセットor生成(ゲーム起動ごとにリセットしたい場合は、ゲーム起動ごとにリセットを呼ぶ）
    /// </summary>
    /// <param name="key">RandomKey</param>
    public static void ResetRandom(string key)
    {
        Random.InitState(key.GetHashCode());
        SaveManager.GetAnyProperty(RandomStatesNotSaved, key, Random.state);
    }


    /// <summary>
    /// 毎回同じ値になるランダムを取得
    /// </summary>
    /// <param name="min">ランダム最小値</param>
    /// <param name="max">ランダム最大値</param>
    /// <param name="key">RandomKey</param>
    /// <param name="isSaved"></param>
    /// <returns></returns>
    public static float Range(float min, float max, string key)
    {
        //使用しようとしていたKeyがまだ存在していなければ、作成する
        if (!RandomStatesNotSaved.ContainsKey(key))
        {
            ResetRandom(key);
        }


        Random.state = RandomStatesNotSaved[key].Value;
        var res = Random.Range(min, max);
        RandomStatesNotSaved[key].Value = Random.state;

#if UNITY_EDITOR
        Debug.Log("ConstRandom." + key + " = " + res);
#endif

        return res;
    }
}
