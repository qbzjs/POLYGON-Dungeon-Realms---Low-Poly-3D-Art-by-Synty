using UnityEngine;
using UnityEngine.AI;

public enum EnemyControlState
{
    WALK,
    RUN,
    PAUSE,
    FOLLOW,
    GOBACK,
    ATTACK,
    DEATH
}

public class EnemyControl: MonoBehaviour
{
    // Enemy properties
    private bool finishedMovement = true;
    private bool finishedPause = false;
    private bool StartPause = false;
    private Animator anim;
    private NavMeshAgent navAgent;
    private Vector3 initialPosition;
    private Vector3 nextDestination;
    private float speed = 1f;
    private float speedAttack = 3f;
    [HideInInspector]
    public EnemyControlState currentState, lastState = EnemyControlState.WALK;

    // Walk Points
    public Transform[] walkPoints;
    private int walk_Index = 0;

    // Player Properties
    private Transform playerTarget;
    private CharacterController playerCaract; // A enlever 

    // Distance between player and enemy
    private float followDistance = 10f;
    private float attackDistance = 3f;
    private float goBackDistance = 20f;

    private float enemyToPlayerDistance;
    private float enemyToInitDistance;

    // Time
    private float waitPauseTime = 0f;
    private float waitTimeLimit = 1.5f;
    private float timeKeepFollowLimit = 100f;
    private float timeKeepFollow = 101f;

    // Provisoire pour effet Attack
    public GameObject alphaSurface;
    private Renderer alphaRenderer;


    void Awake()
    {
        anim = GetComponent<Animator>();
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        navAgent = GetComponent<NavMeshAgent>();
        initialPosition = transform.position;
        playerCaract = playerTarget.GetComponent<CharacterController>();

        alphaRenderer = alphaSurface.GetComponent<Renderer>(); // Provisoire Attack Effect

    }

    void Update()
    {
        // Set Distances
        enemyToPlayerDistance = Vector3.Distance(transform.position, playerTarget.position);
        enemyToInitDistance = Vector3.Distance(transform.position, initialPosition);
        
        GetStateControl(SetEnemyState());
        //Debug.Log("End current State : " + currentState);
    }

    private EnemyControlState SetEnemyState()
    {
        lastState = currentState;

        if (enemyToPlayerDistance < attackDistance) //&& lastState != EnemyControlState.GOBACK
        {
            if (isPlayerHide()) // Do not Follow Player continue walking
            {
                currentState = EnemyControlState.WALK;
                return currentState;
            }
            currentState = EnemyControlState.ATTACK; // Attack
            return currentState;
        }
        if ( enemyToInitDistance > goBackDistance || lastState == EnemyControlState.GOBACK)
        {
            if (lastState == EnemyControlState.FOLLOW)
            {
                StartPause = false;
            }
            if (finishedPause)
            {
                currentState = EnemyControlState.GOBACK; // Puis Go Back Home
                return currentState;
            }

            if (!StartPause)
            {
                currentState = EnemyControlState.PAUSE; // Pause 
                return currentState;
            }
            currentState = EnemyControlState.GOBACK; // Puis Go Back Home
            return currentState;

        } 
        
        if (enemyToPlayerDistance < followDistance && lastState != EnemyControlState.GOBACK) // Zone bleue "détection"
        {

            // if isPlayerHide && lastState == FOLLOW && timeKeepFollow  > timeKeepFollowLimit
            // => timeKeepFollow = 0
            //currentstate = FOLLOW
            if (isPlayerHide() && lastState == EnemyControlState.FOLLOW && timeKeepFollow > timeKeepFollowLimit)
            {
                timeKeepFollow = 0;
                currentState = EnemyControlState.FOLLOW;
                return currentState;
            }
            // if isPlayerHide && lastState == FOLLOW && timeKeepFollow < timeKeepFollowLimit 
            // => 
            // currentState == FOLLOW 
            // timeKeepFollow++;
            if (isPlayerHide() && lastState == EnemyControlState.FOLLOW && timeKeepFollow < timeKeepFollowLimit)
            {
                timeKeepFollow++;
                currentState = EnemyControlState.FOLLOW;
                return currentState;
            }
            // if isPlayerHide && lastState == FOLLOW && timeKeepFollow = timeKeepFollowLimit 
            // => 
            // currentState == WALK 
            // timeKeepFollow++;
            if (isPlayerHide() && lastState == EnemyControlState.FOLLOW && timeKeepFollow == timeKeepFollowLimit)
            {
                timeKeepFollow++;
                currentState = EnemyControlState.WALK;
                return currentState;
            }
            if (isPlayerHide() ) // Do not Follow Player continue walking
            {
                currentState = EnemyControlState.WALK;
                return currentState;
            }

            if (finishedPause) // End Pause so Follow Player
            {
                currentState = EnemyControlState.FOLLOW;
                return currentState;
            }
            if (!StartPause)  // Then start Pause
            {
                currentState = EnemyControlState.PAUSE;
                return currentState;
            }
            currentState = EnemyControlState.WALK;
            if (lastState == EnemyControlState.FOLLOW)
            {
                navAgent.isStopped = true;
            }
            return currentState;

        }

        currentState = EnemyControlState.WALK;
        waitPauseTime = 0;
        finishedPause = false;
        StartPause = false;
        return currentState;

    }

