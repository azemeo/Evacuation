using UnityEngine;
using Pathfinding;
using System.Collections;

public class Builder : AIAgent {

    [SerializeField]
    private Helpers.Resource _cost;

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

    }

    protected override void OnFSMStateComplete(FSMState completedState)
    {
        base.OnFSMStateComplete(completedState);

        if (completedState.StateID == FSMStateTypes.AI.MOVE)
        {
            if (_nextMoveTag == "build")
            {
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

    public void Wander()
    {
        GridCell cell = GridManager.Instance.GetCell(GridManager.Instance.GetCoordinatesFromWorldPosition(transform.position));
        Vector3 min = cell.transform.position;
        min.x -= 0.4f;
        min.z -= 0.4f;

        Vector3 max = cell.transform.position;
        max.x += 0.4f;
        max.z += 0.4f;
        SetState(new AIWanderState(new Rect(min.x, min.y, 1, 1)));
    }

    public Helpers.Resource Cost
    {
        get { return _cost; }
    }
}
