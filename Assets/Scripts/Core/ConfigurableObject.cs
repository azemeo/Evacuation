using UnityEngine;
using System.Collections;
using Helpers;

public abstract class ConfigurableObject : MonoBehaviour, ISpawnableObject
{
    protected JSONObject _configData;

    [SerializeField]
    protected string _templateID = "";

    public string TemplateID
    {
        get
        {
            return _templateID;
        }
    }

    public virtual string ConfigID
    {
        get
        {
            return "ConfigurableObject";
        }
    }

    /// <summary>
    /// Called every time the object is spawned from PoolBoss. Use this to do initialization that you need to happen on every spawn (Start is only called the first time).
    /// </summary>
	public abstract void OnSpawned();

    /// <summary>
    /// Called every time the object is despawned to PoolBoss. Use this to clean up things instead of OnDestroy.
    /// </summary>
	public abstract void OnDespawned();
}
