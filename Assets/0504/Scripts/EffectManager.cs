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
        E1,
        E2,
        Hit,
    }

    [SerializeField] private ParticleSystem commonHit;
    [SerializeField] private ParticleSystem E1, E2;

    //
    public void PlayEffect(Vector3 pos,
        EffectType effectType = EffectType.Hit, Transform parent = null)
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
            case EffectType.E1:
                return E1;
            case EffectType.E2:
                return E2;
        }
        return commonHit;
    }
}