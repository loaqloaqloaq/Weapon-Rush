using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static int maxWeapons;
    public static int nowWeapons;
    
    // Start is called before the first frame update
    void Start()
    {
        maxWeapons = 9;
        nowWeapons = 0;
    }

}
