using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackState : BaseState
{
    public Enemy _enemy;
    private Vector3 _enemyPosition;

    private float _attackReadyTimer = 3f;

    private Vector3 targetPosition;
    private bool flagStartAttack;

    public AttackState(Enemy enemy) : base(enemy.gameObject)
    {
        _enemy = enemy;
    }

    public override Type Tick()
    {

        if (_enemy.Target == null)
            return typeof(WanderState);

        // Assignation des positions
        _enemyPosition = _enemy.transform.position;
        targetPosition = _enemy.Target.position;

        float distance = Vector3.Distance(_enemyPosition, targetPosition);
        _enemy.NavAgent.speed = _enemy.EnemyAttackSpeed;

        Vector3 relativePos = targetPosition - _enemyPosition;
        _enemy.LookAt(relativePos, 10f);

        if (distance <= GameSettings.AttackRange && flagStartAttack == false)
        {
            _enemy.StopMoving();
        }

        if (_enemy.NavAgent.remainingDistance <= 0.5f)
        {
            _enemy.ResetTargets();
            return typeof(ReturnState);
        }

        Companion companion = _enemy.Target.GetComponent<Companion>();
        if (companion != null && companion.AttackNumber <= 0)
        {
            _enemy.ResetTargets();
            return typeof(WanderState);
        }

        _attackReadyTimer -= Time.deltaTime;

        // Position l'enemy pres de sa cible
        if (_attackReadyTimer <= 0f) 
        {
            flagStartAttack = true;
            Vector3 spacePosition = Vector3.Normalize(targetPosition - _enemyPosition);
            _enemy.Move(targetPosition, GameSettings.SpeedAttackWalking); //_enemyPosition + spacePosition
            _attackReadyTimer = GameSettings.AttackEnemyTimer;
        }

        // Arrivé à destination lancé l'attaque
        if (_enemy.NavAgent.remainingDistance < 1f && flagStartAttack == true)
        {
            _enemy.LaunchAttack();
            
        }

        // Après 2s arrêter l'attaque
        if (_attackReadyTimer <= GameSettings.AttackEnemyTimer - 3f && flagStartAttack)
        {
            //Debug.Log("stop attack for : Enemy");
            _enemy.StopAttack();
            flagStartAttack = false;
        }

        // Si le joeur sort de la zone d'attaque
        // Retour à l'état Chase
        if (distance > GameSettings.AttackRange)
        {
            //Debug.Log("Go to ChaseState 1");
            //_enemy.SetTarget(null);
            return typeof(ChaseState);
        }

        //_enemy.StopMoving();
        return null;
    }

}
