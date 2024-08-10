using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BackgroundLooper : MonoBehaviour
{
    protected Collider myCollider;
    [SerializeField] protected Vector3 pushVector;
    [SerializeField] protected float pushMagnitude;

    // Start is called before the first frame update
    void Start()
    {
        myCollider = GetComponent<Collider>();
        myCollider.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        myCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ScrollBackground>())
            other.gameObject.transform.Translate(pushVector.normalized * pushMagnitude, Space.World);
    }


}
