using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Player1Controller : MonoBehaviour
{
    public GameObject weapon;
    public Sprite axe, sword;
    Rigidbody2D rb;
    Animator animator;
    enum Equiment { 
        AXE,SWORD,PUNCH
    };
    Equiment equiment;
    
    public int player = 1;
    float moveSpeed = 5.0f;
    float jumpPow = 7.0f;
    string playerStr;
    bool onGround;
    // Start is called before the first frame update
    void Start()
    {
        equiment=Equiment.AXE;
        playerStr = "Player" + player.ToString();
        weapon = transform.Find("Body/Front arm/Weapon").gameObject;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        onGround = false;
    }

    // Update is called once per frame
    void Update()
    {
        //武器画像
        if (equiment == Equiment.AXE)
        {
            weapon.SetActive(true);
            weapon.GetComponent<SpriteRenderer>().sprite = axe;
            if (Input.GetKeyDown(KeyCode.I)) equiment = Equiment.SWORD;
        }
        else if (equiment == Equiment.SWORD)
        {
            weapon.SetActive(true);
            weapon.GetComponent<SpriteRenderer>().sprite = sword;
            if (Input.GetKeyDown(KeyCode.I)) equiment = Equiment.PUNCH;
        }
        else if (equiment == Equiment.PUNCH) {
            weapon.SetActive(false);            
            if (Input.GetKeyDown(KeyCode.I)) equiment = Equiment.AXE;
        }

        //操作
        bool attacking = animator.GetCurrentAnimatorStateInfo(0).IsName("punch attack") || animator.GetCurrentAnimatorStateInfo(0).IsName("sword attack") || animator.GetCurrentAnimatorStateInfo(0).IsName("axe attack");        
        if ((player==1?Input.GetKeyDown(KeyCode.Space):Input.GetKeyDown(KeyCode.Keypad0)) && !attacking)
        {
            if (equiment == Equiment.AXE)
            {                
                animator.SetTrigger("axe");
            }
            else if (equiment == Equiment.SWORD)
            {
               animator.SetTrigger("sword");
            }
            else if (equiment == Equiment.PUNCH)
            {
                animator.SetTrigger("punch");
            }
        }        
        //移動
        //横
        Vector3 vec = new Vector3(Input.GetAxis(playerStr + "_Horizontal") * moveSpeed * Time.deltaTime, 0, 0);
        transform.Translate(vec);        
        if (Input.GetAxis(playerStr + "_Horizontal") > 0) transform.localScale = new Vector3(-1, 1, 1);
        else if (Input.GetAxis(playerStr + "_Horizontal") < 0) transform.localScale = new Vector3(1, 1, 1);

        //ジャンプ        
        if ((player == 1 ? Input.GetKeyDown(KeyCode.W) : Input.GetKeyDown(KeyCode.UpArrow)) && onGround) {
            rb.velocity = new Vector2(0, jumpPow);
            onGround= false;
        }

        //移動アニメーション
        if(Input.GetAxis(playerStr + "_Horizontal")!=0) animator.SetBool("walking",true);
        else animator.SetBool("walking", false);  
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Floor")) {
            onGround = true;
        }
    }
}
