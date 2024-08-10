using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowShrinkAppearanceUI : MonoBehaviour
{
    [SerializeField] protected Vector3 minScale;
    [SerializeField] protected Vector3 maxScale;
    [SerializeField] protected Vector3 speed;
    protected Vector3 currentScale;
    protected bool growing = false;

    // Start is called before the first frame update
    void Start()
    {
        currentScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (growing)
        {
            currentScale = new Vector3(Mathf.Min(maxScale.x, currentScale.x + (speed.x * Time.deltaTime)),
                                        Mathf.Min(maxScale.y, currentScale.y + (speed.y * Time.deltaTime)),
                                        Mathf.Min(maxScale.z, currentScale.z + (speed.z * Time.deltaTime)));
            transform.localScale = currentScale;
        }
        else
        {
            currentScale = new Vector3(Mathf.Max(minScale.x, currentScale.x - (speed.x * Time.deltaTime)),
                                        Mathf.Max(minScale.y, currentScale.y - (speed.y * Time.deltaTime)),
                                        Mathf.Max(minScale.z, currentScale.z - (speed.z * Time.deltaTime)));
            transform.localScale = currentScale;
        }
    }

    public void SetGrowing(bool b)
    {
        growing = b;
    }
}
