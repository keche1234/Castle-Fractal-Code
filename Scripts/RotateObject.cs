using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rotates an object continously
public class RotateObject : MonoBehaviour
{
    [SerializeField] protected Vector3 rotateSpeed; //speed along x-, y-, and z-axes, in degrees per second

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotateSpeed * Time.deltaTime);
    }
}
