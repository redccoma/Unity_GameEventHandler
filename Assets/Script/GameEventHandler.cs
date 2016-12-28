using UnityEngine;
using System.IO;
using System.Collections.Generic;

public abstract class GameEventHandler : MonoBehaviour
{
    List<GameEventType> handling_event_type_list = new List<GameEventType>();

    // 실제로 이벤트를 받을 자식클래스(game object 등)에서 반드시 override 되어야 함.
    public abstract void HandleGameEvent(GameEvent ge);

    public void RegisterGameEvent(GameEventType event_type, GameEventHandler handler = null)
    {
        if (null == handling_event_type_list)
            return;

        handling_event_type_list.Add(event_type);

        if (null == handler)
            GameEventSubject.RegisterHandler(event_type, this);
        else
            GameEventSubject.RegisterHandler(event_type, handler);
    }

    public void UnregisterGameEvent(GameEventType event_type, GameEventHandler handler = null)
    {
        if (null == handling_event_type_list)
            return;

        handling_event_type_list.Remove(event_type);

        if (null == handler)
            GameEventSubject.UnregisterHandler(event_type, this);
        else
            GameEventSubject.UnregisterHandler(event_type, handler);
    }

    public void UnregisterGameEvent(GameEventHandler handler = null)
    {
        if (null == handling_event_type_list)
            return;

        if (null == handler)
        {
            for (int i = 0; i < handling_event_type_list.Count; ++i)
            {
                GameEventSubject.UnregisterHandler(handling_event_type_list[i], this);
            }
        }
        else
        {
            for (int i = 0; i < handling_event_type_list.Count; ++i)
            {
                GameEventSubject.UnregisterHandler(handling_event_type_list[i], handler);
            }
        }
    }

    public virtual void OnDestroy()
    {
        if (null != handling_event_type_list)
            handling_event_type_list.Clear();
        handling_event_type_list = null;
    }

    protected virtual void OnAwake()
    {

    }
}

public class GameEvent
{
    public GameEventType event_type;
    private MemoryStream stream = null;
    private BinaryWriter writer = null;
    private BinaryReader reader = null;
    private System.Runtime.Serialization.Formatters.Binary.BinaryFormatter mBinaryFormatter = null;
    private bool use_memory_stream;
    public List<System.Action> Callback_List = null;
    public System.Action Callback = null;
    public GameObject sender = null;
    public System.Action Callback_GameEventSubject = null;

    public GameEvent() { }

    public GameEvent(GameEventType in_eventType, bool in_use_memory_stream = false)
    {
        Init(in_eventType, in_use_memory_stream);
    }

    public GameEvent(GameEventType in_eventType, List<System.Action> in_Callback_List, bool in_use_memory_stream = false)
    {
        Callback_List = in_Callback_List;

        Init(in_eventType, in_use_memory_stream);
    }

    public GameEvent(GameEventType in_eventType, System.Action in_Callback, bool in_use_memory_stream = false)
    {
        Callback = in_Callback;

        Init(in_eventType, in_use_memory_stream);
    }

    public void OnCallBack()
    {
        if (null != Callback)
        {
            Callback();
            Callback = null;
        }
    }

    public void OnCallBackList(int in_nIndex)
    {
        if (null != Callback_List
            && in_nIndex < Callback_List.Count)
        {
            Callback_List[in_nIndex]();
            Callback_List[in_nIndex] = null;
        }
    }

