using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ChargeEffect : MonoBehaviour
{
    private ParticleSystem particle;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    public void Play()
    {
        particle.Play();
    }

    public void Stop()
    {
        particle.Clear();
    }

}
