using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1Controller : MonoBehaviour
{
    GameObject weapon;
    public Sprite axe, sword;

    enum Equiment { 
        AXE,SWORD,PUNCH
    };
    Equiment equiment;
    // Start is called before the first frame update
    void Start()
    {
        equiment=Equiment.AXE;
        weapon = GameObject.Find("Weapon");
    }

    // Update is called once per frame
    void Update()
    {
        if (equiment == Equiment.AXE)
        {
            weapon.GetComponent<SpriteRenderer>().sprite = axe;
            if (Input.GetKeyDown(KeyCode.I)) equiment = Equiment.SWORD;
        }
        else if (equiment == Equiment.SWORD) {
            weapon.GetComponent<SpriteRenderer>().sprite = sword;
            if (Input.GetKeyDown(KeyCode.I)) equiment = Equiment.AXE;
        }

        if (Input.GetKeyDown(KeyCode.Z)) {
            weapon.SetActive(true);
            GetComponent<Animator>().SetTrigger("sword");
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            weapon.SetActive(false);
            GetComponent<Animator>().SetTrigger("punch");
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            weapon.SetActive(false);
            GetComponent<Animator>().SetTrigger("punch");
        }
    }
}
