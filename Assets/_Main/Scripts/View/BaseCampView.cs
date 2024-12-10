using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCampView : MonoBehaviour
{
    [SerializeField] Transform spawnMinionPos;

    BaseCampModel model;
    
    public virtual void Init(BaseCampModel model)
    {
        this.model = model;
    }
    
}
