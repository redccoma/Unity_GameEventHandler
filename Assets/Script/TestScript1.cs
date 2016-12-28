using UnityEngine;
using System.Collections;

public class TestScript1 : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return new WaitForSeconds(3);
        GameUtil.SendGameEvent(GameEventType.TEST_EVENT);
        yield return new WaitForSeconds(3);
        

        GameUtil.SendGameEvent_List(GameEventType.TEST_EVENT1, null, 7);
    }
}
