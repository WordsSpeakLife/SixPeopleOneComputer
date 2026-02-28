using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class MeleeEnemyAI : MonoBehaviour, IDamage
{

    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Transform headPOS;
    [SerializeField] string enemyType;
    [SerializeField] GameObject explosionEffect;

    [SerializeField] Animator Anim;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [Range(15, 360)][SerializeField] int FOV;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;


    [SerializeField] GameObject dropItem;
    [SerializeField] float dropHeight = 0.5f;


    Color colorOrig;


    float roamTimer;
    float angleToPlayer;
    float stoppingDistOrig;

    Vector3 playerDir;
    Vector3 startingPos;

    public bool playerInTrigger;
    bool cansee;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;
        if (GameManager.instance.GameType == GameManager.GameGoal.DefeatAllEnemies)
            GameManager.instance.updateGameGoal(1);

    }

    // Update is called once per frame
    void Update()
    {
        if (agent.remainingDistance < 0.01f)
            roamTimer += Time.deltaTime;

        if (playerInTrigger && !canSeePlayer())
        {
            checkRoam();

        }
        else if (!playerInTrigger)
        {
            checkRoam();

        }
    }

    void checkRoam()
    {
        if (agent.remainingDistance < 0.01f && roamTimer >= roamPauseTime)
        {
            roam();
            Anim.SetBool("CanSeePlayer", true);
            Anim.SetLayerWeight(1, 1);
        }
        else if (agent.remainingDistance < 0.01f && roamTimer < roamPauseTime)
        {
            Anim.SetBool("CanSeePlayer", false);
            Anim.SetLayerWeight(1, 0);
        }
    }

    void roam()
    {
        roamTimer = 0;
        agent.stoppingDistance = 0;
        Vector3 ranPos = Random.insideUnitSphere * roamDist;
        ranPos += startingPos;
        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);
        Anim.SetBool("CanSeePlayer", true);
        Anim.SetLayerWeight(1, 1);
    }

    bool canSeePlayer()
    {
        playerDir = (GameManager.instance.player.transform.position - headPOS.position);
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);
        Debug.DrawRay(headPOS.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPOS.position, playerDir, out hit))
        {
            if (angleToPlayer <= FOV && hit.collider.CompareTag("Player"))
            {
                Anim.SetBool("CanSeePlayer", true);
                Anim.SetLayerWeight(1, 1);
                agent.SetDestination(GameManager.instance.player.transform.position);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();

                }

                agent.stoppingDistance = stoppingDistOrig;
                return true;
            }
            else
            {

                Anim.SetBool("CanSeePlayer", false);
                Anim.SetLayerWeight(1, 0);
            }
        }

        agent.stoppingDistance = 0;
        return false;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        GameManager.instance.damageDone += amount;
        SoundManager.instance.PlaySound2D("damage");

        if (HP <= 0)
        {
            if (GameManager.instance.GameType == GameManager.GameGoal.DefeatAllEnemies)
                GameManager.instance.updateGameGoal(-1);
            GameManager.instance.enemysKilled++;

            if (dropItem != null)
            {
                Vector3 spawnPos = transform.position + Vector3.up * dropHeight;

                Instantiate(dropItem, spawnPos, Quaternion.identity);
            }
            SoundManager.instance.PlaySound2D("loose");

            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(flashRed());
        }
    }

    public void die()
    {
        Destroy(gameObject);
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        SoundManager.instance.PlaySound2D("Explosion");
    }

    public void flash()
    {
        StartCoroutine(flashRed());
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    public bool heal(int amount) { return false; }

}
