using UnityEngine;
using TMPro;
using static UIManager;

public class UIManager : MonoBehaviour
{
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

    [SerializeField] private GameObject timeUpText;
    //[SerializeField] private Image image;

    private float interporate = 0.05f;

    private void Awake()
    {
        timeUpText.SetActive(false); 
    }

    void Start()
    { 
        for (int i = 0; i < healthManagers.Length; i++)
        {
            healthManagers[i].healthAmount = 1.0f;
            healthManagers[i].targetAmount = 1.0f;
        }
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
    }

    private void UpdateHealthAmount(HealthManager hm)
    {
        hm.healthAmount = Mathf.Lerp(hm.healthAmount, hm.targetAmount, interporate);
        if (Mathf.Abs(hm.healthAmount - hm.targetAmount) < 0.01f)
        {
            hm.healthAmount = hm.targetAmount;
        }
        hm.healthBarGauge.fillAmount = hm.healthAmount;
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

    public void SetActiveTimeUpText()
    {
        timeUpText.SetActive(true);
    }
}
