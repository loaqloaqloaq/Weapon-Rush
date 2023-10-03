using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swordEffectController : MonoBehaviour
{
    float time;
    float disTime;

    public string attacker;
    // Start is called before the first frame update
    void Start()
    {
        time = 0;
        disTime = 0.3f;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > disTime) Destroy(gameObject);

        transform.Translate(
        new Vector3(
            (
                transform.localScale.x / Mathf.Abs(transform.localScale.x)) * Time.deltaTime * 10,
                0,
                0
            )
        );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.tag);
        if(collision.CompareTag("Block")) Destroy(gameObject);
        //プレイヤにダメージ与える
        if (collision.tag.StartsWith("Player") && !collision.CompareTag(attacker)) collision.GetComponent<PlayerController>().TakeDamage(10, PlayerController.Equiment.SWORD);    
    }
   
}
