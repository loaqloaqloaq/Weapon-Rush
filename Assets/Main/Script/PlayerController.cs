using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using UnityEngine;
using static PlayerController;
using static UnityEditor.PlayerSettings;

public class PlayerController : MonoBehaviour
{
    GameObject weapon, frontArm, backArm;
    public GameObject axePre, swordPre, spearPre;
    public Sprite axe, sword, spear;
    Rigidbody2D rb;
    Animator animator;
    GameObject otherPlayer;

    //エフェクトを表示する位置
    private Transform effectTransform; 

    public float maxHP;

    [HideInInspector]
    public float HP;

    float dashing;
    float dashCoolDown;
    struct KeyBind {
        public string move;
        public KeyCode atk, jump, drop, dash;
       
    }
    KeyBind[] input = new KeyBind[2];
    KeyBind[] gamepad= new KeyBind[2];
    enum Equiment {
        AXE, SWORD, SPEAR, PUNCH, NON
    };
    Equiment equiment;
    GameObject onHoverObject;

    public int player = 1;
    float moveSpeed = 5.0f;
    float jumpPow = 10.0f;
    bool onGround;

    float attack;
    float atkMuiltpler;

    float facing;
    // Start is called before the first frame update
    void Start()
    {   
        weapon = transform.Find("Body/Front arm/Weapon").gameObject;
        frontArm = transform.Find("Body/Front arm").gameObject;
        backArm = transform.Find("Body/Back arm").gameObject;
        effectTransform = transform.Find("Body");
        weapon.GetComponent<CapsuleCollider2D>().enabled = false;
        frontArm.GetComponent<CapsuleCollider2D>().enabled = false;
        backArm.GetComponent<CapsuleCollider2D>().enabled = false;

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
        input[0].dash = KeyCode.LeftShift;
        
        //プレイヤー　2
        input[1].move = "Player2_Horizontal";
        input[1].atk = KeyCode.Keypad0;
        input[1].drop = KeyCode.RightControl;
        input[1].jump = KeyCode.UpArrow;
        input[1].dash = KeyCode.RightShift;

        //プレイヤー数値
        HP = maxHP;
        attack = 10.0f;
        atkMuiltpler = 0.5f;

        equiment = Equiment.PUNCH;
        weapon.SetActive(false);
        animator.SetBool("using spear", false);
    }

