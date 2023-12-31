using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;
using static UIManager;
using static Unity.Collections.AllocatorManager;

public class AIController : MonoBehaviour
{   
    Animator animator;
    GameObject otherPl;
    float moveSpeed, jumpPow;
    GameObject[] blocks;
    GameObject[] walls;
    List<GameObject> weaponsObj = new List<GameObject>();
    GameObject targetWeapon;
    string[] weapons = { "Axe","Spear","Sword"};
    string weapon;
    bool startAttack;   
    float walkaround;

    [HideInInspector]
    public bool enableAttack, enableWalk;

    float prePosX;
    float testStuckTimer;

    PlayerController pc;
    // Start is called before the first frame update
    void Start()
    {  
        animator = GetComponent<Animator>();
        pc = GetComponent<PlayerController>();
        //find enemy
        if (transform.CompareTag("Player1")) otherPl = GameObject.Find("Player2");
        else otherPl = GameObject.Find("Player1");
        
        //get parameter from player controller
        moveSpeed = pc.moveSpeed;
        jumpPow = pc.jumpPow;
        blocks = GameObject.FindGameObjectsWithTag("Block");
        walls = GameObject.FindGameObjectsWithTag("Wall");
        weapon = weapons[UnityEngine.Random.Range(0,3)];
       
        startAttack = false;         
        enableAttack = true;
        enableWalk = true;

        walkaround = 1;
        testStuckTimer = 0;
        prePosX = transform.position.x;
        
    }

    // Update is called once per frame
    void Update()
    {          
        //stop when someone die
        if (pc.HP <= 0 || otherPl.GetComponent<PlayerController>().HP <= 0 || GameDirector.end) return;
        GameObject other = otherPl;

        //find weapon if don't have
        if (pc.equiment == PlayerController.Equiment.PUNCH)
        {
            int allWeaponCnt = GameObject.FindGameObjectsWithTag("Axe").Length + GameObject.FindGameObjectsWithTag("Spear").Length + GameObject.FindGameObjectsWithTag("Sword").Length;
            if (weaponsObj.Count < allWeaponCnt)
            {
                GetClosestWeapon();
            }
            if (targetWeapon != null)
            {
                other = targetWeapon;
            }
        }
        //check if attack is finished
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
        if (horizontalDistance < 1.5f && absVerticalDistance < 1.7f && pc.equiment != PlayerController.Equiment.PUNCH) startAttack = true;
        else if (horizontalDistance > 2.0f || absVerticalDistance > 1.7f) startAttack = false;

        if (!attacking)
        {
            pc.DisableWeapon();
            if (pc.lastAtk < 0) pc.lastAtk = 0;
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
                    if(movedX < 2) pc.Jump();
                }

                //移動
                //プレイヤーのYが同じくらい   
                float direction = 1;                               
                if (verticalDistance > -1f && verticalDistance < 1f) {                    
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
                    
                    if (cpuVertialPosition.y < otherVerticalPosition.y) pc.Jump();     
                    else pc.Down();
                    if (transform.position.x >= 29.3f) walkaround = -1;
                    if (transform.position.x <= -9.3f) walkaround = 1;                    
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
                            pc.Jump();
                        }
                        //左
                        else if (direction == -1 && XdistanceToBlock >= 0 && XdistanceToBlock <= 2)
                        {
                            pc.Jump();
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
            //方向
            float direction = 1;
            if (other.transform.position.x < transform.position.x) direction = -1;
            //画像の向き
            transform.localScale = new Vector3(direction * -1, 1, 1);
            //攻撃
            animator.SetBool("walking", false);
            if (enableAttack && pc.equiment != PlayerController.Equiment.PUNCH)
            {               
                pc.CpuAttack();                
            }
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
        if (pc.player == 3 && collision.CompareTag(weapon) && pc.CurrentWeapon() == PlayerController.Equiment.PUNCH) {
            pc.GetWeapon(collision.gameObject);
        }
    }

    private void GetClosestWeapon() {
        foreach (var tag in weapons)
        {
            var tmp = GameObject.FindGameObjectsWithTag(tag);
            weaponsObj.AddRange(tmp);
        }        
        targetWeapon = null;
        foreach (var w in weaponsObj)
        {
            if (w.CompareTag(weapon))
            {
                float dis = Vector3.Distance(transform.position, w.transform.position);
                if (targetWeapon == null)
                {
                    targetWeapon = w;
                }
                else
                {
                    float preDis = Vector3.Distance(transform.position, targetWeapon.transform.position);
                    if (dis < preDis) targetWeapon = w;
                }
            }
        }
    }
}

