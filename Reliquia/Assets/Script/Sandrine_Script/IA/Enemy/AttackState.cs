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
            _enemy.Move(_enemyPosition + UnityEngine.Random.insideUnitSphere, GameSettings.SpeedWalking);
            return typeof(WanderState);
        }

        // Si la target sort de la zone d'attaque
        // Retour Ã  l'Ã©tat Chase
        if (distance > GameSettings.AttackRange)
        {
            Debug.Log("attackstate 6");
            //Debug.Log("Go to ChaseState 1");
            //_enemy.SetTarget(null);

            _enemy.StopAttack();
            _enemy.Move(_enemyPosition + UnityEngine.Random.insideUnitSphere, GameSettings.SpeedWalking);
            return typeof(ChaseState);
        }


        Debug.Log("attackstate 2");
        _attackReadyTimer -= Time.deltaTime;

        // Position l'enemy pres de sa cible
        if (_attackReadyTimer <= 0f) 
        {
            Debug.Log("attackstate 3");
            CheckIfNeedToChangeTarget();
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


        //_enemy.StopMoving();
        return null;
    }

     private void CheckIfNeedToChangeTarget()
    {
        Debug.Log("CheckIfNeedToChangeTarget");
        RaycastHit hit;
        Quaternion angle = transform.rotation * startingAngle;
        Vector3 direction = angle * Vector3.forward;
        Vector3 rayOrigine = _enemyPosition + Vector3.up;
        Companion companionChaser = null;
        bool isAnotherEnemyInThePlace = false;

        for (var i = 0; i < 50; i++)
        {
            if (Physics.Raycast(rayOrigine, direction, out hit, GameSettings.AggroRadius))
            {
                Debug.DrawRay(rayOrigine, direction, Color.magenta, 5f);
                var target = hit.transform;
                Enemy enemyDetected = target.GetComponent<Enemy>();
                Companion companionDetected = target.GetComponent<Companion>();
                Debug.Log("target is : " + target);
                if (target != null && companionDetected != null && companionDetected.Target == _enemy.transform)
                {
                    Debug.Log("Companion detected");
                    companionChaser = companionDetected;
                }
                if (target != null && enemyDetected != null) //  enemy.Team != gameObject.GetComponent<Enemy>().Team
                {
                    Debug.Log("Enemy detected");
                    isAnotherEnemyInThePlace = true;

                }

            }
            direction = stepAngle * direction;
        }

        if (isAnotherEnemyInThePlace && companionChaser != null)
        {
            Debug.Log("Change Target for : " + companionChaser);
            _enemy.SetChaser(companionChaser.transform);
            _enemy.SetTarget(companionChaser.transform);

        }
    }
}
