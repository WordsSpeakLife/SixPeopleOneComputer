using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{

    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPOS;
    [SerializeField] string enemyType;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [Range(15, 360)][SerializeField] int FOV;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    [SerializeField] GameObject creditsPickupPrefab;
    [SerializeField] int creditsDropAmount = 10;
    [SerializeField] float dropHeight = 0.5f;


    Color colorOrig;

    float shootTimer;
    float angleToPlayer;
    float stoppingDistOrig;

    Vector3 playerDir;
    Vector3 startingPos;

    public bool playerInTrigger;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
        if(GameManager.instance.GameType == GameManager.GameGoal.DefeatAllEnemies)
            GameManager.instance.updateGameGoal(1);

    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;

        if (playerInTrigger)
        {
            canSeePlayer();
        }
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
                agent.SetDestination(GameManager.instance.player.transform.position);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();
                }

                if (shootTimer >= shootRate)
                {
                    shoot();
                }

                agent.stoppingDistance = stoppingDistOrig;
                return true;
            }
        }

        agent.stoppingDistance = 0;
        return false;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x,transform.position.y,playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    void shoot()
    {
        shootTimer = 0;
        if (enemyType == "Basic")
        {
            Instantiate(bullet, shootPos.position, Quaternion.LookRotation(new Vector3(playerDir.x, playerDir.y, playerDir.z)));
            SoundManager.instance.PlaySound3D("shoots", transform.position);
        }
        else if (enemyType == "Burst")
        {
            Instantiate(bullet, shootPos.position, Quaternion.LookRotation(new Vector3(playerDir.x, playerDir.y, playerDir.z)) * Quaternion.Euler(0,15, 0));
            Instantiate(bullet, shootPos.position, Quaternion.LookRotation(new Vector3(playerDir.x, playerDir.y, playerDir.z)));
            Instantiate(bullet, shootPos.position, Quaternion.LookRotation(new Vector3(playerDir.x, playerDir.y, playerDir.z)) * Quaternion.Euler(0,-15, 0));
            SoundManager.instance.PlaySound3D("shoots", transform.position);
        }
    }
    public void takeDamage(int amount)
    {
        HP -= amount;

        if (HP <= 0)
        {
            if (GameManager.instance.GameType == GameManager.GameGoal.DefeatAllEnemies)
                GameManager.instance.updateGameGoal(-1);
            DropCredits();
            Destroy(gameObject);

            SoundManager.instance.PlaySound3D("enemies", transform.position);
        }
        else
        {
            StartCoroutine(flashRed());
        }
    }

    void DropCredits()
    {

        Vector3 spawnPos = transform.position + Vector3.up * dropHeight;

        GameObject drop = Instantiate(creditsPickupPrefab, spawnPos, Quaternion.identity);

        PickupCredits pikup = drop.GetComponent<PickupCredits>();
        if (pikup != null)
            pikup.SetAmount(creditsDropAmount);
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    public bool heal(int amount) { return false; }
}
