using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PlayerController : MonoBehaviour
{
    public GameObject weapon,axePre,swordPre,spearPre;
    public Sprite axe, sword, spear;
    Rigidbody2D rb;
    Animator animator;
    GameObject otherPlayer;
    struct KeyBind {
        public string move;
        public KeyCode atk, jump, drop;
    }
    KeyBind[] input=new KeyBind[2];
    enum Equiment { 
        AXE,SWORD,SPEAR,PUNCH,NON
    };
    Equiment equiment;   
    GameObject onHoverObject;

    public int player = 1;
    float moveSpeed = 5.0f;
    float jumpPow = 7.0f;    
    bool onGround;

    float facing;
    // Start is called before the first frame update
    void Start()
    {
        equiment=Equiment.PUNCH;        
        weapon = transform.Find("Body/Front arm/Weapon").gameObject;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        onGround = false;       
        onHoverObject = null;

        if (player == 1)
        {
            otherPlayer = GameObject.Find("Player2");
        }
        else {
            otherPlayer = GameObject.Find("Player1");
        }

        //キーバインド
        //プレイヤー　１
        input[0].move = "Player1_Horizontal";
        input[0].atk = KeyCode.Space;
        input[0].drop = KeyCode.LeftControl;
        input[0].jump = KeyCode.W;
        //プレイヤー　2
        input[1].move = "Player2_Horizontal";
        input[1].atk = KeyCode.Keypad0;
        input[1].drop = KeyCode.RightControl;
        input[1].jump = KeyCode.UpArrow;

        facing = player == 1 ? -1.0f : 1.0f;

    }

    // Update is called once per frame
    void Update()
    {
        //一時停止時はプレイヤーを動けないようにする
        if (Mathf.Approximately(Time.timeScale, 0.0f))
        {
            return;
        }

        //武器画像
        if (equiment == Equiment.AXE)
        {
            weapon.SetActive(true);
            weapon.GetComponent<SpriteRenderer>().sprite = axe;
        }
        else if (equiment == Equiment.SPEAR)
        {
            weapon.SetActive(true);
            weapon.GetComponent<SpriteRenderer>().sprite = spear;
        }
        else if (equiment == Equiment.SWORD)
        {
            weapon.SetActive(true);
            weapon.GetComponent<SpriteRenderer>().sprite = sword;
        }
        else if (equiment == Equiment.PUNCH) {
            weapon.SetActive(false);   
        }

        
        bool attacking = animator.GetCurrentAnimatorStateInfo(0).IsName("punch attack") || animator.GetCurrentAnimatorStateInfo(0).IsName("sword attack") || animator.GetCurrentAnimatorStateInfo(0).IsName("axe attack");
        if (Input.GetKeyDown(input[player-1].atk)&& !attacking)
        {
            //攻撃
            if (onHoverObject == null)
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
            //武器を拾う
            else if (onHoverObject != null)
            {
                switch (onHoverObject.tag)
                {
                    case "Axe":
                        DropWeapon();
                        equiment = Equiment.AXE;
                        Destroy(onHoverObject);
                        break;
                    case "Sword":
                        DropWeapon();
                        equiment = Equiment.SWORD;
                        Destroy(onHoverObject);
                        break;
                    case "Spear":
                        DropWeapon();
                        equiment = Equiment.SPEAR;
                        Destroy(onHoverObject);
                        break;
                    default: break;
                }                
            }
        }
        //武器捨てる
        if (Input.GetKeyDown(input[player - 1].drop) && !attacking)
        {
            DropWeapon();
            equiment = Equiment.PUNCH;
        }

        //移動
        Vector3 vec = new Vector3(Input.GetAxis(input[player - 1].move) * moveSpeed * Time.deltaTime, 0, 0);
        transform.Translate(vec);
        if (Input.GetAxis(input[player - 1].move) > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            facing = -1.0f;
        }
        else if (Input.GetAxis(input[player - 1].move) < 0)
        {
            facing = 1.0f;
            transform.localScale = new Vector3(1, 1, 1);
        }

        //ジャンプ       
        if (Input.GetKeyDown(input[player - 1].jump) && onGround) {
            rb.velocity = new Vector2(0, jumpPow);
            onGround= false;
        }

        //移動アニメーション
        if(Input.GetAxis(input[player - 1].move) !=0) animator.SetBool("walking",true);
        else animator.SetBool("walking", false);  
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

       if (collision.transform.CompareTag("Floor")) {
            onGround = true;
        }       
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Axe")|| collision.transform.CompareTag("Spear")|| collision.transform.CompareTag("Sword"))
        {
            onHoverObject = collision.gameObject;
        }        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        onHoverObject = null;
    }

    private void DropWeapon()
    {
        if (equiment == Equiment.PUNCH) return;

        GameObject droppedWeapon = null;
        if (equiment == Equiment.AXE)
        {
            droppedWeapon = Instantiate(axePre, transform.position, Quaternion.identity);
        }
        else if(equiment == Equiment.SWORD) {
            droppedWeapon = Instantiate(swordPre, transform.position, Quaternion.identity);
        }
        else if (equiment == Equiment.SPEAR)
        {
            droppedWeapon = Instantiate(spearPre, transform.position, Quaternion.identity);
        }
        droppedWeapon.GetComponent<Rigidbody2D>().velocity=new Vector3(facing*2.0f,5.0f,0 );  
    }
}
