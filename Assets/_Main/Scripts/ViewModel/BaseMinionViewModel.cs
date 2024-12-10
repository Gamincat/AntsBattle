using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BaseMinionViewModel : MonoBehaviour
{
    BaseCampModel.CampType campType;
    
    int id;
    public int GetID { get { return id; } }
    
    Action onDisable;  // 非アクティブ化するためのコールバック

    int hp;
    int attack;
    int speed;
    
    public void Initialize(Action onDisable)
    {
        this.onDisable = onDisable;
    }
    
    public virtual void Init(int id,BaseCampModel.CampType campType)
    {
        this.id = id;
        this.campType = campType;
    }

    public void Death()
    {
        onDisable?.Invoke();
    }
}
