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
        if (distance <= GameSettings.CompanionPlayerRange)
        {
           
            Vector3 destination = Vector3.forward + Vector3.right;
            _companion.Move(destination, GameSettings.SpeedWalking);
            return null;
        }

        // Lorsqu'il est déplacé il s'arrête
        if(_companion.NavAgent.remainingDistance < 0.5f)
        {
            // _companion.SetTarget(null);
            _companion.StopMoving();
            return null;
        }

        chaseTarget = CheckForAggro();
        _companion.SetTarget(chaseTarget);

        if (_companion.Target != null)
        {
            targetPosition = _companion.Target.position;
            Enemy enemy = _companion.Target.GetComponent<Enemy>();

            _companion.LookAtDirection(targetPosition - _companionPosition, 10f);

            isPlayerBlock = checkBeInTheWayOfPlayer();
            //Debug.Log("isPlayerBlock : " + isPlayerBlock);
            // Si le joueur est situé entre la target et le compagnon, le compagnon s'arrête.
            if (isPlayerBlock)
            {
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

            _companion.LookAtDirection(targetPosition - _companionPosition, GameSettings.SpeedWalking); // 

            //Debug.Log("Go to Companion Attack State");
            return typeof(CompanionAttackState);
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

        for (var i = 0; i < 90; i++)
        {

            if (Physics.Raycast(raySource, direction, out hit, GameSettings.AggroRadius))
            {
                var target = hit.transform;

                if (target != null && null != target.GetComponent<Enemy>()) 
                {
                    Debug.DrawRay(raySource, direction * hit.distance, Color.red);
                    Enemy enemy = target.GetComponent<Enemy>();
                    // si l'ennemi est déjà la cible d'un compagnon ou du joueur, alors on cherche un autre ennemi

                    //if (enemy.Chaser == null)
                    //{
                    //    enemy.SetChaser(transform);
                    //    return target.transform;
                    //}
                    //else
                    //{
                    //Debug.Log("enemy is " + enemy.transform);
                    //Debug.Log("targetTemp is " + targetTemp);
                    // inutile
                    //    if (enemy.Target == _companion.Player) //enemy.Chaser == _companion.Player
                    //{
                            //targetTempPlay = enemy.transform;
                            // 1er ennemi target du player, conserve l'ennemi en cible temp
                            if (playerTarget == false)
                            {
                            //Debug.Log("1er enemy, je sette ma target " + enemy.transform);
                                targetTemp = enemy.transform;
                                playerTarget = true;
                                enemy.SetChaser(_companion.Player);
                                break;
                            }
                            if (playerTarget == true && targetTemp != enemy.transform)
                            {
                            //Debug.Log("2e enemy, je set ma target et le chasseur de l'enemy " + enemy.transform);
                            enemy.SetChaser(transform);
                            enemy.SetTarget(transform);
                                targetTemp = enemy.transform;
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
