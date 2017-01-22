﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseBuilderButton : PurchaseButton {

    [SerializeField]
    private Builder _template;

    public override void Buy(string itemName)
    {
        if (GameManager.Instance.HasResource(_template.Cost.Type, _template.Cost.Amount))
        {
            GameManager.Instance.SpendResource(_template.Cost.Type, _template.Cost.Amount);
            Builder b = TemplateManager.Instance.Spawn<Builder>(_template.TemplateID);
            b.SetHome(GameManager.Instance.HQ);
        }
    }

    protected override void DisplayCost()
    {
        UnityEngine.UI.Text costDisplay = transform.FindChild("Cost Display").GetComponent<UnityEngine.UI.Text>();
        if (costDisplay != null && _template != null)
        {
            costDisplay.text = "$" + _template.Cost.Amount;
        }
    }
}
