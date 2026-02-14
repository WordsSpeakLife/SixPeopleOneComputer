using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class TurretEnemyAI : MonoBehaviour, IDamage
{

    [Header("---- Componets ----")]
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPOS;

    [Header("---- HP Settings ----")]
    [SerializeField] int HP;

    [Header("---- Vision Settings ----")]
    [SerializeField] int faceTargetSpeed;
    [Range(15, 360)][SerializeField] int FOV;

    [Header("---- Gun Settings ----")]
    [SerializeField] string enemyType;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    [Header("---- Credit Settings ----")]
    [SerializeField] GameObject creditsPickupPrefab;
    [SerializeField] int creditsDropAmount = 10;
    [SerializeField] float dropHeight = 0.5f;


    [Header("---- Fixed Turret ----")]
    [SerializeField] bool isFixed;
    [SerializeField] float sweepSpeed;
    [SerializeField] float degreesToTurn;
    [SerializeField] Transform targetA;
    [SerializeField] Transform targetB;
    bool ignoreFixed;

    Color colorOrig;

    bool isAtStart;

    float step;
    float shootTimer;
    float angleToPlayer;
    float stoppingDistOrig;

    private Transform currentTarget;
    private bool isTargetingA = true;

    Vector3 playerDir;
    Vector3 shootDir;
    Vector3 startingPos;

    public bool playerInTrigger;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTarget = targetA;
        colorOrig = model.material.color;
        if(GameManager.instance.GameType == GameManager.GameGoal.DefeatAllEnemies)
            GameManager.instance.updateGameGoal(1);

    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;
        if (isFixed)
        {
            step = sweepSpeed * Time.deltaTime;
            Quaternion targetRot;
            if (currentTarget)
                targetRot = Quaternion.LookRotation(new Vector3(transform.rotation.x, transform.rotation.y + degreesToTurn, transform.rotation.z));
            else
                targetRot = Quaternion.LookRotation(new Vector3(transform.rotation.x, transform.rotation.y - (degreesToTurn * 2), transform.rotation.z));

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, step);
            if (Quaternion.Angle(transform.rotation, targetRot) <= 0.1f)
            {
                isTargetingA = !isTargetingA;
                currentTarget = isTargetingA ? targetA : targetB;
            }
            if (shootTimer >= shootRate)
            {
                shoot();
            }
        }
        else if (playerInTrigger)
        {
            canSeePlayer();
        }
    }

    bool canSeePlayer()
    {
        playerDir = (GameManager.instance.player.transform.position - headPOS.position);
        shootDir = (GameManager.instance.player.transform.position - shootPos.position);

        angleToPlayer = Vector3.Angle(playerDir, transform.forward);
        Debug.DrawRay(headPOS.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPOS.position, playerDir, out hit))
        {
            if (angleToPlayer <= FOV && hit.collider.CompareTag("Player"))
            {
                faceTarget();

                if (shootTimer >= shootRate)
                {
                    shoot();
                }
                return true;
            }
        }
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
            Instantiate(bullet, shootPos.position, Quaternion.LookRotation(new Vector3(shootDir.x, shootDir.y, shootDir.z)));
            SoundManager.instance.PlaySound3D("shoots", transform.position);
        }
        else if (enemyType == "Burst")
        {
            Instantiate(bullet, shootPos.position, Quaternion.LookRotation(new Vector3(shootDir.x, shootDir.y, shootDir.z)) * Quaternion.Euler(0,15, 0));
            Instantiate(bullet, shootPos.position, Quaternion.LookRotation(new Vector3(shootDir.x, shootDir.y, shootDir.z)));
            Instantiate(bullet, shootPos.position, Quaternion.LookRotation(new Vector3(shootDir.x, shootDir.y, shootDir.z)) * Quaternion.Euler(0,-15, 0));
            SoundManager.instance.PlaySound3D("shoots", transform.position);
        }
        if (enemyType == "Fixed")
        {
            Instantiate(bullet, shootPos.position, transform.rotation);
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
