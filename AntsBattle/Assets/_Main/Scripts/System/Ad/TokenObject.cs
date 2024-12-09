using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TokenObject : MonoBehaviour
{
    private CancellationTokenSource _cancellation;
    public CancellationToken CancelToken => _cancellation.Token;



    private void OnDisable()
    {
        Cancel();
    }

    private void OnDestroy()
    {
        _cancellation?.Dispose();
    }

    public void Cancel()
    {
        _cancellation?.Cancel();
    }

    public void ResetToken()
    {
        _cancellation?.Dispose();
        _cancellation = new CancellationTokenSource();
        _cancellation.AddTo(this.GetCancellationTokenOnDestroy());
    }
}