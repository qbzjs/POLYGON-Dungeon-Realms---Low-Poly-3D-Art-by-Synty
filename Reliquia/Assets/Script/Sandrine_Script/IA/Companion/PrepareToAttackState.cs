using System;
using UnityEngine;

public class PrepareToAttackState : BaseState
{
    public Companion _companion;
    private Vector3 _companionPosition;
    private Vector3 _companionLastPosition;

    private Vector3 playerPosition;
    private Vector3 targetPosition;
    private Transform chaseTarget;
    private Quaternion startingAngle = Quaternion.AngleAxis(-135, Vector3.up);
    private Quaternion checkPlayerAngle = Quaternion.AngleAxis(-5, Vector3.up); // -20
    private Quaternion stepAngle = Quaternion.AngleAxis(5, Vector3.up);
    private bool playerTarget;
    private bool isPlayerBlock;

    public PrepareToAttackState(Companion companion) : base(companion.gameObject)
    {
        _companion = companion;

    }

    public override Type Tick()
    {
        //Debug.Log("PrepareToAttackState");

        if (_companion.AttackNumber <= 0)
        {
            return typeof(WalkState);
        }
        
        _companionLastPosition = _companion.transform.position;
        _companionPosition = _companion.transform.position;
        playerPosition = _companion.Player.position;

        var distance = Vector3.Distance(_companionPosition, playerPosition);

        // Si le joueur s'en va => le compagnon le suit => walkState
        if (distance > GameSettings.PlayerLeavingRange)
        {
            //Debug.Log("Go to WalkState, player leaving");
            _companion.SetTarget(null);
            return typeof(WalkState);
        }

        // Le compagnon est trop proche du joueur, il se déplace
        // Pe ajouter une contrainte pour le return null => ano. marche dans le vide 
        if (distance <= GameSettings.CompanionPlayerRange)
        {
            //Debug.Log("Compagnon trop proche");
            Vector3 destination = _companion.transform.forward + _companion.transform.right;
            _companion.LookAtDirection(destination - _companionPosition, GameSettings.SpeedWalking);
            _companion.Move(destination, GameSettings.SpeedWalking);
            //return null;
        }

        // Lorsqu'il est déplacé il s'arrête 
        // Pe ajouter une contrainte pour le return null
        if(_companion.NavAgent.remainingDistance < 0.5f)
        {
            //Debug.Log("Compa arrivé à destination");
            // _companion.SetTarget(null);
            _companion.StopMoving();
            //return null;
        }

         if (_companion.Target == null)
        {
            chaseTarget = CheckForAggro();
            _companion.SetTarget(chaseTarget);
        }

        if (_companion.Target != null)
        {
            //Debug.Log("Got Target");
            targetPosition = _companion.Target.position;
            Enemy enemy = _companion.Target.GetComponent<Enemy>();
            float distanceToTarget = Vector3.Distance(targetPosition, _companionPosition);

            _companion.LookAtDirection(targetPosition - _companionPosition, 10f);

            isPlayerBlock = checkBeInTheWayOfPlayer();
            //Debug.Log("isPlayerBlock : " + isPlayerBlock);
            // Si le joueur est situé entre la target et le compagnon, le compagnon s'arrête.
            if (isPlayerBlock && distanceToTarget >= distance)
            {

                _companion.SetTarget(null);
                
                //Debug.Log("Go to WaitState, player in the way");
                return typeof(WaitState);
                /// Provoque des bugs et inutile ou traiter le déplacement autrement
                //Debug.Log("checkBeInTheWayOfPlayer");
                Vector3 newDestination = playerPosition + UnityEngine.Random.insideUnitSphere;// _companion.transform.right; // * 2f
                //Debug.Log("newDestination : " + newDestination);
                RaycastHit hit;
                _companion.LookAtDirection(newDestination, GameSettings.SpeedWalking);
                Quaternion angle = transform.rotation;
                var direction = Vector3.forward; // angle * 
                // Test s'il y a de la place pour se déplacer
                if (Physics.Raycast(_companionPosition, direction, out hit, GameSettings.AggroRadius / 10f))
                {
                    Transform target = hit.transform;
                    if (target != null)
                    {
                        _companion.Move(newDestination, GameSettings.SpeedWalking);
                        if (_companionLastPosition == _companionPosition)
                        {
                            return null;
                        }
                    }
                }
                //pas de place pour se déplacer donc attend.
                //_companion.Target.GetComponent<Enemy>().SetTargetedBy(null);
                _companion.SetTarget(null);
                //Debug.Log("Go to WaitState, player in the way");
                return typeof(WaitState);
            }

           }

        if (_companion.Target != null)
        {

            //Debug.Log("Move to Target with a space destination");
            Vector3 spaceDestination = _companion.Target.position - (playerPosition - _companion.Target.position);
            _companion.LookAtDirection(spaceDestination - _companionPosition, GameSettings.SpeedWalking);
            //_companion.LookAtDirection(spaceDestination, GameSettings.SpeedWalking);
            //_companion.LookAtDirection(_companionPosition - spaceDestination, GameSettings.SpeedWalking);
            _companion.Move(spaceDestination, GameSettings.SpeedWalking);

            if (_companion.NavAgent.remainingDistance < 1f)
            {
                _companion.LookAtDirection(targetPosition - _companionPosition, GameSettings.SpeedWalking);
                //Debug.Log("Go to CompanionAttackState");
                return typeof(CompanionAttackState);
            }
            return null;
        }

        //Debug.Log("Go to WaitState, target is null");
        return typeof(WaitState); //WalkState
    }

