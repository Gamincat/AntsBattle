using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRandomSpawn : MonoBehaviour
{
    public GameObject InstansePrefab;
    public string ParentName;
    private GameObject ParentObject;
    private GameObject Baby;

    [Tooltip("プレハブを生成する時間間隔のランダムレンジ。[Min,Max]")]
    public Vector2 SpawnCycle = new Vector2(5, 7);

    [Tooltip("プレハブを生成する座標のランダムレンジ。[Min[0],Max[1]]")]
    public Vector3[] InitPosition = new Vector3[2];
    private Vector3 RandomPosition;

    [Tooltip("プレハブを生成する回転のランダムレンジ。[Min[0],Max[1]]")]
    public Vector3[] InitRotation = new Vector3[2];
    private Vector3 RandomRotation;

    private float RandomDelay;
    private float elapseTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        ParentObject = GameObject.Find(ParentName);
        if (ParentObject == null)
        {
            ParentObject = this.gameObject;
        }
        RandomSpot();
    }

    // Update is called once per frame
    void Update()
    {
        elapseTime += Time.deltaTime;
        if(elapseTime > RandomDelay)
        {
            Baby = Instantiate(InstansePrefab);
            Baby.transform.parent = ParentObject.transform;
            Baby.transform.localPosition = RandomPosition;
            Baby.transform.localRotation = Quaternion.Euler(RandomRotation);
            elapseTime = 0f;
            RandomSpot();
        }
    }

    void RandomSpot()
    {
        RandomDelay = Random.Range(SpawnCycle[0], SpawnCycle[1]);
        for(int i = 0; i < 3; i++)
        {
            RandomPosition[i] = Random.Range(InitPosition[0][i], InitPosition[1][i]);
            RandomRotation[i] = Random.Range(InitRotation[0][i], InitRotation[1][i]);
        }
    }
}
