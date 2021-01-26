using System;
using UnityEngine;

public class WaitState : BaseState
{
    public Companion _companion;
    private Vector3 _companionPosition;
    private Vector3 playerPosition;
    private Vector3 _direction;
    private Quaternion _desiredRotation;

    public WaitState(Companion companion) : base(companion.gameObject)
    {
        _companion = companion;

    }

    public override Type Tick()
    {
        _companionPosition = _companion.transform.position;
        playerPosition = _companion.Player.position;
        var distance = Vector3.Distance(_companionPosition, playerPosition);

        if (distance <= GameSettings.CompanionPlayerRange)
        {
            _companion.StopMoving();
        }

        // Utiliser pour revenir à l'état précédent dans PrepareToAtteckState
        // _companion.SetLastState(this);

        // si le compagnon est toujours en train de courrir alors il doit s'arrêter.
        if (!_companion.Anim.IsInTransition(0) && _companion.Anim.GetCurrentAnimatorStateInfo(0).IsName("Running"))
        {
            _companion.StopMoving();
            return null;
        }

        // si le player marche et s'éloigne des compagnons, le compagnon passe à l'état Walk
        if (distance > GameSettings.DistanceToWalk
            // && (!_companion.AnimPlayer.IsInTransition(0) && _companion.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("Walking")) 
            )
        {
            return typeof(WalkState);
        }

        // Si le compagnon marche il doit s'arrêter
        if (!_companion.Anim.IsInTransition(0) && _companion.Anim.GetCurrentAnimatorStateInfo(0).IsName("Walking")) {
        
            _companion.Anim.SetBool("Avancer", false);
            _companion.NavAgent.isStopped = true;
            return null;
        }

        // Lorsue le compagnon s'est arrêté, il regarde vers le player
        if (!_companion.Anim.IsInTransition(0) && _companion.Anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            _direction = playerPosition - _companionPosition;
            _companion.LookAtDirection(_direction, 5f);
            
        }

        // si le player attaque alors compagnon attaque aussi
        if (!_companion.AnimPlayer.IsInTransition(0) && 
            _companion.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("Punching"))
        {

            return typeof(PrepareToAttackState);
        }


        // Not Sure.... Du coup l'IA s'arrete derrière les murs ... à voir
        RaycastHit hit;
        Quaternion angle = transform.rotation;
        Vector3 direction = angle * Vector3.forward ;
        Vector3 rayOrigine = _companionPosition + Vector3.up * 0.5f;

        if (Physics.Raycast(rayOrigine, direction, out hit, distance))
        {
            Transform target = hit.transform;
            Debug.DrawRay(rayOrigine, direction * hit.distance,  Color.green);
            Enemy enemy = target.GetComponent<Enemy>();
            Companion otherCompanion = target.GetComponent<Companion>();
            if (target != null && otherCompanion != null)
            {
                return null;
            }
            if (target != null && target != _companion.Player)
            {
                //_companion.Move(playerPosition, GameSettings.SpeedWalking);
                return typeof(LostState); //WalkState
            }
            if (enemy != null)
            {
                return typeof(LostState); //ou PrepareToAttackState
            }
        }
        return null;

    }


}
