using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackState : BaseState
{
    public Enemy _enemy;
    private Vector3 _enemyPosition;

    private float _attackReadyTimer = 5f;

    private Vector3 targetPosition;
    private bool checkAttackTurn;
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

        if (distance <= GameSettings.AttackRange)
        {
            _enemy.StopMoving();
        }

        Companion companion = _enemy.Target.GetComponent<Companion>();
        if (companion != null && companion.AttackNumber <= 0)
        {
            _enemy.ResetTargets();
            return typeof(WanderState);
        }
        // Remplace        
        //Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        //transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10f);
        _attackReadyTimer -= Time.deltaTime;

        checkAttackTurn = false;
        // A faire : l'IA Attaque (animation, dégats ...)
        //if (_attackReadyTimer <= 0f)
        //{
        //    checkAttackTurn = CheckForAttack();
        //}

        if (_attackReadyTimer <= 0f) // checkAttackTurn
        {
            flagStartAttack = true;
            _enemy.LaunchAttack();
            _attackReadyTimer = GameSettings.AttackEnemyTimer;
        }
        


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

    private bool CheckForAttack()
    {
        // check si la cible attaque 
        Transform target = _enemy.Target;
        Animator targetAnim = target.GetComponent<Animator>();
        //Animator targetAnim = target.GetComponentInParent<Animator>();

        //Debug.Log("Component  Animator " + targetCompanion);
        //Debug.Log("Component Animator in Parent" + targetAnim);

        if (!targetAnim.IsInTransition(0) && targetAnim.GetCurrentAnimatorStateInfo(0).IsName("Punching"))
        {
            //Debug.Log("Compagnon attaque return false");
            return false;
        }
        // Si non => attaquer
        // Si oui attendre.
        //DebugDebug.Log("Enemy can attack return true");
        return true;
    }


}
