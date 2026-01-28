using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IDamage, IPickup
{
    [Header("---- Componets ----")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] Renderer model;
    [SerializeField] Transform ShootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject lineRenderer;

    [Header("---- Aim / Reticle ----")]
    [SerializeField] Camera mainCamera;
    [SerializeField] LayerMask aimMask;
    [SerializeField] Transform reticle;
    [SerializeField] float reticleYOffset = 0.02f;
    [SerializeField] float reticleDistance = 12f;

    bool hasAimPoint;
    Vector3 aimPoint;


    [Header("---- Stats ----")]
    [Range(1, 10)][SerializeField] int Hp;
    [Range(0, 10)][SerializeField] int speed;
    [Range(0, 10)][SerializeField] int sprintMod;

    [Header("---- Jump ----")]
    [Range(0, 20)][SerializeField] int jumpSpeed;
    [Range(0, 10)][SerializeField] int jumpMax;

    [Header("---- Wall Jump ----")]
    //[Range(0, 20)][SerializeField] int wallJumpSpeed;
    [Range(0, 20)][SerializeField] int wallJumpUpPower;
    [Range(0, 20)][SerializeField] int wallJumpSideforce;
    [Header("---- Wall Run ----")]
    [Range(0, 20)][SerializeField] int wallRunSpeed;
    [Range(0, 100)][SerializeField] float wallRunTimeOnWall;
    [Range(0, 20)][SerializeField] float wallStickForce = 5f;
    //[Range(0, 20)][SerializeField] int wallRunMax;

    [Header("---- Dash ----")]
    [Range(0, 50)][SerializeField] int dashSpeed;
    [Range(0, 1)][SerializeField] float dashTime;
    [Range(0, 1)][SerializeField] float DashResetTime;
    int DashCount;
    [Range(0, 2)][SerializeField] int Dashmax;

    bool isDashing;



    [Header("---- player camera ----")]
    [Range(0, 50)][SerializeField] int sens;
    [Range(0, 1)][SerializeField] int MouseOn;
    [Range(0, 0.5f)][SerializeField] float turnCalmVelocity;
    [Range(0, 1)][SerializeField] float turnCalmTime;

    [Header("---- Physics ----")]
    [Range(0, 35)][SerializeField] int gravity;
    [Range(0, -35)][SerializeField] float wallRunGravity;
    [SerializeField] float RayDistance;
    [SerializeField] float BottomRayDistance;

    //[SerializeField] float wallRunRayBottomDistance;
    [Range(0, 10)][SerializeField] float airDrag;

    [Header("---- Guns ----")]
    [SerializeField] List<WeaponStat> weaponList = new List<WeaponStat>();
    [Range(0, 20)][SerializeField] int ShootDamage;
    [Range(0, 50)][SerializeField] float ShootDistance;
    [Range(0, 10)][SerializeField] float ShootRate;
    [Range(0, 10)][SerializeField] float ShootSpeed;
    [Range(0, 1)][SerializeField] int gunRayOn;

    public int ammoHold;
    public int ammoAdd;
    public int ammoReload;
    public bool isTri;

    bool wallRunActive = false;

    RaycastHit GroundHit;
    bool wallRunActive;

    int jumpCount;

    int OriginalHp;
    int gravityOrig;
    int weaponListPos;

    public Sprite weaponIcon;

    float shootTimer;


    Vector3 moveDir;
    Vector3 PlayerVelo;
    string prevWallJumpName;
    string prevWallRunName;

    RaycastHit hit;
    RaycastHit currentWallHit;
    bool timerRunning = false;
    float timer;
    float duration;
    Vector3 wallMoveVector;
    bool hasWallForRun;
    float GroundCheck;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OriginalHp = Hp;
        gravityOrig = gravity;
        duration = wallRunTimeOnWall;
        //     GroundCheck = BottomRayDistance;
    }

    // Update is called once per frame
    void Update()
    {

        UpdateAimPoint();
        UpdateReticle();
        Movement();





        void Movement()
        {

            bool isGrounded = Physics.Raycast(controller.transform.position, -controller.transform.up, out GroundHit, BottomRayDistance, ~ignoreLayer);
            Debug.DrawRay(controller.transform.position, -controller.transform.up * BottomRayDistance, isGrounded ? Color.black : Color.red);
            // bool isGrounded = Physics.Raycast(transform.position, Vector3.down, (controller.height / 2) + GroundCheck, ~ignoreLayer);
            // Debug.DrawRay(transform.position, Vector3.down * ((controller.height / 2) + GroundCheck), isGrounded ? Color.green : Color.red);
            //for wallJump and wallRun
            // Debug.DrawRay(controller.transform.position, controller.transform.right * RayDistance, Color.green);
            // Debug.DrawRay(controller.transform.position, -controller.transform.right * RayDistance, Color.blue);
            //Debug.DrawRay(controller.transform.position, -controller.transform.up * BottomRayDistance, isGrounded ? Color.green : Color.red);
            // Debug.DrawRay(controller.transform.position, controller.transform.forward * RayDistance, Color.green);
            // Debug.DrawRay(controller.transform.position, -controller.transform.forward * RayDistance, Color.blue);
            // // for shoot Distance
            // Debug.DrawRay(controller.transform.position, controller.transform.forward * ShootDistance, Color.cyan);
            shootTimer += Time.deltaTime;
            RotatePlayerYawToMouse();
            moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            // controller.Move(moveDir * speed * Time.deltaTime);  
            PlayerVelo.x = Mathf.Lerp(PlayerVelo.x, 0, Time.deltaTime * airDrag);
            PlayerVelo.z = Mathf.Lerp(PlayerVelo.z, 0, Time.deltaTime * airDrag);
            wallMoveVector = Vector3.zero;
            if (wallRunActive && timerRunning)
            {

                RaycastHit leftHit;
                RaycastHit rightHit;
                bool hitLeft = Physics.Raycast(controller.transform.position, -controller.transform.right, out leftHit, RayDistance, ~ignoreLayer);
                bool hitRight = Physics.Raycast(controller.transform.position, controller.transform.right, out rightHit, RayDistance, ~ignoreLayer);
                hasWallForRun = false;
                if (hitLeft && !IsRayOnGround(leftHit) && leftHit.collider.CompareTag("wall"))
                {
                    currentWallHit = leftHit;
                    hasWallForRun = true;
                }
                else if (hitRight && !IsRayOnGround(rightHit) && rightHit.collider.CompareTag("wall"))
                {
                    currentWallHit = rightHit;
                    hasWallForRun = true;
                }
                if (!hasWallForRun)
                {
                    TimerFinished();
                }
                else
                {
                    timer += Time.deltaTime;
                    Vector3 wallFoward = Vector3.Cross(currentWallHit.normal, Vector3.up);
                    if (Vector3.Dot(transform.forward, wallFoward) < 0)
                    {
                        wallFoward = -wallFoward;
                    }
                    wallMoveVector = wallFoward * wallRunSpeed;
                    Vector3 stickForce = -currentWallHit.normal * wallStickForce;
                    wallMoveVector += stickForce;
                    if (timer >= duration)
                    {
                        TimerFinished();
                    }
                }
            }

            if (isGrounded && PlayerVelo.y <= 0)
            {
                jumpCount = 0;
                DashCount = 0;
                PlayerVelo.y = -2f;
                prevWallJumpName = null;
                prevWallRunName = null;
                wallRunActive = false;
                model.material.color = Color.cyan;
                TurnGravityOn();
            }
            else
            {
                if (!wallRunActive)
                {
                    PlayerVelo.y -= gravity * Time.deltaTime;
                }
            }
            HandleButtonPress(isGrounded);
            Vector3 movement = (moveDir * speed) + PlayerVelo; //+ wallMoveVector;
            controller.Move(movement * Time.deltaTime + (Vector3.up * wallRunGravity * Time.deltaTime));
        }

        void HandleButtonPress(bool grounded)
        {

            if (Input.GetButtonDown("Jump"))
            {

                if (!grounded && canWallJumpCheck())
                {
                    wallJump();
                }
                else if (grounded || jumpCount < jumpMax)
                {
                    Jump();
                }
            }
            else if (!grounded && !wallRunActive)
            {
                //Debug.Log("controller said grounded");
                wallRun();
            }

            if (Input.GetButtonDown("Fire2") && !grounded)
            {
                wallRunActive = false;
                timerRunning = false;
                gravity = gravityOrig;

                if (DashCount <= Dashmax)
                {

                    StartCoroutine(Dash());
                }

            }


            if (Input.GetButton("Fire1") && shootTimer >= ShootRate)
            {
                shoot();
            }
        }
        void Jump()
        {
            if (!wallRunActive)
            {
                PlayerVelo.y = jumpSpeed;
                // controller.Move(moveDir * speed * Time.deltaTime);
                jumpCount++;
                SoundManager.instance.PlaySound3D("Jumps", transform.position);
            }
        }


        void wallJump()
        {

            model.material.color = Color.magenta;
            RaycastHit hit;
            wallRunActive = false;
            timerRunning = false;
            // TurnGravityOn();

            RaycastHit GroundHit;
            if (Physics.Raycast(controller.transform.position, -controller.transform.right, out hit, RayDistance, ~ignoreLayer) ||
                Physics.Raycast(controller.transform.position, controller.transform.right, out hit, RayDistance, ~ignoreLayer) ||
                Physics.Raycast(controller.transform.position, -controller.transform.forward, out hit, RayDistance, ~ignoreLayer) ||
                Physics.Raycast(controller.transform.position, controller.transform.forward, out hit, RayDistance, ~ignoreLayer))
            {

                if (Physics.Raycast(controller.transform.position, -controller.transform.up, out GroundHit, BottomRayDistance, ~ignoreLayer))
                {
                    //Debug.Log(" nuh huh ");
                    return;
                }
                else if (!IsRayOnGround(hit) && (prevWallJumpName == null || prevWallJumpName != hit.collider.name))
                {DashCount= 0;
                    Debug.Log(hit.collider.name + " wall Jump");
                    //PlayerVelo.y = WallJumpPower;
                    //PlayerVelo.x = hit.normal.x * WallJumpPower;
                    // TurnGravityOn();
                    Vector3 JumpDirection = transform.up * wallJumpUpPower + hit.normal * wallJumpSideforce;
                    PlayerVelo = JumpDirection;
                    prevWallJumpName = hit.collider.name;
                    jumpCount = 1;
                    SoundManager.instance.PlaySound3D("Jumps", transform.position);
                }
            }
        }
        void wallRun()
        {
            //Debug.Log("hit wall runnnn")

            RaycastHit leftHit;
            RaycastHit rightHit;
            bool hitLeft = Physics.Raycast(controller.transform.position, -controller.transform.right, out leftHit, RayDistance, ~ignoreLayer);
            bool hitRight = Physics.Raycast(controller.transform.position, controller.transform.right, out rightHit, RayDistance, ~ignoreLayer);


            if (hitLeft || hitRight)
            {
                if (Physics.Raycast(controller.transform.position, -controller.transform.up, out GroundHit, BottomRayDistance, ~ignoreLayer))
                {
                    // Debug.Log(" nuh huh ");
                    TurnGravityOn();
                    wallRunActive = false;
                    return;
                }
                if (hitLeft && !IsRayOnGround(leftHit) && (prevWallRunName == null || prevWallRunName != leftHit.collider.name))
                {
                    if (Mathf.Abs(leftHit.normal.x) > 0.6f && leftHit.collider.CompareTag("wall") && !IsRayOnGround(leftHit))
                    {DashCount= 0;
                        currentWallHit = leftHit;
                        prevWallRunName = leftHit.collider.name;
                        wallRunActive = true;
                        StartTimer();
                        wallRunRayCastDirection(leftHit);
                        return;

                    }
                }
                if (hitRight && !IsRayOnGround(rightHit) && (prevWallRunName == null || prevWallRunName != rightHit.collider.name))
                {
                    if (Mathf.Abs(rightHit.normal.x) > 0.6f && rightHit.collider.CompareTag("wall") && !IsRayOnGround(rightHit))
                    {DashCount= 0;
                        currentWallHit = rightHit;
                        prevWallRunName = rightHit.collider.name;
                        wallRunActive = true;
                        StartTimer();
                        wallRunRayCastDirection(rightHit);
                        return;

                    }
                }

            }
            TurnGravityOn();
            wallRunActive = false;
        }
        bool IsRayOnGround(RaycastHit hit)
        {
            return hit.collider.tag.Contains("ground");
        }

        IEnumerator Dash()
        {

            float time = Time.time;
            if (DashCount < Dashmax)
            {

                DashCount++;
                while (Time.time < time + dashTime)
                {
                    //Debug.Log("  time start ");

                    controller.Move(transform.forward.normalized * dashSpeed * Time.deltaTime);
                    model.material.color = Color.green;


                    yield return null;

                    // Debug.Log("  time end ");
                }
            }
        }
        void wallRunRayCastDirection(RaycastHit hit)
        {
            //Debug.Log(hit.collider.name + "  Wall run");
            TurnGravityOf();
            PlayerVelo.y = 0;
            model.material.color = Color.blue;
            jumpCount = 1;

        }
    }

    private void shoot()
    {
        shootTimer = 0;

        weaponList[weaponListPos].ammoCur--;

        Vector3 shootOrigin = ShootPos ? ShootPos.position : transform.position;
        Vector3 shootDir = GetAimDirection();

        if (gunRayOn == 1)
        {
            RaycastHit hit;
            if (Physics.Raycast(shootOrigin, shootDir, out hit, ShootDistance, ~ignoreLayer))
            {
                IDamage dmg = hit.collider.GetComponent<IDamage>();
                if (dmg != null) dmg.takeDamage(ShootDamage);
            }
        }
        Quaternion bulletRot = Quaternion.LookRotation(shootDir);
        if (!isTri)
        {
            Instantiate(bullet, shootOrigin, bulletRot);
        }
        else if (isTri)
        {
            Instantiate(bullet, shootOrigin, transform.rotation * Quaternion.Euler(0, 15, 0));
            Instantiate(bullet, shootOrigin, transform.rotation);
            Instantiate(bullet, shootOrigin, transform.rotation * Quaternion.Euler(0, -15, 0));
        }
            SoundManager.instance.PlaySound3D("shoots", transform.position);
    }


    void reload()
    {
        if (Input.GetButtonDown("Reload") && weaponList.Count > 0)
        {
            weaponList[weaponListPos].ammoCur = weaponList[weaponListPos].ammoMax;
        }
    }
    public void takeDamage(int amount)
    {
        Hp -= amount;
        model.material.color = Color.red;
        StartCoroutine(wait(0.2f, false));

        GameManager.instance.HealthBar.GetComponent<Slider>().value = Hp;

        //check if the player is dead
        if (Hp <= 0)
        {
            GameManager.instance.youLose();
        }
    }
    void TurnGravityOn()
    {
        gravity = gravityOrig;
    }
    void TurnGravityOf()
    {
        gravity = 0;
    }


    public bool heal(int amount)
    {
        if (Hp >= OriginalHp) return false;
        Hp += amount;
        if (Hp > OriginalHp)
        {
            Hp = OriginalHp;
        }
        GameManager.instance.HealthBar.GetComponent<Slider>().value = Hp;
        return true;
    }
    IEnumerator wait(float amount, bool Randcolor)
    {
        if (Randcolor)
        {
            model.material.color = Random.ColorHSV();
        }
        else
        {
            model.material.color = Color.cyan;
        }
        yield return new WaitForSeconds(amount);
        TurnGravityOn();
        wallRunActive = false;


    }



    public void GetWeaponStats(WeaponStat weapon)
    {
        weaponList.Add(weapon);
        weaponListPos = weaponList.Count - 1;

        changeWep();
    }
    void changeWep()
    {
        ShootDamage = weaponList[weaponListPos].shootDamage;
        ShootDistance = weaponList[weaponListPos].shootDistance;
        ShootRate = weaponList[weaponListPos].shootRate;
        ShootSpeed = weaponList[weaponListPos].shootSpeed;
        isTri = weaponList[weaponListPos].isTri;
        //weaponIcon = weaponList[weaponListPos].weaponIcon;
        //GameManager.instance.weaponIcon = weaponIcon;
        

    }
    void selectWep()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && weaponListPos < weaponList.Count - 1)
        {
            weaponListPos++;
            changeWep();
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && weaponListPos > 0)
        {
            weaponListPos--;
            changeWep();
        }
    }



    bool TryGetMouseAimPoint(out Vector3 point)
    {
        point = Vector3.zero;

        if (!mainCamera)
            mainCamera = Camera.main;

        if (!mainCamera) return false;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 500f, aimMask))
        {
            point = hit.point;
            return true;
        }

        return false;
    }

    void UpdateAimPoint()
    {
        hasAimPoint = TryGetMouseAimPoint(out aimPoint);
    }



    void UpdateReticle()
    {
        if (hasAimPoint)
        {
            reticle.position = aimPoint + Vector3.up * reticleYOffset;
        }
        else
        {
            if (!mainCamera)
                mainCamera = Camera.main;


            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            reticle.position = ray.origin + ray.direction * reticleDistance;

        }

        reticle.rotation = Quaternion.Euler(90f, 0f, 0f);
    }




    Vector3 GetAimDirection()
    {
        Vector3 shootOrigin = ShootPos ? ShootPos.position : transform.position;

        if (hasAimPoint)
        {
            return (aimPoint - shootOrigin).normalized;
        }

        return transform.forward;
    }

    void RotatePlayerYawToMouse()
    {
        if (!hasAimPoint) return;

        Vector3 flatDir = aimPoint - transform.position;
        flatDir.y = 0f;

        if (flatDir.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(flatDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 15f * Time.deltaTime);
    }

    public void StartTimer()
    {
        timerRunning = true;
        timer = 0f;
        hasWallForRun = false;
    }

    IEnumerator MoveToPosition(Vector3 targetPosition, float timeToMove)

    {
        Vector3 currentPosition = transform.position;
        float timeElapsed = 0;

        while (timeElapsed < timeToMove)
        {
            float t = timeElapsed / timeToMove;
            transform.position = Vector3.Lerp(currentPosition, targetPosition, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }
    void TimerFinished()
    {

        prevWallJumpName = null;
        prevWallRunName = null;
        timerRunning = false;
        wallRunActive = false;
        hasWallForRun = false;
        timer = 0f;

        TurnGravityOn();
        PlayerVelo.y = -2f;
        Debug.Log("Timer finished!");

    }
    bool canWallJumpCheck()
    {
        return Physics.Raycast(controller.transform.position, controller.transform.right, out hit, RayDistance, ~ignoreLayer) ||
                Physics.Raycast(controller.transform.position, -controller.transform.right, out hit, RayDistance, ~ignoreLayer) ||
                Physics.Raycast(controller.transform.position, controller.transform.forward, out hit, RayDistance, ~ignoreLayer) ||
                Physics.Raycast(controller.transform.position, -controller.transform.forward, out hit, RayDistance, ~ignoreLayer);
    }

}

