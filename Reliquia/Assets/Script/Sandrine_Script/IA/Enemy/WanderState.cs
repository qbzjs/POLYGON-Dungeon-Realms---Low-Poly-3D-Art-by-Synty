using System;
using System.Collections.Generic;
using UnityEngine;

public class WanderState : BaseState
{
    // Enemy propriétés
    public Enemy _enemy;
    public List<Transform> _path;

    private int _currentPathPoint = 0;
    private Vector3 _enemyPosition;
    private Vector3 _enemyLastPosition;

    private Vector3 _destination;
    private Vector3 _direction;
    private Quaternion _desiredRotation;

    private Quaternion startingAngle = Quaternion.AngleAxis(-90, Vector3.up);
    private Quaternion stepAngle = Quaternion.AngleAxis(5, Vector3.up);

    // Player propriétés
    private Transform playerTarget;
    private Vector3 playerPosition;
    private Transform chaseTarget;

    public WanderState(Enemy enemy, List<Transform> path) : base(enemy.gameObject)
    {
        _enemy = enemy;
        _path = path;
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;       
    }

    public override Type Tick()
    {

        // assigne les positions 
        _enemyPosition = _enemy.transform.position;
        _enemyLastPosition = _enemyPosition;
        playerPosition = playerTarget.transform.position;


        // Deplacer plus bas
        //_enemy.Anim.SetBool("Avancer", true);
        //_enemy.Anim.SetBool("Course", false);
        //_enemy.NavAgent.isStopped = false;
        //_enemy.NavAgent.speed = _enemy.EnemyWanderSpeed;

        // Détecte le joueur si la cible est nulle
        if (_enemy.Target == null)
        {
            chaseTarget = CheckForAggro(); 
        }

        if (chaseTarget != null)
        {
            // Une fois le joueur détecté, trouver la cible la plus proche
            // Non pas pour l'instant.
            //var chaseTargetClosest = CheckForClosest();

            //if (chaseTargetClosest != null)
            //{
            //    chaseTarget = chaseTargetClosest;
            //}
            _enemy.SetTarget((Transform)chaseTarget);
            return typeof(ChaseState); // Change l'état pour "Chase" 
        }
        if(_enemy.ShouldMove)
        {
            _enemy.NavAgent.isStopped = false;
            if (_enemy.NavAgent.remainingDistance <= 0.5f) // l'agent a atteint sa destination
            {
                FindRandomDestination(); // assigne une nouvelle destination et rotation          

                _enemy.LookAtDirection(_direction, 0.5f);

                _enemy.Move(_destination, _enemy.EnemyWanderSpeed);

            }
        }

        float distance = Vector3.Distance(_enemyPosition, playerPosition);
        bool playerInAttackZone = distance < GameSettings.AttackRange; //Zone d'attaque

        if (playerInAttackZone)
        {
            var checkAttackState = CheckAttackState(); // Regarde si le joueur s'est caché

            if (checkAttackState != null) // Le joueur n'est pas caché
            {
                _enemy.SetTarget((Transform)checkAttackState);
                return typeof(AttackState); //AttackState
            }
        }
        /*
        // Ano sur l'Enemy bloqué à modifier
        _enemyPosition = _enemy.transform.position;
        if (_enemyLastPosition == _enemyPosition)
        {
            FindRandomDestination(); // assigne une nouvelle destination et rotation          

            _enemy.LookAtDirection(_direction, 0.5f);

            _enemy.Move(_destination, _enemy.EnemyWanderSpeed);
        }*/
        return null;
    }

    private Transform CheckForClosest()
    {
        RaycastHit hit;
        var angle = transform.rotation * startingAngle;
        var direction = angle * Vector3.forward;

        for (var i = 0; i < 35; i++)
        {
            if (Physics.Raycast(_enemyPosition, direction, out hit, GameSettings.AggroRadius))
            {

                var target = hit.transform;
                if (target != null && null != target.GetComponent<Companion>())
                {

                    Debug.DrawRay(_enemyPosition, direction * hit.distance, Color.red);
                    return target.transform;
                }
                if (target != null && target == playerTarget)
                {
                    Debug.DrawRay(_enemyPosition, direction * hit.distance, Color.red);
                    return target.transform;
                }
            }

            direction = stepAngle * direction;
        }
        return null; 
    }

    /*****
     * La fonction retourne true si le joeur est caché
     * sinon false
     ******/
    private Transform CheckAttackState()
    {
        RaycastHit hit;
        var target = transform;

        if (Physics.Raycast(_enemyPosition, playerPosition - _enemyPosition, out hit, GameSettings.AttackRange))
        {
            target = hit.transform;
            Debug.DrawRay(_enemyPosition, (playerPosition - _enemyPosition) * hit.distance, Color.green); // TO SUP
        }
        else
        {
            Debug.DrawRay(_enemyPosition, (playerPosition - _enemyPosition) * hit.distance, Color.blue); // TO SUP
        }

        if (target != null && target == playerTarget)
        {
            _enemy.SetTarget((Transform)playerTarget);
            _enemy.SetChaser((Transform)playerTarget);
            return playerTarget;

        }
        return null;
    }

    /****
     * La fonction détecte le joueur dans une rayon de (AggroRadius = 15f) et un angle de 180°
     * Elle retourne le joeur transform s'il est détecté, sinon null
    *****/
    private Transform CheckForAggro()
    {
        RaycastHit hit;
        Quaternion angle = transform.rotation * startingAngle;
        Vector3 direction = angle * Vector3.forward;
        Vector3 rayOrigine = _enemy.Eyes.position;

        for (var i = 0; i < 35; i++)
        {
            if (Physics.Raycast(rayOrigine, direction, out hit, GameSettings.AggroRadius))
            {

                var target = hit.transform;
                if (target != null && target == playerTarget) //  enemy.Team != gameObject.GetComponent<Enemy>().Team
                {
                    Debug.DrawRay(rayOrigine, direction * hit.distance, Color.red);
                    return playerTarget.transform;
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
        return null;

    }
    /*****
    * Assigne une destination aléatoire à de + ou - 4.5 f sur l'axe x et z.
    * Si besoin, on peut changer ici la destination aléatoire pour les "Walk Points" de la scène.
    *****/
    private void FindRandomDestination()
    {
        // Gestion de la patrouille
        if (_path.Count > 0)
        {
            _destination = _path[_currentPathPoint].position;
            _currentPathPoint = (_currentPathPoint >= _path.Count - 1) ? 0 : _currentPathPoint + 1;
        }
        // Gestion du déplacement aléatoire si aucune patrouille renseignée
        else
        {
            Vector3 testPosition = (_enemyPosition + (transform.forward * 4f))
                    + new Vector3(x: UnityEngine.Random.Range(-4.5f, 4.5f), y: 0f, z: UnityEngine.Random.Range(-4.5f, 4.5f));

            _destination = new Vector3(testPosition.x, y: 1f, testPosition.z);
        }

        _direction = Vector3.Normalize(_destination - _enemyPosition);
        _direction = new Vector3(_direction.x, y: 0f, _direction.z);
    }

}
