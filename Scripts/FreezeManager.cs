using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FreezeManager : MonoBehaviour
{
    protected Dictionary<string, float> freezeTags;
    // Start is called before the first frame update
    void Start()
    {
        freezeTags = new Dictionary<string, float>();
    }

    //private void Awake()
    //{
    //    DontDestroyOnLoad(this);
    //}

    // Update is called once per frame
    void Update()
    {
        for (int i = freezeTags.Count - 1; i >= 0; i--)
        {
            freezeTags[freezeTags.ElementAt(i).Key] -= Time.deltaTime;
            //Debug.Log(freezeTags[freezeTags.ElementAt(i).Key]);
            if (freezeTags[freezeTags.ElementAt(i).Key] <= 0)
                freezeTags.Remove(freezeTags.ElementAt(i).Key);
        }
    }

    public float FreezeTagAdd(string tag, float time)
    {
        if (freezeTags.ContainsKey(tag))
            freezeTags[tag] += time;
        else
            freezeTags[tag] = time;

        return freezeTags[tag];
    }

    public void FreezeTagReplace(string tag, float time)
    {
        freezeTags[tag] = time;
    }

    public float GetFreezeTime(string tag)
    {
        if (freezeTags.ContainsKey(tag))
            return freezeTags[tag];
        return -1;
    }
}
