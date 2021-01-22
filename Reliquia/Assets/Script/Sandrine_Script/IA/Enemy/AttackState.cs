using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackState : BaseState
{
    public Enemy _enemy;
    private Vector3 _enemyPosition;

    private float _attackReadyTimer;

    private Vector3 targetPosition;

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

        _enemy.NavAgent.speed = _enemy.EnemyAttackSpeed;

        Vector3 relativePos = targetPosition - _enemyPosition;
        _enemy.LookAt(relativePos, 10f);
        // Remplace        
        //Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        //transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10f);
        //_attackReadyTimer -= Time.deltaTime;

        // A faire : l'IA Attaque (animation, dégats ...)
        if (_attackReadyTimer <= 0f)
        {
            _enemy.LaunchAttack();

        }
        var distance = Vector3.Distance(_enemyPosition, targetPosition);

        // Si le joeur sort de la zone d'attaque
        // Retour à l'état Chase
        if (distance > GameSettings.AttackRange - 1)
        {
            return typeof(ChaseState);
        }


        return null;
    }

    
}
