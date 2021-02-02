using System;
using UnityEngine;

public class ChasePlayerState : BaseState
{
    public Enemy _enemy;
    private Vector3 _enemyPosition;
    private Vector3 targetLastPosition;
    private Vector3 targetPosition;
    private float seekCounter = 0;
    private Vector3 _destination;
    private Vector3 _direction;
    private Quaternion _desiredRotation;
    private bool flagStartState = true;

    private float targetSpeed;

    private Quaternion startingAngle = Quaternion.AngleAxis(-90, Vector3.up);
    private Quaternion stepAngle = Quaternion.AngleAxis(5, Vector3.up);


    public ChasePlayerState(Enemy enemy) : base(enemy.gameObject)
    {
        _enemy = enemy;

    }

    public override Type Tick()
    {
        if (_enemy.Target == null)
        {
            //Debug.Log("Go back wanserstate : " + _enemy.gameObject.name);
            _enemy.SetChaser(null);
            return typeof(WanderState);
        }
        RaycastHit hit;
        Quaternion angle = transform.rotation * startingAngle;
        Vector3 direction = angle * Vector3.forward;
        Vector3 rayOrigine = _enemyPosition + Vector3.up;

        for (var i = 0; i < 35; i++)
        {
            if (Physics.Raycast(rayOrigine, direction, out hit, GameSettings.AggroRadius))
            {

                var target = hit.transform;
                if (target != null && target == gameObject.CompareTag("Player")) //  enemy.Team != gameObject.GetComponent<Enemy>().Team
                {
                    Debug.DrawRay(rayOrigine, direction * hit.distance, Color.red);
                    _enemy.SetTarget(target.transform);
                    _enemy.SetChaser(target.transform);
                    return typeof(ChaseState);
                }
                else
                {
                    Debug.DrawRay(rayOrigine, direction * hit.distance, Color.yellow);
                }
            }
            else
            {
                Debug.DrawRay(rayOrigine, direction * hit.distance, Color.white);
            }
            direction = stepAngle * direction;
        }
        return typeof(WanderState);
    }


}
