using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExInput
{
    public class SwipeInput : MonoBehaviour
    {
        [SerializeField] private float inputScale = 1000f;
        [SerializeField] private float limitDistance = 15f;
        [SerializeField] private float accelSpeed = 5f;
        private Vector3 _startInputPoint;
        private Vector3 _currentInputPoint;
        private Vector3 _prevInputPoint;
        private bool _currentTap;
        private bool _prevTap;


        private float _swipePathMagnitude;
        private Vector3 _lastVector;

        //
        public Vector3 InputVector
        {
            get
            {
                var vel = _currentInputPoint - _startInputPoint;

                vel = vel.normalized * Mathf.Min(limitDistance, vel.magnitude);

                return vel;
            }
        }

        public bool IsTapEnter => _currentTap && !_prevTap;
        public bool IsTapStay => (_currentTap && _prevTap) || IsTapEnter;
        public bool IsTapExit => !_currentTap && _prevTap;
        public Vector3 Offset => _currentInputPoint - _prevInputPoint;


        private void OnDisable()
        {
            _currentTap = false;
            _prevTap = false;
            ResetInputVector();
        }


        private void Update()
        {
            TapMove();
        }


        private void TapMove()
        {
            _prevTap = _currentTap;
            _prevInputPoint = _currentInputPoint;
            _currentTap = Input.GetMouseButton(0);
            if (!IsTapStay)
            {
                if (IsTapExit)
                {
                    ResetInputVector();
                }

                return;
            }

            _currentInputPoint = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            _currentInputPoint.x -= 0.5f;
            _currentInputPoint.y -= 0.5f;
            _currentInputPoint *= 2f;

            var verticalScale = Screen.height / Screen.width;
            _currentInputPoint.y *= verticalScale;

            _currentInputPoint *= inputScale;
            if (IsTapEnter) ResetInputVector();

            if (!(Offset.magnitude > 0)) return;

            _lastVector = Vector3.Lerp(_lastVector, Offset, Time.deltaTime * accelSpeed);
            _swipePathMagnitude = Mathf.Lerp(_swipePathMagnitude, _lastVector.magnitude, Time.deltaTime * 15f);
        }


        public void ResetInputVector()
        {
            _lastVector = Vector3.zero;

            _swipePathMagnitude = 0f;
            _prevInputPoint = _currentInputPoint;
            _startInputPoint = _currentInputPoint;
        }
    }
}
