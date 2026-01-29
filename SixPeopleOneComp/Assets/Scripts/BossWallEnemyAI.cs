using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossWallEnemyAI : MonoBehaviour, IDamage
{

    [SerializeField] Renderer model;
    [SerializeField] GameObject Face;
    [SerializeField] GameObject EyeLeft;
    [SerializeField] GameObject EyeRight;
    [SerializeField] Transform[] shootPos;
    [SerializeField] string enemyType;

    [SerializeField] int HP;
    [Range(15, 360)][SerializeField] int FOV;

    [SerializeField] GameObject[] projectiles;
    [SerializeField] float shootRate;

    Color colorOrig;
    float shootTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootRate)
        {
            StartCoroutine(shootProjectile());
        }
    }

    public bool heal(int amount) {return false;}

    public void takeDamage(int amount)
    {
        HP -= amount;

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

}
