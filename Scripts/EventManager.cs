using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void TriggerAction();
    //public static event TriggerAction OnFreeze;

    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    void Update()
    {
        //if (Character.GetFreezeTargets().Count > 0 && OnFreeze != null)
        //{
        //    OnFreeze();
        //    Character.GetFreezeTargets().RemoveAll(_ => true);
        //    Character.GetFreezeDurs().RemoveAll(_ => true);
        //}
    }
}
