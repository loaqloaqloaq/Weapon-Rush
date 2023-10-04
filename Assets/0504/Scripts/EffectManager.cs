using UnityEngine;

public class EffectManager : MonoBehaviour
{
    //Singleton
    private static EffectManager m_Instance;
    public static EffectManager Instance
    {
        get
        {
            if (m_Instance == null) m_Instance = FindObjectOfType<EffectManager>();
            return m_Instance;
        }
    }

    //
    public enum EffectType
    {
        PunchHit,
        WeaponHit,
        GetWeapon,
        ThrowWeapon
    }

    [SerializeField] private ParticleSystem commonHit;
    [SerializeField] private ParticleSystem WeaponHit;
    [SerializeField] private ParticleSystem GetWeapon;
    [SerializeField] private ParticleSystem ThrowWeapon;
    //
    public void PlayEffect(Vector3 pos,
        EffectType effectType = EffectType.WeaponHit, Transform parent = null)
    {
        var targetEffect = GetEffect(effectType); 

        var effect = Instantiate(targetEffect, pos, Quaternion.identity);

        if (parent != null)
        {
            effect.transform.SetParent(parent);
        }

        effect.Play();
    }

    private ParticleSystem GetEffect(EffectType effectType)
    { 
        switch (effectType)   
        {
            case EffectType.WeaponHit:
                return WeaponHit;
            case EffectType.GetWeapon:
                return GetWeapon;
            case EffectType.ThrowWeapon:
                return ThrowWeapon;
        }
        return commonHit;
    }
}