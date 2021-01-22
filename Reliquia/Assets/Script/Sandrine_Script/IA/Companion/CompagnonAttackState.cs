using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CompagnonAttackState : BaseState
{
    public Companion _companion;
    private Vector3 _companionPosition;
    private Vector3 targetPosition;
    private Vector3 playerPosition;

    private float _attackReadyTimer;

    public CompagnonAttackState(Companion companion) : base(companion.gameObject)
    {
        _companion = companion;

    }

    public override Type Tick()
    {
        if (_companion.Target == null)
            return typeof(WalkState);

        // Assignation des positions
        _companionPosition = _companion.transform.position;
        targetPosition = _companion.Target.position;
        playerPosition = _companion.Player.position;

        _companion.SetSpeed(_companion.CompanionAttackSpeed);

        var distance = Vector3.Distance(_companionPosition, targetPosition);
        var distanceToPlayer = Vector3.Distance(_companionPosition, playerPosition);

        
        _attackReadyTimer -= Time.deltaTime;

        if (_companion.NavAgent.remainingDistance <= 0.5f)
        {

            Vector3 relativePos = targetPosition - _companionPosition;
            _companion.LookAt(relativePos, 10f);
            _companion.Attack(3f);

        }

        // Si le joeur sort de la zone d'attaque
        // Retour à l'état Chase
        if (_companion.NotAttacking() && distanceToPlayer > 6f)
        {
            _companion.SetTarget(null);
            return typeof(WalkState);
        }

        // Suivre l'ennemi et continuer à attaquer
        if (distance >= GameSettings.FollowInAttackStateDistance) //2f
        {
            Vector3 relativePos = targetPosition - _companionPosition;
            _companion.LookAt(relativePos, 10f);

            _companion.Move(targetPosition, GameSettings.SpeedAttackWalking); //3f

        }

        return null; 
    }
}
