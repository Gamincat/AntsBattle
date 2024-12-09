using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Screen = UnityEngine.Device.Screen;

public class MoneyManager : MonoBehaviour
{
    static MoneyManager _main;
    static int RewardCoin => 500;

    public static MoneyManager Main
    {
        get
        {
            if (_main) return _main;
            _main = FindObjectOfType<MoneyManager>(true);
            return _main;
        }
    }

    [SerializeField] ParticleSystem coinParticle;
    [SerializeField] RectTransform coinIcon;
    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] float coinAnimationDuration = 0.1f;
    [SerializeField] float coinAnimationDelay = 0.05f;
    [SerializeField] GameObject loadingCover;
    [SerializeField] GameObject coinDialog;
    [SerializeField] float moneySweepSpeed = 3f;
    [SerializeField] LevelValue moneyValue;

    public LevelValue Money => moneyValue;

    ParticleSystem.Particle[] Particles { get; set; }
    Vector3[] ParticleMovePoints { get; set; }

    int PrevCoinValue { get; set; }

    Transform CoinExitPoint { get; set; }

	public void Init()
	{
 		//Particle
        Particles = new ParticleSystem.Particle[coinParticle.main.maxParticles];
        ParticleMovePoints = new Vector3[coinParticle.main.maxParticles];

        //CoinValue
        var coinValue = Money;
        PrevCoinValue = coinValue.Value;
        coinText.text = ConvertPriceValue(PrevCoinValue);

        coinValue.Subscribe(
            value =>
            {
                var doValueAnimation = DOVirtual.Int(PrevCoinValue, value, coinAnimationDuration,
                    count => { coinText.text = ConvertPriceValue(count); });
                doValueAnimation.SetLink(gameObject);
                if (PrevCoinValue < value) doValueAnimation.SetDelay(coinAnimationDelay);
                PrevCoinValue = value;
            },
            gameObject);

        //UIPoint生成
        CoinExitPoint = new GameObject("CoinExitPoint").transform;
        CoinExitPoint.transform.parent = Camera.main.transform;

		UpdateCoinExitPoint();
	}
    
    void UpdateCoinExitPoint()
    {
        //CoinExitPointのワールド座標をcoinIconの座標に合わせる
        var pos = coinIcon.position;
        pos.z = 3f;
        CoinExitPoint.transform.position = Camera.main.ScreenToWorldPoint(pos);
    }

    /// <summary>
    /// intの数値をstringに変換する、桁表示切り替えを使わないならToStringするだけ
    /// </summary>
    /// <param name="price"></param>
    /// <param name="isCompactUnit">数字の表記をK,Mなどの桁表示するようにするか否か</param>
    /// <returns></returns>
    public static string ConvertPriceValue(int price, bool isCompactUnit = false)
    {
        if (price >= 1000 && isCompactUnit)
        {
            //KMBに対応しつつ、1.111Kといった小数点三位までの表示にも対応する
            return price >= 1000000 ? $"{price / 1000000f:F3}M" :
                price >= 1000 ? $"{price / 1000f:F3}K" : price.ToString();
        }
        else
        {
            return price.ToString();
        }
    }

    public void AddMoney(int coin)
    {
        Money.Value += coin;
    }
    
    public void ChangeMoney(int coin)
    {
        Money.Value = coin;
    }

    public void EmitParticle(int price, Vector3 emitPoint)
    {
        UpdateCoinExitPoint();

        coinParticle.transform.position = emitPoint;
        coinParticle.Emit(price);
    }

    public void GameUpdate()
    {
        var targetPoint = CoinExitPoint.position;

        var particleNum = coinParticle.GetParticles(Particles);
        for (var i = 0; i < particleNum; i++)
        {
            var particle = Particles[i];
            var pos = particle.position;

            var weight = particle.remainingLifetime * moneySweepSpeed;
            
            if (weight >= 1f)
            {
                ParticleMovePoints[i] = pos;
            }
            else
            {
                pos = Vector3.Lerp(targetPoint, ParticleMovePoints[i], weight);
                Particles[i].position = pos;
            }
        }

        coinParticle.SetParticles(Particles, particleNum);
    }
    
    public void SetActive(bool flag)
    {
        gameObject.SetActive(flag);
    }

    public void SubtractMoney(int unlockCoin, Action okAction, Action ngAction)
    {
        if (Money.Value >= unlockCoin)
        {
            //コインが足りているからOK
            Money.Value -= unlockCoin;
            okAction?.Invoke();
        }
        else
        {
            //コインが足りていないのでダイアログ表示
            if (coinDialog) coinDialog.SetActive(true);
            ngAction?.Invoke();
        }
    }
}