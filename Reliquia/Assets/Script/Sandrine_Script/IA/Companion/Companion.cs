using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Companion : MonoBehaviour
{
    // Propriétés privées de l'IA
    private Animator anim;
    private NavMeshAgent navAgent;
    [SerializeField] private float companionAttackSpeed = 2f;

    public String Name;

    private Transform player;
    private Animator animPlayer;

    public Transform Target { get; private set; }

    public Transform Player => player;
    public Animator Anim => anim;
    public NavMeshAgent NavAgent => navAgent;

    public Animator AnimPlayer => animPlayer;

    public float CompanionAttackSpeed => companionAttackSpeed;


    // A supprimer
    public GameObject alphaSurface;  // Provisoire à sup
    [HideInInspector]
    public Renderer alphaRenderer;  // Provisoire à sup

    private void Awake()
    {
        InitializeStateMachine();
        anim = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        animPlayer = player.GetComponent<Animator>();

        // A supprimer
        alphaRenderer = alphaSurface.GetComponent<Renderer>(); // Provisoire Attack Effect
    }

    internal void SetTarget(Transform target)
    {
        Target = target;
    }

    internal void SetSpeed(float speed)
    {
        NavAgent.speed = speed;
    }

    //internal void SetDestination(Vector3 targetPosition)
    //{
    //    navAgent.SetDestination(targetPosition);
    //}

    private void InitializeStateMachine()
    {
        var states = new Dictionary<Type, BaseState>()
        {
            {typeof(ReachPlayerState), new ReachPlayerState(companion: this) },
            {typeof(WalkState), new WalkState(companion: this) },
            {typeof(WaitState), new WaitState(companion: this) },
            {typeof(PrepareToAttackState), new PrepareToAttackState(companion: this) },
            {typeof(CompagnonAttackState), new CompagnonAttackState(companion: this) }
        };

        GetComponent<StateMachine>().SetStates(states);
    }

    internal void Attack(float speed)
    {
        NavAgent.isStopped = true;
        Anim.SetBool("Avancer", false);
        //Anim.SetBool("Attaque", true);
        NavAgent.speed = speed;
    }

    internal void LookAt(Vector3 lookAtPosition, float speed)
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

       Anim.SetBool(animation, true);
       NavAgent.SetDestination(destination);
    }

    // ToDo 
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.GetComponent<Enemy>() != null)
    //    {
    //        SetTarget(other.transform);
    //    }
    //}

    internal void StopMoving()
    {
        Anim.SetBool("Course", false);
        Anim.SetBool("Avancer", false);
        NavAgent.speed = 0;
    }

    internal bool NotAttacking()
    {
        if ( ! anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Puching"))
        {
            return false;
        }
        return true;
    }

    internal bool ReachDestination()
    {
        if (navAgent.remainingDistance <= 0.5f)
        {
            return true;
        }
        return false;
        
    }

    internal void LaunchAttack()
    {
        //navAgent.isStopped = true;
        anim.SetBool("Attaque", true);
    }

    internal void StopAttack()
    {
        anim.SetBool("Attaque", false);
    }
}