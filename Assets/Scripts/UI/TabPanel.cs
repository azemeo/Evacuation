using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class TabPanel : UIPanel {

    [SerializeField]
    private List<Toggle> _tabButtons;

    public void OpenToTab(string tabID)
    {
        gameObject.SetActive(true);
        for (int i = 0; i < _tabButtons.Count; i++)
        {
            if (_tabButtons[i].name == tabID)
            {
                _tabButtons[i].isOn = true;
            }
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
