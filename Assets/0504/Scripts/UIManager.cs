using UnityEngine;
using TMPro;
using static UIManager;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    
    GameObject player1, player2;    
    Camera camera;    
    CanvasGroup cg;

    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<UIManager>();

            return instance;
        }
    }

    public enum Player
    { 
        P1,
        P2,
        MaxCount
    }

    public HealthManager[] healthManagers = new HealthManager[(int)Player.MaxCount];

    [SerializeField] private TextMeshProUGUI timeText;


    private float interporate = 0.15f;

    private void Awake()
    {
    }

    void Start()
    { 
        for (int i = 0; i < healthManagers.Length; i++)
        {
            healthManagers[i].healthAmount = 1.0f;
            healthManagers[i].targetAmount = 1.0f;
        }
        if (player1 == null) player1 = GameObject.Find("Player1");
        if (player2 == null) player2 = GameObject.Find("Player2");
        if (camera == null) camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        if (cg == null) cg = GameObject.Find("Canvas/UIPanel/Panel").GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var healthManager in healthManagers)
        {
            if (healthManager.healthAmount != healthManager.targetAmount)
            {
                UpdateHealthAmount(healthManager);
            }            
        }
        if (Camera.main.WorldToViewportPoint(player1.transform.position).y > 0.73f || Camera.main.WorldToViewportPoint(player2.transform.position).y > 0.73f)
        {
            cg.alpha = 0.4f;
        }
        else
        {
          cg.alpha=1f;
        }


    }

    public void UpdateDash(Player player, float amount)
    {
        if (healthManagers[(int)player].dashCoolDownImage == null)
        {
            Debug.Log("dashCoolDownImage reference is Null!");
            return;
        }
        healthManagers[(int)player].dashCoolDownImage.fillAmount = amount;
    }

    public void UpdatePlayerWeaponImage(Player player, PlayerController.Equiment equipment, bool isDrop = false)
    {
        int playerNum = (int)(player);
        int equipNum = (int)equipment;

        foreach (var image in healthManagers[playerNum].weaponImages)
        {
            image.enabled = false;
        }
        if (isDrop) return;

        if (healthManagers.Length <= (int)player || (int)equipment > healthManagers[playerNum].weaponImages.Length)
        {
            Debug.Log("Array Index out of Range");
            return;
        }

        if (healthManagers[playerNum].animator != null)
        {
            healthManagers[playerNum].animator.SetTrigger("changeWeapon");
        }
        else
        {
            Debug.Log("HM animator is NULL");
        }


        healthManagers[playerNum].weaponImages[equipNum].enabled = true;

    }


    private void UpdateHealthAmount(HealthManager hm)
    {
        hm.healthAmount = Mathf.Lerp(hm.healthAmount, hm.targetAmount, interporate);
        if (Mathf.Abs(hm.healthAmount - hm.targetAmount) < 0.01f)
        {
            hm.healthAmount = hm.targetAmount;
        }
        hm.healthBarGauge.fillAmount = hm.healthAmount;

        if (hm.animator != null)
        {
            hm.animator.SetTrigger("damaged");
        }
    }

    public void UpdatePlayerHealth(Player player, float currentHp, float maxHp)
    {
        if (healthManagers.Length <= (int)player) 
        {
            Debug.Log("healthBar array Index out of Range");
            return; 
        }
        
        float amount = currentHp / maxHp;
        if (amount < 0)
        {
            amount = 0f;
        }
        healthManagers[(int)player].targetAmount = amount;
    }

    public void UpdateTimerText(int time)
    {
        if (!timeText)
        {
            Debug.Log("TimeText reference is null");
        }
        timeText.text = time.ToString();
    }

}
