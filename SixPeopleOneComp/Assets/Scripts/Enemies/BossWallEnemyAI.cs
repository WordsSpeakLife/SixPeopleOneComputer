using System.Collections;
using System.Net;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class BossWallEnemyAI : MonoBehaviour, IDamage
{

    [SerializeField] GameObject mainObject;
    [SerializeField] Animator animator;
    [SerializeField] AudioSource rumbleplayer;
    [SerializeField] AudioClip rumble;
    [SerializeField] Renderer model;
    [SerializeField] PlayableDirector Cutscene;
    [SerializeField] GameObject Face;
    [SerializeField] GameObject Eyes;
    [SerializeField] GameObject EyerisLeft;
    [SerializeField] GameObject EyerisRight;
    [SerializeField] GameObject EyeHurtLeft;
    [SerializeField] GameObject EyeHurtRight;
    [SerializeField] GameObject EyeLeftPos;
    [SerializeField] GameObject EyeRightPos;
    [SerializeField] Transform[] shootPos;
    [SerializeField] Transform[] spawnPos;
    [SerializeField] Transform shieldPos;
    [SerializeField] string enemyType;
    [SerializeField] GameObject bullet;

    [SerializeField] int HP;
    int HpOrig;

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
    bool shootStart = true;
    bool phaseTwo;
    bool phaseThree;

    bool phaseTwoStart;
    bool phaseThreeStart;

    float shootTimer;
    float waveTimer;
    float lazerTimer;
    float blinkTimer;

    bool isHurt = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HpOrig = HP;
        colorOrig = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;
        blinkTimer += Time.deltaTime;
        waveTimer += Time.deltaTime;
        lazerTimer += Time.deltaTime;

        playerDirRight = (GameManager.instance.player.transform.position - EyeRightPos.transform.position);
        playerDirLeft = (GameManager.instance.player.transform.position - EyeLeftPos.transform.position);

        if (shootTimer >= shootRate && shootStart)
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
                step = 0f;
            }
        }

        if (phaseThreeStart)
        {
            step = moveSpeed * Time.deltaTime;
            mainObject.transform.position = Vector3.MoveTowards(mainObject.transform.position, thirdPhasePos.position, step);
            if (mainObject.transform.position == thirdPhasePos.position)
            {
                phaseThreeStart = false;
                step = 0f;
            }
        }
        if (blinkTimer >= Random.Range(2f,4.5f) && !isHurt)
        {
            StartCoroutine(blink());
            blinkTimer = 0;
        }
    }

    public bool heal(int amount) {return false;}

    public void takeDamage(int amount)
    {
        HP -= amount;
        GameManager.instance.BossHealthBar.GetComponent<Image>().fillAmount = (float)HP / HpOrig;

        if (HP <= 0)
        {
            lazerStart = false;
            waveStart = false;
            shootStart = false;
            StartCoroutine(clearProjectiles());
            GameManager.instance.playerCamera.GetComponentInParent<cameraFollow>().followPlayer = false;
            animator.SetBool("Shake", true);
            GameManager.instance.playerScript.StopPlayerMovement(true);
            Cutscene.Play();
        }
        else if (HP <= 150 && !phaseThree)
        {
            phaseThree = true;
            lazerStart = true;
            SoundManager.instance.PlaySound3D("ShieldUp", transform.position);
            Instantiate(shield, shieldPos);
            phaseThreeStart = true;
            Camera.main.GetComponent<CameraShake>().StartCameraShake();
            SecondPlatforms.startDroppingPlats();
            StartCoroutine(playRumble(15.6f));
        }
        else if(HP <= 250 && !phaseTwo)
        {
            phaseTwo = true;
            waveStart = true;
            SoundManager.instance.PlaySound3D("ShieldUp", transform.position);
            Instantiate(shield, shieldPos);
            phaseTwoStart = true;
            Camera.main.GetComponent<CameraShake>().StartCameraShake();
            FirstPlatforms.startDroppingPlats();
            StartCoroutine(playRumble(10.5f));
        }
        else
        {
            StartCoroutine(flashRed());
            if (!isHurt)
                StartCoroutine(hurt());
        }
    }

    public void destroyBoss()
    {
        if (GameManager.instance.GameType == GameManager.GameGoal.DefeatAllEnemies)
            GameManager.instance.updateGameGoal(-1);
        Destroy(gameObject);
    }

    IEnumerator blink()
    {
        Eyes.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        Eyes.SetActive(true);
    }

    IEnumerator hurt()
    {
        isHurt = true;
        EyerisLeft.SetActive(false);
        EyerisRight.SetActive(false);
        EyeHurtLeft.SetActive(true);
        EyeHurtRight.SetActive(true);
        yield return new WaitForSeconds(1f);
        EyerisLeft.SetActive(true);
        EyerisRight.SetActive(true);
        EyeHurtLeft.SetActive(false);
        EyeHurtRight.SetActive(false);
        yield return new WaitForSeconds(.4f);
        isHurt = false;
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    IEnumerator clearProjectiles()
    {
        for (int i = 0; i < 10; i++)
        {
            CleanUpManager.instance.RemoveClonedObjects();
            yield return new WaitForSeconds(0.2f);
            CleanUpManager.instance.RemoveClonedObjects();
        }
    }

    IEnumerator playRumble(float dur)
    {
        rumbleplayer.clip = rumble;
        rumbleplayer.Play();
        yield return new WaitForSeconds(dur);
        rumbleplayer.Stop();
    }

    IEnumerator shootProjectile()
    {
        shootTimer = 0;
        yield return new WaitForSeconds(1f);
        Vector3 spawnPos = shootPos[Random.Range(0, shootPos.Length - 1)].position;
        Quaternion spawnRot = shootPos[Random.Range(0, shootPos.Length - 1)].rotation;
        Instantiate(projectiles[Random.Range(0,projectiles.Length - 1)], spawnPos, spawnRot);
        
    }
    IEnumerator shootWave()
    {
        waveTimer = 0;
        yield return new WaitForSeconds(1f);
        Vector3 spawnPos = shootPos[9].position;
        Quaternion spawnRot = shootPos[9].rotation;
        GameObject newObject = Instantiate(projectiles[2], spawnPos, spawnRot);

    }
    IEnumerator shootLazer()
    {
        lazerTimer = 0;
        yield return new WaitForSeconds(1f);
        Instantiate(bullet, EyeRightPos.transform.position, Quaternion.LookRotation(new Vector3(playerDirRight.x, playerDirRight.y, playerDirRight.z)));
        Instantiate(bullet, EyeLeftPos.transform.position, Quaternion.LookRotation(new Vector3(playerDirLeft.x, playerDirLeft.y, playerDirLeft.z)));
        SoundManager.instance.PlaySound3D("Laser", transform.position);
    }

}
