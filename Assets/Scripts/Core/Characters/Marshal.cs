using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marshal : AIAgent
{
    private GridObject _home;
    private Road _targetRoad;
    private GameObject _targetSafeZone;

    private string _nextMoveTag = "";
    private Vector3 _nextMoveDest;

    public void SetHome(GridObject home)
    {
        _home = home;
        transform.position = _home.transform.position;
        Wander();
    }

    public void CollectCivilians(Road container)
    {
        _targetRoad = container;

        _nextMoveTag = "collect";
        SetState(new AIMoveAction(_targetRoad.GetRandomPositionInArea(), 1f));
    }

    protected override void OnFSMStateComplete(FSMState completedState)
    {
        base.OnFSMStateComplete(completedState);

        if (completedState.StateID == FSMStateTypes.AI.MOVE)
        {
            if (_nextMoveTag == "collect")
            {
                _targetRoad.Collect(this);
                _nextMoveTag = "dropoff";
                SetState(new AIMoveAction(_targetSafeZone.transform.position, 0.5f));
            }
            else if (_nextMoveTag == "dropoff")
            {
                SetState(new AIWaitAction(2f));

                _nextMoveTag = "home";
            }
            else if (_nextMoveTag == "home")
            {
                Wander();
            }
        }
        if (completedState.StateID == FSMStateTypes.AI.WAIT)
        {
            ReturnHome();
        }
    }

    public void ReturnHome()
    {
        if (_home != null)
        {
            _nextMoveTag = "home";
            SetState(new AIMoveAction(_home.GetRandomPositionInArea(), 1f));
        }
    }

}
