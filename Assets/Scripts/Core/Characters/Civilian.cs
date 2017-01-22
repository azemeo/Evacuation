using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Civilian : AIAgent {

    [SerializeField]
    private int _reward;

    [SerializeField]
    private float _maxHitPoints;

    [SerializeField]
    private float _healthDrainRate;

    private Marshal _leader;

    public float MaxHitPoints
    {
        get { return _maxHitPoints; }
    }

    public float CurrentHitPoints
    {
        get { return _currentHitPoints; }
    }

    //[SerializeField]
    //private EffectSpawner _deathEffects;

    private float _currentHitPoints = 100f;
    private bool _isAlive = true;
    private bool _drowning = false;
    private bool _saved = false;

    public override void OnSpawned()
    {
        base.OnSpawned();
        _currentHitPoints = _maxHitPoints;
        _isAlive = true;
        _drowning = false;
        _saved = false;
        _leader = null;

        Wander();
    }

    public void SetLeader(Marshal leader)
    {
        _leader = leader;
        FollowLeader();
    }

    private void FollowLeader()
    {
        if(_leader != null)
        {
            SetState(new AIMoveAction(GridManager.Instance.GetCell(GridManager.Instance.GetCoordinatesFromWorldPosition(_leader.transform.position)).Occupant.GetRandomPositionInArea(), 1, usePathfinder: false));
        }
    }

    protected override void OnFSMStateComplete(FSMState completedState)
    {
        base.OnFSMStateComplete(completedState);

        if (completedState.StateID == FSMStateTypes.AI.MOVE)
        {
            if (!_saved)
            {
                FollowLeader();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        if(IsSwimming)
        {
            TakeDamage(_healthDrainRate * Time.deltaTime);
        }

        if (_leader == null)
        {
            if (IsSwimming && !_drowning)
            {
                SetAnimationBool("Drown", true);
                _drowning = true;
            }
            else if (_drowning && !IsSwimming)
            {
                SetAnimationBool("Drown", false);
                _drowning = false;
            }
        }
        else
        {
            if (_drowning)
            {
                SetAnimationBool("Drown", false);
                _drowning = false;
            }
        }
    }



    public virtual void TakeDamage(float damage)
    {
        if (_saved) return;

        if (_currentHitPoints < damage)
        {
            damage = _currentHitPoints;
        }

        if (IsAlive)
        {
            if (damage > 0)
            {
                SetHealth(_currentHitPoints - damage);

                if (_currentHitPoints == 0f)
                {
                    Die();
                    //_deathEffects.PlayEffect();
                }
                else
                {
                    //_hitEffects.PlayEffect();
                }
            }
        }
    }

    public virtual void Die()
    {
        _isAlive = false;

        GameManager.Instance.ShowMessage(Name + " has died! :'(");

        Destroy(gameObject);
        //DamageBarPanel.AddDamageBar(this, UIAnchor, Vector3.up * 2);

        //AudioManager.Instance.Play(AudioSFXDatabase.Instance.BuildingDestroyedSFX, AudioManager.AudioGroup.Building_Destroyed);
    }

    public virtual void SetHealth(float health)
    {
        _currentHitPoints = Mathf.Clamp(health, 0, MaxHitPoints);
    }

    private void OnTriggerEnter(Collider C)
    {
        if(C.CompareTag("SafeZone"))
        {
            Rescue();
        }
    }

    public void Rescue()
    {
        GameManager.Instance.AddResource(Helpers.ResourceType.Cash, _reward);
        _saved = true;
    }

    protected override void OnWaveApproach()
    {
        base.OnWaveApproach();
        SetAnimationBool("Panic", true);
    }

    protected override void OnWaveRecede()
    {
        base.OnWaveRecede();
        SetAnimationBool("Panic", false);
    }

    public bool IsAlive
    {
        get
        {
            return _isAlive;
        }
    }
}
