using System;
using UnityEngine;

public class ChaseState : BaseState
{
    public Enemy _enemy;
    private Vector3 _enemyPosition;
    private Vector3 targetLastPosition;
    private Vector3 targetPosition;
    private float seekCounter = 0;
    private Vector3 _destination;
    private Vector3 _direction;
    private Quaternion _desiredRotation;

    private float targetSpeed;

    public ChaseState(Enemy enemy) : base(enemy.gameObject)
    {
        _enemy = enemy;

    }

    public override Type Tick()
    {
        if (_enemy.Target == null)
        {
            _enemy.SetChaser(null);
            return typeof(WanderState);
        }
            

        // Assigne la position de l'ennemi
        _enemyPosition = _enemy.transform.position;

        targetLastPosition = targetPosition;
        targetPosition = _enemy.Target.position;
        //targetSpeed = Vector3.Distance(targetPosition, targetLastPosition) / Time.deltaTime;

        _enemy.NavAgent.speed = _enemy.EnemyChaseSpeed;

        // l'IA arrête de suivre la cible et revient à sa position initiale
        // si elle est au bord du Nav Mesh 
        // L'unique cas dans cet état où elle peut rejoindre sa destination.
        if (_enemy.NavAgent.remainingDistance <= 0.5f)
        {
            _enemy.ResetTargets();
            return typeof(ReturnState);
        }
        // Suivre la cible
        followTarget(_enemy.Target);

        _enemy.LookAt(_direction, 0.5f);

        _enemy.Move(_destination, _enemy.EnemyChaseSpeed); // GameSettings.SpeedWalking);

        var distance = Vector3.Distance(_enemyPosition, _enemy.Target.position);

        // Si le player passe en zone rouge => chgmt d'état vers AttackState.
        // Dans cette zone le joueur ne peut plus se cacher.
        if (distance <= GameSettings.AttackRange) 
        {
            if (_enemy.Target != null)
                return typeof(AttackState);
        }

        // si le joueur sort de la zone de pistage
        // le joeur n'est plus la cible de l'IA.
        if (distance > GameSettings.ChaseRange) 
        {
            //_enemy.SetTarget(null);
            //return null;
            // Plutôt
            //Debug.Log("Go to ReturnState");
            _enemy.ResetTargets();
            return typeof(ReturnState);
        }

        // si le joueur se cache
        // le joeur n'est plus la cible de l'IA
        if (seekCounter >= GameSettings.ChaseWaintingTime && false == CheckToContinue()) // Check if target hide.
        {
            //Debug.Log("Go to WanderState");
            _enemy.ResetTargets();

            return null;
        }

        // Non l'ennemi ne court pas.
        //// si le player court alors compagnon cours aussi
        //if (targetSpeed >= GameSettings.SpeedRunning - 5)
        //{
        //    _enemy.Move(_destination, GameSettings.SpeedRunning, "Course");
        //    return null;

        //}

        _enemy.Anim.SetBool("Course", false);
        seekCounter++; // compteur utilisé pour la fonction CheckToContinue

        return null;
    }

    //// <summary>
    /// La fonctionne assigne la position du joueur comme destination de l'IA
    /// </summary>
    /// <param name="target">La cible de l'IA</param>
    private void followTarget(Transform target)
    {
        _enemy.NavAgent.isStopped = false;
        _enemy.Anim.SetBool("Avancer", true);

        _destination = new Vector3(targetPosition.x, y: 1f, targetPosition.z);

        _direction = Vector3.Normalize(_destination - _enemyPosition);
        _direction = new Vector3(_direction.x, y: 0f, _direction.z);

    }

    //// <summary>
    /// Vérifie si le joueur est caché 
    /// </summary>
    /// <param name="waitTime">Internvelle de temps entre 2 vérifications</param>
    /// <returns>retourne true s'il est caché et false sinon </returns>
    private bool CheckToContinue()
    {
        RaycastHit hit;
        seekCounter = 0;

        if (Physics.Raycast(_enemyPosition, _enemy.Target.position - _enemyPosition, out hit, GameSettings.ChaseRange))
        {
            var target = hit.transform;

            if (target != null && target != _enemy.Target.transform)
            {
                return false;
            }
        }
        
        return true;
    }

}
