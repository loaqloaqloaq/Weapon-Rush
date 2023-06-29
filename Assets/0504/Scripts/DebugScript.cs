using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UIManager.Instance.healthManagers[(int)UIManager.Player.P1].targetAmount -= 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UIManager.Instance.healthManagers[(int)UIManager.Player.P2].targetAmount -= 0.2f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Vector3 pos = GameObject.FindWithTag("Player1").transform.position;
            EffectManager.Instance.PlayHitEffect(pos, EffectManager.EffectType.Hit);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Vector3 pos = GameObject.FindWithTag("Player1").transform.position;
            EffectManager.Instance.PlayHitEffect(pos, EffectManager.EffectType.E1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SoundManager.Instance.Play("Sounds/BGM/bgm_Battle", SoundManager.Sound.BGM);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Vector3 pos = GameObject.FindWithTag("Player1").transform.position;
            SoundManager.Instance.Play("Sounds/SFX/hit", SoundManager.Sound.P1_Effect);
        }
    }
}
