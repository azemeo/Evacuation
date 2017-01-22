using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : BaseBuilding
{
    private List<Civilian> _localPopulation = new List<Civilian>();

    private Marshal _assignedMarshal;

    public override int GridObjectType
    {
        get
        {
            return GridObjectTypes.Building.ROAD;
        }
    }

    public void AddCivilians(int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            Civilian newCivi = TemplateManager.Instance.Spawn<Civilian>(GameManager.Instance.GetRandomCivilianTemplate(), position: transform.position);
            _localPopulation.Add(newCivi);
        }


        Collider.enabled = true;
    }

    public override void Placed()
    {
        base.Placed();
        Collider.enabled = false;
    }

    public override void Selected()
    {
        base.Selected();

        SendMarshal();
    }

    public void SendMarshal()
    {
        if (_assignedMarshal == null)
        {
            _assignedMarshal = GameManager.Instance.AssignMarshal();
            if (_assignedMarshal != null)
            {
                _assignedMarshal.CollectCivilians(this);
                GameManager.Instance.ShowMessage("Marshal " + _assignedMarshal.Name + " is on his way!");
            }
            else
            {
                GameManager.Instance.ShowMessage("No Marshals Available!");
            }
        }
        else
        {
            GameManager.Instance.ShowMessage("Marshal " + _assignedMarshal.Name + " is already coming!");
        }
    }

    public void Collect(Marshal m)
    {
        if (_localPopulation.Count > 0)
        {
            for (int i = 0; i < _localPopulation.Count; i++)
            {
                _localPopulation[i].SetLeader(m);
            }

            _localPopulation.Clear();
            Collider.enabled = false;
            _assignedMarshal = null;
        }
    }

    public int LocalPopulation
    {
        get { return _localPopulation.Count; }
    }
}
