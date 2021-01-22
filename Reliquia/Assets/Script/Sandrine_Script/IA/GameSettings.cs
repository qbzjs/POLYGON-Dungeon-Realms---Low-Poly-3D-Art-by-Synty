using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    // Enemy Setting
    //[SerializeField] private float speed = 2f;
    public float speed = 2f; // TOSUP
    //[SerializeField] private float aggroRadius = 20f;
    public float aggroRadius = 20f;
    //[SerializeField] private float attackRange = 3f;
    public float attackRange = 5f;
    //[SerializeField] private float chaseRange = 15f;
    public float chaseRange = 30f;
    //[SerializeField] private float outOfChaseRange = 20f;
    public float outOfChaseRange = 30f;
    //[SerializeField] private float chaseWaitingTime = 4500;
    public float chaseWaitingTime = 4500;


    // Companion Settings
    //[SerializeField] private float distanceToWalk = 4f;
    public float distanceToWalk = 3.5f;

    public float speedWalking = 5f;

    public float speedAttackWalking = 3f;

    public float companionAttackRange = 2f;

    public float followInAttackStateDistance = 2f;


    // Compagnon et Ennemi 
    //[SerializeField] 
    public float speedRunning = 16f; 


    public static GameSettings Instance { get; private set; }

    public static float Speed => Instance.speed;
    public static float AggroRadius => Instance.aggroRadius;
    public static float AttackRange => Instance.attackRange;
    public static float ChaseRange => Instance.chaseRange;
    public static float OutOfChaseRange => Instance.outOfChaseRange;
    public static float ChaseWaintingTime => Instance.chaseWaitingTime;
    public static float DistanceToWalk => Instance.distanceToWalk;
    public static float SpeedRunning => Instance.speedRunning;  // Companion et Enemy to check spped target
    public static float SpeedWalking => Instance.speedWalking;
    public static float SpeedAttackWalking => Instance.speedAttackWalking;
    public static float CompanionAttackRange => Instance.companionAttackRange;
    public static float FollowInAttackStateDistance => Instance.followInAttackStateDistance;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        //AttackRange = Instance.attackRange;
    }
}