    protected void Init(GameEventType in_eventType, bool in_use_memory_stream = false)
    {
        event_type = in_eventType;
        use_memory_stream = in_use_memory_stream;
        if (use_memory_stream)
        {
            if (null == stream)
            {
                stream = new MemoryStream();
            }
            else
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.SetLength(0);
            }

            if (null == writer)
                writer = new BinaryWriter(stream);
            if (null == reader)
                reader = new BinaryReader(stream);
            if (null == mBinaryFormatter)
                mBinaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        }
    }

    public void Close()
    {
        if (null != writer)
            writer.Close();
        writer = null;

        if (null != reader)
            reader.Close();
        reader = null;

        mBinaryFormatter = null;

        if (null != Callback_List)
            Callback_List.Clear();
        Callback_List = null;

        if (null != stream)
            stream.Close();
        stream = null;
    }

    public virtual void HandleUsedEvent()
    {
        // do nothing
    }

    public void Reset()
    {
        if (use_memory_stream)
            stream.Seek(0, SeekOrigin.Begin);
    }

    public void Write(object in_data)
    {
        if (typeof(long) == in_data.GetType()) { writer.Write(System.Convert.ToInt64(in_data)); }
        else if (typeof(int) == in_data.GetType()) { writer.Write(System.Convert.ToInt32(in_data)); }
        else if (typeof(double) == in_data.GetType()) { writer.Write(System.Convert.ToDouble(in_data)); }
        else if (typeof(float) == in_data.GetType()) { writer.Write(System.Convert.ToSingle(in_data)); }
        else if (typeof(bool) == in_data.GetType()) { writer.Write(System.Convert.ToBoolean(in_data)); }
        else if (typeof(string) == in_data.GetType()) { writer.Write(System.Convert.ToString(in_data)); }
        else { writer.Write(System.Convert.ToInt32(in_data)); }
    }

    public void WriteClass(object in_data)
    {
        if (null == mBinaryFormatter)
            return;

        mBinaryFormatter.Serialize(stream, in_data);
    }

    public object ReadClass()
    {
        if (null == mBinaryFormatter)
            return null;

        return mBinaryFormatter.Deserialize(stream);
    }

    //void Read(out int out_data) { out_data = reader.ReadInt32(); }
    public int ReadInt()
    {
        if (null == reader)
            return 0;

        return reader.ReadInt32();
    }

    //void Read(out long out_data) { out_data = reader.ReadInt64(); }
    public long ReadLong()
    {
        if (null == reader)
            return 0L;

        return reader.ReadInt64();
    }

    //void Read(out float out_data) { out_data = reader.ReadSingle(); }
    public float ReadFloat()
    {
        if (null == reader)
            return 0.0f;

        return reader.ReadSingle();
    }

    //void Read(out bool out_data) { out_data = reader.ReadBoolean(); }
    public bool ReadBool()
    {
        if (null == reader)
            return false;

        return reader.ReadBoolean();
    }

    //void Read(out string out_data) { out_data = reader.ReadString(); }
    public string ReadString()
    {
        if (null == reader)
            return "";

        return reader.ReadString();
    }

    void ReadEnum<T>(out T out_data)
    {
        out_data = default(T);

        // ArgumentException: enumType is not an Enum type.
        // 위 에러로 이곳에 들어오셨다면.. 해당 타입은 지원하지 않는 타입입니다.(ex. double short)
        // 필요하시다면 위 Write, Read 와 같은 함수를 작성하신 후 사용하세요.
        // 이 제네릭 함수는 Enum을 위함입니다.
        if (typeof(long) == reader.GetType()) { long temp = reader.ReadInt64(); out_data = (T)System.Enum.Parse(typeof(T), temp.ToString()); }
        else if (typeof(int) == reader.GetType()) { int temp = reader.ReadInt32(); out_data = (T)System.Enum.Parse(typeof(T), temp.ToString()); }
        else if (typeof(double) == reader.GetType()) { double temp = reader.ReadDouble(); out_data = (T)System.Enum.Parse(typeof(T), temp.ToString()); }
        else if (typeof(float) == reader.GetType()) { float temp = reader.ReadSingle(); out_data = (T)System.Enum.Parse(typeof(T), temp.ToString()); }
        else { int temp = reader.ReadInt32(); out_data = (T)System.Enum.Parse(typeof(T), temp.ToString()); }
    }
    public T ReadEnum<T>()
    {
        T out_data = default(T);
        if (null == reader)
            return out_data;

        ReadEnum<T>(out out_data);
        return out_data;
    }
}

public static class GameEventSubject
{
    static public Dictionary<GameEventType, List<GameEventHandler>> game_event_handlers = new Dictionary<GameEventType, List<GameEventHandler>>();
    static public List<GameEvent> reserved_remove_async_game_event_list = new List<GameEvent>();
    // UnregisterGameEvent 처리되는 경우 최대 game_event_handlers 값이 줄어들기 때문에 값으로 저장해서 처리합니다
    static public int nCountLoad = 0;
    static public int nMaxLoad = 0;

    static public void RegisterHandler(GameEventType event_type, GameEventHandler handler)
    {
        if (null == game_event_handlers)
            return;

        if (game_event_handlers.ContainsKey(event_type) == false)
            game_event_handlers.Add(event_type, new List<GameEventHandler>());

        List<GameEventHandler> handler_list = game_event_handlers[event_type];
        if (handler_list.Contains(handler) == false)
            handler_list.Add(handler);
    }

    static public void UnregisterHandler(GameEventType event_type, GameEventHandler handler)
    {
        if (null == game_event_handlers)
            return;
        if (game_event_handlers.ContainsKey(event_type) == false)
            return;

        game_event_handlers[event_type].Remove(handler);
    }

    static public bool Subject(GameEvent in_ge, bool in_bLoad = false)
    {
        if (null == in_ge)
            return false;
        if (null == game_event_handlers)
            return false;

        if (game_event_handlers.ContainsKey(in_ge.event_type)
            && 0 < game_event_handlers[in_ge.event_type].Count)
        {
            // UnregisterGameEvent 처리되는 경우 최대 game_event_handlers 값이 줄어들기 때문에 값으로 저장해서 처리합니다
            List<GameEventHandler> handlers = new List<GameEventHandler>();
            List<GameEventHandler> handlers_tmp = game_event_handlers[in_ge.event_type];
            for (int i = 0; i < handlers_tmp.Count; ++i)
            {
                handlers.Add(handlers_tmp[i]);
            }

            if (true == in_bLoad)
            {
                nCountLoad = 0;
                nMaxLoad = handlers.Count;
            }

            for (int i = 0; i < handlers.Count; ++i)
            {
                in_ge.Reset();
                if (true == in_bLoad)
                    in_ge.Callback_GameEventSubject = Callback_GameEventSubject;
                handlers[i].HandleGameEvent(in_ge);
            }

            in_ge.HandleUsedEvent();
            in_ge.Close();

            handlers.Clear();
            handlers = null;
        }
        else
        {
            Debug.Log("생성되었지만 사용되지 않은 게임 이벤트 : " + in_ge.event_type.ToString());
            return false;
        }

        return true;
    }

    static public void Callback_GameEventSubject()
    {
        Debug.Log(string.Format("Callback_GameEventSubject : nCountLoad : {0}, nMaxLoad : {1}", nCountLoad, nMaxLoad));
    }

    static public void OnDestroy()
    {
        if (null != game_event_handlers)
            game_event_handlers.Clear();
        game_event_handlers = null;

        if (null != reserved_remove_async_game_event_list)
            reserved_remove_async_game_event_list.Clear();
        reserved_remove_async_game_event_list = null;
    }
}