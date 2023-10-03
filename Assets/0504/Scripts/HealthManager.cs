using UnityEngine.UI;
using UnityEngine;
[System.Serializable]
public class HealthManager
{
    [HideInInspector] public float healthAmount;
    [HideInInspector] public float targetAmount;

    public Animator animator;

    public Image healthBarGauge;
    public Image dashCoolDownImage;
    public Image[] weaponImages;
}
