using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : BaseBuilding
{
    private List<Civilian> _localPopulation = new List<Civilian>();

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

        //show selection panel
    }

    public void SendMarshal()
    {
        Marshal m = GameManager.Instance.AssignMarshal();
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
        }
    }

    public int LocalPopulation
    {
        get { return _localPopulation.Count; }
    }
}
