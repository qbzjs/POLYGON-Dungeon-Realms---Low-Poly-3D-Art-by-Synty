using System;
using UnityEngine;

public class WalkState : BaseState
{
    public Companion _companion;
    private Vector3 playerLastPosition;
    private Vector3 _companionPosition;
    private Vector3 _companionLastPosition;

    private Vector3 playerPosition;
    private Vector3 _destination;
    private Vector3 _direction;

    private Quaternion stepAngle = Quaternion.AngleAxis(15, Vector3.up);

    public WalkState(Companion companion) : base(companion.gameObject)
    {
        _companion = companion;

    }

    public override Type Tick()
    {
        //Debug.Log(this);
        _companionLastPosition = _companion.transform.position;
        _companionPosition = _companion.transform.position;
        _companion.StopAttack();
        _companion.NavAgent.isStopped = false;
        _companion.Anim.SetBool("Avancer", true);

        // Utiliser pour revenir à l'état précédent dans PrepareToAtteckState
        // _companion.SetLastState(this);

        playerLastPosition = playerPosition;
        playerPosition = _companion.Player.position;

        var distance = Vector3.Distance(_companionPosition, playerPosition);

        if (distance <= GameSettings.CompanionPlayerRange)
        {
            _companion.StopMoving();
        }
        

        FollowPlayer(); // assigne une nouvelle destination et rotation au compagnon

        Vector3 lookAtDirection = _companionPosition + _direction;
        _companion.LookAtDirection(lookAtDirection, 0.5f);

        // Si le compagnon arrive trop pres du player
        // Alors return typeof(WaitState);
        if (distance <= GameSettings.DistanceToWalk) // || distance < GameSettings.DistanceToWalk
        {
            return typeof(WaitState);

        }

        // si le player attaque alors compagnon attaque aussi
        if (_companion.AttackNumber > 0 && !_companion.AnimPlayer.IsInTransition(0) && _companion.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("Punching"))
        {
            // Par défaut
           
            return typeof(PrepareToAttackState);
        }

        // si le player court alors compagnon cours aussi
        if ( _companion.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("Running"))
        {
            _companion.LookAtDirection(_direction, GameSettings.SpeedWalking);
            _companion.Move(_destination, GameSettings.SpeedRunning, "Course");
            return null;

        }
        else
        {
            // Par défaut
            float speed = GameSettings.SpeedWalking; // + Vector3.Distance(_destination, _companionPosition) / (Time.deltaTime * 20f);
            _companion.LookAtDirection(_direction, GameSettings.SpeedWalking);
            _companion.Move(_destination, speed);
        }

        // si le player s'arrete alors compagnon s'arrete aussi 
        if (!_companion.AnimPlayer.IsInTransition(0) && _companion.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("run to stop") && distance < 2.2f)
        {
            _companion.StopMoving();

            return typeof(WaitState);
        }

        // Si le player arrive à destination ou que la distance <= DistanceToWalk
        // Alors return typeof(WaitState);
        if (_companion.NavAgent.remainingDistance <= 0.05f && _companion.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("Idle")) // playerLastPosition == playerPosition || distance < GameSettings.DistanceToWalk
        {
            return typeof(WaitState);

        }

        // Si le player arrive à destination
        // Alors return typeof(WaitState);
        if (_companion.NavAgent.remainingDistance <= 0.05f) // || distance < GameSettings.DistanceToWalk
        {
            return typeof(WaitState);

        }

        // La destination est hors navmeh ou le companon est bloqué
        // Alors return typeof(WaitState);
        //if (_companion.NavAgent.remainingDistance <= 2f && _companion.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        //{
        //    _companion.StopMoving();
        //    return typeof(WaitState);

        //}

        

        if (_companionLastPosition == _companion.transform.position && _companion.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            _companion.StopMoving();
            return typeof(WaitState);
        }

        

        return null;
    }

    
    private void FollowPlayer()
    {

        Vector3 _lastDirection = _direction == Vector3.zero ? playerPosition - _companionPosition : _direction;

        // Par défaut Compagnon = Roxane
        _destination = playerPosition - (_companion.Player.forward * 2f) + (_companion.Player.right * 0.5f);

        if (_companion.Name == "David")
        {
            _destination = playerPosition - (_companion.Player.forward * 3f) - (_companion.Player.right * 0.8f);
        }

        //// ---- Begin Positionne le compagnon à côté du Player et non derrière => je laisse au cas où se serait utilise par la suite.
        //_destination = playerPosition + _companion.Player.right;

        //if (_companion.Name == "David")
        //{
        //    _destination = playerPosition - _companion.Player.right;
        //}

        //// Vérifie si la destination du compagnon est libre
        //// Sinon place le compagnon de l'autre côté du player
        //float rayDistance = Vector3.Distance(_destination, _companionPosition);
        //RaycastHit hit;
        //Vector3 rayDirection = (_destination - _companionPosition) + Vector3.up;

        //for (var i = 0; i < 2; i++)
        //{
        //    if (Physics.Raycast(_companionPosition, rayDirection, out hit, rayDistance))
        //    {
        //        Debug.DrawRay(_companionPosition, rayDirection, Color.green);
        //        var target = hit.transform;
        //        if (target != null && target != _companion.Player && target != transform)
        //        {
        //            _destination = playerPosition - _companion.Player.right * 2f;
        //            if (_companion.Name == "David")
        //            {
        //                _destination = playerPosition + _companion.Player.right * 2f;
        //            }
        //            break;
        //        }
        //    }

        //    rayDirection = stepAngle * rayDirection;
        //}

        ///// ---- End Positionnement du compagnon à côté du player.

        //_destination = new Vector3(_destination.x, y: 1f, _destination.z);

        // _direction vers laquelle regarde le compagnon en se déplaçant
        _direction = playerPosition - playerLastPosition == Vector3.zero ? _lastDirection : (playerPosition - playerLastPosition );
        
    }

}
