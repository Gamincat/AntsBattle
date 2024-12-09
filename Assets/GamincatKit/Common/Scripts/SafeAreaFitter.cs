using System;
using UnityEngine;

namespace GamincatKit.Common
{
    [ExecuteAlways]
    public class SafeAreaFitter : MonoBehaviour
    {
        [SerializeField] private RectTransform safeAreaContent;
        
        private Rect _lastSafeArea;
        private ScreenOrientation _screenOrientation;

#if UNITY_EDITOR
        private void Start()
        {
            Debug.Assert(safeAreaContent != null);
        }

        private void Reset()
        {
            safeAreaContent = GetComponent<RectTransform>();
        }
#endif

        private void Update()
        {
            if (safeAreaContent == null) return;

            var safeArea = Screen.safeArea;
            if (safeArea == _lastSafeArea && Screen.orientation == _screenOrientation) return;

            ApplySafeArea(safeArea);

            _lastSafeArea = safeArea;
            _screenOrientation = Screen.orientation;
        }

        private void ApplySafeArea(Rect safeArea)
        {
            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            safeAreaContent.anchorMin = anchorMin;
            safeAreaContent.anchorMax = anchorMax;
        }
    }
}