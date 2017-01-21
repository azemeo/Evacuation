using UnityEngine;
using System.Collections.Generic;
using Helpers;

public abstract class DamageableBuilding : BaseBuilding {

    protected UIDamageBarPanel _damageBarPanel;
    public UIDamageBarPanel DamageBarPanel
    {
        get
        {
            if (_damageBarPanel == null)
            {
                _damageBarPanel = UIManager.Instance.Get<UIDamageBarPanel>(UIManager.PanelID.DamageBarsPanel);
            }
            return _damageBarPanel;
        }
    }

    public int MaxHitPoints
	{
		get { return _buildingLevel.MaxHitPoints; }
	}

	public int CurrentHitPoints
	{
		get { return _currentHitPoints; }
	}

	[SerializeField]
	private GameObject _rubbles;

    //[SerializeField]
    //private EffectSpawner _deathEffects;

	private int _currentHitPoints = 100;

	protected override void BuildStarted ()
	{
		base.BuildStarted ();
		_currentHitPoints = _buildingLevel.MaxHitPoints;
	}

	public override void BuildComplete (System.DateTime timeCompleted)
	{
		base.BuildComplete (timeCompleted);
		_currentHitPoints = _buildingLevel.MaxHitPoints;
	}


	public virtual void TakeDamage(int damage)
	{

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
		Model.SetActive(false);
		Collider.enabled = false;

		if(_rubbles != null)
			_rubbles.SetActive(true);

        DamageBarPanel.AddDamageBar(this, UIAnchor, Vector3.up * 2);

        AudioManager.Instance.Play(AudioSFXDatabase.Instance.BuildingDestroyedSFX, AudioManager.AudioGroup.Building_Destroyed);
	}

	public virtual void SetHealth(int health)
	{
		_currentHitPoints = Mathf.Clamp(health, 0, MaxHitPoints);
	}

	public bool IsAlive
	{
        get
        {
            return _isAlive;
        }
	}

	public Transform GetTransform()
	{
		return Model.transform;
	}


    public override void OnSpawned()
    {
        base.OnSpawned();
        if(_rubbles != null)
        {
            _rubbles.gameObject.SetActive(false);
        }
    }

}
