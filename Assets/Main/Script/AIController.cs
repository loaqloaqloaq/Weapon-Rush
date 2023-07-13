using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UIManager;
using static Unity.Collections.AllocatorManager;

public class AIController : MonoBehaviour
{   
    Animator animator;
    GameObject other;
    float moveSpeed, jumpPow;
    GameObject[] blocks;
    GameObject[] walls;
    string[] weapons = { "Axe","Spear","Sword"};
    string weapon;
    bool startAttack;   
    float walkaround;

    [HideInInspector]
    public bool enableAttack, enableWalk;

    float prePosX;
    float testStuckTimer;
    // Start is called before the first frame update
    void Start()
    {  
        animator = GetComponent<Animator>();
        if (transform.CompareTag("Player1")) other = GameObject.Find("Player2");
        else other = GameObject.Find("Player1");
        
        moveSpeed = GetComponent<PlayerController>().moveSpeed;
        jumpPow = GetComponent<PlayerController>().jumpPow;
        blocks = GameObject.FindGameObjectsWithTag("Block");
        walls = GameObject.FindGameObjectsWithTag("Wall");
        weapon = weapons[UnityEngine.Random.Range(0,2)];       
        startAttack = false;
        // weapons = GameObject.Find("Weapons").GetComponentsInChildren<GameObject>();  
        enableAttack = true;
        enableWalk = true;

        walkaround = 1;
        testStuckTimer = 0;
        prePosX = transform.position.x;
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
        float horizontalDistance = Vector3.Distance(cpuHorizontalPosition, otherHorizontalPosition);

        Vector3 cpuVertialPosition = transform.position;
        cpuHorizontalPosition.x = 0;
        Vector3 otherVerticalPosition = other.transform.position;
        otherHorizontalPosition.x = 0;

        float verticalDistance = cpuVertialPosition.y - otherVerticalPosition.y;
        float absVerticalDistance = Math.Abs(verticalDistance); 
        
        //追って攻撃する
        if (horizontalDistance < 1.5f && absVerticalDistance < 1) startAttack = true;
        else if (horizontalDistance > 2.0f && absVerticalDistance > 1) startAttack = false;

        if (!attacking)
        {
            GetComponent<PlayerController>().DisableWeapon();
            if (GetComponent<PlayerController>().lastAtk < 0) GetComponent<PlayerController>().lastAtk = 0;
        } 

        if (!startAttack)
        {            
            if (enableWalk)
            {
                testStuckTimer += Time.deltaTime;
                if (testStuckTimer >= 0.5f) {
                    float movedX = Math.Abs(prePosX - transform.position.x)/ testStuckTimer;                    
                    prePosX = transform.position.x;
                    testStuckTimer = 0;
                    if(movedX < 2) GetComponent<PlayerController>().Jump();
                }

                //移動
                //プレイヤーのYが同じくらい   
                float direction = 1;                               
                if (verticalDistance > -1f && verticalDistance < 1f) {
                    Debug.Log("水平");
                    //移動方向                    
                    if (other.transform.position.x < transform.position.x) direction = -1;
                    walkaround = direction;
                    //画像の向き
                    transform.localScale = new Vector3(direction * -1, 1, 1);
                    Vector3 vec = new Vector3(direction * moveSpeed * Time.deltaTime, 0, 0);
                    transform.Translate(vec);
                    animator.SetBool("walking", true);
                }                
                //プレイヤーが上か下 
                else
                {
                    
                    if (cpuVertialPosition.y < otherVerticalPosition.y) GetComponent<PlayerController>().Jump();                    
                    if (transform.position.x >= 29.3f) walkaround = -1;
                    if (transform.position.x <= -9.3f) walkaround = 1;
                    Debug.Log(walkaround);
                    transform.localScale = new Vector3(walkaround * -1, 1, 1);                    
                    transform.Translate(new Vector3(walkaround * moveSpeed * Time.deltaTime, 0, 0));
                    
                    animator.SetBool("walking", true);
                }
               

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
                //方向
                float direction = 1;
                if (other.transform.position.x < transform.position.x) direction = -1;
                //画像の向き
                transform.localScale = new Vector3(direction * -1, 1, 1);
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

