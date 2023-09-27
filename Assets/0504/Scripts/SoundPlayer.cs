using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip m_Audioclip;
    [SerializeField] private SoundManager.Sound type;
    [SerializeField] [Range(0, 3.0f)] private float fitch = 1.0f;

    public void PlaySound()
    {
        SoundManager.Instance.Play(m_Audioclip, type, fitch);
    }
}
