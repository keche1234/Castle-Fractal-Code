using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlManager : MonoBehaviour
{
    [SerializeField] protected List<ControlPreset> presets;
    // Start is called before the first frame update
    void Start()
    {
        if (presets == null)
            presets = new List<ControlPreset>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool ContainsName(string n)
    {
        for (int i = 0; i < presets.Count; i++)
            if (presets[i].GetPresetName() == n)
                return true;
        return false;
    }

    /*
     * Adding Presets
     */
    public bool AddControlPreset(ControlPreset p)
    {
        if (ContainsName(p.GetPresetName()))
        {
            Debug.LogWarning("There is already a preset with the name \"" + p.GetPresetName() + "\" in this ControlManager!");
            return false;
        }

        presets.Add(p);
        return true;
    }

    // Attempts to create a new preset with this name
    public bool AddControlPreset(string n)
    {
        if (ContainsName(n))
        {
            Debug.LogWarning("There is already a preset with the name \"" + n + "\" in this ControlManager!");
            return false;
        }

        ControlPreset p = new ControlPreset();
        p.gameObject.transform.parent = transform;
        presets.Add(p);
        return true;
    }

    /*
     * Setting names
     */
    public bool SetPresetName(int i, string n)
    {
        if (ContainsName(n))
        {
            Debug.LogWarning("There is already a preset with the name \"" + n + "\" in this ControlManager!");
            return false;
        }

        if (i < 0 || i >= presets.Count)
        {
            Debug.LogWarning("Tried to get preset out of range!");
            return false;
        }

        presets[i].SetPresetName(n);
        return true;
    }

    /*
     * Getting Control Presets
    */
    public ControlPreset GetControlPreset(int i)
    {
        if (i < 0 || i >= presets.Count)
        {
            Debug.LogWarning("Tried to get preset out of range!");
            return null;
        }

        return presets[i];
    }

    public ControlPreset GetControlPreset(string n)
    {
        for (int i = 0; i < presets.Count; i++)
            if (presets[i].GetPresetName() == n)
                return presets[i];

        Debug.LogWarning("No preset with the name \"" + n + "\"!");
        return null;
    }

    public int PresetCount()
    {
        return presets.Count;
    }
}
