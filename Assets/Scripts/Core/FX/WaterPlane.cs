using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPlane : MonoBehaviour {

    [SerializeField]
    private SpriteRenderer _sprite;

    [SerializeField]
    private Color _color;

    bool updateTexture = true;

    public SpriteRenderer Sprite
    {
        get
        {
            if(_sprite == null)
            {
                _sprite = GetComponent<SpriteRenderer>();
            }
            return _sprite;
        }
    }

    void Awake()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.up);
    }

    public void SetFill(float fillAmount)
    {
        _color.a = fillAmount;
        Sprite.color = _color;

        if (_color.a > 0 && !gameObject.activeSelf) gameObject.SetActive(true);
        else if (_color.a == 0 && gameObject.activeSelf) gameObject.SetActive(false);
    }

    void Update()
    {
        if (updateTexture)
        {
            Vector2 offset = Sprite.material.mainTextureOffset;
            offset.y += 1f * Time.deltaTime;
            Sprite.material.SetTextureOffset("_MainTex", offset);
        }
    }
}
