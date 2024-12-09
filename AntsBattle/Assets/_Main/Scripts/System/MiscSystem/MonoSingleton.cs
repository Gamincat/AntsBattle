using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _singleton;

    public static T Singleton
    {
        get
        {
            if (_singleton) return _singleton;

            // インスタンスがまだ存在しない場合はシーン内から検索
            _singleton = FindObjectOfType<T>(true);
            if (_singleton == null)
            {
                Debug.LogError($"{typeof(T).Name} シングルトンインスタンスが見つかりません。シーンに追加してください。");
            }

            return _singleton;
        }
    }

    protected virtual void Awake()
    {
        // 派生クラスでの多重インスタンスチェック、絶対に呼ぶ必要がある訳ではない
        if (_singleton != null && _singleton != this)
        {
            Debug.LogError($"{typeof(T).Name} の複数のインスタンスが検出されました。シングルトンは一つだけ必要です。");
            Destroy(gameObject);
        }
        else
        {
            _singleton = this as T;
        }
    }
}