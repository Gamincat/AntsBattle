using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    static ObjectPoolManager _instance;
    public static ObjectPoolManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ObjectPoolManager>();
            }

            return _instance;
        }
    }
    
    [SerializeField] BaseMinionViewModel minionPrefab;  // オブジェクトプールで管理するオブジェクト
    ObjectPool<BaseMinionViewModel> minionPool;  // オブジェクトプール本体

    public void Init()
    {
        minionPool = new ObjectPool<BaseMinionViewModel>(
            createFunc: () => OnCreateObject(),
            actionOnGet: (obj) => OnGetObject(obj),
            actionOnRelease: (obj) => OnReleaseObject(obj),
            actionOnDestroy: (obj) => OnDestroyObject(obj),
            collectionCheck: true,
            defaultCapacity: 3,
            maxSize: 10
        );
    }
    
    // プールからオブジェクトを取得する
    public BaseMinionViewModel GetMinion()
    {
        return minionPool.Get();
    }

    // プールの中身を空にする
    public void ClearMinion()
    {
        minionPool.Clear();
    }

    // プールに入れるインスタンスを新しく生成する際に行う処理
    private BaseMinionViewModel OnCreateObject()
    {
        return Instantiate(minionPrefab, transform);
    }

    // プールからインスタンスを取得した際に行う処理
    private void OnGetObject(BaseMinionViewModel minionObject)
    {
        minionObject.transform.position = Random.insideUnitSphere * 5;
        minionObject.Initialize(() => minionPool.Release(minionObject));
        minionObject.gameObject.SetActive(true);
    }

    // プールにインスタンスを返却した際に行う処理
    private void OnReleaseObject(BaseMinionViewModel enemyObject)
    {
        Debug.Log("Release");  // BaseMinionViewModel側で非アクティブにするのでログ出力のみ。ここで非アクティブにするパターンもある。
    }

    // プールから削除される際に行う処理
    private void OnDestroyObject(BaseMinionViewModel enemyObject)
    {
        Destroy(enemyObject.gameObject);
    }
}