    // Update is called once per frame
    void Update()
    {
        
        //一時停止時や死んだの時、プレイヤーを動けないようにする 
        if (Mathf.Approximately(Time.timeScale, 0.0f) || HP<=0)
        {
            return;
        }
        //攻撃アニメーションが終わった判定
        bool attacking = animator.GetCurrentAnimatorStateInfo(0).IsName("punch attack") || animator.GetCurrentAnimatorStateInfo(0).IsName("sword attack") || animator.GetCurrentAnimatorStateInfo(0).IsName("axe attack") || animator.GetCurrentAnimatorStateInfo(0).IsName("spear attack");
        if (!attacking)
        {
            weapon.GetComponent<CapsuleCollider2D>().enabled = false;
            frontArm.GetComponent<CapsuleCollider2D>().enabled = false;
            backArm.GetComponent<CapsuleCollider2D>().enabled = false;
        }
        if (Input.GetKeyDown(input[player - 1].atk) && !attacking)
        {
            //攻撃            
            if (equiment == Equiment.AXE)
            {
                animator.SetTrigger("axe");
                Invoke("EnableWeapon", 0.24f);
            }
            else if (equiment == Equiment.SWORD)
            {
                animator.SetTrigger("sword");
                Invoke("EnableWeapon", 0.2f);
            }
            else if (equiment == Equiment.SPEAR)
            {
                animator.SetTrigger("spear");
                Invoke("EnableWeapon", 0.05f);
            }
            else if (equiment == Equiment.PUNCH)
            {
                frontArm.GetComponent<CapsuleCollider2D>().enabled = true;
                backArm.GetComponent<CapsuleCollider2D>().enabled = true;
                animator.SetTrigger("punch");
            }
            
                      
        }
        //武器捨てる and 武器を拾う  
        if (Input.GetKeyDown(input[player - 1].drop) && !attacking)
        {
            if (onHoverObject != null)
            {
                GetWeapon(onHoverObject);                
            }
            else
            {
                DropWeapon();
                equiment = Equiment.PUNCH;
            }
        }
        //移動とダッシュ
        float dashTime = 0.15f;
        float dashSpeed = 1.0f;
        //ダッシュ Cool Down
        float cooldown = 1.0f;
        if (Input.GetKeyDown(input[player - 1].dash) && dashing <= 0 && dashCoolDown <= 0)
        {
            dashing = dashTime;
            dashCoolDown = cooldown;
        }
        if (dashing > 0)
        {
            dashSpeed = 3.5f;
            dashing -= Time.deltaTime;
            if (dashing < 0) dashing = 0;
        }
        if(dashCoolDown > 0) dashCoolDown -= Time.deltaTime;
        Debug.Log(Input.GetAxis(input[player - 1].move));
        if (Input.GetAxis(input[player - 1].move) > 0.2f || Input.GetAxis(input[player - 1].move) < -0.2f)
        {
            Vector3 vec = new Vector3(Input.GetAxis(input[player - 1].move) * moveSpeed * dashSpeed * Time.deltaTime, 0, 0);
            transform.Translate(vec);
            animator.SetBool("walking", true);
        }
        else
        {
            animator.SetBool("walking", false);
        }

        //画像の向きと
        if (Input.GetAxis(input[player - 1].move) > 0.2f)
        {
            facing = -1.0f;
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (Input.GetAxis(input[player - 1].move) < -0.2f)
        {
            facing = 1.0f;
            transform.localScale = new Vector3(1, 1, 1);
        }
        //ジャンプ       
        if (Input.GetKeyDown(input[player - 1].jump) && onGround)
        {
            rb.velocity = new Vector2(0, jumpPow);
            onGround = false;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

       if (collision.transform.CompareTag("Floor")) {
            onGround = true;
        }       
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //武器の上にいます
        if (collision.transform.CompareTag("Axe")|| collision.transform.CompareTag("Spear")|| collision.transform.CompareTag("Sword"))
        {
            onHoverObject = collision.gameObject;
        }
        //攻撃受けた
        else if (collision.transform.CompareTag("Attack")) {}
        //攻撃した
        else if (collision.transform.CompareTag("Player1") || collision.transform.CompareTag("Player2")) {
            weapon.GetComponent<CapsuleCollider2D>().enabled = false;
            frontArm.GetComponent<CapsuleCollider2D>().enabled = false;
            backArm.GetComponent<CapsuleCollider2D>().enabled = false;
            collision.GetComponent<PlayerController>().TakeDamage(attack * atkMuiltpler, equiment);                              
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        onHoverObject = null;
    }
    private void GetWeapon(GameObject weaponObject) {
        switch (weaponObject.tag)
        {
            case "Axe":
                DropWeapon();
                equiment = Equiment.AXE;
                atkMuiltpler = 1.5f;
                weapon.SetActive(true);
                weapon.GetComponent<SpriteRenderer>().sprite = axe;
                animator.SetBool("using spear", false);
                weapon.transform.localScale = Vector3.one;
                Destroy(onHoverObject);
                break;
            case "Sword":
                DropWeapon();
                equiment = Equiment.SWORD;
                atkMuiltpler = 1.0f;
                weapon.SetActive(true);
                weapon.GetComponent<SpriteRenderer>().sprite = sword;
                animator.SetBool("using spear", false);
                weapon.transform.localScale = Vector3.one;
                Destroy(onHoverObject);
                break;
            case "Spear":
                DropWeapon();
                equiment = Equiment.SPEAR;
                atkMuiltpler = 0.8f;
                weapon.SetActive(true);
                weapon.GetComponent<SpriteRenderer>().sprite = spear;
                animator.SetBool("using spear", true);
                weapon.transform.localScale = new Vector3(1,1.3f,1);
                Destroy(onHoverObject);
                break;
            default: break;
        }
    }
    private void DropWeapon()
    {
        if (equiment == Equiment.PUNCH) return;


        if (equiment == Equiment.AXE)
        {
            GameObject droppedWeapon = Instantiate(axePre, transform.position, Quaternion.identity);
            droppedWeapon.GetComponent<Rigidbody2D>().velocity = new Vector3(facing * -2.0f, 5.0f, 0);
        }
        else if (equiment == Equiment.SWORD)
        {
            GameObject droppedWeapon = Instantiate(swordPre, transform.position, Quaternion.identity);
            droppedWeapon.GetComponent<Rigidbody2D>().velocity = new Vector3(facing * -2.0f, 5.0f, 0);
        }
        else if (equiment == Equiment.SPEAR)
        {
            GameObject droppedWeapon = Instantiate(spearPre, transform.position, Quaternion.identity);
            droppedWeapon.GetComponent<Rigidbody2D>().velocity = new Vector3(facing * -2.0f, 5.0f, 0);
        }
        weapon.SetActive(false);
        animator.SetBool("using spear", false);
        atkMuiltpler = 0.5f;
    }
    void EnableWeapon()
    {
        weapon.GetComponent<CapsuleCollider2D>().enabled = true;
        if (equiment == Equiment.AXE)
        {
            weapon.GetComponent<CapsuleCollider2D>().size = new Vector2(0.38f, 0.32f);
            weapon.GetComponent<CapsuleCollider2D>().offset = new Vector2(-0.1f, 0.35f);
        }
        else if (equiment == Equiment.SWORD)
        {
            weapon.GetComponent<CapsuleCollider2D>().size = new Vector2(0.25f, 0.76f);
            weapon.GetComponent<CapsuleCollider2D>().offset = new Vector2(-0.22f, 0.485f);
        }
        else if (equiment == Equiment.SPEAR)
        {
            weapon.GetComponent<CapsuleCollider2D>().size = new Vector2(0.232f, 0.469f);
            weapon.GetComponent<CapsuleCollider2D>().offset = new Vector2(0.003f, 0.8835f);
        }

        PlayAttackSound(equiment);
    }
    public void TakeDamage(float atk, Equiment equipment) 
    {
        HP -= atk;
        
        UIManager.Instance.UpdatePlayerHealth((UIManager.Player)(player - 1), HP, maxHP);
        EffectManager.Instance.PlayEffect(effectTransform.position, EffectManager.EffectType.Hit);

        PlayHitSound(equipment);

        if (HP > 0) animator.SetTrigger("hurt");
        else
        {
            HP = 0;
            animator.SetTrigger("dead");
            GetComponent<BoxCollider2D>().enabled = false;
            rb.isKinematic = true;
        }
    }

    private void PlayHitSound(Equiment equipment)
    { 
        switch (equipment) 
        {
            case Equiment.PUNCH:
                SoundManager.Instance.Play("Sounds/SFX/hit_punch", SoundManager.Sound.P_Effect);
                break;
            case Equiment.SPEAR:
                SoundManager.Instance.Play("Sounds/SFX/hit_spear", SoundManager.Sound.P_Effect);
                break;
            case Equiment.SWORD:
                SoundManager.Instance.Play("Sounds/SFX/hit_sword", SoundManager.Sound.P_Effect);
                break;
            case Equiment.AXE:
                SoundManager.Instance.Play("Sounds/SFX/hit_axe", SoundManager.Sound.P_Effect);
                break;
        }
    }

    private void PlayAttackSound(Equiment equipment)
    {
        switch (equipment)
        {
            case Equiment.PUNCH:
                //SoundManager.Instance.Play("Sounds/SFX/attack_punch", SoundManager.Sound.P_Effect);
                break;
            case Equiment.SPEAR:
                SoundManager.Instance.Play("Sounds/SFX/attack_spear", SoundManager.Sound.P_Effect);
                break;
            case Equiment.SWORD:
                SoundManager.Instance.Play("Sounds/SFX/attack_sword", SoundManager.Sound.P_Effect);
                break;
            case Equiment.AXE:
                SoundManager.Instance.Play("Sounds/SFX/attack_axe", SoundManager.Sound.P_Effect);
                break;
        }
    }
}
