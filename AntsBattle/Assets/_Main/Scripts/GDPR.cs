using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GDPR : MonoBehaviour
{
    public bool IsComplete { get; private set; }

    public static bool IsGDPRTarget =>
        MaxSdk.IsInitialized() && MaxSdk.GetSdkConfiguration().ConsentFlowUserGeography ==
        MaxSdkBase.ConsentFlowUserGeography.Gdpr;

    private void Start()
    {
        Init().Forget();
    }

    private async UniTaskVoid Init()
    {
        var ct = gameObject.GetCancellationTokenOnDestroy();
        AdsManager.Instance.InitAds(ct);
        await UniTask.WaitUntil(() => AdsManager.Instance.IsInitialize && MaxSdk.IsInitialized(),
            cancellationToken: ct);

        if (!IsGDPRTarget)
        {
            IsComplete = true;
            SceneManager.LoadScene("Init");
        }
        else
        {
            //GDPR圏は同意ステータスが取れないので送信しない
            const bool analyticsConsent = false;


#if UNITY_EDITOR
            Debug.Log($"GDPR. Status={MaxSdk.GetSdkConfiguration().ConsentFlowUserGeography}");
#endif


            IsComplete = true;
            SceneManager.LoadScene("Init");
        }
    }

    public static void ShowDynamic()
    {
        MaxSdk.CmpService.ShowCmpForExistingUser(null);
    }
}