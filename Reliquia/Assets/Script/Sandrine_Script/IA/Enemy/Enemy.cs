using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    // Propriétés privées de l'IA
    private Animator anim;
    private NavMeshAgent navAgent;
    private Vector3 initPosition;
    [SerializeField] private float enemyWanderSpeed = 1f;
    [SerializeField] private float enemyChaseSpeed = 2f;
    [SerializeField] private float enemyAttackSpeed = 2f;
    public List<Transform> Path;

    public Transform Target; // { get; private set; }
    public Transform Chaser;// { get; private set; } //private set
    //public bool playerTarget { get; private set; } //private set

    // Les getters des propriétés de l'IA
    public StateMachine StateMachine => GetComponent<StateMachine>();
    public Vector3 InitPosition => initPosition;
    public float EnemyWanderSpeed => enemyWanderSpeed;
    public float EnemyChaseSpeed => enemyChaseSpeed;
    public float EnemyAttackSpeed => enemyAttackSpeed;
    public Animator Anim { get { return anim; } }
    public NavMeshAgent NavAgent { get { return navAgent; } }


    private void Awake()
    {
        InitializeStateMachine();
        anim = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        initPosition = transform.position;
    }

    private void InitializeStateMachine()
    {
        var states = new Dictionary<Type, BaseState>()
        {
            {typeof(WanderState), new WanderState(enemy: this, path: Path) },
            {typeof(ReturnState), new ReturnState(enemy: this) },
            {typeof(ChaseState), new ChaseState(enemy: this) },
            {typeof(ChasePlayerState), new ChasePlayerState(enemy: this) },
            {typeof(AttackState), new AttackState(enemy: this) }
        };

        GetComponent<StateMachine>().SetStates(states);
    }

    // Suspendu il n'y a pas de combat au corps à corps => A conserver tant qu'on ne sais pas détaecter la cible du joueur
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) //other.GetComponent<Companion>() != null || 
        {
            // des qu'il entre dans son rayon le joueur devient la cible et le chasseur.
            //// On change sa cible par celle qui l'attaque ? 
            //Animator anim = other.GetComponent<Animator>();
            //if (anim != null && anim.GetCurrentAnimatorStateInfo(0).IsName("Punching"))
            //{
            //    SetTarget(other.transform);
            //    SetChaser(other.transform);
            //    return;
            //}
            //// Uniquement si c'est le joueur, on sette targetedBy ? Pourquoi ?
            //// Le compagnon a déjà renseigné cette variable dans PrepareToAttackState.
            ////if (Target != null && Target == other)
            ////{
            ////    SetTargetedBy(Target.transform);
            ////    return;
            ////}
            SetTarget(other.transform);
            SetChaser(other.transform);

        }
    }

    /// <summary>
    /// fonction qui décrit les actions de l'attaque 
    /// Spécifique à chaque ennemi
    /// Pour l'instant l'IA s'arrête et sa couleur est modifée
    /// </summary>
    internal void LaunchAttack()
    {
        navAgent.isStopped = true;
        anim.SetBool("Avancer", false);
        anim.SetBool("Attaque", true);

    }

    /// <summary>
    /// Assigne la cible à l'IA
    /// </summary>
    /// <param name="target">la cible</param>
    public void SetTarget(Transform target)
    {
        Target = target;
    }

    public void SetChaser(Transform adverser)
    {
        Chaser = adverser;
    }

    public void ResetTargets()
    {
        SetTarget(null);
        SetChaser(null);
    }

    internal void LookAtDirection(Vector3 lookAtPosition, float speed)
    {
        Vector3 relativePos = lookAtPosition; // targetPosition - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * speed);
    }

    internal void Move(Vector3 destination, float speed, string animation = "")
    {

        NavAgent.isStopped = false;
        NavAgent.speed = speed;
        Anim.SetBool("Avancer", true);
        Anim.SetBool("Attaque", false);
        Anim.SetBool("Course", false);

        if (animation != "")
        {
            Anim.SetBool(animation, true);
        }
        NavAgent.SetDestination(destination);
    }

    internal void StopMoving()
    {

        NavAgent.isStopped = true;
        Anim.SetBool("Course", false);
        Anim.SetBool("Avancer", false);
        NavAgent.speed = 0;
    }

    internal void StopAttack()
    {
        anim.SetBool("Attaque", false);
        NavAgent.isStopped = false;


    }

}