    private bool checkBeInTheWayOfPlayer()
    {

        RaycastHit hit;
        var angle = _companion.transform.rotation * checkPlayerAngle;
        var direction = angle * Vector3.forward;
        _companion.LookAtDirection(_companion.Target.position, 10f);

        for (var i = 0; i < 5; i++)
        {
            if (Physics.Raycast(_companionPosition, direction, out hit, GameSettings.AggroRadius))
            {
                Debug.DrawRay(_companionPosition, direction * hit.distance, Color.white, 2f);
                var target = hit.transform;
                //Debug.Log("target : " + target);
                if (target != null && target == _companion.Player )
                {
                    Debug.DrawRay(_companionPosition, direction * hit.distance, Color.red, 5f);
                    return true;
                }
            } else
            { 
                Debug.DrawRay(_companionPosition, direction * hit.distance, Color.green, 5f);
            }

            direction = stepAngle * direction;
        }
        return false;
    }

    private Transform CheckForAggro()
    {
        //Debug.Log("Compagnon détecte nouvel ennemy");
        RaycastHit hit;
        var angle = transform.rotation * startingAngle;
        var direction = angle * Vector3.forward;
        Transform targetTemp = null;
        //Transform targetTempPlay = null;
        Vector3 raySource = _companionPosition + Vector3.up * 0.5f;
        playerTarget = false;
        bool beTheTarget = false;

        for (var i = 0; i < 90; i++)
        {

            if (Physics.Raycast(raySource, direction, out hit, GameSettings.AggroRadius))
            {
                var target = hit.transform;

                if (target != null && null != target.GetComponent<Enemy>()) 
                {
                    Debug.DrawRay(raySource, direction * hit.distance, Color.red);
                    Enemy enemyDetected = target.GetComponent<Enemy>();
                    // si l'ennemi est déjà la cible d'un compagnon ou du joueur, alors on cherche un autre ennemi

                    //if (enemyDetected.Chaser == null)
                    //{
                    //    enemyDetected.SetChaser(transform);
                    //    return target.transform;
                    //}
                    //else
                    //{
                    //Debug.Log("enemy is " + enemyDetected.transform);
                    //Debug.Log("targetTemp is " + targetTemp);
                    // inutile
                    //    if (enemyDetected.Target == _companion.Player) //enemy.Chaser == _companion.Player
                    //{
                    //targetTempPlay = enemy.transform;
                    // 1er ennemi target du player, conserve l'ennemi en cible temp
                    if (playerTarget == false)
                            {
                            //Debug.Log("1er enemy, je sette ma target " + enemy.transform);
                                targetTemp = enemyDetected.transform;
                                playerTarget = true;
                                enemyDetected.SetChaser(_companion.Player);
                                break;
                            }
                            //Si j'ai déja un annemi sur le joueur et que j'ai détecté un autre ennemi et que sa target n'est pas l'autre companion
                            if (playerTarget == true && targetTemp != enemyDetected.transform && enemyDetected.Target.GetComponent<Companion>() == null)
                            {
                            //Debug.Log("2e enemy, je set ma target et le chasseur de l'enemy " + enemy.transform);
                            enemyDetected.SetChaser(_companion.transform);
                            enemyDetected.SetTarget(_companion.transform);
                                targetTemp = enemyDetected.transform;
								beTheTarget = true;
                                //enemy.SetTarget(transform);
                                break;
                            }
                        //}
                    //}
                    
                }
                else
                {
                    Debug.DrawRay(raySource, direction * hit.distance, Color.yellow);
                }
            }
            else
            {
                Debug.DrawRay(_companionPosition, direction * hit.distance, Color.white);
            }
            
            direction = stepAngle * direction;

        }
        //Debug.Log("targetTemp final is " + targetTemp);
        // Dans tous les cas la target est celui chassé  par le joueur car 2 compagnons n'attaquent pas un ennemi.
        //Debug.Log(_companion.Name + ", ma target est : " + targetTemp);
        //targetTemp = targetTempPlay;
        return targetTemp;

    }

}
