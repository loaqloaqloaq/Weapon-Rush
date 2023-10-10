using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trapsController : MonoBehaviour
{
    [SerializeField] private AudioClip se;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameDirector.end)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag.StartsWith("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(1000, PlayerController.Equiment.NON);
            
            //‰¼
            SoundManager.Instance.Play(se, SoundManager.Sound.P_Effect, 1.0f);
        }
        if (collision.gameObject.CompareTag("Sword") || collision.gameObject.CompareTag("Axe") || collision.gameObject.CompareTag("Spear"))
        {
            Destroy(collision.gameObject);
            WeaponManager.nowWeapons--;
        }
    }
}
