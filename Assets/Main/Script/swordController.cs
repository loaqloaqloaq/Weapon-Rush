using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class swordController : MonoBehaviour
{
    bool throwing = false;
    string plTag="";

    // Update is called once per frame
    void Update()
    {
        
    }

    public void throwSword(string plTag) {
        throwing = true;
        this.plTag = plTag;
    }

    void dropOnFloor() {
        throwing = false;
        plTag = "";
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        transform.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        dropOnFloor();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (throwing && !collision.transform.CompareTag(plTag))
        {
            if (collision.transform.tag.StartsWith("Player"))
            {
                collision.GetComponent<PlayerController>().TakeDamage(10, PlayerController.Equiment.SWORD);
                throwing = false;
            }
            //dropOnFloor();
        }
    }
}
