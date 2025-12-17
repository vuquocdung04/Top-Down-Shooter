using System;
using UnityEngine;

public class Enemy_AnimationEvents : MonoBehaviour
{
    private Enemy enemy;
    private Enemy_Melee enemyMelee;
    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
        enemyMelee = GetComponentInParent<Enemy_Melee>();
    }

    public void AnimationTrigger() => enemy.AnimationTrigger();

    public void StartManualMovement() => enemy.ActivateManualMovement(true);
    public void StopManualMovement() => enemy.ActivateManualMovement(false);
    
    public void StartManualRotation() => enemy.ActivateManualRotation(true);
    public void StopManualRotation() => enemy.ActivateManualRotation(false);

    public void AbilityEvent() => enemy.AbilityTrigger();

    public void EnableIK() => enemy.visuals.EnableIk(true, true, 1.5f);

    public void BeginMeleeAttackCheck()
    {
        enemyMelee?.EnableAttackCheck(true);
    }

    public void FinishMeleeAttackCheck()
    {
        enemyMelee?.EnableAttackCheck(false);
    }
}