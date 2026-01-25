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

    public List<string> bridgeIDs = new List<string>();
    public List<bool> bridgeExtended = new List<bool>();

    public List<string> crateIDs = new List<string>();
    public List<bool> crateBroken = new List<bool>();
    public List<bool> cratePickupCollected = new List<bool>();

    public List<string> collectedPickupIDs = new List<string>();
}
