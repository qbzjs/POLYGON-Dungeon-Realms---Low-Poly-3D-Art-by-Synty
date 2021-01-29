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

    public Transform Target; // { get; private set; }
    public BaseState LastState { get; private set; }

    public float AttackReadyTimer { get; private set; }

    public int AttackNumber { get; private set; }

    public Transform Player => player;
    public Animator Anim => anim;
    public NavMeshAgent NavAgent => navAgent;

    public Animator AnimPlayer => animPlayer;

    public float CompanionAttackSpeed => companionAttackSpeed;


    // A supprimer
    public GameObject alphaSurface;  // Provisoire à sup
    [HideInInspector]
    public Renderer alphaRenderer;  // Provisoire à sup

    public bool flagAttack; // Provsoire To Sup

    private void Awake()
    {
        InitializeStateMachine();
        anim = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        animPlayer = player.GetComponent<Animator>();

        // A supprimer
        alphaRenderer = alphaSurface.GetComponent<Renderer>(); // Provisoire Attack Effect
        SetAttackReadyTimer();
        AttackNumber = 500; // 30;
    }

    public void DecreaseAttackNumber()
    {
        AttackNumber--;
    }
    public void SetTarget(Transform target)
    {

        Target = target;
    }

    internal void SetAttackReadyTimer()
    {
        AttackReadyTimer = Name == "Roxane" ? 10f : 15f;
    }

    internal void SetLastState(BaseState state)
    {
        LastState = state;
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
            {typeof(CompanionAttackState), new CompanionAttackState(companion: this) },
            {typeof(LostState), new LostState(companion: this) }
        };

        GetComponent<StateMachine>().SetStates(states);
    }

    internal void LaunchAttack(float speed)
    {
        //Debug.Log("Companion launch attack");
        NavAgent.isStopped = true;
        Anim.SetBool("Avancer", false);
        Anim.SetBool("Attaque", true);
        NavAgent.speed = speed;
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
        if (animation == "Reculer")
        {
            Anim.SetBool("Avancer", false);
        }
        
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
        NavAgent.isStopped = false;
        Anim.SetBool("Course", false);
        Anim.SetBool("Avancer", false);
        Anim.SetBool("Reculer", false);
        NavAgent.speed = 0;
    }

    internal bool IsAttacking()
    {
        if ( !anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Puching"))
        {
            return true;
        }
        return false;
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