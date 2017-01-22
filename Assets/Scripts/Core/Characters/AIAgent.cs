using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Helpers;
using System;

//Base
public abstract class AIAgent : ConfigurableObject {

	public float CurrentSpeed
	{
		get
		{
			return _currentSpeed;
		}

		set
		{
			_currentSpeed = value;
		}
	}

    public override string TemplateID
    {
        get
        {
            return Name;
        }
    }

    [Header("AIAgent Setup")]
    [SerializeField]
    protected string _name;

    public string Name
    {
        get
        {
            return _name;
        }
    }

    [SerializeField]
    private Resource _cost;

    private AIFSM _fsm;

    [SerializeField]
    protected float _movementSpeed;

	[SerializeField]
	protected bool _lookAtTarget = true;

    [SerializeField]
	protected Animator _animator;
	protected float _movementSpeedModifier = 1.0f;

    private bool _swimming = false;

    [SerializeField]
    protected SphereCollider _collider;
    [SerializeField]
    protected Rigidbody _rigidBody;

    private Vector3 _facingDirection;
    public Vector3 FacingDirection
    {
        get { return _facingDirection; }
    }

	protected float _currentSpeed = 0;

    protected virtual void OnEnable()
    {
        WaveManager.onWaveApproach += OnWaveApproach;
        WaveManager.onWaveArrival += OnWaveArrive;
        WaveManager.onWaveRecede += OnWaveRecede;
    }

    protected virtual void OnDisable()
    {
        WaveManager.onWaveApproach -= OnWaveApproach;
        WaveManager.onWaveArrival -= OnWaveArrive;
        WaveManager.onWaveRecede -= OnWaveRecede;
    }

    protected virtual void Awake()
    {
        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }

        _fsm = new AIFSM(new AIIdleState(), this);
        _fsm.onStateChanged += OnFSMStateChanged;
        _fsm.onStateErrorOccurred += OnFSMStateErrorOccurred;
        _fsm.onStateComplete += OnFSMStateComplete;
    }

    protected virtual void Update()
    {
        if (_fsm != null)
            _fsm.Update();

        GridCell currentCell = GridManager.Instance.GetCell(GridManager.Instance.GetCoordinatesFromWorldPosition(transform.position));
        if (currentCell.Occupant != null && currentCell.Occupant.CurrentFillAmount >= 0.6f && !_swimming)
        {
            _swimming = true;
            SetAnimationBool("Swim", true);
        }
        else if (_swimming && currentCell.Occupant != null && currentCell.Occupant.CurrentFillAmount < 0.6f)
        {
            _swimming = false;
            SetAnimationBool("Swim", false);
        }
    }

    void OnDestroy()
    {
        if (_fsm != null)
        {
            _fsm.onStateChanged -= OnFSMStateChanged;
            _fsm.onStateErrorOccurred -= OnFSMStateErrorOccurred;
			_fsm.onStateComplete -= OnFSMStateComplete;
        }
    }

    void OnApplicationFocus(bool focus)
    {
        if (_fsm != null)
            _fsm.OnApplicationFocus(focus);
    }

    void OnApplicationQuit()
    {
        if (_fsm != null)
            _fsm.OnApplicationQuit();
    }

    protected virtual void OnFSMStateErrorOccurred(FSMState failedState, string error)
    {

    }

    protected virtual void OnFSMStateChanged(FSMState previousState, FSMState currentState)
    {

    }

    protected virtual void OnFSMStateComplete(FSMState completedState)
    {

    }

    protected virtual void OnWaveApproach()
    {

    }
    protected virtual void OnWaveArrive()
    {

    }

    protected virtual void OnWaveRecede()
    {

    }

    public void SetState(AIState newState)
    {
        _fsm.SetState(newState);
    }

    public void RevertToPreviousState()
    {
        _fsm.RevertToPreviousState();
    }

	/// <summary>
	/// Rotates the Agent towards the target at the given rotation speed if smoothRotate is true (only for 3D agents, ignored for 2D sprites)
	/// </summary>
	/// <param name="target">The target.</param>
	/// <param name="rotationSpeed">Rotation speed. (ignored for 2D sprite agents)</param>
	/// <param name="smoothRotate">If set to <c>true</c> lerps the rotation value giving a smooth result. (ignored for 2D sprite agents)</param>
	public Vector3 LookAt(Vector3 targetPosition, float rotationSpeed = 1.0f, bool smoothRotate = false)
	{
		if(!_lookAtTarget)
			return Vector3.zero;

		Vector3 facingDirection = targetPosition - transform.position;
		facingDirection.y = 0;
		facingDirection.Normalize();

		if (facingDirection.sqrMagnitude > 0)
		{
			Quaternion lookAtRotation = Quaternion.LookRotation(facingDirection, Vector3.up);
			rotateTowards(lookAtRotation, rotationSpeed, smoothRotate);
		}

		return facingDirection;
	}

	private void rotateTowards(Quaternion targetRotation, float rotationSpeed, bool smoothRotate)
	{
		if (smoothRotate)
		{
			_rigidBody.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation, (180f / rotationSpeed) * Time.deltaTime));
		}
		else
		{
			_rigidBody.MoveRotation(targetRotation);
		}
	}

	private void setFacingDirection(Vector3 facingDirection)
    {
        if (facingDirection.x != _facingDirection.x && facingDirection.z != _facingDirection.z)
        {
            _facingDirection = facingDirection;
        }
    }

    public override void OnSpawned()
    {

    }

    public override void OnDespawned()
    {

    }

    protected virtual void Reset()
    {
        SetAnimationSpeed(1f);
    }

    public void Wander()
    {
        GridCell cell = GridManager.Instance.GetCell(GridManager.Instance.GetCoordinatesFromWorldPosition(transform.position));
        Vector3 min = cell.transform.position;
        min.x -= 0.4f;
        min.z -= 0.4f;

        SetState(new AIWanderState(new Rect(min.x, min.z, 0.8f, 0.8f)));
    }

    //Convienience methods for setting animator properties
    public void SetAnimationTrigger(string property)
    {
        if (Animator != null && Animator.gameObject.activeInHierarchy)
        {
            Animator.SetTrigger(property);
        }
    }

    public void SetAnimationFloat(string property, float value)
    {
        if (Animator != null && Animator.gameObject.activeInHierarchy)
        {
            Animator.SetFloat(property, value);
        }
    }

    public void SetAnimationBool(string property, bool value)
    {
        if (Animator != null && Animator.gameObject.activeInHierarchy)
        {
            Animator.SetBool(property, value);
        }
    }

    public void SetAnimationSpeed(float speed)
    {
        if (Animator != null && Animator.gameObject.activeInHierarchy)
        {
            Animator.speed = speed;
        }
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        if (_fsm != null)
        {
            _fsm.DrawDebug();
        }
    }
#endif

    public float MovementSpeed
    {
        get
        {
			return _movementSpeed * _movementSpeedModifier;
        }
    }

	public float MovementSpeedModifier
	{
		get
		{
			return _movementSpeedModifier;
		}

		set
		{
			_movementSpeedModifier = value;
		}
	}

    public Animator Animator
    {
        get
        {
            return _animator;
        }
    }

	public FSMState CurrentState
    {
        get
        {
            return _fsm.CurrentState;
        }
    }

    public SphereCollider Collider
    {
        get
        {
            return _collider;
        }
    }

    public Rigidbody RigidBody
    {
        get
        {
            return _rigidBody;
        }
    }

    public bool IsSwimming
    {
        get
        {
            return _swimming;
        }
    }

    public Helpers.Resource Cost
    {
        get { return _cost; }
    }
}
