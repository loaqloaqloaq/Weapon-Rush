using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakFloor : MonoBehaviour
{
    private Animator break_anim;

    private BoxCollider2D floor_collider;
    private float break_time;
    private float break_count;
    private bool floor_break;

    // Start is called before the first frame update
    void Start()
    {
        break_anim = GetComponent<Animator>();
        floor_collider = GetComponent<BoxCollider2D>();
        break_time = 5.0f;
        break_count = 0;
        floor_break = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(break_count > 0)
        {
            break_count -= Time.deltaTime;
        }
        else if(floor_break)
        {
            break_anim.SetTrigger("Revival");         
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag.StartsWith("Player") && !floor_break)
        { 
            break_anim.SetTrigger("break");
        }
    }

    public void Break()
    {
        floor_collider.enabled = false;
        floor_break = true;
        break_count = break_time;
    }

    public void Revive()
    {
        floor_break = false;
        floor_collider.enabled = true;
    }
}
