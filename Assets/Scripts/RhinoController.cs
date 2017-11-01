using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class RhinoController : MonoBehaviour
{
    //常數宣告
    private string roarTrigger = "roar";
    private string crystalTag = "crystal";
    private string deadTrigger = "dead";
    //衝撞攻擊行為參數
    public float impactChargeTime = 1f;
    public Transform impactTarget;
    public float impactSpeed = 10f;
    public int impactDamage = 50;
    public ParticleSystem explosionEffect;
    public float walkingSpeed = 3f;
    public Slider aimSlider;
    //音效宣告
    public AudioClip roarSound;
    public AudioClip deadSound;
    public AudioClip impactSound;
    //依賴的組件宣告
    private NavMeshAgent navMeshAgent;
    private AudioSource audioSource;
    private Animator animator;
    private Collider mCollider;

    public float hitOffset = 1;
    public Transform target;
    private bool isImpacting;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        mCollider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        var go = GameObject.FindGameObjectWithTag(crystalTag);
        if(go != null)
        {
            target = go.transform;
            StartCoroutine("ProcessState");
        }
    }

    private IEnumerator ProcessState()
    {
        while(target != null)
        {
            navMeshAgent.speed = walkingSpeed;
            var randomRad = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = Vector3.Distance(impactTarget.position, transform.position);
            var randomPos = target.position + new Vector3(Mathf.Cos(randomRad), 0, Mathf.Sin(randomRad)) * distance;
            navMeshAgent.SetDestination(randomPos);
            yield return null;
            while(navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            {
                yield return null;
            }
            if (target == null)
                yield break;
            yield return StartCoroutine("ProcessImpact");
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator ProcessImpact()
    {
        transform.LookAt(target);
        aimSlider.gameObject.SetActive(true);
        aimSlider.value = aimSlider.minValue;
        aimSlider.maxValue = impactChargeTime;
        animator.SetTrigger(roarTrigger);
        while (aimSlider.value < aimSlider.maxValue)
        {
            aimSlider.value += Time.deltaTime;
            yield return null;
        }
        aimSlider.gameObject.SetActive(false);
        navMeshAgent.speed = impactSpeed;
        RaycastHit hit;
        float distance = Vector3.Distance(impactTarget.position, transform.position);
        Vector3 targetPos = impactTarget.position;
        if (Physics.Raycast(transform.position, transform.forward, out hit, distance))
        {
            if(hit.collider.CompareTag(crystalTag))
            {
                targetPos = hit.point;
            }
        }
        navMeshAgent.SetDestination(targetPos - transform.forward * hitOffset);
        yield return null;
        isImpacting = true;
        while (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {
            yield return null;
        }
        isImpacting = false;
    }

    public void TriggerRoarSound()
    {
        audioSource.PlayOneShot(roarSound);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isImpacting)
        {
            PlayHitEffect();
            var attackableObj = other.GetComponent<AttackableBehavior>();
            if (attackableObj != null)
                attackableObj.Hurt(impactDamage);
        }
    }

    private void PlayHitEffect()
    {
        explosionEffect.Play();
        audioSource.PlayOneShot(impactSound);
    }

    public void OnDead()
    {
        StopAllCoroutines();
        aimSlider.gameObject.SetActive(false);
        navMeshAgent.enabled = false;
        audioSource.PlayOneShot(deadSound);
        animator.SetTrigger(deadTrigger);
        mCollider.enabled = false;
    }
}
