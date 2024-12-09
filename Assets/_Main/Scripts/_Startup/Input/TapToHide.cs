using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExInput
{
    public class TapToHide : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                gameObject.SetActive(false);
            }
        }
    }
}
