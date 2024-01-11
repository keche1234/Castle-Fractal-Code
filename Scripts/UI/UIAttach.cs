using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAttach : MonoBehaviour
{
    [SerializeField] protected GameObject obj;
    [SerializeField] protected Camera cam;
    [SerializeField] protected Vector2 offset;
    // Start is called before the first frame update
    void Start()
    {
        //offsetPos = transform.position - obj.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (cam != null)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(obj.transform.position);
            //Vector3 toCamera = (cam.transform.position - obj.transform.position).normalized * 2;
            transform.position = cam.ScreenToWorldPoint(new Vector3(screenPos.x + offset.x, screenPos.y + offset.y, cam.transform.position.y - obj.transform.position.y));
        }
    }

    public void Setup(GameObject g, Camera c, Vector2 os)
    {
        obj = g;
        cam = c;
        offset = os;
    }

    public void NewOffset(Vector2 os)
    {
        offset = os;
    }
}
