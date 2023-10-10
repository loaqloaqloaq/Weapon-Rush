using UnityEngine;

public class PlayerController : MonoBehaviour
{
    GameObject weapon, frontArm, backArm;
    public GameObject axePre, swordPre, spearPre;
    public GameObject effect_sword;
    public GameObject otherPlayer;
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
        public string move,down, atk, jump, drop, dash;        
    }   
    KeyBind[] input = new KeyBind[2];   
    
    public enum Equiment {
        AXE, SWORD, SPEAR, PUNCH, NON, FLYINGSWORD
    };
    public Equiment equiment;
    GameObject onHoverObject;

    // 1＝プレイヤー１ ／ 2＝プレイヤー２ ／ 3＝CPU
    public int player = 1;

    [HideInInspector]
    public float moveSpeed, jumpPow, dashMaxSpeed;

    [SerializeField]
    bool onGround, onStage;    

    float attack;
    float atkMuiltpler;

    float facing;

    [HideInInspector]
    public float lastAtk;
    float axeCD, swordCD, spearCD,puncCD;
    float chargeAttackTime,holdTime;

    float spearSpecialAttack;
    bool chargeAttacked;

    //チャージエフェクト
    private ChargeEffect chargeEffect;
    private bool isCharging = false;

    //ダッシュ
    private TrailRenderer dashTrail;

    private bool pressDown;
    private bool weaponEnabled;

    private void Awake()
    {
        chargeEffect = GetComponentInChildren<ChargeEffect>();
        dashTrail = GetComponentInChildren<TrailRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (transform.CompareTag("Player1"))
        {
            player = 1;
            otherPlayer = GameObject.Find("Player2");
        }
        else
        {
            if (PlayerPrefs.GetString("mode") == "PVE") player = 3;  
            else if (PlayerPrefs.GetString("mode") == "PVP") player = 2;

            otherPlayer = GameObject.Find("Player1");
        }


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
        onStage = false;
        onHoverObject = null;      

        //キーバインド
        //プレイヤー　１
        input[0].move = "Player1_Horizontal";
        input[0].down = "Player1_Vertical";
        input[0].atk = "Player1_Attack";
        input[0].drop = "Player1_Drop";
        input[0].jump = "Player1_Jump";
        input[0].dash = "Player1_Dash";

        //プレイヤー　2
        input[1].move = "Player2_Horizontal";
        input[1].down = "Player2_Vertical";
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
        puncCD = 0.5f;
        
        chargeAttackTime = 0.5f;
        holdTime = 0;

        weaponEnabled = false;


    }

    // Update is called once per frame
    void Update()
    {
        if (
            (otherPlayer.GetComponent<PlayerController>().HP <= 0 || (GameDirector.end && HP > otherPlayer.GetComponent<PlayerController>().HP) ) &&
            animator.GetCurrentAnimatorStateInfo(0).IsName("idle")
        ) {  
            animator.SetTrigger("win");           
        }

        //一時停止時や死んだの時、プレイヤーを動けないようにする 
        if (Mathf.Approximately(Time.timeScale, 0.0f) || HP <= 0 || GameDirector.end)
        {            
            return;
        }
        
        if (lastAtk >= 0) lastAtk += Time.deltaTime;       
        
        if (player < 3) { 
            //攻撃アニメーションが終わった判定
            bool attacking = animator.GetCurrentAnimatorStateInfo(0).IsName("punch attack") || animator.GetCurrentAnimatorStateInfo(0).IsName("sword attack") || animator.GetCurrentAnimatorStateInfo(0).IsName("axe attack") || animator.GetCurrentAnimatorStateInfo(0).IsName("spear attack");
            if (!attacking)
            {
                if (equiment == Equiment.SWORD)
                {
                    atkMuiltpler = 1.1f;
                }
                else if (equiment == Equiment.AXE)
                {
                    atkMuiltpler = 1.8f;
                }
                else if (equiment == Equiment.SPEAR)
                {
                    atkMuiltpler = 0.8f;
                }
                DisableWeapon();
                holdTime = 0;
                animator.speed = 1;
                chargeAttacked = false;                
                if (equiment!=Equiment.SPEAR) weapon.transform.localScale = Vector3.one;
                if (lastAtk < 0) lastAtk = 0;
            }
            //チャージ攻撃
            else
            {
                var aniTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                if (
                    ((equiment == Equiment.SWORD && aniTime > (16.0f / 38.0f)) ||
                    (equiment == Equiment.AXE && aniTime > (16.0f / 38.0f)) ||
                    (equiment == Equiment.SPEAR && aniTime > (6.0f / 38.0f))) &&
                    !weaponEnabled
                ) EnableWeapon();

                if (Input.GetButton(input[player - 1].atk) && !chargeAttacked)
                {                    
                    if (equiment == Equiment.SWORD)
                    {                        
                        if (aniTime > (15.0f / 38.0f))
                        {
                            holdTime += Time.deltaTime;
                            animator.speed = 0;
                            if (!isCharging)
                            {
                                isCharging = true;
                                chargeEffect.Play();
                            }
                        }
                    }
                    else if (equiment == Equiment.AXE)
                    {                        
                        if (aniTime > (15.0f / 38.0f))
                        {
                            holdTime += Time.deltaTime;
                            animator.speed = 0;
                            weapon.transform.localScale += new Vector3(4, 4, 4) * Time.deltaTime;                            
                            if (!isCharging)
                            {
                                isCharging = true;
                                chargeEffect.Play();
                            }
                        }
                    }
                    else if (equiment == Equiment.SPEAR) {                        
                        if (aniTime > (5.0f / 38.0f))
                        {
                            holdTime += Time.deltaTime;
                            animator.speed = 0;
                            if (!isCharging)
                            {
                                isCharging = true;
                                chargeEffect.Play();
                            }
                        }
                    }
                }               
                if ((Input.GetButtonUp(input[player - 1].atk) || holdTime >= chargeAttackTime) && !chargeAttacked) {
                    animator.speed = 1;
                    chargeAttacked = true;
                    
                    if (holdTime >= chargeAttackTime) {
                        if (equiment == Equiment.SWORD)
                        {
                            var pos = transform.position;
                            pos.y += 0.81f;
                            pos.x += 0.81f * transform.localScale.x * -1;
                            var effect = Instantiate(effect_sword, pos, Quaternion.identity);
                            effect.transform.localScale = new Vector3(0.3f * transform.localScale.x * -1, 0.3f, 1);
                            effect.GetComponent<swordEffectController>().attacker = transform.tag;
                        }
                        else if (equiment == Equiment.AXE) {
                            atkMuiltpler *= (2 * (holdTime / chargeAttackTime));
                        }
                        else if (equiment == Equiment.SPEAR)
                        {
                            spearSpecialAttack = 0.2f;
                        }                        
                    }
                    isCharging = false;
                    chargeEffect.Stop();
                    PlayAttackSound(equiment);
                }
            }
            //槍チャージ攻撃
            if (spearSpecialAttack > 0f) {
                var target = (otherPlayer.transform.position - transform.position).normalized;
                target.z = 0f;
                transform.Translate(target*Time.deltaTime*20);
                transform.localScale= new Vector3(target.x>0?-1:1,1,1);
                spearSpecialAttack -= Time.deltaTime;
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
                dashTrail.emitting = true;
                SoundManager.Instance.Play("Sounds/SFX/dash", SoundManager.Sound.P_Effect);
                EffectManager.Instance.PlayEffect(effectTransform.position, EffectManager.EffectType.Dash);
            }
            if (dashing > 0)
            {
                dashSpeed = dashMaxSpeed;
                dashing -= Time.deltaTime;
                if (dashing < 0)
                {
                    dashing = 0;
                    dashTrail.emitting = false;
                } 
            }
            if (dashCoolDown > 0)
            {
                dashCoolDown -= Time.deltaTime;
                int playerNum = 0;
                if (transform.CompareTag("Player2")) playerNum = 1;
                UIManager.Instance.UpdateDash((UIManager.Player)playerNum, dashCoolDown);
            } 

            //not charging or using spear
            if ( holdTime <= (equiment == Equiment.SPEAR ? 0.15f:0f) )
            {
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
                //ジャンプ       
                if (Input.GetButtonDown(input[player - 1].jump) && onGround)
                {
                    Jump();
                }
                //下へ   
                if (Input.GetAxis(input[player - 1].down) > 0.2f && onStage && !pressDown)
                {
                    pressDown = true;
                    Down();
                }
                if (Input.GetAxis(input[player - 1].down) <= 0.2f && !onStage) {
                    pressDown = false;
                }

            }
            if (spearSpecialAttack <= 0)
            {
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
            }
            
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Floor") || collision.transform.CompareTag("Block"))
        {
            GetComponent<BoxCollider2D>().isTrigger = false;
            onStage = false;
            onGround = true;
        }
        else if (collision.transform.CompareTag("Stage") || collision.transform.CompareTag("Cloud"))
        {
            onStage = true;
            onGround = true;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Floor") || collision.transform.CompareTag("Block"))
        {
            GetComponent<BoxCollider2D>().isTrigger = false;
            onStage = false;
            onGround = true;
        }
        else if (collision.transform.CompareTag("Stage") || collision.transform.CompareTag("Cloud"))
        {
            onStage = true;
            onGround = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Floor") || collision.transform.CompareTag("Block") || collision.transform.CompareTag("Stage") || collision.transform.CompareTag("Cloud"))
        {
            onGround = false;
            onStage = false;
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
            if (spearSpecialAttack > 0) {
                collision.GetComponent<PlayerController>().DropWeapon(false,true);                
            }
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
        if (collision.transform.CompareTag("Stage")) {
            GetComponent<BoxCollider2D>().isTrigger = false;
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
                moveSpeed = 4.0f;
                chargeAttackTime = 0.5f;
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
                moveSpeed = 5.0f;
                chargeAttackTime = 0.8f;
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
                moveSpeed = 5.0f;
                chargeAttackTime = 0.5f;
                Destroy(weaponObject);
                break;
            default: break;
        }

        if (player == 1) { GameData.p1.weaponCount++; }
        else { GameData.p2.weaponCount++; }
        EffectManager.Instance.PlayEffect(effectTransform.position, EffectManager.EffectType.GetWeapon);
        SoundManager.Instance.Play("Sounds/SFX/getWeapon", SoundManager.Sound.P_Effect);

        int playerNum = 0;
        if (transform.CompareTag("Player2")) playerNum = 1;
        UIManager.Instance.UpdatePlayerWeaponImage((UIManager.Player)playerNum, equiment);
    }
    //武器を捨てる
    public void DropWeapon(bool throwSword = true, bool attacked = false)
    {
        lastAtk = 1f;
        if (equiment == Equiment.PUNCH) return;

        GameObject droppedWeapon = null;
        float forceX = -2.0f, forceY = 5.0f;
        if (equiment == Equiment.AXE)
        {
            if(!attacked || !isCharging) droppedWeapon = Instantiate(axePre, transform.position, Quaternion.identity);            
        }
        else if (equiment == Equiment.SWORD)
        {
            Vector3 pos = transform.position;
            pos.y += 0.5f;
            droppedWeapon = Instantiate(swordPre, pos, Quaternion.identity);
            
            if (throwSword)
            {              
                droppedWeapon.transform.rotation = Quaternion.Euler(0.0f, 0.0f, facing * 90);
                forceX = -10.0f;
                forceY = 3.0f;
                droppedWeapon.GetComponent<swordController>().throwSword(transform.tag);
                EffectManager.Instance.PlayEffect(effectTransform.position, EffectManager.EffectType.ThrowWeapon);
                SoundManager.Instance.Play("Sounds/SFX/throwSword", SoundManager.Sound.P_Effect);
            }
        }
        else if (equiment == Equiment.SPEAR)
        {            
            droppedWeapon = Instantiate(spearPre, transform.position, Quaternion.identity);           
        }
        else droppedWeapon = null;
        if (droppedWeapon != null)
        {
            droppedWeapon.GetComponent<Rigidbody2D>().velocity = new Vector3(facing * forceX, forceY, 0);
            weapon.SetActive(false);
            animator.SetBool("using spear", false);
            atkMuiltpler = 0.5f;
            equiment = Equiment.PUNCH;
        }

        int playerNum = 0;
        if (transform.CompareTag("Player2")) playerNum = 1;
        UIManager.Instance.UpdatePlayerWeaponImage((UIManager.Player)playerNum, equiment, true);
    }
    //ジャンプ
    public void Jump() {
        if (onGround)
        {
            rb.velocity = new Vector2(0, jumpPow);
            onGround = false;
            onStage = false;
        }
    }
    public void Down()
    {
        if (onStage)
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
            Invoke("jumper", 0.34f);
            onStage = false;
            onGround = false;
        }
    }
    private void jumper() {
        GetComponent<BoxCollider2D>().isTrigger = false;
    }
    //攻撃
    public void Attack()
    {
        if (equiment == Equiment.AXE)
        {
            if (lastAtk < axeCD) return;
            animator.SetTrigger("axe");
            //Invoke("EnableWeapon", 0.24f);
            lastAtk = -1;            
        }
        else if (equiment == Equiment.SWORD)
        {
            if (lastAtk < swordCD) return;
            animator.SetTrigger("sword");
            //Invoke("EnableWeapon", 0.24f);
            lastAtk = -1;
        }
        else if (equiment == Equiment.SPEAR)
        {
            if (lastAtk < spearCD) return;
            animator.SetTrigger("spear");
            //Invoke("EnableWeapon", 0.2f);
            lastAtk = -1;
        }
        else if (equiment == Equiment.PUNCH)
        {
            if (lastAtk < puncCD) return;
            frontArm.GetComponent<CapsuleCollider2D>().enabled = true;
            backArm.GetComponent<CapsuleCollider2D>().enabled = true;
            animator.SetTrigger("punch");
            lastAtk = -1;
        }
    }
    public void CpuAttack()
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
            Invoke("EnableWeapon", 0.2f);
            lastAtk = -1;
        }
        else if (equiment == Equiment.PUNCH)
        {
            if (lastAtk < puncCD) return;
            frontArm.GetComponent<CapsuleCollider2D>().enabled = true;
            backArm.GetComponent<CapsuleCollider2D>().enabled = true;
            animator.SetTrigger("punch");
            lastAtk = -1;
        }
    }
    //武器の攻撃判定
    public void DisableWeapon() {
        weapon.GetComponent<CapsuleCollider2D>().enabled = false;
        frontArm.GetComponent<CapsuleCollider2D>().enabled = false;
        backArm.GetComponent<CapsuleCollider2D>().enabled = false;
        weaponEnabled = false;
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
        weaponEnabled = true;
    }
    //ダメージを受ける
    public void TakeDamage(float atk, Equiment equipment)
    {
        if (equipment == Equiment.FLYINGSWORD) {
            if (spearSpecialAttack > 0) return;
            else equipment = Equiment.SWORD;
        }
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
        animator.speed = 1;
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
                GameDirector.CameraShake();
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
