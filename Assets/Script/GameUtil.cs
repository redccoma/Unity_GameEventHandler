using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GameUtil
{
    public static bool SendGameEvent(GameEventType in_geType, System.Action in_Callback, params object[] in_objectArray)
    {
        GameEvent ge = new GameEvent(in_geType, true);

        ge.Callback = in_Callback;

        for (int i = 0; i < in_objectArray.Length; i++)
            ge.Write(in_objectArray[i]);

        return GameEventSubject.Subject(ge);
    }

    public static bool SendGameEvent_List_Template<T>(GameEventType in_geType, List<T> in_list)
    {
        GameEvent ge = new GameEvent(in_geType, true);

        ge.Write(in_list.Count);

        for (int i = 0; i < in_list.Count; i++)
            ge.Write(in_list[i]);

        return GameEventSubject.Subject(ge);
    }

    public static bool SendGameEvent_List(GameEventType in_geType, List<System.Action> in_Callback_List, params object[] in_objectArray)
    {
        GameEvent ge = new GameEvent(in_geType, true);

        ge.Callback_List = in_Callback_List;

        for (int i = 0; i < in_objectArray.Length; i++)
            ge.Write(in_objectArray[i]);

        return GameEventSubject.Subject(ge);
    }

    public static bool SendGameEvent_Load(GameEventType in_geType)
    {
        return GameEventSubject.Subject(new GameEvent(in_geType), true);
    }

    public static bool SendGameEvent(GameEventType in_geType, System.Action in_Callback = null)
    {
        //CommonInfo.ge.Init(in_geType);
        //GameEventSubject.Subject(CommonInfo.ge);
        return GameEventSubject.Subject(new GameEvent(in_geType, in_Callback));
    }

    public static bool SendGameEvent_NEW(GameEventType in_geType, params object[] in_objectArray)
    {
        GameEvent ge = new GameEvent(in_geType, true);

        for (int i = 0; i < in_objectArray.Length; i++)
            ge.Write(in_objectArray[i]);

        return GameEventSubject.Subject(ge);
    }

    public static bool SendGameEvent_Class<T>(GameEventType in_geType, T in_object, params object[] in_objectArray)
    {
        GameEvent ge = new GameEvent(in_geType, true);

        ge.WriteClass(in_object);

        for (int i = 0; i < in_objectArray.Length; i++)
            ge.Write(in_objectArray[i]);

        return GameEventSubject.Subject(ge);
    }

    public static bool SendGameEvent_Class_List<T>(GameEventType in_geType, List<System.Action> in_Callback_List, T in_object)
    {
        GameEvent ge = new GameEvent(in_geType, true);

        ge.Callback_List = in_Callback_List;

        ge.WriteClass(in_object);

        return GameEventSubject.Subject(ge);
    }

    public static bool SendGameEvent_GameObject(GameEventType in_geType, GameObject in_GameObject, params object[] in_objectArray)
    {
        GameEvent ge = new GameEvent(in_geType, true);

        ge.sender = in_GameObject;

        for (int i = 0; i < in_objectArray.Length; i++)
            ge.Write(in_objectArray[i]);

        return GameEventSubject.Subject(ge);
    }
}
