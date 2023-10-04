using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakFloor : MonoBehaviour
{
    bool standing;

    public SpriteRenderer sp;

    Color curColor;
    private BoxCollider2D floor_collider;

    float revivalTime, revivalTimeCnt;

    // Start is called before the first frame update
    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        curColor = sp.color;

        floor_collider = GetComponent<BoxCollider2D>();

        revivalTime = 2.0f;
        revivalTimeCnt = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (standing) curColor.a -= 0.5f * Time.deltaTime;
        else if (curColor.a != 0f) curColor.a += 0.5f * Time.deltaTime;

        if (curColor.a <= 0)
        {
            curColor.a = 0;
            floor_collider.enabled = false;
        }

        if (curColor.a >= 1)
        {
            curColor.a = 1;
        }

        if (!floor_collider.isActiveAndEnabled)
        {
            revivalTimeCnt += Time.deltaTime;
            if (revivalTimeCnt >= revivalTime)
            {
                revivalTimeCnt = 0;
                curColor.a += 0.5f * Time.deltaTime;
                floor_collider.enabled = true;
            }
        }

        sp.color = new Color(curColor.r, curColor.g, curColor.b, curColor.a);

    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.tag.StartsWith("Player"))
        {
            standing = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.StartsWith("Player"))
        {
            standing = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.StartsWith("Player"))
        {
            standing = false;
        }
    }

    public void Break()
    {

    }

    public void Revive()
    {

    }
}

