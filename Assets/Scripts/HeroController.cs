using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class HeroController : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    public float attackRadius;
    public RectTransform attackRangeRect;
    public LayerMask enemyLayerMask;
    public Color detectColor;
    public Color normalColor;
    public Image attackProbeCircle;
    public AttackableBehavior target;
    public float turnSmooth = 15f;
    public int gunDamage = 10;
    private Animator animator;
    public string attackBool = "attack";
    private AudioSource audioSource;
    public AudioClip fireSound;
    public AudioClip deadSound;
    public ParticleSystem gunShotEffect;
    private RagdollBehavior ragdollBehavior;
    private Collider mCollider;

    private void Awake()
    {
        attackRadius = attackRangeRect.rect.width / 2;
        ragdollBehavior = GetComponent<RagdollBehavior>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        mCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
            return;
        if(navMeshAgent.remainingDistance == 0)
        {
            Vector3 lookPos = target.transform.position - transform.position;
            lookPos.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, turnSmooth * Time.deltaTime);
            animator.SetBool(attackBool, true);
        }
        else
        {
            animator.SetBool(attackBool, false);
        }
    }

    public void Move(Vector3 target)
    {
        if (navMeshAgent.enabled == false)
            return;
        navMeshAgent.SetDestination(target);
        navMeshAgent.isStopped = false;
    }

    public void FixedUpdate()
    {
        Collider[] allCollider = Physics.OverlapSphere(transform.position, attackRadius, enemyLayerMask);
        if (allCollider.Length > 0)
        {
            attackProbeCircle.color = detectColor;
            target = allCollider[0].GetComponent<AttackableBehavior>();
        }
        else
        {
            attackProbeCircle.color = normalColor;
            target = null;
            animator.SetBool(attackBool, false);
        }
    }

    public void OnGunTrigger()
    {
        if(target != null)
            target.Hurt(gunDamage);
        audioSource.PlayOneShot(fireSound);
        gunShotEffect.Play();
    }

    public void OnDead()
    {
        navMeshAgent.enabled = false;
        animator.enabled = false;
        attackProbeCircle.enabled = false;
        mCollider.enabled = false;
        enabled = false;
        ragdollBehavior.ToggleRagdoll(true);
        audioSource.PlayOneShot(deadSound);
    }
}
