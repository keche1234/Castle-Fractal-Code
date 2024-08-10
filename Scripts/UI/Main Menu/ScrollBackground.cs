using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ScrollBackground : MonoBehaviour
{
    protected Rigidbody backgroundRb;
    [SerializeField] protected Vector3 scrollDirection;
    [SerializeField] protected float scrollSpeed;

    // Start is called before the first frame update
    void Start()
    {
        backgroundRb = GetComponent<Rigidbody>();
        scrollDirection = scrollDirection.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        scrollDirection = scrollDirection.normalized;
        backgroundRb.velocity = scrollDirection * scrollSpeed;
    }
}
