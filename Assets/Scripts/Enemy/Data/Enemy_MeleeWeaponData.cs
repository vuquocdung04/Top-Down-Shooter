using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemy Data/Melee Weapon Data", order = 0)]
public class Enemy_MeleeWeaponData : ScriptableObject
{
    public List<AttackData_EnemyMelee> attackDatas;
    public float turnSpeed = 10f;
}