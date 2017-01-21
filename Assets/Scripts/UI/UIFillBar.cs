using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIFillBar : MonoBehaviour {
	[SerializeField] private Image _barFill;
	[SerializeField] private Text _primaryLabel;
	[SerializeField] private Text _secondaryLabel;

    private CanvasRenderer _fillRenderer;

	public void SetFillPercentage(float percent)
	{
        if (_barFill == null)
        {
            return;
        }
            _barFill.fillAmount = percent;
	}

	public void SetFillPercentage(float currentValue, float maxValue)
	{
        if (maxValue > 0)
        {
            SetFillPercentage(Mathf.Clamp01(currentValue / maxValue));
        }
        else
        {
            SetFillPercentage(0);
        }
	}

	public void SetPrimaryLabel(string labelText)
	{
		if (_primaryLabel == null || string.IsNullOrEmpty(labelText))
			return;

		_primaryLabel.text = labelText;
	}

	public void SetSecondaryLabel(string labelText)
	{
		if (_secondaryLabel == null || string.IsNullOrEmpty(labelText))
			return;

		_secondaryLabel.text = labelText;
	}

	public void SetLabels(string primaryText, string secondaryText = "")
	{
		SetPrimaryLabel(primaryText);
		SetSecondaryLabel(secondaryText);
	}

	public void Set(float percent, string primaryText, string secondaryText = "")
	{
		SetFillPercentage(percent);
		SetLabels(primaryText, secondaryText);
	}

	public virtual void Set(float currentValue, float maxValue, string primaryText, string secondaryText = "")
	{
		SetFillPercentage(currentValue, maxValue);
		SetLabels(primaryText, secondaryText);
	}

    public virtual void SetColor(Color color)
    {
        if (_barFill != null)
        {
            if (_fillRenderer == null)
            {
                _fillRenderer = _barFill.GetComponent<CanvasRenderer>();
            }
            if (_fillRenderer != null)
            {
                _fillRenderer.SetColor(color);
            }
        }
    }
}
