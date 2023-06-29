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

    [SerializeField] private ParticleSystem commonHitEffectPrefab;
    [SerializeField] private ParticleSystem E1, E2;

    //
    public void PlayHitEffect(Vector3 pos,
        EffectType effectType = EffectType.Hit, Transform parent = null)
    {
        var targetPrefab = commonHitEffectPrefab;

        if (effectType == EffectType.E1)
            targetPrefab = E2;

        if (effectType == EffectType.E2)
            targetPrefab = E2;

        var effect = Instantiate(targetPrefab, pos, Quaternion.identity);

        if (parent != null)
        {
            effect.transform.SetParent(parent);
        }

        effect.Play();
    }
}