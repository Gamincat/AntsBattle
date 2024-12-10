using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCampModel
{   
    public enum CampType
    {
        None,
        Player,
        Enemy,
    }
    CampType campType;
    public CampType GetCampType { get { return campType; } }

    BaseCampView view;
    List<BaseMinionViewModel> minionList;

    int hp;
    float spawnRate;
    
    public BaseCampModel(BaseCampView view,int hp,CampType campType)
    {
        this.hp = hp;
        this.campType = campType;
        this.view = view;

        minionList = new List<BaseMinionViewModel>();
        
        view.Init(this);
    }
}
