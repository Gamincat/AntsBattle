using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;


/// <summary>
/// セーブマネージャー
/// </summary>
public class SaveManager : MonoBehaviour
{
    private static readonly Dictionary<string, ReactiveProperty<float>> FloatsData =
        new Dictionary<string, ReactiveProperty<float>>();
    private static readonly Dictionary<string, ReactiveProperty<int>> IntsData =
        new Dictionary<string, ReactiveProperty<int>>();
    private static readonly Dictionary<string, ReactiveProperty<string>> StringsData =
        new Dictionary<string, ReactiveProperty<string>>();

    // private static List<string> _floatIDs = ;　全てのID列挙が必要になったら、IDを別途保存するようにする
    // private static List<string> _intIDs = ;
    // private static List<string> _stringIDs = ;


    /// <summary>
    /// 対象のReactivePropertyを監視し、値を保存するようにする
    /// 監視を開始する際、該当のデータが存在した場合、値をロードする
    /// </summary>
    /// <param name="id">保存名</param>
    /// <param name="defaultValue">値の初期値</param>
    public static ReactiveProperty<float> GetFloatProperty(string id, float defaultValue = 0f)
    {
        var saveData = FloatsData;
        if (!saveData.ContainsKey(id)) CreateFloat(id, defaultValue);
        return saveData[id];
    }
    public static ReactiveProperty<int> GetIntProperty(string id, int defaultValue = 0)
    {
        var saveData = IntsData;
        if (!saveData.ContainsKey(id)) CreateInt(id, defaultValue);
        return saveData[id];
    }
    public static ReactiveProperty<string> GetStringProperty(string id, string defaultValue = "")
    {
        var saveData = StringsData;
        if (!saveData.ContainsKey(id)) CreateString(id, defaultValue);
        return saveData[id];
    }

    public static ReactiveProperty<T> GetAnyProperty<T>(Dictionary<string, ReactiveProperty<T>> propertyList, string id, T defaultValue)
    {
        var saveData = propertyList;
        if (!saveData.ContainsKey(id)) CreateAny(saveData,id, defaultValue);
        return saveData[id];
    }


    /// <summary>
    /// セーブよう変数を生成
    /// </summary>
    /// <param name="id"></param>
    /// <param name="defaultValue"></param>
    private static void CreateFloat(string id, float defaultValue)
    {
        //Float
        var newValue = new ReactiveProperty<float>(defaultValue);
        FloatsData.Add(id, newValue); //追加
        newValue.Value = PlayerPrefs.GetFloat(id, newValue.Value);
        newValue.Subscribe(value => PlayerPrefs.SetFloat(id, value));
    }
    private static void CreateInt(string id, int defaultValue)
    {
        //Int
        var newValue = new ReactiveProperty<int>(defaultValue);
        IntsData.Add(id, newValue); //追加
        newValue.Value = PlayerPrefs.GetInt(id, newValue.Value);
        newValue.Subscribe(value => PlayerPrefs.SetInt(id, value));
    }
    private static void CreateString(string id, string defaultValue)
    {
        //String
        var newValue = new ReactiveProperty<string>(defaultValue);
        StringsData.Add(id, newValue); //追加
        newValue.Value = PlayerPrefs.GetString(id, newValue.Value);
        newValue.Subscribe(value => PlayerPrefs.SetString(id, value));
    }








    /// <summary>
    /// セーブ用＜T>を生成
    /// </summary>
    /// <param name="propertyList"></param>
    /// <param name="id"></param>
    /// <param name="defaultValue"></param>
    /// <typeparam name="T"></typeparam>
    private static void CreateAny<T>(Dictionary<string, ReactiveProperty<T>> propertyList, string id, T defaultValue)
    {
        var newValue = new ReactiveProperty<T>(defaultValue);
        propertyList.Add(id, newValue); //追加
        newValue.Value = LoadAny(id, defaultValue);
        newValue.Subscribe(value => SaveAny(id, value));
    }


    /// <summary>
    /// 保存していたデータを取得
    /// </summary>
    /// <param name="id"></param>
    /// <param name="defaultValue"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private static T LoadAny<T>(string id, T defaultValue)
    {
        try
        {
            var jsonString = PlayerPrefs.GetString(id);
            return JsonUtility.FromJson<T>(jsonString);
        }
        catch
        {
            return defaultValue;
        }
    }


    /// <summary>
    /// データを保存
    /// </summary>
    /// <param name="id"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private static void SaveAny<T>(string id, T value)
    {
        var jsonValue = JsonUtility.ToJson(value);
        PlayerPrefs.SetString(id, jsonValue);
        PlayerPrefs.Save();
    }


    /// <summary>
    /// 現在保持しているセーブのログを出力
    /// </summary>
    public static string GetLogString()
    {
        var res = "";

        var keys = new List<string>();

        foreach (var kv in FloatsData) keys.Add(kv.Key + ":\t" + kv.Value.ToString() + "\n");
        foreach (var kv in IntsData) keys.Add(kv.Key + ":\t" + kv.Value.ToString() + "\n");
        foreach (var kv in StringsData) keys.Add(kv.Key + ":\t" + kv.Value.ToString() + "\n");

        foreach (var key in keys)
        {
            res += key;
        }

#if UNITY_EDITOR
        Debug.Log(res);
#endif

        return res;
    }
}
