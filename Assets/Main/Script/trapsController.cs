using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trapsController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag.StartsWith("Player")) {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(1000, PlayerController.Equiment.PUNCH);
        }
    }
}
