using UnityEngine;

public class RotateUI : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private bool _cCW = false;
    [SerializeField] private bool _step = false;
    [SerializeField] private float _angle = 15f;

    private RectTransform _rectTransform;
    private float totalAngle = 0f;

    void Start()
    {
        _rectTransform = (RectTransform)this.transform;
    }

    void Update()
    {
        if (!_step)
        {
            _rectTransform.Rotate(new Vector3(0f, 0f, _speed));
        }
        else
        {
            totalAngle += _speed;
            if (_speed >= 0f)
            {
                if (totalAngle >= _angle)
                {
                    if (_cCW == true)
                        _rectTransform.Rotate(new Vector3(0f, 0f, _angle));
                    else
                        _rectTransform.Rotate(new Vector3(0f, 0f, _angle * -1f));
                    totalAngle = 0f;
                }
            }
            else
            {
                if (totalAngle <= _angle)
                {
                    if (_cCW == true)
                        _rectTransform.Rotate(new Vector3(0f, 0f, _angle));
                    else
                        _rectTransform.Rotate(new Vector3(0f, 0f, _angle * -1f));
                    totalAngle = 0f;
                }
            }
        }
    }
}
