using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UIManager;

public class AIController : MonoBehaviour
{   
    Animator animator;
    GameObject other;
    float moveSpeed, jumpPow;
    GameObject[] blocks;
    string[] weapons = { "Axe","Spear","Sword"};
    string weapon;
    bool startAttack;

    [HideInInspector]
    public bool enableAttack, enableWalk;
    // Start is called before the first frame update
    void Start()
    {  
        animator = GetComponent<Animator>();
        if (transform.CompareTag("Player1")) other = GameObject.Find("Player2");
        else other = GameObject.Find("Player1");
        moveSpeed = GetComponent<PlayerController>().moveSpeed;
        jumpPow = GetComponent<PlayerController>().jumpPow;
        blocks = GameObject.FindGameObjectsWithTag("Block");
        weapon = weapons[Random.Range(0,2)];       
        startAttack = false;
        // weapons = GameObject.Find("Weapons").GetComponentsInChildren<GameObject>();  
        enableAttack = true;
        enableWalk = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<PlayerController>().HP <= 0) return;
        
        bool attacking = animator.GetCurrentAnimatorStateInfo(0).IsName("punch attack") || animator.GetCurrentAnimatorStateInfo(0).IsName("sword attack") || animator.GetCurrentAnimatorStateInfo(0).IsName("axe attack") || animator.GetCurrentAnimatorStateInfo(0).IsName("spear attack");
        //距離を測る
        Vector3 cpuHorizontalPosition = transform.position;
        cpuHorizontalPosition.y = 0;
        Vector3 otherHorizontalPosition = other.transform.position;
        otherHorizontalPosition.y = 0;
        float distance = Vector3.Distance(cpuHorizontalPosition, otherHorizontalPosition);

        //追って攻撃する
        if (distance < 1.5f) startAttack = true;
        else if (distance > 2.5f) startAttack = false;

        //移動方向
        float direction = 1;
        if (other.transform.position.x < transform.position.x) direction = -1;
        //画像の向き
        transform.localScale = new Vector3(direction * -1, 1, 1);

        if (!startAttack)
        {
            if (!attacking) { 
                GetComponent<PlayerController>().DisableWeapon();
            }
            if (enableWalk)
            {
                //移動
                Vector3 vec = new Vector3(direction * moveSpeed * Time.deltaTime, 0, 0);
                transform.Translate(vec);
                animator.SetBool("walking", true);

                //石の前ジャンプ
                foreach (var block in blocks)
                {
                    float XdistanceToBlock = transform.position.x - block.transform.position.x;
                    float YdistanceToBlock = transform.position.y - block.transform.position.y;
                    float distanceToBlock = Vector3.Distance(block.transform.position, transform.position);
                    if (distanceToBlock < 2 && YdistanceToBlock >= -1f)
                    {
                        //右
                        if (direction == 1 && XdistanceToBlock >= -2 && XdistanceToBlock <= 0)
                        {
                            GetComponent<PlayerController>().Jump();
                        }
                        //左
                        else if (direction == -1 && XdistanceToBlock >= 0 && XdistanceToBlock <= 2)
                        {
                            GetComponent<PlayerController>().Jump();
                        }
                    }
                }
            }
            else {
                animator.SetBool("walking", false);
            }
        }
        else if (!attacking) {
            animator.SetBool("walking", false);
            if (enableAttack) GetComponent<PlayerController>().Attack();
        }
        else
        {
            animator.SetBool("walking", false);
        }

        
    }

    public void setMoveSpeed(float s) {
        moveSpeed = s;
    }
    public void setJumpPow(float j) { 
        jumpPow = j;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GetComponent<PlayerController>().player == 3 && collision.CompareTag(weapon) && GetComponent<PlayerController>().CurrentWeapon() == PlayerController.Equiment.PUNCH) {
            GetComponent<PlayerController>().GetWeapon(collision.gameObject);
        }
    }
}

