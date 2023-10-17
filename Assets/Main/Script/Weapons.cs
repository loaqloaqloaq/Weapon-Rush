using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapon
{
    public float atkMuiltpler, moveSpeed, chargeAttackTime, atkCD;
}

[System.Serializable]
public class WeaponKind {
    public Weapon Axe, Sword, Spear, Punch;
}

public class Weapons {
    public WeaponKind weapons;
}
