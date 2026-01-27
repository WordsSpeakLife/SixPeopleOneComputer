using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class MeleeEnemyAI : MonoBehaviour, IDamage
{

    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Transform headPOS;
    [SerializeField] string enemyType;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [Range(15, 360)][SerializeField] int FOV;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;


    [SerializeField] GameObject dropItem;



    Color colorOrig;

    float roamTimer;
    float angleToPlayer;
    float stoppingDistOrig;

    Vector3 playerDir;
    Vector3 startingPos;

    bool playerInTrigger;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
        if (GameManager.instance.GameType == GameManager.GameGoal.DefeatAllEnemies)
            GameManager.instance.updateGameGoal(1);

    }

    // Update is called once per frame
    void Update()
    {
        //shootTimer += Time.deltaTime;

        playerDir = (GameManager.instance.player.transform.position - transform.position);

        agent.SetDestination(GameManager.instance.player.transform.position);

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            faceTarget();
        }

    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    public void takeDamage(int amount)
    {
        HP -= amount;

        if (HP <= 0)
        {
            if (GameManager.instance.GameType == GameManager.GameGoal.DefeatAllEnemies)
                GameManager.instance.updateGameGoal(-1);

            if (dropItem != null)
                Instantiate(dropItem, transform.position, transform.rotation);

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
