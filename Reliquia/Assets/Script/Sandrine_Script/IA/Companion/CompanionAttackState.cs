using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class CompanionAttackState : BaseState
{
    public Companion _companion;
    private Vector3 _companionPosition;
    private Vector3 targetPosition;
    private Vector3 playerPosition;
    private Vector3 lastPosition;

    private float _attackReadyTimer = 1000f;
    private bool checkAttackTurn;




    private float _attackEndTimer = 0f;

    private bool flagEndAttack;
    private bool flagStartAttack;
    private bool playerTarget;

    private Quaternion startingAngle = Quaternion.AngleAxis(-135, Vector3.up);
    private Quaternion stepAngle = Quaternion.AngleAxis(5, Vector3.up);

    public CompanionAttackState(Companion companion) : base(companion.gameObject)
    {
        _companion = companion;
        

    }

    public override Type Tick()
    {
        //Debug.Log("Companion AttackState");
        if (_attackReadyTimer == 1000f)
        {
            _attackReadyTimer = _companion.AttackReadyTimer - 5f;
        }
        if (_companion.Target == null)
        {
            //Debug.Log("Go to WalkState, target is null");
            return typeof(WalkState);
        }
        _attackReadyTimer -= Time.deltaTime;

        if (_companion.AttackNumber <= 0 && _attackReadyTimer <= 0f)
        {
            _companion.SetTarget(null);
            return typeof(WaitState);
        }

        
        // Assignation des positions
        _companionPosition = _companion.transform.position;
        targetPosition = _companion.Target.transform.position;
        playerPosition = _companion.Player.position;
        //lastPosition = _companionPosition;

        _companion.SetSpeed(_companion.CompanionAttackSpeed);

        var distance = Vector3.Distance(_companionPosition, targetPosition);
        var distanceToPlayer = Vector3.Distance(_companionPosition, playerPosition);

        if (distance <= GameSettings.CompanionPlayerRange)
        {
            _companion.StopMoving();
        }


        //Debug.Log("disatance attack state: " + distance);
        //Debug.Log("_attackReadyTimer for : " + _companion.Name + " " + _attackReadyTimer);
        //Debug.Log("IsAttacking ? " + _companion.IsAttacking());

        if (distance <= GameSettings.CompanionAttackRange )
        {
            
            _companion.LookAt(targetPosition - _companionPosition, GameSettings.SpeedAttackWalking);
            _companion.StopMoving();
        }

        checkAttackTurn = false;
        if (_attackReadyTimer <= 0f ) // _companion.NavAgent.remainingDistance <= 5f
        {
            
            CheckNewEnemy();
            checkAttackTurn = CheckForAttack();
            //Debug.Log("Name checkAttackTurn : " + _companion.Name + " " + checkAttackTurn);
        }


        if (checkAttackTurn && _attackReadyTimer <= 0f) 
        {
            _companion.flagAttack = true;
            //Debug.Log("start attack for : " + _companion.Name);
            lastPosition = _companionPosition;
            Vector3 attackPosition = _companionPosition + (targetPosition - _companionPosition) / 2;

            Vector3 relativePos = targetPosition - _companionPosition;  // 
            _companion.LookAt(relativePos, 10f);

            //_companion.Move(attackPosition, GameSettings.SpeedAttackWalking);

            flagStartAttack = true;

            // Set enemy targeted : déjà fait dans le prepare to attack
            _companion.Attack(10f);
            
            _attackReadyTimer = _companion.AttackReadyTimer;
            //return null;

        }
        if (_attackReadyTimer <= _companion.AttackReadyTimer - 3f && flagStartAttack)
        {
            //Debug.Log("stop attack for : " + _companion.Name);
            _companion.flagAttack = false;
            _companion.StopAttack();
            
            flagStartAttack = false;
            _companion.DecreaseAttackNumber();
        }

         

        // Si le joeur sort de la zone d'attaque
        // Retour à l'état Chase
        if (!_companion.IsAttacking() && distanceToPlayer > GameSettings.PlayerLeavingRange)
        {

            _companion.Target.GetComponent<Enemy>().ResetTargets();
            _companion.SetTarget(null);
            //Debug.Log("Go to walkstate, player is leaving");
            return typeof(WalkState);
        }

        // Suivre l'ennemi et continuer à attaquer
        if (distance >= 5f ) // To Replace GameSettings.FollowInAttackStateDistance) //2f
        {
            Vector3 relativePos = targetPosition - _companionPosition;// - _companionPosition;
            _companion.LookAt(relativePos, 10f);

            _companion.Move(targetPosition, GameSettings.SpeedAttackWalking); //3f

        }

        return null; 
    }

    private void CheckNewEnemy()
    {
        //Debug.Log("CheckNewEnemy");
        RaycastHit hit;
        var angle = transform.rotation * startingAngle;
        var direction = angle * Vector3.forward;
        Transform targetTemp = null;
        //Transform targetTempPlay = null;
        Vector3 raySource = _companionPosition + Vector3.up * 0.5f;
        playerTarget = false;

        for (var i = 0; i < 90; i++)
        {

            if (Physics.Raycast(raySource, direction, out hit, GameSettings.AggroRadius))
            {
                Transform target = hit.transform;
                Enemy enemyCast = target.GetComponent<Enemy>();

                if (target != null && null != enemyCast)
                {
                    Debug.DrawRay(raySource, direction * hit.distance, Color.red);
                    

                    if (targetTemp != null && targetTemp != enemyCast.transform)
                    {
                        // il y a 2 ennemis
                        //
                        _companion.Target.GetComponent<Enemy>().SetTarget(transform);
                        _companion.Target.GetComponent<Enemy>().SetChaser(transform);
                        //Debug.Log("CheckNewEnemy Yes : tragetTemp & enemy.transform : " + targetTemp + " " + enemyCast.transform);
                        return;
                    }
                    targetTemp = enemyCast.transform;

                }
            }

            direction = stepAngle * direction;

        }

    }

    private bool CheckForAttack()
    {
        return true;
        Enemy enemy = _companion.Target.GetComponent<Enemy>();
        Transform chaser = enemy.Chaser;
        


        if (chaser == null)
        {
            //enemy.SetChaser(transform);
            //enemy.SetTarget(transform);
            //Debug.Log("No Chase punching return true");
            return true;
        }

        // Est-ce que l'IA Compagnon peut attaquer le même ennemi que le joueur => true
        if (chaser.CompareTag("Player"))
        {
            //Animator playerAnim = chaser.GetComponent<Animator>();
            //if (playerAnim != null && !playerAnim.IsInTransition(0) 
            //    && playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Punching"))
            //{
            //    Debug.Log("Player punching return false");
            //    return false;
            //}
            //Debug.Log("Player not punching return true");
            return true;
        }
        // Si l'ennemi m'attaque, je n'attaque pas => non
        //if ( !enemy.Anim.IsInTransition(0) && enemy.Anim.GetCurrentAnimatorStateInfo(0).IsName("Puching")
        //    && enemy.Target == transform)
        //{
        //    Debug.Log("Enemy, my target is punching me return false");
        //    return false;
        //}

        Companion otherCompanion = chaser.GetComponent<Companion>();

        // Debug A suupprimer
        if (otherCompanion != null)
        {
            //Debug.Log("Enemy is targeted by : " + otherCompanion.Name);
        }

        if (otherCompanion != null && chaser == transform && !otherCompanion.AnimPlayer.IsInTransition(0)
            && otherCompanion.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("Punching"))
        {
            //Debug.Log(otherCompanion.Name + ", me, punching return false");
            return false;
        }

        if (otherCompanion != null && chaser == transform)
        {
            //Debug.Log(otherCompanion.Name + " chase and not punching return true");
            //enemy.SetChaser(transform);
            enemy.SetTarget(transform);
            return true;
        }

        // impossible
        //if (otherCompanion != null && !otherCompanion.AnimPlayer.IsInTransition(0) 
        //    && otherCompanion.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("Punching"))
        //{
        //    Debug.Log(otherCompanion.Name + ", not me,  punching return false");
        //    return false;
        //}
      
        

        //Debug.Log(otherCompanion.Name + " and me, not punching return true");

        //if (otherCompanion.flagAttack == true)
        //{
        //    return false;
        //}

        //enemy.SetChaser(transform);
        enemy.SetTarget(transform);
        return true;

    }
}
