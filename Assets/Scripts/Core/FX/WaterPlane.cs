using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPlane : MonoBehaviour {

    [SerializeField]
    private SpriteRenderer _sprite;

    [SerializeField]
    private Color _color;

    public void SetFill(float fillAmount)
    {
        _color.a = fillAmount;
        _sprite.color = _color;

        if (_color.a > 0 && !gameObject.activeSelf) gameObject.SetActive(true);
        else if (_color.a == 0 && gameObject.activeSelf) gameObject.SetActive(false);
    }
}
