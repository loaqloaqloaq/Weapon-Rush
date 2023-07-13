using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerPointerArrow : MonoBehaviour
{
    GameObject player;
    GameObject otherPlayer;
    public int playernum=1;
    // Start is called before the first frame update
    void Start()
    {
        if (playernum == 1)
        {
            player = GameObject.Find("Player1");
            otherPlayer = GameObject.Find("Player2");
        }
        else {
            player = GameObject.Find("Player2");
            otherPlayer = GameObject.Find("Player1");
        }        
    }

    // Update is called once per frame
    void Update()
    {

        float distance = Vector3.Distance(player.transform.position, otherPlayer.transform.position);

        if (distance > 9.23f)
        {
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            Vector3 _direction = (otherPlayer.transform.position - player.transform.position).normalized;
            transform.position = player.transform.position + _direction + new Vector3(0, 1, 0);

            transform.localScale = new Vector3(_direction.x < 0 ? -1 : 1, 1, 1);
            Quaternion rotation = Quaternion.LookRotation(_direction);
            transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);
        }
        else {
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        }
    }
}
