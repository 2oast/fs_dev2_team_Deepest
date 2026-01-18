using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public int hp;
    public float stamina;
    public Vector3 playerPosition;

    public int meleeLevel;
    public int rangedLevel;
    public int sprintLevel;
    public int toughnessLevel;

    public float meleeXP;
    public float rangedXP;
    public float sprintXP;
    public float toughnessXP;

    public string currentWeaponName;
    public string currentArmorName;
    public string currentRingName;

    public List<string> inventoryItemNames = new List<string>();

    public bool isPoisoned;
    public float poisonTimeRemaining;

}

