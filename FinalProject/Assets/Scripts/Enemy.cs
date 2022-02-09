using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    bool isDead = false;
    bool currentlyInHitAnimation = false;
    int counter = 0;
    bool canHit = true;
    bool targetingPlayer = false;
    bool doOnce = true;
    bool doOnce2 = true;
    bool doOnce3 = true;
    bool cantReach = false;
    bool playerHitted = false;
    NavMeshAgent agent;
    GameObject Target;
    GameObject TargetPlayer;
    public int health;
    public int enemyDamage;
    float attackRate;
    public float attacRateCustomize;
    public int killingPrize;
    public Animator animator;
    public AudioSource[] audioSources;
    Generator generatorScript;
    GameObject gameControllerObj;
    GameControl gameController;

    NavMeshPathStatus status = new NavMeshPathStatus();

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        gameControllerObj = GameObject.FindWithTag("GameControl");

        gameController = gameControllerObj.GetComponent<GameControl>();

        StartCoroutine(GiveDamageCoolDown());

        animator.SetTrigger("StartRun");

        audioSources[1].Play();
    }

    public void SetTargetPoint(GameObject targetNormal, GameObject targetPlayer, bool isTargetPlayer)
    {
        if (isTargetPlayer)
            targetingPlayer = true;
   
        TargetPlayer = targetPlayer;
        Target = targetNormal;

        generatorScript = Target.GetComponent<Generator>();
    }

    void Update()
    {

        if (doOnce && targetingPlayer)
        {
            agent.SetDestination(TargetPlayer.transform.position);
            doOnce = false;
        }

        if (!audioSources[0].isPlaying && !audioSources[1].isPlaying)
            audioSources[1].Play();

        if (targetingPlayer)
        {
            status = agent.pathStatus;

            if (status == NavMeshPathStatus.PathPartial)
                cantReach = true;

            if (cantReach)
            {
                targetingPlayer = false;
                animator.SetTrigger("Idle");
                animator.SetTrigger("StartRun");
            }
            else if(currentlyInHitAnimation)
            {
                agent.velocity = Vector3.zero;
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(TargetPlayer.transform.position);
            }

            if(status != NavMeshPathStatus.PathPartial && playerHitted)
            {                
                if(agent.remainingDistance > 3f)
                {
                    animator.SetTrigger("Idle");
                    animator.SetTrigger("StartRun");
                }
            }
        }
        else
        {
            if (currentlyInHitAnimation)
            {
                agent.velocity = Vector3.zero;
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
                generatorScript = Target.GetComponent<Generator>();
                agent.SetDestination(Target.transform.position);
            }    
        }

        if (generatorScript.genoratorAlive == false)
        {
            gameController.DynamicTargetSet(gameObject);
        }

        if (isDead)
        {
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Genorator"))
        {
            if (!audioSources[0].isPlaying)
            {
                audioSources[1].Pause();
                audioSources[0].Play();
            }

            animator.SetTrigger("StopRun");
            animator.SetTrigger("StartKicking");
            
            Generator genorator = other.gameObject.GetComponent<Generator>();

            if(!isDead)
                genorator.GetDamage(enemyDamage);
         
        }

        if (other.gameObject.CompareTag("Player"))
        {
            if (!audioSources[0].isPlaying)
            {
                audioSources[1].Pause();
                audioSources[0].Play();
                FindObjectOfType<AudioManager>().Play("Knife HitSurface2");
            }
            

            animator.SetTrigger("StopRun");
            animator.SetTrigger("StartPunching");
            playerHitted = true;

            if (doOnce2 && !isDead)
            {
                gameController.GetDamageAsPlayer(enemyDamage);
                
                doOnce2 = false;
            }

            if (canHit && !isDead)
            {
                gameController.GetDamageAsPlayer(enemyDamage);
            }
            
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Genorator"))
        {
            if (!audioSources[0].isPlaying)
            {
                audioSources[1].Pause();
                audioSources[0].Play();
            }

            Generator genorator = other.gameObject.GetComponent<Generator>();
            animator.SetTrigger("StartAttack");
            if (Time.time > attackRate && !isDead)
            {
                GiveDamageToGenorator(genorator);
                attackRate = Time.time + attacRateCustomize;
            }  
        }
        if (other.gameObject.CompareTag("Player"))
        {
            if (!audioSources[0].isPlaying)
            {
                audioSources[1].Pause();
                audioSources[0].Play();
                FindObjectOfType<AudioManager>().Play("Knife HitSurface2");
            }

            animator.SetTrigger("StartPunching");
            if (Time.time > attackRate && !isDead)
            {
                gameController.GetDamageAsPlayer(enemyDamage);
                attackRate = Time.time + attacRateCustomize;
            }
        }
    }

    public void GiveDamageToGenorator(Generator genorator)
    {
        genorator.GetDamage(enemyDamage);
    }

    public void GetDamage(int damage)
    {
        health -= damage;

        if (!currentlyInHitAnimation && !isDead)
        {
            StartCoroutine(GetHitAnimation());
        }

        if (health <= 0 && doOnce3)
        {
            doOnce3 = false;
            Died();
        }
    }

    IEnumerator GiveDamageCoolDown()
    {
        while (true)
        {
            canHit = false;
            yield return new WaitForSeconds(1.5f);
            canHit = true;
            yield return new WaitForSeconds(0.1f);   
        }    
    }

    IEnumerator GetHitAnimation()
    {
        currentlyInHitAnimation = true;
        if(counter % 2 == 0)
        {
            animator.Play("Zombie Reaction Hit");
            counter++;
            yield return new WaitForSeconds(2.2f);
        }
        else
        {
            animator.Play("Zombie Reaction Hit (1)");
            counter++;
            yield return new WaitForSeconds(2f);
        }
        currentlyInHitAnimation = false;
        animator.SetTrigger("StartRun");
    }

    void Died()
    {
        isDead = true;
        audioSources[0].Pause();
        audioSources[1].Pause();
        audioSources[2].Play();
        gameController.zombieLeft--;
        gameController.playerMoney += killingPrize;
        gameController.killCount++;
        gameController.playerKillText.text = gameController.killCount + " Kills";
        animator.Play("Zombie Dying");
        Destroy(gameObject, 2.7f);       
    }

}
