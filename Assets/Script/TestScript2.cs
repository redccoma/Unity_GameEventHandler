using UnityEngine;
using System.Collections;

public class TestScript2 : GameEventHandler
{
    void Awake()
    {
        RegisterGameEvent(GameEventType.TEST_EVENT);
        RegisterGameEvent(GameEventType.TEST_EVENT1);
    }

    public override void OnDestroy()
    {
        UnregisterGameEvent();

        base.OnDestroy();
    }

    public override void HandleGameEvent(GameEvent ge)
    {
        switch (ge.event_type)
        { 
            case GameEventType.TEST_EVENT:
                Debug.Log("이벤트를 잘 받았습니당.");
                break;
            case GameEventType.TEST_EVENT1:
                Debug.Log(ge.ReadInt() + " / 이벤트를 잘 받았습니당.");
                break;
            default:
                break;
        }
    }
}
