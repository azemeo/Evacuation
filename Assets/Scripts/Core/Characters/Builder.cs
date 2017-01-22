using UnityEngine;
using Pathfinding;
using System.Collections;

public class Builder : AIAgent {



    private GridObject _home;
	private GridObject _gridObjectTarget;

    [SerializeField]
    private GameObject _teleportEffect;

    [SerializeField]
    private GameObject _buildEffect;

    [SerializeField]
    private AudioClip _teleportSound;

    [SerializeField]
    private AudioClip _buildSound;

    private string _nextMoveTag = "";
    private Vector3 _nextMoveDest;

    public void SetHome(GridObject home)
    {
        _home = home;
        transform.position = _home.transform.position;
        Wander();
    }

	public void SetTarget(GridObject gridObject)
    {
        _gridObjectTarget = gridObject;
        gameObject.SetActive(true);

        _nextMoveTag = "build";
        _nextMoveDest = _gridObjectTarget.GetRandomPositionInArea();
        SetState(new AIMoveAction(_gridObjectTarget.transform.position, 1));
        GameManager.Instance.ShowMessage("Builder " + Name + " is on it!");
    }

    protected override void OnFSMStateComplete(FSMState completedState)
    {
        base.OnFSMStateComplete(completedState);

        if (completedState.StateID == FSMStateTypes.AI.MOVE)
        {
            if (_nextMoveTag == "build")
            {
                _gridObjectTarget.StartBuild();
                SetState(new AIUpgradeState(_gridObjectTarget));
            }
            else if (_nextMoveTag == "home")
            {
                Wander();
            }
        }
        if(completedState.StateID == FSMStateTypes.AI.UPGRADE)
        {
            Wander();
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
