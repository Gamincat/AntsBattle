using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ExInput.WorldButton
{
    public class WorldButton : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onClick;
        [SerializeField] private Collider _selfCollider;
        [SerializeField] private Camera _rayCamera;
        [SerializeField] private float _maxDistance = 100f;


        private void Update()
        {
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }

            var ray = _rayCamera.ScreenPointToRay(Input.mousePosition);
            if (!_selfCollider.Raycast(ray, out var hit, _maxDistance))
            {
                return;
            }

            _onClick?.Invoke();
        }
    }
}