    private bool isPlayerHide()
    {
        RaycastHit hit;
        Vector3 rayStart = transform.position + Vector3.up * 1.5f;
        Vector3 rayDirection = new Vector3(playerTarget.transform.position.x - transform.position.x, 0f, playerTarget.transform.position.z - transform.position.z) ;

        if (Physics.Raycast(rayStart, rayDirection, out hit, followDistance))
        {
            //Debug.DrawRay(rayStart, playerTarget.transform.TransformDirection(Vector3.left) * hit.distance, Color.yellow);
            Debug.DrawRay(rayStart, rayDirection * 8, Color.yellow);
            // if hit.transform == playerTarget => pause puis attack
            // else not follow player
            if (hit.transform != playerTarget)
            {
                return true; // player hidden
            }
            // Player can hide only if enemy walk or follow or pause first only if follow
        }
        else
        {
            Debug.DrawRay(rayStart, rayDirection * 8, Color.white);

        }
        return false;
    }

    void GetStateControl(EnemyControlState enemyState)
    {
        switch (enemyState)
        {
            case EnemyControlState.WALK:
                alphaRenderer.material.SetColor("_ColorTint", Color.white); // Provisoire 
                anim.SetBool("Avancer", true);
                move();
                break;
            case EnemyControlState.RUN:
                break;
            case EnemyControlState.PAUSE:
                if (finishedPause) // New Pause, reset timer
                {
                    waitPauseTime = 0f;
                    finishedPause = false;
                    StartPause = true;
                }
                waitPauseTime += Time.deltaTime;
                anim.SetBool("Avancer", false);
                navAgent.isStopped = true;

                if (waitPauseTime > waitTimeLimit) // End Pause 
                {
                    finishedPause = true;
                }
                break;
            case EnemyControlState.FOLLOW:
                alphaRenderer.material.SetColor("_ColorTint", Color.white); // Provisoire 
                anim.SetBool("Avancer", true);
                followPlayer();
                break;
            case EnemyControlState.GOBACK:
                alphaRenderer.material.SetColor("_ColorTint", Color.white); // Provisoire 
                anim.SetBool("Avancer", true);
                goBackHome();
                break;
            case EnemyControlState.ATTACK:
                alphaRenderer.material.SetColor("_ColorTint", Color.red); // Provisoire
                anim.SetBool("Avancer", false);
                attackPlayer();
                navAgent.isStopped = true;
                break;
            case EnemyControlState.DEATH:
                break;
            default:
                break;
        }

    }

    void move()
    {
        float distance = Vector3.Distance(transform.position, playerTarget.position);

        navAgent.isStopped = false ;
        //Debug.Log("nextDestination : " + nextDestination);
        //Debug.Log("navAgent.remainingDistance : " + navAgent.remainingDistance);
        //Debug.Log("playerTarget.position : " + playerTarget.position);
        //Debug.Log("transform.position : " + transform.position);
        //Debug.Log("walk_Index : " + walk_Index);
        //Debug.Log("walkPoints[walk_Index].position : " + walkPoints[walk_Index].position);

        //bool needNewDestination = false;

        //if (nextDestination.x != walkPoints[walk_Index].position.x || nextDestination.z != walkPoints[walk_Index].position.z)
        //{
        //    needNewDestination = true;
        //}


        if (navAgent.remainingDistance <= 0.5f || lastState == EnemyControlState.FOLLOW)  // set new destination else continue to the destination
        {
            nextDestination = walkPoints[walk_Index].position;
            navAgent.isStopped = false;
            navAgent.speed = speed;
            navAgent.SetDestination(nextDestination);

            if (walk_Index >= walkPoints.Length - 1)
            {
                walk_Index = 0;
            }
            else
            {
                walk_Index++;
            }

        }
    }

    void followPlayer()
    {
        navAgent.isStopped = false;
        navAgent.speed = speedAttack;
        Vector3 relativePos = playerTarget.position - transform.position;

        // the second argument, upwards, defaults to Vector3.up
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime);
        //transform.rotation = rotation;
        navAgent.SetDestination(playerTarget.position);
    }

    void goBackHome()
    {
        if (navAgent.remainingDistance >= 0.1f)
        {
            navAgent.isStopped = false;
            navAgent.speed = speed;
            navAgent.SetDestination(initialPosition);
        } else
        {
            currentState = EnemyControlState.WALK;
        }
    }

    void attackPlayer()
    {
        Vector3 relativePos = playerTarget.position - transform.position;
        navAgent.isStopped = false;
        // the second argument, upwards, defaults to Vector3.up
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime);
    }
}
