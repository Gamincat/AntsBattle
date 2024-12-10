using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }

            return _instance;
        }
    }

    int money;
    
    public enum GamePhase
    {
        None,
        Home,
        Battle,
        Result,
    }
    GamePhase gamePhase;
    public GamePhase GetGamePhase { get { return gamePhase; } }

    void Awake()
    {
        gamePhase = GamePhase.Home;
        
        ObjectPoolManager.Instance.Init();
        UIManager.Instance.Init();
        MinionManager.Instance.Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
