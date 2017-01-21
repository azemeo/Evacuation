using UnityEngine;
using System.Collections.Generic;
using Helpers;

public class UIDamageBarPanel : UIPanel {
    [SerializeField]
    private UIHealthBar _healthBarPrefab;

    private Dictionary<DamageableBuilding, UIHealthBar> _damageBars = new Dictionary<DamageableBuilding, UIHealthBar>();

	public void AddDamageBar(DamageableBuilding entity, Transform objectToFollow, Vector3? offset = null)
    {
        if (_damageBars.ContainsKey(entity))
        {
            return;
        }

        UIHealthBar damageBar = null;
        damageBar = Instantiate(_healthBarPrefab.gameObject).GetComponent<UIHealthBar>();
        damageBar.transform.SetParent(transform, true);
        damageBar.transform.localScale = Vector3.one;
        damageBar.transform.localRotation = Quaternion.identity;

		if(offset.HasValue)
			damageBar.transform.localPosition = offset.Value;

        _damageBars.Add(entity, damageBar);

        if (damageBar != null)
        {
            UIWorldPositioner worldPos = damageBar.GetComponent<UIWorldPositioner>();
            if (worldPos != null)
            {
                worldPos.ObjectToFollow = objectToFollow;

				if(offset.HasValue)
					worldPos.Offset = offset.Value;
            }

            damageBar.SetEntity(entity);
        }
    }

    public void RemoveDamageBar(DamageableBuilding entity)
    {
        if (_damageBars.ContainsKey(entity))
        {
            Destroy(_damageBars[entity].gameObject);
            _damageBars.Remove(entity);
        }
    }
}
