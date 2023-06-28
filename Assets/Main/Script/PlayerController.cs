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
    GameObject otherPlayer;
    enum Equiment { 
        AXE,SWORD,SPEAR,PUNCH
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

        if (player == 1)
        {
            otherPlayer = GameObject.Find("Player2");
        }
        else {
            otherPlayer = GameObject.Find("Player1");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //����摜
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

        //����
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
        //�ړ�
        //��
        Vector3 vec = new Vector3(Input.GetAxis(playerStr + "_Horizontal") * moveSpeed * Time.deltaTime, 0, 0);
        transform.Translate(vec);        
        if (Input.GetAxis(playerStr + "_Horizontal") > 0) transform.localScale = new Vector3(-1, 1, 1);
        else if (Input.GetAxis(playerStr + "_Horizontal") < 0) transform.localScale = new Vector3(1, 1, 1);

        //�W�����v        
        if ((player == 1 ? Input.GetKeyDown(KeyCode.W) : Input.GetKeyDown(KeyCode.UpArrow)) && onGround) {
            rb.velocity = new Vector2(0, jumpPow);
            onGround= false;
        }

        //�ړ��A�j���[�V����
        if(Input.GetAxis(playerStr + "_Horizontal")!=0) animator.SetBool("walking",true);
        else animator.SetBool("walking", false);  
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Floor")) {
            onGround = true;
        }
        if (collision.transform.CompareTag("Axe") ) {
            equiment = Equiment.AXE;
        }
        if (collision.transform.CompareTag("Sword")) {
            equiment = Equiment.SWORD;
        }
        if (collision.transform.CompareTag("Spear")) {
            equiment = Equiment.SPEAR;
        }
    }
}
