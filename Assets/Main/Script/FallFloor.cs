using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallFloor : MonoBehaviour
{
    private Animator fall_anim;

    // Start is called before the first frame update
    void Start()
    {
        fall_anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag.StartsWith("Player"))
        {
            fall_anim.SetTrigger("fallfloor");
        }

    }
}
