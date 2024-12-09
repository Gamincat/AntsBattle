using UnityEngine;

namespace GamincatKit.Common
{
    public class DontDestroyOnLoadGameObject : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}