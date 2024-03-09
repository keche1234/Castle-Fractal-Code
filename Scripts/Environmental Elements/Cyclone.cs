using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Cyclone : MonoBehaviour
{
    // Pull targets into center
    [Tooltip("m/s")]
    [SerializeField] protected float strength;
    [SerializeField] protected string targetTag;
    protected Collider pullField;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerStay(Collider other)
    {
        GameObject otherObj = other.gameObject;
        if (otherObj.tag == targetTag)
        {
            Vector3 direction = (transform.position - otherObj.transform.position).normalized;
            direction = new Vector3(direction.x, 0, direction.z);
            otherObj.transform.position += direction * strength * Time.deltaTime;
        }
    }
}
