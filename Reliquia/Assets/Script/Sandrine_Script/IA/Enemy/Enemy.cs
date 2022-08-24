using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour, IFighter
{
    // Propriétés privées de l'IA
    private Animator anim;
    private NavMeshAgent navAgent;
    private Vector3 initPosition;
    private bool alive = true;

    public Transform Eyes;
    [SerializeField] private int enemyHealth = 100;
    [SerializeField] private int enemyStrength = 10;
    [SerializeField] private float enemyWanderSpeed = 1f;
    [SerializeField] private float enemyChaseSpeed = 2f;
    [SerializeField] private float enemyAttackSpeed = 2f;
    public List<Transform> Path;

    public Transform Target; // { get; private set; }
    public Transform Chaser;// { get; private set; } //private set
    //public bool playerTarget { get; private set; } //private set

    private AIPouvoirPraesidium Praesidium;
    private int HitCount = 0;
    private AIPouvoirLighting Lighting;
    public AIPouvoirPulsate Pulsate;
    [NonSerialized] public bool hasPulsate;

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

        Praesidium = GetComponent<AIPouvoirPraesidium>();
        Lighting = GetComponent<AIPouvoirLighting>();
        hasPulsate = Pulsate != null;
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
        if(alive)
        {
            navAgent.isStopped = true;
            anim.SetBool("Avancer", false);
            anim.SetBool("Attaque", true);
        }
    }

    internal void Punch()
    {
        if (alive)
        {
            // Pour permettre la recharge du pulsate
            if(hasPulsate) Pulsate.Actif = true;

            Debug.Log("punch");
            anim.SetBool("Distance", false);
            Lighting.Stop();
        }
    }

    internal bool UsePulsate()
    {
        if (hasPulsate && alive && Pulsate.CanUse())
        {
            Debug.Log("pulsate");
            anim.SetBool("Distance", false);
            Anim.SetTrigger("Pulsate");
            Lighting.Stop();
            Pulsate.Use();

            return true;
        }
        return false;
    }

    public void UseLighting()
    {
        if (alive)
        {
            // Pour empêcher la recharge du pulsate
            if (hasPulsate) Pulsate.Actif = false;

            Debug.Log("lighting");
            anim.SetBool("Distance", true);
            Lighting.TargetPos = Vector3.Normalize(Target.position - transform.position);
            Lighting.Use();
        }
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
        if(alive)
        {
            Vector3 relativePos = lookAtPosition; // targetPosition - transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * speed);
        }
    }

    internal void Move(Vector3 destination, float speed, string animation = "")
    {
        if(alive)
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
        Lighting.Stop();
    }

    public int GetStrength()
    {
        return enemyStrength;
    }

    public void Hurt(int damage)
    {
        if(alive && !Praesidium.Actif)
        {
            enemyHealth -= damage;

            if (enemyHealth > 0)
            {
                // Active l'une des deux animations de blessure
                Anim.SetTrigger("Blesser");
                if (UnityEngine.Random.value >= 0.5) Anim.SetTrigger("Gauche");

                // vérification si utilisation de Praesidium
                HitCount++;
                if(HitCount > 2)
                {
                    Praesidium.Use();
                    HitCount = 0;
                }
            }
            else Die();
        }
    }

    public void Die()
    {
        alive = false;
        StopAttack();
        StopMoving();
        Lighting.Stop();

        Anim.SetTrigger("Mort");
        StartCoroutine(FadeBody(gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(), 4f));
    }

    private void ChangeRenderMode(Material material)
    {
        material.SetFloat("_Mode", 2);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
    }

    /// <summary>
    /// Fait disparaitre le mesh en un temps donné
    /// </summary>
    /// <param name="meshes">Tous les SkinnedMeshRenderer du mesh</param>
    /// <param name="duration">Temps (en secondes)</param>
    /// <returns></returns>

    private IEnumerator FadeBody(SkinnedMeshRenderer[] meshes, float duration)
    {
        float time = 0f;

        // Attends que l'animation se termine
        yield return new WaitForSeconds(Anim.GetCurrentAnimatorStateInfo(0).length + Anim.GetCurrentAnimatorStateInfo(0).normalizedTime);

        // Change le mode des materials du mesh en Fade
        foreach (SkinnedMeshRenderer mesh in meshes)
            ChangeRenderMode(mesh.material);

        // Fade le mesh
        while (time < duration)
        {
            float alpha = Mathf.Lerp(1, 0, time / duration);
            foreach (SkinnedMeshRenderer mesh in meshes)
            {
                Color meshColor = mesh.material.color;
                mesh.material.color = new Color(meshColor.r, meshColor.g, meshColor.b, alpha);
            }
            time += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    public void PulsateAnimEvent()
    {
        Pulsate.PulsateAnimEvent();
    }
}
