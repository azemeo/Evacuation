using UnityEngine;
using System.Collections.Generic;

public class TemplateManager : SingletonBehavior<TemplateManager> {

    private Dictionary<string, ConfigurableObject> _templates = new Dictionary<string, ConfigurableObject>();
    private Dictionary<string, ConfigurableObject> _spawnedTemplates = new Dictionary<string, ConfigurableObject>();

    [SerializeField]
    private List<ConfigurableObject> _templateList = new List<ConfigurableObject>();    //list for inspector

    private Transform _templateRoot;

    protected override void Init()
    {
        base.Init();

        for(int i = 0; i < _templateList.Count; i++)
        {
            if (_templateList[i] != null)
            {
                Register(_templateList[i]);
            }
        }
        _templateList.Clear();
        _templateList = null;

        SceneLoaderManager.onLevelLoadedCompleted += onLevelLoaded;
    }

    private void onLevelLoaded()
    {
        _spawnedTemplates.Clear();
    }

    public void Register(ConfigurableObject template)
    {
        if (!_templates.ContainsKey(template.TemplateID))
        {
            _templates.Add(template.TemplateID, template);
        }
        else
        {
            Debug.LogError("Template '" + template.TemplateID + "' is already registered with TemplateManager. Please specify a unique template ID");
        }
    }

    public T Get<T>(string id) where T : ConfigurableObject
    {
        if (_templates.ContainsKey(id))
        {
            if (_templates[id] is T)
            {
                return getSpawnedTemplate<T>(id);
            }
            else
            {
                Debug.LogError("Template mathcing ID '" + id + "' is not of type '" + typeof(T).ToString() + "'.");
            }
        }
        else
        {
            Debug.LogError("No Template with id '" + id + "' registered with TemplateManager.");
        }
        return null;
    }

    public List<T> GetAll<T>(System.Type interfaceType = null) where T : ConfigurableObject
    {
        List<T> retList = new List<T>();

        foreach (KeyValuePair<string,ConfigurableObject> kvp in _templates)
        {
            if (kvp.Value is T)
            {
                if (interfaceType != null)
                {
                    if (kvp.Value.GetComponent(interfaceType) == null)
                    {
                        continue;
                    }
                }
                retList.Add(getSpawnedTemplate<T>(kvp.Key));
            }
        }

        return retList;
    }

    private T getRawTemplate<T>(string id) where T :ConfigurableObject
    {
        if (_templates.ContainsKey(id))
        {
            if (_templates[id] is T)
            {
                return _templates[id] as T;
            }
            else
            {
                Debug.LogError("Template mathcing ID '" + id + "' is not of type '" + typeof(T).ToString() + "'.");
            }
        }
        else
        {
            Debug.LogError("No Template with id '" + id + "' registered with TemplateManager.");
        }
        return null;
    }

    private T getSpawnedTemplate<T>(string id) where T : ConfigurableObject
    {
        if (_spawnedTemplates.ContainsKey(id))
        {
            if (_spawnedTemplates[id] != null)
            {
                return _spawnedTemplates[id] as T;
            }
            else
            {
                //If a template in the list is null, then we are possibly accessing it after a scene change and they were cleaned up.
                // Clear the list and spawn new ones.
                _spawnedTemplates.Clear();
            }
        }

        if (Exists<T>(id))
        {
            ConfigurableObject templateInstance = Instantiate(getRawTemplate<T>(id)).GetComponent<ConfigurableObject>();
            if (_templateRoot == null)
            {
                _templateRoot = new GameObject("SpawnedTemplates").transform;
            }

            templateInstance.transform.SetParent(_templateRoot);
            templateInstance.transform.localPosition = Vector3.zero;
            templateInstance.gameObject.SetActive(false);
            _spawnedTemplates.Add(id, templateInstance);
            return templateInstance as T;
        }
        return null;

    }

    public bool Exists<T>(string templateID) where T : ConfigurableObject
    {
        if (_templates.ContainsKey(templateID))
        {
            return (_templates[templateID] is T);
        }

        return false;
    }

    public T Spawn<T>(string templateID, Transform parent = null) where T : ConfigurableObject
    {
        T template = getSpawnedTemplate<T>(templateID);
        if (template == null)
        {
            Debug.LogError("Could not spawn template '" + templateID + "'!");
            return default(T);
        }

        return Spawn<T>(template, parent);
    }

    public T Spawn<T>(ConfigurableObject template, Transform parent = null) where T : ConfigurableObject
    {
        T retObject = PoolBoss.Spawn(template.transform, Vector3.zero, Quaternion.identity, null, false).GetComponent<T>();

        if (retObject != null)
        {
            if (parent != null)
            {
                retObject.transform.SetParent(parent);
            }
            retObject.gameObject.SetActive(true);

            ISpawnableObject[] spawnListeners = retObject.GetComponents<ISpawnableObject>();

			if (spawnListeners != null && spawnListeners.Length > 0)
			{
				for(int i = 0; i < spawnListeners.Length; ++i)
					spawnListeners[i].OnSpawned();
			}

            return retObject;
        }
        else
        {
            Debug.LogError("Could not spawn template!");
            return null;
        }
    }
}
