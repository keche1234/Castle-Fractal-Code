using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAttach : MonoBehaviour
{
    [SerializeField] protected GameObject obj;
    [SerializeField] protected Camera cam;
    [SerializeField] protected bool anchorIsUI = false;
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
            if (anchorIsUI && GetComponent<RectTransform>() != null)
            {
                Vector3 correction = obj.transform.position - cam.transform.position;
                GetComponent<RectTransform>().anchoredPosition = offset + new Vector2(correction.x, correction.z);
            }
            else
            {

                Vector3 screenPos = cam.WorldToScreenPoint(obj.transform.position);
                //Vector3 toCamera = (cam.transform.position - obj.transform.position).normalized * 2;
                float depth = Mathf.Min(new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z).magnitude, cam.transform.position.y - 1f);
                transform.position = cam.ScreenToWorldPoint(new Vector3(screenPos.x + offset.x, screenPos.y + offset.y, depth));
            }
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

    public void NewCamera(Camera c)
    {
        cam = c;
    }

    public GameObject GetAnchor()
    {
        return obj;
    }
}
