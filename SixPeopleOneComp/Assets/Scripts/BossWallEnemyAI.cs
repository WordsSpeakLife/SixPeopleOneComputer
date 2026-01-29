using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossWallEnemyAI : MonoBehaviour, IDamage
{

    [SerializeField] Renderer model;
    [SerializeField] GameObject Face;
    [SerializeField] GameObject EyeLeftPos;
    [SerializeField] GameObject EyeRightPos;
    [SerializeField] Transform[] shootPos;
    [SerializeField] string enemyType;
    [SerializeField] GameObject bullet;

    [SerializeField] int HP;

    [SerializeField] GameObject[] projectiles;
    [SerializeField] float shootRate;
    [SerializeField] float waveRate;
    [SerializeField] float lazerRate;

    Color colorOrig;
    Vector3 playerDirRight;
    Vector3 playerDirLeft;

    bool waveStart;
    bool lazerStart;

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
    }

    public bool heal(int amount) {return false;}

    public void takeDamage(int amount)
    {
        HP -= amount;
        GameManager.instance.BossHealthBar.GetComponent<Slider>().value = HP;

        if(HP <= 200)
        {
            waveStart = true;
        }
        if (HP <= 100)
        {
            lazerStart = true;
        }

        if (HP <= 0)
        {
            if (GameManager.instance.GameType == GameManager.GameGoal.DefeatAllEnemies)
                GameManager.instance.updateGameGoal(-1);

            Destroy(gameObject);
            SoundManager.instance.PlaySound3D("enemies", transform.position);
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

}
