using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseMarshalButton : PurchaseButton {

    [SerializeField]
    private Marshal _template;

    public override void Buy(string itemName)
    {
        if (GameManager.Instance.HasResource(_template.Cost.Type, _template.Cost.Amount))
        {
            GameManager.Instance.SpendResource(_template.Cost.Type, _template.Cost.Amount);
            Marshal m = TemplateManager.Instance.Spawn<Marshal>(GameManager.Instance.GetRandomMarshalTemplate());
            GameManager.Instance.AddMarshal(m, false);
            m.SetHome(GameManager.Instance.HQ);
            GameManager.Instance.ShowMessage("A new Marshall has arrived!");
        }
        else
        {
            GameManager.Instance.ShowMessage("You cannot afford a marhsall :(");
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
