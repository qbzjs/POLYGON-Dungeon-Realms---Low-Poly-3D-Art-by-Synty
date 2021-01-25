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
            return typeof(WalkState);
        }

        if (distance <= GameSettings.CompanionPlayerRange)
        {
            _companion.StopMoving();
        }

        //chaseTarget = CheckForAggro();
        _companion.SetTarget(CheckForAggro());

        if (_companion.Target != null)
        {
            targetPosition = _companion.Target.position;
            Vector3 relativePos = targetPosition - _companionPosition;
            float targetDistance = Vector3.Distance(targetPosition, _companionPosition);
            Enemy enemy = _companion.Target.GetComponent<Enemy>();

            // Le joueur et l'ennemi se combatte le compagnon ne fait rien => pb si le joueur est chasseur des 2 Ennemis
            // Je commente pour l'instant le compagnon peut attaquer la cible du joueur
            if (enemy.Chaser == _companion.Player)
            {
                //_companion.SetTarget(null);
                //return typeof(WaitState);
            }
            //_companion.Target.GetComponent<Enemy>().SetChaser(transform);

            // _companion.LookAt(relativePos, 10f);
            _companion.LookAt(targetPosition - _companionPosition, 10f); //

            // Si le joueur est sistué entre la target et le compagnon, le compagnon s'arrête.
            if (checkBeInTheWayOfPlayer())
            {
                //Debug.Log("checkBeInTheWayOfPlayer");
                // se déplace à droite où à gauche du joueur s'il y a de la place (pas de mur) => plus tard pe inutile
                //var randomDeplacement = new System.Random().Next(-5, 5);
                Vector3 newDestination = _companion.Player.position + transform.right * 2f;
                //Debug.Log("newDestination : " + newDestination);
                RaycastHit hit;
                _companion.LookAt(newDestination, GameSettings.SpeedWalking);
                Quaternion angle = transform.rotation;
                var direction = angle * Vector3.forward;
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

            Vector3 attackPosition = targetPosition;
            String anim = "";

            if (targetDistance <= GameSettings.CompanionPlayerRange)
            {
                attackPosition = _companionPosition - (transform.forward );
                anim = "Reculer";
            }
            _companion.LookAt(targetPosition - _companionPosition, GameSettings.SpeedWalking); // 
            _companion.Move(attackPosition, GameSettings.SpeedAttackWalking, anim); // targetPosition

            //Debug.Log("Go to Companion Attack State");
            return typeof(CompanionAttackState);
        }

        //chaseTarget = CheckForAggro();
        if (chaseTarget != null)
        {

            _companion.SetTarget((Transform)chaseTarget);            
            return null;
            
        }
        //Debug.Log("Go to WaitState, target is null");
        return typeof(WaitState); //WalkState
    }

    private bool checkBeInTheWayOfPlayer()
    {

        RaycastHit hit;
        var angle = transform.rotation * checkPlayerAngle;
        var direction = angle * Vector3.forward;

        for (var i = 0; i < 5; i++)
        {
            if (Physics.Raycast(_companionPosition, direction, out hit, GameSettings.AggroRadius))
            {
                Debug.DrawRay(_companionPosition, direction * hit.distance, Color.white, 10f);
                var target = hit.transform;
                if (target != null && target == _companion.Player )
                {
                    Debug.DrawRay(_companionPosition, direction * hit.distance, Color.red, 10f);
                    return true;
                }
            } else
            { 
                Debug.DrawRay(_companionPosition, direction * hit.distance, Color.green, 10f);
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
