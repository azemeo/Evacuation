using UnityEngine;
using Helpers;

public class UIHealthBar : MonoBehaviour
{
    [SerializeField]
    private UIFillBar _fillBar;

    [SerializeField]
    private Gradient _fillColors;

    private DamageableBuilding _entity;

    public void SetEntity(DamageableBuilding entity)
    {
        _entity = entity;
        if (_entity != null)
        {
            updateFillBar();
        }
    }

    void Update()
    {
        if (_entity != null)
        {
            updateFillBar();
        }
    }

    private void updateFillBar()
    {
        float fillAmount = _entity.CurrentHitPoints / (float)_entity.MaxHitPoints;
        _fillBar.SetFillPercentage(fillAmount);
        _fillBar.SetColor(_fillColors.Evaluate(fillAmount));
    }


}
