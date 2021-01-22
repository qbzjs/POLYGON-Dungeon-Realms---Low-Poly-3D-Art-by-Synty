using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ReturnState : BaseState
{

    public Enemy _enemy;
    private Vector3 _destination;

    public ReturnState(Enemy enemy) : base(enemy.gameObject)
    {
        _enemy = enemy;
    }

    public override Type Tick()
    {
        // Assigne les propriété de l'IA
        _enemy.Anim.SetBool("Avancer", true);
        _enemy.NavAgent.isStopped = false;
        _enemy.NavAgent.speed = _enemy.EnemyWanderSpeed;

        // Assigne sa position initiale comme destination à l'IA
        _destination = _enemy.InitPosition;
        transform.LookAt(_destination);

        _enemy.NavAgent.SetDestination(_destination);

        // Retourne à l'état WanderState
        return typeof(WanderState);

    }

}
