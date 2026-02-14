using System.Collections;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class BossWallEnemyAI : MonoBehaviour, IDamage
{

    [SerializeField] GameObject mainObject;
    [SerializeField] Renderer model;
    [SerializeField] GameObject Face;
    [SerializeField] GameObject EyeLeftPos;
    [SerializeField] GameObject EyeRightPos;
    [SerializeField] Transform[] shootPos;
    [SerializeField] Transform[] spawnPos;
    [SerializeField] Transform shieldPos;
    [SerializeField] string enemyType;
    [SerializeField] GameObject bullet;

    [SerializeField] int HP;


    [SerializeField] float moveSpeed;
    [SerializeField] Transform secondPhasePos;
    [SerializeField] Transform thirdPhasePos;

    [SerializeField] PlatformManager FirstPlatforms;
    [SerializeField] PlatformManager SecondPlatforms;

    [SerializeField] GameObject[] projectiles;
    [SerializeField] GameObject shield;
    [SerializeField] float shootRate;
    [SerializeField] float waveRate;
    [SerializeField] float lazerRate;


    Color colorOrig;
    Vector3 playerDirRight;
    Vector3 playerDirLeft;

    float step;

    bool waveStart;
    bool lazerStart;


    bool phaseTwoStart;
    bool phaseThreeStart;

    float shootTimer;
    float waveTimer;
    float lazerTimer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;
        waveTimer += Time.deltaTime;
        lazerTimer += Time.deltaTime;

        playerDirRight = (GameManager.instance.player.transform.position - EyeRightPos.transform.position);
        playerDirLeft = (GameManager.instance.player.transform.position - EyeLeftPos.transform.position);

        if (shootTimer >= shootRate)
        {
            StartCoroutine(shootProjectile());
        }
        if(waveTimer >= waveRate && waveStart)
        {
            StartCoroutine(shootWave());
        }
        if (lazerTimer >= lazerRate && lazerStart)
        {
            StartCoroutine(shootLazer());
        }

        if (phaseTwoStart)
        {
            step = moveSpeed * Time.deltaTime;
            mainObject.transform.position = Vector3.MoveTowards(mainObject.transform.position, secondPhasePos.position, step);
            if (mainObject.transform.position == secondPhasePos.position)
            {
                phaseTwoStart = false;
            }
        }
    }

    public bool heal(int amount) {return false;}

    public void takeDamage(int amount)
    {
        HP -= amount;
        GameManager.instance.BossHealthBar.GetComponent<Slider>().value = HP;

        if (HP <= 0)
        {
            if (GameManager.instance.GameType == GameManager.GameGoal.DefeatAllEnemies)
                GameManager.instance.updateGameGoal(-1);

            Destroy(gameObject);
            SoundManager.instance.PlaySound3D("enemies", transform.position);
        }
        else if (HP <= 150)
        {
            lazerStart = true;
            //StartCoroutine(dropPlats(SecondPlatforms));
        }
        else if(HP <= 250)
        {
            waveStart = true;
            Instantiate(shield, shieldPos);
            phaseTwoStart = true;
            StartCoroutine(dropPlats(FirstPlatforms));
        }
        else
        {
            StartCoroutine(flashRed());
        }
    }


    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    IEnumerator shootProjectile()
    {
        shootTimer = 0;
        yield return new WaitForSeconds(1f);
        Instantiate(projectiles[Random.Range(0,projectiles.Length - 1)], shootPos[Random.Range(0, shootPos.Length - 1)]);
        
    }
    IEnumerator shootWave()
    {
        waveTimer = 0;
        yield return new WaitForSeconds(1f);
        Instantiate(projectiles[2], shootPos[9]);

    }
    IEnumerator shootLazer()
    {
        lazerTimer = 0;
        yield return new WaitForSeconds(1f);
        Instantiate(bullet, EyeRightPos.transform.position, Quaternion.LookRotation(new Vector3(playerDirRight.x, playerDirRight.y, playerDirRight.z)));
        Instantiate(bullet, EyeLeftPos.transform.position, Quaternion.LookRotation(new Vector3(playerDirLeft.x, playerDirLeft.y, playerDirLeft.z)));
    }

    IEnumerator dropPlats(PlatformManager manager)
    {
        yield return new WaitForSeconds(1f);
        manager.startDroppingPlats();
    }

}
