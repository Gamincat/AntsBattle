using UnityEngine;
using System;
using System.ComponentModel;


/// <summary>
/// 全般のデバッグ機能
/// </summary>
public partial class SROptions
{
    #region 定数

    /// <summary>
    /// 全般カテゴリ
    /// </summary>
    private const string GeneralCategory = "General";

    #endregion


    #region デバッグ機能
/*
    [Category(GeneralCategory)]
    [DisplayName("ChangeMoney")]
    [Sort(1)]
    [Increment(50)]
    public int ChangeMoney
    {
        get { return MoneyManager.Main.Money.Value; }
        set
        {
            MoneyManager.Main.ChangeMoney(value);
        }
    }
    
    [Category(GeneralCategory)]
    [DisplayName("ResetBinder")]
    [Sort(2)]
    public void ResetBinder()
    {
        Binder.Main.ResetBinder();
    }
    
    [Category(GeneralCategory)]
    [DisplayName("CompleteBinder")]
    [Sort(3)]
    public void CompleteBinder()
    {
        Binder.Main.CompleteBinder();
    }
*/
    #endregion
}