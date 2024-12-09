using System.Threading;
using Cysharp.Threading.Tasks;
using GaminCat.Analytics;
using UnityEngine;
using UnityEngine.SceneManagement;
//using com.adjust.sdk;
// using Firebase;
// using Firebase.Analytics;
using GaminCat.Analytics;

public class Startup : MonoBehaviour
{
    private void Start()
    {
        LoadAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }


    private async UniTaskVoid LoadAsync(CancellationToken ct)
    {
        var isFirst = !PlayerPrefs.HasKey("FirstFlag");
        if (isFirst)
        {
            PlayerPrefs.SetInt("FirstFlag", 1);
            PlayerPrefs.Save();
        }
        
        
        await UniTask.Delay(200, cancellationToken: ct); //UIを表示するために少し待機
        var nextScene = SceneManager.LoadSceneAsync("Game");
        nextScene.allowSceneActivation = false;
        await UniTask.WaitUntil(() => Time.realtimeSinceStartup > 1f, cancellationToken: ct); //起動から1秒は待つ
//            _ = AdsManager.Instance; //一度インスタンスにアクセスする事で初期化


        var initStartTime = Time.time;
        
        // Adjust
        await AdjustSDKProvider.Init(ct);

        GameAnalytics.Initialize(); 
        
        //Maxの初期化はGDPRシーンで行っている
        AdsManager.Instance.LoadAds();
        
        await UniTask.WaitUntil(() => (Time.time - initStartTime) > 1f, cancellationToken: ct); //バージョン確認用に最低でも1秒表示する

        nextScene.allowSceneActivation = true;
    }
}