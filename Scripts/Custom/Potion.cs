using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    [SerializeField] protected int attribute; //0-6
    [SerializeField] protected List<Material> colors;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<MeshRenderer>().material = colors[attribute];
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player") && collider.gameObject.GetComponent<PlayerController>() != null && collider.gameObject.GetComponent<PlayerController>().GivePotion(attribute))
        {
            Destroy(gameObject);
        }
    }

    public bool SetPotionAttribute(int a)
    {
        if (a >= 0 && a <= 6)
        {
            attribute = a;
            return true;
        }
        return false;
    }
}
