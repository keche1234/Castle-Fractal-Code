using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] protected Transform cam;

    // Update is called once per frame
    void LateUpdate()
    {
        if (cam != null)
            transform.LookAt(transform.position + cam.forward);
    }

    public void SetCamera(Camera c)
    {
        cam = c.transform;
    }
}
