using System;
using UnityEngine;

public class LostState : BaseState
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

    public LostState(Companion companion) : base(companion.gameObject)
    {
        _companion = companion;

    }
    public override Type Tick()
    {
        
        _companionPosition = _companion.transform.position;
        playerPosition = _companion.Player.position;

        var distance = Vector3.Distance(_companionPosition, playerPosition);
        Vector3 destination = playerPosition;

        if (distance >= 10f)
        {
            return typeof(WalkState);
        }
        _direction = playerPosition; 

        Vector3 lookAtDirection = _direction; 
        transform.LookAt(lookAtDirection);

        RaycastHit hit;
        Quaternion angle = transform.rotation;
        var direction = angle * Vector3.forward;
        Vector3 rayOrigine = _companionPosition + Vector3.up * 0.5f;

        if (Physics.Raycast(rayOrigine, direction, out hit, GameSettings.AggroRadius / 5f))
        {
            Transform target = hit.transform;
            Debug.DrawRay(rayOrigine, direction * hit.distance, Color.blue);
            
            Companion otherCompanion = target.GetComponent<Companion>();
            Enemy enemy = target.GetComponent<Enemy>();
            if (target != null && otherCompanion != null)
            {
                return typeof(WaitState);
            }
            if (target != null && target == _companion.Player)
            {
                return typeof(WaitState); //WalkState
            }
            if (enemy != null)
            {
                return typeof(WaitState); //ou PrepareToAttackState
            }
            //Debug.Log("LostState continue : target is" + target);
        }

        if (distance <= GameSettings.CompanionPlayerRange)
        {
            //  _companion.StopMoving();
            _companion.LookAtDirection(transform.right * 3f, GameSettings.SpeedWalking);
            _companion.Move(transform.forward * 2f, GameSettings.SpeedWalking);
            return null;
            
        }

        _companion.Move(destination, GameSettings.SpeedWalking);

        return null;
    }
}
