using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionManager : MonoBehaviour
{
    static MinionManager _instance;
    public static MinionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MinionManager>();
            }

            return _instance;
        }
    }
    
    [SerializeField] BaseCampView playerCamp;
    [SerializeField] BaseCampView enemyCamp;

    BaseCampModel playerCampModel;
    BaseCampModel enemyCampModel;

    int countID;
    
    public void Init()
    {
        countID = 0;
        
        playerCampModel = new BaseCampModel(playerCamp, 1, BaseCampModel.CampType.Player);
        enemyCampModel = new BaseCampModel(enemyCamp, 100, BaseCampModel.CampType.Enemy);
    }
}
