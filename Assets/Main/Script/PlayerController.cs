using UnityEngine;

public class PlayerController : MonoBehaviour
{
    GameObject weapon, frontArm, backArm;
    public GameObject axePre, swordPre, spearPre;
    public Sprite axe, sword, spear;
    Rigidbody2D rb;
    Animator animator; 

    //エフェクトを表示する位置
    private Transform effectTransform; 

    public float maxHP;

    [HideInInspector]
    public float HP;

    float dashing;
    float dashCoolDown;
    struct KeyBind {
        public string move, atk, jump, drop, dash;        
    }   
    KeyBind[] input = new KeyBind[2];   
    
    public enum Equiment {
        AXE, SWORD, SPEAR, PUNCH, NON
    };
    public Equiment equiment;
    GameObject onHoverObject;

    // 1＝プレイヤー１ ／ 2＝プレイヤー２ ／ 3＝CPU
    public int player = 1;

    [HideInInspector]
    public float moveSpeed, jumpPow, dashMaxSpeed;
   
    bool onGround;

    float attack;
    float atkMuiltpler;

    float facing;

    [HideInInspector]
    public float lastAtk;
    float axeCD, swordCD, spearCD;
    // Start is called before the first frame update
    void Start()
    {
        if (transform.CompareTag("Player1")) player = 1;
        else if (PlayerPrefs.GetString("mode") == "PVE") player = 3;
        else if (PlayerPrefs.GetString("mode") == "PVP") player = 2;

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

        //キーバインド
        //プレイヤー　１
        input[0].move = "Player1_Horizontal";
        input[0].atk = "Player1_Attack";
        input[0].drop = "Player1_Drop";
        input[0].jump = "Player1_Jump";
        input[0].dash = "Player1_Dash";

        //プレイヤー　2
        input[1].move = "Player2_Horizontal";  
        input[1].atk = "Player2_Attack";
        input[1].drop = "Player2_Drop";
        input[1].jump = "Player2_Jump";
        input[1].dash = "Player2_Dash";

        //プレイヤー数値
        HP = maxHP;
        attack = 10.0f;
        atkMuiltpler = 0.5f;
        moveSpeed = 5.0f;
        jumpPow = 10.0f;
        dashMaxSpeed = 3.5f;
        //初期武器
        equiment = Equiment.PUNCH;
        weapon.SetActive(false);
        animator.SetBool("using spear", false);

        if (player < 3)
        {
            GetComponent<AIController>().enabled = false;
        }
        else {
            GetComponent<AIController>().enabled = true;
            GetComponent<AIController>().setJumpPow(jumpPow);
            GetComponent<AIController>().setMoveSpeed(moveSpeed);
        }

        //武器数値
        lastAtk = 1f;
        axeCD = 0.7f;
        swordCD = 0.5f;
        spearCD = 0.3f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("l")) HP = 0;
        if(lastAtk>=0) lastAtk += Time.deltaTime;
        //一時停止時や死んだの時、プレイヤーを動けないようにする 
        if (Mathf.Approximately(Time.timeScale, 0.0f) || HP <= 0 || GameDirector.end)
        {
            return;
        }
        if (player < 3) { 
            //攻撃アニメーションが終わった判定
            bool attacking = animator.GetCurrentAnimatorStateInfo(0).IsName("punch attack") || animator.GetCurrentAnimatorStateInfo(0).IsName("sword attack") || animator.GetCurrentAnimatorStateInfo(0).IsName("axe attack") || animator.GetCurrentAnimatorStateInfo(0).IsName("spear attack");
            if (!attacking)
            {
                DisableWeapon();  
                if(lastAtk < 0) lastAtk = 0;
            }
            //攻撃  
            if (Input.GetButtonDown(input[player - 1].atk) && !attacking)
            {
                Attack();
            }
            //武器捨てる and 武器を拾う  
            if (Input.GetButtonDown(input[player - 1].drop) && !attacking)
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
            //ダッシュ処理
            float dashTime = 0.15f;
            float dashSpeed = 1.0f;
            //ダッシュ Cool Down
            float cooldown = 1.0f;
            if (Input.GetButtonDown(input[player - 1].dash) && dashing <= 0 && dashCoolDown <= 0)
            {
                dashing = dashTime;
                dashCoolDown = cooldown;
            }
            if (dashing > 0)
            {
                dashSpeed = dashMaxSpeed;
                dashing -= Time.deltaTime;
                if (dashing < 0) dashing = 0;
            }
            if (dashCoolDown > 0) dashCoolDown -= Time.deltaTime;
            //移動処理
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

            //画像の向き
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
            if (Input.GetButtonDown(input[player - 1].jump) && onGround)
            {
                Jump();
            }
        }
    }   
    private void OnCollisionEnter2D(Collision2D collision)
    {

       if (collision.transform.CompareTag("Floor") || collision.transform.CompareTag("Block")) {
            onGround = true;
        }       
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {        
        //攻撃受けた
        if (collision.transform.CompareTag("Attack")) {}
        //攻撃当たった
        else if (collision.transform.CompareTag("Player1") || collision.transform.CompareTag("Player2")) {
            weapon.GetComponent<CapsuleCollider2D>().enabled = false;
            frontArm.GetComponent<CapsuleCollider2D>().enabled = false;
            backArm.GetComponent<CapsuleCollider2D>().enabled = false;
            collision.GetComponent<PlayerController>().TakeDamage(attack * atkMuiltpler, equiment);                              
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        //武器の近くにいる
        if (collision.transform.CompareTag("Axe") || collision.transform.CompareTag("Spear") || collision.transform.CompareTag("Sword"))
        {
            onHoverObject = collision.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //武器から離れた
        if (collision.transform.CompareTag("Axe") || collision.transform.CompareTag("Spear") || collision.transform.CompareTag("Sword"))
        {
            onHoverObject = null;
        }        
    }
    //武器を拾う
    public void GetWeapon(GameObject weaponObject) {
        lastAtk = 1f;
        switch (weaponObject.tag)
        {
            case "Axe":
                DropWeapon();
                equiment = Equiment.AXE;
                atkMuiltpler = 1.8f;
                weapon.SetActive(true);
                weapon.GetComponent<SpriteRenderer>().sprite = axe;
                animator.SetBool("using spear", false);
                weapon.transform.localScale = Vector3.one;
                Destroy(weaponObject);
                break;
            case "Sword":
                DropWeapon();
                equiment = Equiment.SWORD;
                atkMuiltpler = 1.1f;
                weapon.SetActive(true);
                weapon.GetComponent<SpriteRenderer>().sprite = sword;
                animator.SetBool("using spear", false);
                weapon.transform.localScale = Vector3.one;
                Destroy(weaponObject);
                break;
            case "Spear":
                DropWeapon();
                equiment = Equiment.SPEAR;
                atkMuiltpler = 0.8f;
                weapon.SetActive(true);
                weapon.GetComponent<SpriteRenderer>().sprite = spear;
                animator.SetBool("using spear", true);
                weapon.transform.localScale = new Vector3(1,1.3f,1);
                Destroy(weaponObject);
                break;
            default: break;
        }

        if (player == 1) { GameData.p1.weaponCount++; }
        else { GameData.p2.weaponCount++; }
        EffectManager.Instance.PlayEffect(effectTransform.position, EffectManager.EffectType.GetWeapon);
        SoundManager.Instance.Play("Sounds/SFX/getWeapon", SoundManager.Sound.P_Effect);
    }
    //武器を捨てる
    private void DropWeapon()
    {
        lastAtk = 1f;
        if (equiment == Equiment.PUNCH) return;

        GameObject droppedWeapon;
        float forceX = -2.0f, forceY = 5.0f;
        if (equiment == Equiment.AXE)
        {
            droppedWeapon = Instantiate(axePre, transform.position, Quaternion.identity);            
        }
        else if (equiment == Equiment.SWORD)
        {
            Vector3 pos = transform.position;
            pos.y += 0.5f;
            droppedWeapon = Instantiate(swordPre, pos, Quaternion.identity);
            droppedWeapon.transform.rotation = Quaternion.Euler(0.0f, 0.0f, facing*90);
            forceX = -10.0f;
            forceY = 3.0f;
            droppedWeapon.GetComponent<swordController>().throwSword(transform.tag);
        }
        else if (equiment == Equiment.SPEAR)
        {            
            droppedWeapon = Instantiate(spearPre, transform.position, Quaternion.identity);           
        }
        else droppedWeapon = null;
        if (droppedWeapon != null) droppedWeapon.GetComponent<Rigidbody2D>().velocity = new Vector3(facing * forceX, forceY, 0);
        weapon.SetActive(false);
        animator.SetBool("using spear", false);
        atkMuiltpler = 0.5f;
    }
    //ジャンプ
    public void Jump() {
        if (onGround)
        {
            rb.velocity = new Vector2(0, jumpPow);
            onGround = false;
        }
    }
    //攻撃
    public void Attack()
    {
        if (equiment == Equiment.AXE)
        {
            if (lastAtk < axeCD) return;
            animator.SetTrigger("axe");
            Invoke("EnableWeapon", 0.24f);
            lastAtk = -1;
        }
        else if (equiment == Equiment.SWORD)
        {
            if (lastAtk < swordCD) return;
            animator.SetTrigger("sword");
            Invoke("EnableWeapon", 0.24f);
            lastAtk = -1;
        }
        else if (equiment == Equiment.SPEAR)
        {
            if (lastAtk < spearCD) return;
            animator.SetTrigger("spear");
            Invoke("EnableWeapon", 0.05f);
            lastAtk = -1;
        }
        else if (equiment == Equiment.PUNCH)
        {
            frontArm.GetComponent<CapsuleCollider2D>().enabled = true;
            backArm.GetComponent<CapsuleCollider2D>().enabled = true;
            animator.SetTrigger("punch");
        }
    }
    //武器の攻撃判定
    public void DisableWeapon() {
        weapon.GetComponent<CapsuleCollider2D>().enabled = false;
        frontArm.GetComponent<CapsuleCollider2D>().enabled = false;
        backArm.GetComponent<CapsuleCollider2D>().enabled = false;
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
    //ダメージを受ける
    public void TakeDamage(float atk, Equiment equipment)
    {
        HP -= atk;

        int playerNum = 0;
        if (transform.CompareTag("Player2")) playerNum = 1;
        UIManager.Instance.UpdatePlayerHealth((UIManager.Player)playerNum, HP, maxHP);
        PlayEffect(equipment);
        PlayHitSound(equipment);
        if (HP > 0) {
            if(this.equiment != Equiment.AXE) animator.SetTrigger("hurt");
        } 
        else
        {
            HP = 0;
            animator.SetTrigger("dead");
            GetComponent<BoxCollider2D>().enabled = false;
            rb.isKinematic = true;
        }
    }    
    private void PlayEffect(Equiment equipment)
    {
        switch (equipment)
        {
            case Equiment.PUNCH:
                EffectManager.Instance.PlayEffect(effectTransform.position, EffectManager.EffectType.PunchHit);
                break;
            case Equiment.SPEAR:
                EffectManager.Instance.PlayEffect(effectTransform.position, EffectManager.EffectType.WeaponHit);
                break;
            case Equiment.SWORD:
                EffectManager.Instance.PlayEffect(effectTransform.position, EffectManager.EffectType.WeaponHit);
                break;
            case Equiment.AXE:
                EffectManager.Instance.PlayEffect(effectTransform.position, EffectManager.EffectType.WeaponHit);
                break;
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
                SoundManager.Instance.Play("Sounds/SFX/attack_punch", SoundManager.Sound.P_Effect);
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

    public Equiment CurrentWeapon() {
        return equiment;
    }
}
