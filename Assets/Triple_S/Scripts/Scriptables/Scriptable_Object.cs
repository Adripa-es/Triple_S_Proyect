using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Scriptable_Object", menuName = "Triple S / Object Data")]

public class Scriptable_Object : ScriptableObject
{
    [Header("Objetos")]
    [Space]
    public GameObject item;
    public int coinValue;
    public int healthValue;
    public float pemDuration;
    public float pemRadius;
        
    public int timeDespawn;
}