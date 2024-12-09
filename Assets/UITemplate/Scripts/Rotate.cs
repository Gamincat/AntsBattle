using System.Collections;
using UnityEngine;

public class Rotate : MonoBehaviour
{

    public float speed = 10f; //回転する速度を設定します。マイナス値にすると、逆回転になります。
    public float sinWave = 0.0f;
    public float initialSinWave = 0.0f;
    public bool initialRandom = false;
    private float initial = 0.0f;
    public Vector3 axis = new Vector3(0, 1, 0);

    private void Start()
    {
        var initialAngle = transform.rotation.y;
        if (initialRandom)
        {
            initial = Random.Range(0, 360);
            transform.Rotate(0, initial, 0);
        }
        if(initialAngle != 0.0f)
        {
            transform.Rotate(0, initialAngle, 0);
        }
    }

    void LateUpdate()
    {
        transform.Rotate
        (
            (speed + Mathf.Sin(Time.time + initialSinWave) * sinWave) * Time.deltaTime * axis.x,
            (speed + Mathf.Sin(Time.time + initialSinWave) * sinWave) * Time.deltaTime * axis.y,
            (speed + Mathf.Sin(Time.time + initialSinWave) * sinWave) * Time.deltaTime * axis.z
        ); //設定した速さで、Y軸に回転します。
    }
}