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
    [SerializeField] Animator Anim;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [Range(15, 360)][SerializeField] int FOV;

    [SerializeField] GameObject bullet;
    [SerializeField] GameObject chargeBall;
    [SerializeField] Color colorChargeOrig;
    [SerializeField] float shootRate;

    [SerializeField] GameObject creditsPickupPrefab;
    [SerializeField] int creditsDropAmount = 10;
    [SerializeField] float dropHeight = 0.5f;


    Color colorOrig;
    bool canSee;

    float shootTimer;
    float angleToPlayer;
    float stoppingDistOrig;

    int faceTargetSpeedOrig;

    Vector3 playerDir;
    Vector3 shootDir;
    Vector3 startingPos;

    public bool playerInTrigger;
    bool firing = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        faceTargetSpeedOrig = faceTargetSpeed;
        colorOrig = model.material.color;
        if (GameManager.instance.GameType == GameManager.GameGoal.DefeatAllEnemies)
            GameManager.instance.updateGameGoal(1);

    }

    // Update is called once per frame
    void Update()
    {

        shootTimer += Time.deltaTime;
        if (playerInTrigger)
        {
            canSeePlayer();
            Anim.SetBool("CanSeePlayer", true);
        }
        else
        {
            Anim.SetBool("CanSeePlayer", false);
            Anim.SetLayerWeight(1, 0);
        }
    }

    bool canSeePlayer()
    {
        playerDir = (GameManager.instance.player.transform.position - headPOS.position);
        shootDir = (GameManager.instance.player.transform.position - shootPos.position);
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);
        Debug.DrawRay(headPOS.position, playerDir + Vector3.up * 1f, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(headPOS.position, playerDir, out hit))
        {
            if (angleToPlayer <= FOV && hit.collider.CompareTag("Player"))
            {
                agent.SetDestination(GameManager.instance.player.transform.position);
                canSee = true;

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();
                }

                if (shootTimer >= shootRate && !firing)
                {
                    shoot();
                }

                agent.stoppingDistance = stoppingDistOrig;
                return true;
            }
            else
            {
                canSee = false;
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

    void shoot()
    {
        shootTimer = 0;
        if (enemyType == "Basic")
        {
            Anim.speed = 1;
            Anim.SetLayerWeight(1, 1);
            Anim.SetTrigger("Shoot");
            Instantiate(bullet, shootPos.position, Quaternion.LookRotation(new Vector3(shootDir.x, shootDir.y, shootDir.z) + Vector3.up * 1f));
            SoundManager.instance.PlaySound3D("shoots", transform.position);
            StartCoroutine(waitToReset());
        }
        else if (enemyType == "Burst")
        {
            Anim.speed = 1;
            Anim.SetLayerWeight(1, 1);
            Anim.SetTrigger("Shoot");
            Instantiate(bullet, shootPos.position, Quaternion.LookRotation(new Vector3(shootDir.x, shootDir.y, shootDir.z) + Vector3.up * 1f) * Quaternion.Euler(0, 15, 0));
            Instantiate(bullet, shootPos.position, Quaternion.LookRotation(new Vector3(shootDir.x, shootDir.y, shootDir.z) + Vector3.up * 1f));
            Instantiate(bullet, shootPos.position, Quaternion.LookRotation(new Vector3(shootDir.x, shootDir.y, shootDir.z) + Vector3.up * 1f) * Quaternion.Euler(0, -15, 0));
            SoundManager.instance.PlaySound3D("shoots", transform.position);
            StartCoroutine(waitToReset());
        }
        else if (enemyType == "Charged")
        {
            StartCoroutine(fireLazer());
            Anim.speed = 0.5f;
            Anim.SetTrigger("Shoot");
            Anim.SetLayerWeight(1, 1);
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

    IEnumerator fireLazer()
    {
        firing = true;
        GameObject charge = Instantiate(chargeBall, shootPos);
        for (int i = 0; i < 4; i++)
        {
            charge.GetComponent<MeshRenderer>().material.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            charge.GetComponent<MeshRenderer>().material.color = colorChargeOrig;
        }
        Destroy(charge);
        faceTargetSpeed = 1;
        GameObject lazer = Instantiate(bullet, shootPos);
        SoundManager.instance.PlaySound3D("shoots", transform.position);
        agent.stoppingDistance = stoppingDistOrig;
        yield return new WaitForSeconds(4f);
        Destroy(lazer);
        faceTargetSpeed = faceTargetSpeedOrig;
        yield return new WaitForSeconds(4f);
        firing = false;
    }

    public bool heal(int amount) { return false; }
    IEnumerator waitToReset()
    {
        yield return new WaitForSeconds(0.5f);
        Anim.SetLayerWeight(1, 0);
    }
}
