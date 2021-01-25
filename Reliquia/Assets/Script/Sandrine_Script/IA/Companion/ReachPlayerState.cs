using System;
using UnityEngine;

public class ReachPlayerState : BaseState
{
    public Companion _companion;
    private Vector3 playerLastPosition;
    private Vector3 _companionPosition;

    private Vector3 playerPosition;

    private float seekCounter = 0;
    private bool flagStopWainting = false;

    private Vector3 _destination;
    private Vector3 _direction;
    private Quaternion _desiredRotation;

    private Quaternion stepAngle = Quaternion.AngleAxis(15, Vector3.up);

    public ReachPlayerState(Companion companion) : base(companion.gameObject)
    {
        _companion = companion;

    }
    public override Type Tick()
    {
        
        _companionPosition = _companion.transform.position;
        playerPosition = _companion.Player.position;

        var distance = Vector3.Distance(_companionPosition, playerPosition);

        if (distance >= 10f)
        {
            Debug.Log("Go to walkState 6");
            return typeof(WalkState);
        }

        // Not Sure
        // Si le compagnon arrive trop pres du player
        // Alors return typeof(WaitState);
        if (distance <= GameSettings.DistanceToWalk) // || distance < GameSettings.DistanceToWalk
        {
            Debug.Log("Go to WaitState 10");
            return typeof(WaitState);

        }

        _destination = playerPosition - _companion.Player.forward   - _companion.Player.right;
        if (_companion.Name == "David")
        {
            _destination = playerPosition - _companion.Player.forward * 2 ;
        }
        _destination = new Vector3(_destination.x, y: 1f, _destination.z);

        _direction = playerPosition; 



        Vector3 lookAtDirection = _direction; 
        transform.LookAt(lookAtDirection);

        _desiredRotation = Quaternion.LookRotation(_direction);

        transform.rotation = Quaternion.Lerp(transform.rotation, _desiredRotation, Time.deltaTime * 0.5f);
        //_companion.NavAgent.SetDestination(_destination);

        //_companion.Anim.SetBool("Course", false);
        //_companion.Anim.SetBool("Avancer", true);
        //_companion.NavAgent.speed = GameSettings.SpeedWalking;
        //_companion.NavAgent.isStopped = false;
        // Replace by
        _companion.Move(_destination, GameSettings.SpeedWalking);

        if (_companion.NavAgent.remainingDistance <= 1f && _companion.NavAgent.remainingDistance != 0)
        {
            Debug.Log("Go to WaitState 6");
            return typeof(WaitState);
        }

        // Si le player arrive à destination ou que la distance <= DistanceToWalk
        // Alors return typeof(WaitState);
        if (_companion.NavAgent.remainingDistance <= 2f && _companion.AnimPlayer.GetCurrentAnimatorStateInfo(0).IsName("Idle")) // playerLastPosition == playerPosition || distance < GameSettings.DistanceToWalk
        {
            Debug.Log("Go to WaitState 7 ");
            _companion.Move(playerPosition, GameSettings.SpeedWalking);
            return typeof(WaitState);

        }

        return null;
    }
}
