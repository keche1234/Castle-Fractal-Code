using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InfoFlash : MonoBehaviour
{
    [SerializeField] protected List<InfoFlash> otherFlashes;

    [Header("Appearing and Fading")]
    [SerializeField] protected float appearTime;
    [SerializeField] protected float fadeTime;
    protected float appearTimeRemaining = 0;
    protected float fadeTimeRemaining = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract void DrawInfoFlash();

    public void OverrideOtherFlashes()
    {
        foreach (InfoFlash flash in otherFlashes)
        {
            if (flash)
                flash.gameObject.SetActive(false);
            else
                Debug.LogWarning("Other flash list in " + this.gameObject + " contains null!");
        }
        gameObject.SetActive(true);
    }
}
