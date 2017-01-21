using UnityEngine;
using System.Collections;

public class Message {

    public enum eMessageType
    {
        TARGET_DESTROYED,
        TARGET_MOVED,
        BATTLE_COMPLETE,
        TROOP_DAMAGED,
        TROOP_DESTROYED,
        FORCE_SAVE
    }

    private eMessageType _type;
    private JSONObject _data;

    public Message(eMessageType type, JSONObject data = null)
    {
        _type = type;
        if (data == null)
        {
            _data = JSONObject.nullJO;
        }
        else
        {
            _data = data;
        }
    }

    public eMessageType MessageType
    {
        get
        {
            return _type;
        }
    }

    public JSONObject Data
    {
        get
        {
            return _data;
        }
    }
}
