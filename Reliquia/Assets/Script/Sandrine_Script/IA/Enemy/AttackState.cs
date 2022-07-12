﻿using System;
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
    private float _distanceRecorded = 0f;

    private Quaternion startingAngle = Quaternion.AngleAxis(-90, Vector3.up);
    private Quaternion stepAngle = Quaternion.AngleAxis(10, Vector3.up);


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
        Vector3 closePosition = Vector3.Normalize(targetPosition - _enemyPosition);
        _enemy.LookAtDirection(relativePos, 10f);
        _enemy.Move(targetPosition, GameSettings.SpeedAttackWalking);


        if (flagStartAttack == false && distance <= GameSettings.AttackRange - 3f) //  Trop pret du joueur
        {
            _enemy.StopMoving();
        }


        Companion companion = _enemy.Target.GetComponent<Companion>();
        // Si la target sort de la zone d'attaque
        // Retour Ã  l'Ã©tat Chase
        if (distance > GameSettings.AttackRange)
        {

            _enemy.StopAttack();
            flagStartAttack = false;
            relativePos = targetPosition - _enemyPosition;
            _enemy.LookAtDirection(relativePos, GameSettings.SpeedAttackWalking);
            if (companion != null)
            {
                _enemy.ResetTargets();
            }
            
            return typeof(ChaseState);
        }


        if (companion != null && companion.AttackNumber <= 0)
        {
            _enemy.StopAttack();
            return typeof(ChasePlayerState);
        }

        _attackReadyTimer -= Time.deltaTime;

        // Position l'enemy pres de sa cible
        if (_attackReadyTimer <= 0f) 
        {
            CheckIfNeedToChangeTarget();
            flagStartAttack = true;
            targetPosition = _enemy.Target.position;
            _enemy.LookAtDirection(closePosition, GameSettings.SpeedAttackWalking);
            _enemy.Move(targetPosition, GameSettings.SpeedAttackWalking); //_enemyPosition + spacePosition
            _attackReadyTimer = GameSettings.AttackEnemyTimer;
        }


        // Arrivé à destination lancé l'attaque
        if (flagStartAttack && _enemy.NavAgent.remainingDistance < 1.5f)
        {
            //Debug.Log("LaunchAttack");
            _enemy.LaunchAttack();
            _distanceRecorded = distance;


        }

        // Après 2s arrêter l'attaque
        if (flagStartAttack && _attackReadyTimer <= GameSettings.AttackEnemyTimer - 3f) // || _distanceRecorded - distance > 1f
        {
            _enemy.StopAttack();
            flagStartAttack = false;
        }

        return null;
    }

    private void CheckIfNeedToChangeTarget()
    {
        //Debug.Log("CheckIfNeedToChangeTarget");
        RaycastHit hit;
        Quaternion angle = transform.rotation * startingAngle;
        Vector3 direction = angle * Vector3.forward;
        Vector3 rayOrigine = _enemyPosition + Vector3.up;
        Companion companionChaser = null;
        bool isAnotherEnemyInThePlace = false;

        for (var i = 0; i < 90; i++)
        {
            if (Physics.Raycast(rayOrigine, direction, out hit, GameSettings.AggroRadius))
            {
                Debug.DrawRay(rayOrigine, direction, Color.magenta, 5f);
                var target = hit.transform;
                Enemy enemyDetected = target.GetComponent<Enemy>();
                Companion companionDetected = target.GetComponent<Companion>();
                //Debug.Log("target is : " + target);
                if (target != null && companionDetected != null && companionDetected.Target == _enemy.transform)
                {
                    //Debug.Log("Companion detected");
                    companionChaser = companionDetected;
                }
                if (target != null && enemyDetected != null) //  enemy.Team != gameObject.GetComponent<Enemy>().Team
                {
                    //Debug.Log("Enemy detected");
                    isAnotherEnemyInThePlace = true;

                }

            }
            direction = stepAngle * direction;
        }

        if (isAnotherEnemyInThePlace && companionChaser != null && companionChaser.Target == _enemy.transform && companionChaser.AttackNumber >= 0)
        {
            //Debug.Log("Change Target of " + _enemy.gameObject.name + " for : " + companionChaser);
            _enemy.SetChaser(companionChaser.transform);
            _enemy.SetTarget(companionChaser.transform);

        }
    }
}
