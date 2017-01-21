using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Helpers;
using System;

public class PlayerProfileManager : SingletonBehavior<PlayerProfileManager> {
    public delegate void ResourceUpdatedEvent(ResourceType type, int amount, int capacity);
    public static ResourceUpdatedEvent OnResourceUpdated;

    [SerializeField]
    private int _startingCash = 1000;

    public void ResourceUpdated(ResourceType type, int amount, int capacity)
    {
        if (OnResourceUpdated != null)
        {
            OnResourceUpdated(type, amount, capacity);
        }
    }

    private Dictionary<ResourceType, ResourceBank> _resources = new Dictionary<ResourceType, ResourceBank>();

    protected override void Init()
    {
        base.Init();

        //create starting values here (for now)
        CreateResource(ResourceType.Cash, _startingCash, int.MaxValue);
    }

    public void UpdateAllResources()
    {
        foreach (KeyValuePair<ResourceType, ResourceBank> kvp in _resources)
        {
            ResourceUpdated(kvp.Key, kvp.Value.Balance, kvp.Value.UpperLimit);
        }
    }

    public void CreateResource(ResourceType type, int initialBalance, int limit)
    {
        if (!_resources.ContainsKey(type))
        {
            _resources.Add(type, new ResourceBank(type, initialBalance, limit));
            ResourceUpdated(type, initialBalance, limit);
        }
        else
        {
            Debug.LogWarning("Resource '" + type.ToString() + "' already exists!");
        }
    }

    /// <summary>
    /// Add resources of a given type to the players inventory.
    /// </summary>
    /// <param name="type">ResourceType to add.</param>
    /// <param name="amount">Amount of resources to add.</param>
    /// <returns>True if the full amount was added</returns>
    public bool AddResources(ResourceType type, int amount)
    {
        if (_resources.ContainsKey(type))
        {
            ResourceBank bank = _resources[type];
            if (bank != null)
            {
                if (bank.Balance == bank.UpperLimit)
                {
                    return false;
                }

                if (bank.Balance + amount > bank.UpperLimit)
                {
                    bank.Add(bank.UpperLimit - bank.Balance);
                }
                else
                {
                    bank.Add(amount);
                }
                ResourceUpdated(type, bank.Balance, bank.UpperLimit);

                return true;
            }
		}
		else
		{
			CreateResource(type, amount, amount);
            return true;
		}

		return false;
    }

	public bool SpendResources(ResourceType type, int amount)
    {
        if (_resources.ContainsKey(type))
        {
            ResourceBank bank = _resources[type];
            if (bank != null)
            {
                if (bank.Balance >= amount)
                {
                    bank.Subtract(amount);

                    ResourceUpdated(type, bank.Balance, bank.UpperLimit);

                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Get the current balance of a given resource.
    /// </summary>
    /// <param name="type">ResourceType to check.</param>
    /// <returns>The current balance of the ResourceType</returns>
    public int GetResourceBalance(ResourceType type)
    {
        if (_resources.ContainsKey(type))
        {
            return _resources[type].Balance;
        }
        return 0;
    }

    public void SetResource(ResourceType type, int balance)
    {
        if (_resources.ContainsKey(type))
        {
            ResourceBank bank = _resources[type];
            if (bank != null)
            {
                if (bank.Balance > balance)
                {
                    bank.Subtract(Mathf.Clamp((bank.Balance-balance), 0, bank.UpperLimit));
                }
                else
                {
                    bank.Add(Mathf.Clamp((balance - bank.Balance), 0, bank.UpperLimit));
                }

                ResourceUpdated(type, bank.Balance, bank.UpperLimit);
            }
        }
    }

    public void SetResourceCap(ResourceType type, int newCap)
    {
        if (_resources.ContainsKey(type))
        {
            _resources[type].SetLimit(newCap);
            ResourceUpdated(type, _resources[type].Balance, _resources[type].UpperLimit);
        }
        else
        {
            CreateResource(type, 0, newCap);
        }
    }

    public int GetResourceCap(ResourceType type)
    {
        if (_resources.ContainsKey(type))
        {
            return _resources[type].UpperLimit;
        }
        return 0;
    }

    public int GetResourceDeficit(ResourceType type)
    {
        return GetResourceCap(type) - GetResourceBalance(type);
    }
}

[Serializable]
public class ResourceBank
{
    [SerializeField]
    private string _name;
    [SerializeField]
    private int _balance;
    [SerializeField]
    private int _upperLimit;

    public int Balance
    {
        get { return _balance; }
    }

    public int UpperLimit
    {
        get { return _upperLimit; }
    }

    public ResourceBank(ResourceType type, int initialBalance, int upperLimit)
    {
        _name = type.ToString();
        _balance = initialBalance;
        _upperLimit = upperLimit;
    }

    public ResourceBank(JSONObject savedData)
    {
        Deserialize(savedData);
    }

    public bool Add(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("Can not add negative or zero values to " + _name + ". Please use Subtract if this is what you meant to do.");
            return false;
        }

        if (_balance + amount > _upperLimit)
        {
            Debug.LogWarning("Upper limit of " + _name + " too low to add " + amount + " to.");
            return false;
        }

        _balance += amount;
        return true;
    }

    public bool Subtract(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("Can not subtract negative or zero values from " + _name + ". Please use Add if this is what you meant to do.");
            return false;
        }
        if (_balance - amount < 0)
        {
            //Debug.LogWarning("Balance of " + _name + " too low to subtract " + amount + " from.");
            return false;
        }

        _balance -= amount;
        return true;
    }

    public bool SetLimit(int newLimit)
    {
        if (newLimit < 1f)
        {
            Debug.LogWarning("Upper limit of " + _name + " must be 1 or higher.");
            return false;
        }

        if (_balance > newLimit)
        {
            _balance = newLimit;
        }

        _upperLimit = newLimit;
        return true;
    }

    public JSONObject Serialize()
    {
        JSONObject saveData = JSONObject.obj;

        saveData.AddField("name", _name);
        saveData.AddField("balance", _balance);
        saveData.AddField("upperLimit", _upperLimit);

        return saveData;
    }

    public void Deserialize(JSONObject savedData)
    {
        _name = savedData.GetField("name").str;
        _balance = (int)savedData.GetField("balance").f;
        _upperLimit = (int)savedData.GetField("upperLimit").f;
    }

    public bool ShouldSerialize()
    {
        return true;
    }

    public bool ShouldDeserialize()
    {
        return true;
    }
}
