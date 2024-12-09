using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public static class PoolManager
{
    private static readonly Dictionary<string, Transform> QueueViews = new Dictionary<string, Transform>();

    private static readonly Dictionary<string, object> QueueModelsDisable = new Dictionary<string, object>();


    private static GameObject parentObject;

    private static void Check<T>(string queueName) where T : Component
    {
        if (QueueModelsDisable.ContainsKey(queueName)) return;

        QueueModelsDisable.Add(queueName, new Queue<T>());

        if (!parentObject)
        {
            parentObject = new GameObject("Parent");
            GameObject.DontDestroyOnLoad(parentObject);
        }

        var obj = new GameObject(queueName)
        {
            transform =
            {
                parent = parentObject.transform
            }
        };

        QueueViews.Add(queueName, obj.transform);
    }


    private static T NewObject<T>(string queueName, T original, bool active = false) where T : Component
    {
        var obj = Object.Instantiate(original);

        var gameObject = obj.gameObject;
        gameObject.name = queueName;
        gameObject.transform.SetParent(QueueViews[queueName]);
        gameObject.SetActive(active);


        return obj;
    }


    /// 指定したオブジェクトを、countになるよう追加で増やす、キューの中身がすでにcount以上あったら何もしない
    // public static void InitQueueArray(Component original, int count)
    // {
    //     var queueName = original.gameObject.name;
    //     Check(queueName);
    //     var queue = QueueModels[queueName];
    //     for (var i = queue.Count; i < count; i++)
    //     {
    //         var obj = NewObject(queueName, original);
    //         ActiveObjects.Remove(obj.transform);
    //         queue.Enqueue(obj);
    //     }
    // }

    //プールから取り出す
    public static T InstantiateObject<T>(T original, bool active, bool isAutoNew) where T : Component
    {
        var queueName = original.gameObject.name;
        Check<T>(queueName);

        var queueDisable = QueueModelsDisable[original.gameObject.name] as Queue<T>;


        var obj = Dequeue(original, isAutoNew, queueDisable, queueName);
        if (obj) obj.gameObject.SetActive(active);


        return obj;
    }


    private static T Dequeue<T>(T original, bool isAutoNew, Queue<T> queue, string queueName) where T : Component
    {
        T obj = null;


        if (queue.Count > 0)
        {
            obj = queue.Dequeue(); //プールの中身があればそれを取り出して返す
        }
        else if (isAutoNew)
        {
            obj = NewObject(queueName, original); //isAutoNewによって生成
        }


        return obj;
    }


    //プールに返す
    public static void RePool<T>(T obj) where T : Component
    {
        var queueName = obj.gameObject.name;
        Check<T>(queueName);
        obj.transform.parent = QueueViews[queueName];
        obj.gameObject.SetActive(false);

        var queueDisable = QueueModelsDisable[queueName] as Queue<T>;


        if (queueDisable.Contains(obj)) return;


        obj.transform.position = Vector3.zero;

        queueDisable.Enqueue(obj);
    }


    public static Transform GetQueueParent<T>(T obj) where T : Component
    {
        var queueName = obj.gameObject.name;
        Check<T>(queueName);
        return QueueViews[queueName];
    }


    private static List<T> ToType<T>(T type, object obj) where T : Component
    {
        return obj as List<T>;
    }
}