using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Scriptable_Turret", menuName = "Triple S / Turret Data")]

public class Scriptable_Turret : ScriptableObject
{
    public float damage;
    public float shootSpeed;
    public float rotationSpeed;    
    public int AmountTrackingBullets;


}