using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IDamage, IPickup
{
    [Header("---- Componets ----")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] Renderer model;
    [SerializeField] Transform ShootPos;
    [SerializeField] Animator animator;
    [SerializeField] GameObject WallRunPully;

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
    [Range(1, 10)][SerializeField] int OriginalHp;
    [Range(0, 10)][SerializeField] float speed;
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
    int DashCountGround;
    [Range(0, 2)][SerializeField] int Dashmax;
    [Range(0, 2)][SerializeField] int DashmaxGround;
    bool isDashing;

    [Header("---- player camera ----")]
    [Range(0, 50)][SerializeField] int sens;
    [Range(0, 1)][SerializeField] int MouseOn;
    [Range(0, 0.5f)][SerializeField] float turnCalmVelocity;
    [Range(0, 1)][SerializeField] float turnCalmTime;

    [Header("---- Physics ----")]
    [Range(0, 35)][SerializeField] public int gravity;
    [Range(0, -35)][SerializeField] float wallRunGravity;
    [SerializeField] float RayDistance;
    [SerializeField] float WallJumpRayDistance;
    [SerializeField] float WallRunRayDistance;

    [SerializeField] float BottomRayDistance;

    [SerializeField] float HardFallDistance;
    [SerializeField] float SoftFallDistance;

    //[SerializeField] float wallRunRayBottomDistance;
    [Range(0, 10)][SerializeField] float airDrag;

    [Header("---- Guns ----")]
    [SerializeField] List<WeaponStat> weaponList = new List<WeaponStat>();
    [Range(0, 20)][SerializeField] int ShootDamage;
    [Range(0, 500)][SerializeField] float ShootDistance;
    [Range(0.1f, 3)][SerializeField] float ShootRate;
    [Range(0, 10)][SerializeField] float ShootSpeed;
    [Range(0, 1)][SerializeField] int gunRayOn;

    [SerializeField] GameObject FlameThrowerHitbox;

    [Range(1, 8)] public int bulletAmount;
    [SerializeField] GameObject constantHitbox;
    public int ammoHold;
    public int ammoAdd;
    public int ammoReload;
    [Range(1, 5)] public int shootType;
    bool reloading;

    bool wallRunActive = false;

    RaycastHit GroundHit;

    int jumpCount;

    public int gravityOrig;
    int weaponListPos;

    public Image weaponIcon;
    public Image weaponIconFill;

    float shootTimer;

    Vector3 dashDir;
    Vector3 moveDir;
    public Vector3 PlayerVelo;
    string prevWallJumpName;
    string prevWallRunName;

    RaycastHit hit;
    RaycastHit currentWallHit;
    bool timerRunning = false;
    float timer;
    float duration;
    // bool isGrounded;
    bool isGroundedCyote;
    bool cyoteTimeActive;
    Vector3 wallMoveVector;
    bool hasWallForRun;
    float GroundCheck;

    bool Fast;
    bool canMove = true;
    float speeedOrig;
    bool NoArms;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OriginalHp = Hp;
        GameManager.instance.HealthBar.fillAmount = Hp;
        gravityOrig = gravity;
        duration = wallRunTimeOnWall;
        weaponList[weaponListPos].ammoCur = weaponList[weaponListPos].ammoMax;
        animator.SetFloat("Speed", 0);
        updateIconFill();
        speeedOrig = speed;
        //     GroundCheck = BottomRayDistance;
    }
    void Update()
    {
        if (canMove)
        {
            UpdateAimPoint();
            UpdateReticle();
            Movement();
            ShowAngle();
            if (!cyoteTimeActive)
            {
                StartCoroutine(CyoteTime(0.3f));
            }
        }
        else
        {
            StopPlayerMovement(true);
        }
        void Movement()
        {
            bool isGrounded = Physics.Raycast(controller.transform.position, -controller.transform.up, out GroundHit, BottomRayDistance, ~ignoreLayer);
            bool isNotSoftFall = Physics.Raycast(controller.transform.position, -controller.transform.up, out GroundHit, SoftFallDistance, ~ignoreLayer);
            bool LandAnim = Physics.Raycast(controller.transform.position, -controller.transform.up, out GroundHit, HardFallDistance, ~ignoreLayer);
            //isGroundedCyote = Physics.Raycast(controller.transform.position, -controller.transform.up, out GroundHit, BottomRayDistance, ~ignoreLayer);
            //Debug.DrawRay(controller.transform.position, -controller.transform.up * BottomRayDistance, isGrounded ? Color.black : Color.red);
            // Debug.DrawRay(controller.transform.position, -controller.transform.up * SoftFallDistance, isNotSoftFall ? Color.black : Color.red);
            // Debug.DrawRay(controller.transform.position, controller.transform.right * WallJumpRayDistance, Color.green);
            // Debug.DrawRay(controller.transform.position, -controller.transform.right * WallJumpRayDistance, Color.blue);
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
                RaycastHit FrontHit;
                RaycastHit BackHit;
                bool hitLeft = Physics.Raycast(controller.transform.position, -controller.transform.right, out leftHit, RayDistance, ~ignoreLayer);
                bool hitRight = Physics.Raycast(controller.transform.position, controller.transform.right, out rightHit, RayDistance, ~ignoreLayer);
                bool hitFront = Physics.Raycast(controller.transform.position, controller.transform.forward, out FrontHit, RayDistance, ~ignoreLayer);
                bool hitBack = Physics.Raycast(controller.transform.position, -controller.transform.forward, out BackHit, RayDistance, ~ignoreLayer);
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
                else if (hitFront && !IsRayOnGround(FrontHit) && FrontHit.collider.CompareTag("wall"))
                {
                    currentWallHit = FrontHit;
                    hasWallForRun = true;
                }
                else if (hitBack && !IsRayOnGround(BackHit) && BackHit.collider.CompareTag("wall"))
                {
                    currentWallHit = BackHit;
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
                PlayerVelo.y = -2f;
                prevWallJumpName = null;
                prevWallRunName = null;
                wallRunActive = false;
                NoArms = false;
                model.material.color = Color.red;
                DashCount = 0;
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
            controller.Move(movement * Time.deltaTime);
            if (moveDir == Vector3.zero)
            {
                animator.SetFloat("Speed", 0f);
                animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime / 0.1f));
            }
            else
            {
                animator.SetFloat("Speed", 0.02f);
                animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime / 0.3f));

            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                Fast = !Fast;
            }
            if (Fast)
            {
                speed = 40;
            }
            else
            {
                speed = 6;
            }
            if (!isGrounded && !isNotSoftFall && LandAnim)
            {
                animator.SetTrigger("Land");
                animator.SetBool("IsSoftFalling", false);
            }
            if (isGrounded && isNotSoftFall)
            {
                animator.SetBool("IsOnGround", true);
                animator.SetBool("IsSoftFalling", false);
                animator.SetTrigger("Land");
            }
            else if (!isNotSoftFall)
            {
                animator.SetBool("IsSoftFalling", true);
                animator.SetLayerWeight(1, 0);
                if (LandAnim)
                {
                    animator.SetTrigger("Land");
                }
            }
            else if (!isGrounded && isNotSoftFall)
            {
                animator.SetBool("IsSoftFalling", false);
                animator.SetBool("IsOnGround", false);
                animator.SetLayerWeight(1, 0);
            }
        }
        void HandleButtonPress(bool grounded)
        {
            if (Input.GetKeyDown(GameManager.instance.keyBinds.Jump))
            {
                if (!grounded && canWallJumpCheck())
                {
                    wallJump();
                }
                else if (grounded && jumpCount < jumpMax)
                {
                    RaycastHit[] traps = Physics.RaycastAll(controller.transform.position, -controller.transform.up, 0.9f, ~ignoreLayer);
                    if (traps.Length == 1 && traps[0].collider.CompareTag("Trap"))
                    {
                        return;
                    }
                    else if (traps.Length == 2)
                    {
                        Jump();
                        return;
                    }
                    Jump();
                }
                else if (isGroundedCyote && !grounded && jumpCount < jumpMax)
                {
                    CyoteJump();
                }
            }
            else if (!grounded && !wallRunActive)
            {
                //Debug.Log("controller said grounded");
                wallRun();
            }
            if (Input.GetKeyDown(GameManager.instance.keyBinds.Dash))
            {
                wallRunActive = false;
                timerRunning = false;
                gravity = gravityOrig;
                if (DashCount <= Dashmax && !grounded)
                {
                    dashDir = controller.transform.forward.normalized;
                    StartCoroutine(Dash());
                }
                else if (DashCountGround <= DashmaxGround)
                {
                    dashDir = controller.transform.forward.normalized;
                    StartCoroutine(DashOnGround());
                }
            }
            if (Input.GetButton("Fire1") && weaponList.Count > 0 && weaponList[weaponListPos].ammoCur > 0 && shootTimer >= ShootRate && Time.deltaTime > 0)
            {
                Debug.Log("Pew Pew 111");
                if (weaponList.Count == 0)
                {
                    return;
                }
                shoot();
            }
            // if (Input.GetButton("Fire1") && weaponList.Count > 0 && weaponList[weaponListPos].ammoCur <= 1)
            // SoundManager.instance.PlaySound3D("Jumps", transform.position);
            changeWep();
            selectWep();
            reload();
        }
        void Jump()
        {
            if (!wallRunActive)
            {
                NoArms = true;
                animator.SetLayerWeight(1, 0);
                animator.SetTrigger("Jump");
                PlayerVelo.y = jumpSpeed;
                // controller.Move(moveDir * speed * Time.deltaTime);
                jumpCount++;
                SoundManager.instance.PlaySound3D("Jumps", transform.position);
            }
        }
        void CyoteJump()
        {
            if (!wallRunActive)
            {
                NoArms = true;
                animator.SetLayerWeight(1, 0);
                animator.SetTrigger("Jump");
                PlayerVelo.y = jumpSpeed;
                // controller.Move(moveDir * speed * Time.deltaTime);
                jumpCount++;
                SoundManager.instance.PlaySound3D("Jumps", transform.position);
            }
        }
        void wallJump()
        {
            RaycastHit hit;
            wallRunActive = false;
            timerRunning = false;
            // TurnGravityOn();
            RaycastHit GroundHit;
            if (Physics.Raycast(controller.transform.position, -controller.transform.right, out hit, WallJumpRayDistance, ~ignoreLayer) ||
                Physics.Raycast(controller.transform.position, controller.transform.right, out hit, WallJumpRayDistance, ~ignoreLayer) ||
                Physics.Raycast(controller.transform.position, -controller.transform.forward, out hit, WallJumpRayDistance, ~ignoreLayer) ||
                Physics.Raycast(controller.transform.position, controller.transform.forward, out hit, WallJumpRayDistance, ~ignoreLayer))
            {
                if (Physics.Raycast(controller.transform.position, -controller.transform.up, out GroundHit, BottomRayDistance, ~ignoreLayer))
                {
                    //Debug.Log(" nuh huh ");
                    return;
                }
                else if (!IsRayOnGround(hit) && (prevWallJumpName == null || prevWallJumpName != hit.collider.name) && hit.collider.CompareTag("wall"))
                {
                    DashCount = 0;
                    Debug.Log(hit.collider.name + " wall Jump");
                    //PlayerVelo.y = WallJumpPower;dwa
                    //PlayerVelo.x = hit.normal.x * WallJumpPower;
                    // TurnGravityOn();
                    Vector3 JumpDirection = transform.up * wallJumpUpPower + hit.normal * wallJumpSideforce;
                    PlayerVelo = JumpDirection;
                    prevWallJumpName = hit.collider.name;
                    model.material.color = Color.magenta;
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
            RaycastHit FrontHit;
            RaycastHit BackHit;
            bool hitLeft = Physics.Raycast(controller.transform.position, -controller.transform.right, out leftHit, WallRunRayDistance, ~ignoreLayer);
            bool hitRight = Physics.Raycast(controller.transform.position, controller.transform.right, out rightHit, WallRunRayDistance, ~ignoreLayer);
            bool hitFront = Physics.Raycast(controller.transform.position, controller.transform.forward, out FrontHit, WallRunRayDistance, ~ignoreLayer);
            bool hitBack = Physics.Raycast(controller.transform.position, -controller.transform.forward, out BackHit, WallRunRayDistance, ~ignoreLayer);
            if (hitLeft || hitRight || hitFront || hitBack)
            {
                if (Physics.Raycast(controller.transform.position, -controller.transform.up, out GroundHit, BottomRayDistance, ~ignoreLayer))
                {
                    // Debug.Log(" nuh huh ");
                    TurnGravityOn();
                    wallRunActive = false;
                    return;
                }
                else if (hitLeft && !IsRayOnGround(leftHit) && (prevWallRunName == null || prevWallRunName != leftHit.collider.name))
                {
                    if (Mathf.Abs(leftHit.normal.x) > 0.0f && leftHit.collider.CompareTag("wall") && !IsRayOnGround(leftHit))
                    {
                        DashCount = 0;
                        currentWallHit = leftHit;
                        prevWallRunName = leftHit.collider.name;
                        // WallRunPully.transform.position = leftHit.normal;
                        // WallRunPully.SetActive(true);
                        wallRunActive = true;
                        StartTimer();
                        wallRunRayCastDirection(leftHit);
                        return;
                    }
                }
                else if (hitRight && !IsRayOnGround(rightHit) && (prevWallRunName == null || prevWallRunName != rightHit.collider.name))
                {
                    if (Mathf.Abs(rightHit.normal.x) > 0.0f && rightHit.collider.CompareTag("wall") && !IsRayOnGround(rightHit))
                    {
                        DashCount = 0;
                        currentWallHit = rightHit;
                        prevWallRunName = rightHit.collider.name;
                        // WallRunPully.transform.position = rightHit.point;
                        // WallRunPully.SetActive(true);
                        wallRunActive = true;
                        StartTimer();
                        wallRunRayCastDirection(rightHit);
                        return;
                    }
                }
                else if (hitFront && !IsRayOnGround(FrontHit) && (prevWallRunName == null || prevWallRunName != FrontHit.collider.name))
                {
                    if (Mathf.Abs(FrontHit.normal.x) > 0.0f && FrontHit.collider.CompareTag("wall") && !IsRayOnGround(FrontHit))
                    {
                        DashCount = 0;
                        currentWallHit = FrontHit;
                        prevWallRunName = FrontHit.collider.name;
                        wallRunActive = true;
                        StartTimer();
                        wallRunRayCastDirection(FrontHit);
                        return;
                    }
                }
                else if (hitBack && !IsRayOnGround(BackHit) && (prevWallRunName == null || prevWallRunName != BackHit.collider.name))
                {
                    if (Mathf.Abs(BackHit.normal.x) > 0.0f && BackHit.collider.CompareTag("wall") && !IsRayOnGround(BackHit))
                    {
                        DashCount = 0;
                        currentWallHit = BackHit;
                        prevWallRunName = BackHit.collider.name;
                        wallRunActive = true;
                        StartTimer();
                        wallRunRayCastDirection(BackHit);
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
                SoundManager.instance.PlaySound2D("Dash");
                DashCount++;
                while (Time.time < time + dashTime)
                {
                    //Debug.Log("  time start ");
                    controller.Move(dashDir * dashSpeed * Time.deltaTime);

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
        Debug.Log("Pew Pew");
        shootTimer = 0;
        weaponList[weaponListPos].ammoCur--;
        Vector3 shootOrigin = ShootPos ? ShootPos.position : transform.position;
        Vector3 shootDir = GetAimDirection();
        float addAngle = 0;
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
        if (shootType == 1) //Single Shot!
        {
            Instantiate(weaponList[weaponListPos].bullet, shootOrigin, bulletRot);
            SoundManager.instance.PlaySound2D("shoots");
        }
        else if (shootType == 2) //Burst Shot!
        {
            if (bulletAmount % 2 == 1) //Odd number
            {
                Instantiate(weaponList[weaponListPos].bullet, shootOrigin, bulletRot);
                for (int i = 0; i < bulletAmount / 2; i++)
                {
                    Instantiate(weaponList[weaponListPos].bullet, shootOrigin, bulletRot * Quaternion.Euler(0, (45 / bulletAmount) + addAngle, 0));
                    Instantiate(weaponList[weaponListPos].bullet, shootOrigin, bulletRot * Quaternion.Euler(0, (-45 / bulletAmount) - addAngle, 0));
                    addAngle = addAngle + (45 / bulletAmount);
                    SoundManager.instance.PlaySound2D("Burst");
                }
            }
            else if (bulletAmount % 2 == 0) // Even number
            {
                for (int i = 0; i < bulletAmount / 2; i++)
                {
                    Instantiate(weaponList[weaponListPos].bullet, shootOrigin, bulletRot * Quaternion.Euler(0, ((45 / bulletAmount) / 2 + addAngle), 0));
                    Instantiate(weaponList[weaponListPos].bullet, shootOrigin, bulletRot * Quaternion.Euler(0, ((-45 / bulletAmount) / 2 - addAngle), 0));
                    addAngle = addAngle + (45 / bulletAmount);
                    SoundManager.instance.PlaySound2D("Burst");
                }
            }
        }
        else if (shootType == 3) //Radial Shot!
        {
            if (bulletAmount % 2 == 1)
            {
                Instantiate(weaponList[weaponListPos].bullet, shootOrigin, bulletRot);
                for (int i = 0; i < bulletAmount; i++)
                {
                    Instantiate(weaponList[weaponListPos].bullet, shootOrigin, bulletRot * Quaternion.Euler(0, (360 / bulletAmount) + addAngle, 0));
                    addAngle += (360 / bulletAmount);
                    SoundManager.instance.PlaySound2D("Heavy");
                }
            }
            else if (bulletAmount % 2 == 0)
            {
                for (int i = 0; i < bulletAmount; i++)
                {
                    Instantiate(weaponList[weaponListPos].bullet, shootOrigin, bulletRot * Quaternion.Euler(0, (360 / bulletAmount) + addAngle, 0));
                    addAngle += (360 / bulletAmount);
                    SoundManager.instance.PlaySound2D("Heavy");
                }
            }
        }
        // else if (shootType == 4)  //Hitbox
        // {
        //     if (FlameThrowerHitbox.activeSelf == false)
        //     {
        //         if (Input.GetKey(GameManager.instance.keyBinds.Shoot))
        //         {
        //             Instantiate(FlameThrowerHitbox);
        //         }
        //     }
        //     else
        //     {
        //         if (Input.GetKeyDown(GameManager.instance.keyBinds.Shoot))
        //         {
        //                 Destroy(FlameThrowerHitbox);
        //         }
        //     }
        // }

        SoundManager.instance.PlaySound3D("shoots", transform.position);
    }

    void reload()
    {
        if (Input.GetButtonDown("Reload") && weaponList.Count > 0)
        {
            //reloading = true; 
            //if(reloading == true)
            //{
            //    for (int i = 0; i < 50f * Time.deltaTime; i++)
            //    {
            weaponList[weaponListPos].ammoCur = weaponList[weaponListPos].ammoMax;
            //    }
            //}

            SoundManager.instance.PlaySound2D("reload");
        }
    }
    public void takeDamage(int amount)
    {
        SoundManager.instance.PlaySound2D("damage");
        Hp -= amount;
        SoundManager.instance.PlaySound3D("damage", transform.position);
        bool overDamage = false;
        int tempAmount = Hp;
        if (amount > Hp)
        {
            overDamage = true;
            tempAmount = Hp;
            Hp -= tempAmount;
        }
        else
            Hp -= amount;
        GameManager.instance.HealthBar.fillAmount = (float)Hp / OriginalHp;
        model.material.color = Color.red;

        GameManager.instance.leftPos.transform.position = new Vector3(GameManager.instance.leftPos.position.x + (Screen.width / 102) * ((overDamage) ? tempAmount : amount), GameManager.instance.leftPos.position.y, GameManager.instance.leftPos.position.z);

        GameManager.instance.dmgIndLeft.color = new Color(GameManager.instance.dmgIndLeft.color.r, GameManager.instance.dmgIndLeft.color.g, GameManager.instance.dmgIndLeft.color.b, GameManager.instance.dmgIndLeft.color.a + .05f * ((overDamage) ? tempAmount : amount));

        GameManager.instance.rightPos.transform.position = new Vector3(GameManager.instance.rightPos.position.x - (Screen.width / 102) * ((overDamage) ? tempAmount : amount), GameManager.instance.rightPos.position.y, GameManager.instance.rightPos.position.z);
        GameManager.instance.dmgIndRight.color = new Color(GameManager.instance.dmgIndRight.color.r, GameManager.instance.dmgIndRight.color.g, GameManager.instance.dmgIndRight.color.b, GameManager.instance.dmgIndRight.color.a + .05f * ((overDamage) ? tempAmount : amount));

        StartCoroutine(wait(0.2f, false));
        Color barOrig = GameManager.instance.HealthBar.color;
        StartCoroutine(FlashDamage(amount, barOrig));


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
        Color barOrig = new Color(0.3824615f, 1, 0);
        if (Hp >= OriginalHp) return false;
        Hp += amount;
        if (Hp > OriginalHp)
        {
            Hp = OriginalHp;
        }
        GameManager.instance.HealthBar.fillAmount = (float)Hp / OriginalHp;

        Color curColor = GameManager.instance.HealthBar.color;

        GameManager.instance.HealthBar.color = Color.Lerp(curColor, barOrig, 0.3f * amount);

        GameManager.instance.leftPos.transform.position = new Vector3(GameManager.instance.leftPos.position.x - ((Screen.width / 102) * amount), GameManager.instance.leftPos.position.y, GameManager.instance.leftPos.position.z);

        GameManager.instance.dmgIndLeft.color = new Color(GameManager.instance.dmgIndLeft.color.r, GameManager.instance.dmgIndLeft.color.g, GameManager.instance.dmgIndLeft.color.b, GameManager.instance.dmgIndLeft.color.a - (.05f * amount));

        GameManager.instance.rightPos.transform.position = new Vector3(GameManager.instance.rightPos.position.x + ((Screen.width / 102) * amount), GameManager.instance.rightPos.position.y, GameManager.instance.rightPos.position.z);
        GameManager.instance.dmgIndRight.color = new Color(GameManager.instance.dmgIndRight.color.r, GameManager.instance.dmgIndRight.color.g, GameManager.instance.dmgIndRight.color.b, GameManager.instance.dmgIndRight.color.a - (.05f * amount));

        return true;
    }
    IEnumerator wait(float amount, bool Randcolor)
    {
        yield return new WaitForSeconds(amount);
        TurnGravityOn();
        wallRunActive = false;
    }


    public void GetWeaponStats(WeaponStat weapon)
    {
        bool weaponInList = false;
        for (int i = 0; i < weaponList.Count; i++)
        {
            if (weaponList[i] == weapon)
            {
                weaponInList = true; break;
            }
        }
        if (!weaponInList)
        {
            weaponList.Add(weapon);
            weaponListPos = weaponList.Count - 1;
        }
        else if (weaponInList)
        {
            for (int i = 0; i < weaponList.Count; i++)
            {
                if (weaponList[i] == weapon)
                {
                    weaponList[i].ammoCur = weaponList[i].ammoMax;
                }
            }
        }
        changeWep();
    }
    void changeWep()
    {
        ShootDamage = weaponList[weaponListPos].shootDamage;
        ShootDistance = weaponList[weaponListPos].shootDistance;
        ShootRate = weaponList[weaponListPos].shootRate;
        ShootSpeed = weaponList[weaponListPos].shootSpeed;
        shootType = weaponList[weaponListPos].shootType;
        bulletAmount = weaponList[weaponListPos].bulletAmount;
        // isHoming = weaponList[weaponListPos].isHoming;
        weaponIcon.sprite = weaponList[weaponListPos].weaponIcon;
        weaponIconFill.sprite = weaponList[weaponListPos].weaponIconFill;
        GameManager.instance.weaponIcon.sprite = weaponList[weaponListPos].weaponIcon;
        GameManager.instance.weaponIconFill.sprite = weaponList[weaponListPos].weaponIconFill;
        updateIconFill();
        //weaponIcon = weaponList[weaponListPos].weaponIcon;
        //GameManager.instance.weaponIcon = weaponIcon;
        //GameManager.instance.CurrentWeapon.GetComponent<SpriteRenderer>().sprite = weaponIcon.GetComponent<SpriteRenderer>().sprite;
    }

    public void updateIconFill()
    {
        weaponIconFill.fillAmount = (float)weaponList[weaponListPos].ammoCur / weaponList[weaponListPos].ammoMax;
        GameManager.instance.weaponIconFill.fillAmount = (float)weaponList[weaponListPos].ammoCur / weaponList[weaponListPos].ammoMax;
    }
    void selectWep()
    {
        if (Time.deltaTime > 0)
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
        return Physics.Raycast(controller.transform.position, controller.transform.right, out hit, WallRunRayDistance, ~ignoreLayer) ||
                Physics.Raycast(controller.transform.position, -controller.transform.right, out hit, WallJumpRayDistance, ~ignoreLayer) ||
                Physics.Raycast(controller.transform.position, controller.transform.forward, out hit, WallJumpRayDistance, ~ignoreLayer) ||
                Physics.Raycast(controller.transform.position, -controller.transform.forward, out hit, WallJumpRayDistance, ~ignoreLayer);
    }
    IEnumerator CyoteTime(float amount)
    {
        cyoteTimeActive = true;
        yield return new WaitForSeconds(amount);
        isGroundedCyote = Physics.Raycast(controller.transform.position, -controller.transform.up, out GroundHit, BottomRayDistance, ~ignoreLayer);
        cyoteTimeActive = false;
    }
    IEnumerator DashOnGround()
    {
        float time = Time.time;
        SoundManager.instance.PlaySound3D("dash 2", transform.position);
        if (DashCountGround < DashmaxGround)
        {
            DashCountGround++;
            while (Time.time < time + dashTime)
            {
                //Debug.Log("  time start ");
                controller.Move(dashDir * dashSpeed * Time.deltaTime);

                yield return null;
                // Debug.Log("  time end ");
            }
            StartCoroutine(WaitAndResetDashCount());
        }
    }
    IEnumerator WaitAndResetDashCount()
    {
        yield return new WaitForSeconds(1);
        DashCountGround -= 1;
    }
    public int GetHP()
    {
        return Hp;
    }

    public void SetHP(int value)
    {
        Hp = value;
        GameManager.instance.HealthBar.fillAmount = (float)Hp / OriginalHp;
    }

    public void TeleportTo(Vector3 pos)
    {
        PlayerVelo = Vector3.zero;
        moveDir = Vector3.zero;
        controller.enabled = false;
        transform.position = pos;
        controller.enabled = true;
    }


    IEnumerator FlashDamage(int amount, Color barOrig)
    {
        GameManager.instance.DamageFlash.SetActive(true);
        GameManager.instance.HealthBar.color = Color.Lerp(barOrig, Color.magenta, 0.1f * amount);
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.DamageFlash.SetActive(false);

    }
    void ShowAngle()
    {
        if (Vector3.Dot(transform.forward, Vector3.forward) > 0.7f)
        {
            if (moveDir.z > 0 && moveDir.x > 0) { SetBlend(1f, 1f); if (!NoArms) { animator.SetLayerWeight(1, 1); } speed = speeedOrig; }
            else if (moveDir.z > 0 && moveDir.x < 0) { SetBlend(-1f, 1f); if (!NoArms) { animator.SetLayerWeight(1, 1); } speed = speeedOrig; }
            else if (moveDir.z < 0 && moveDir.x > 0) { SetBlend(1f, -1f); if (!NoArms) { animator.SetLayerWeight(1, 1); } speed = speeedOrig - 2; }
            else if (moveDir.z < 0 && moveDir.x < 0) { SetBlend(-1f, -1f); if (!NoArms) { animator.SetLayerWeight(1, 1); } speed = speeedOrig - 2; }
            else if (moveDir.z > 0) { SetBlend(0f, 1f); animator.SetLayerWeight(1, 1); speed = speeedOrig; }//foward 
            else if (moveDir.z < 0) { SetBlend(0f, -1f); if (!NoArms) { animator.SetLayerWeight(1, 1); } speed = speeedOrig; }//backward
            else if (moveDir.x > 0) { SetBlend(1f, 0f); if (!NoArms) { animator.SetLayerWeight(1, 1); } speed = speeedOrig - 1; }//right
            else if (moveDir.x < 0) { SetBlend(-1f, 0f); if (!NoArms) { animator.SetLayerWeight(1, 1); } speed = speeedOrig - 1; }//left
            // Debug.Log("looking forward");
        }
        else if (Vector3.Dot(transform.forward, Vector3.right) > 0.7f)
        {
            if (moveDir.z > 0 && moveDir.x > 0) { SetBlend(-1f, 1f); if (!NoArms) { animator.SetLayerWeight(1, 1); } speed = speeedOrig - 2; }
            else if (moveDir.z > 0 && moveDir.x < 0) { SetBlend(-1f, -1f); if (!NoArms) { animator.SetLayerWeight(1, 1); } speed = speeedOrig - 2; }
            else if (moveDir.z < 0 && moveDir.x > 0) { SetBlend(1f, 1f); if (!NoArms) { animator.SetLayerWeight(1, 1); } speed = speeedOrig - 2; }
            else if (moveDir.z < 0 && moveDir.x < 0) { SetBlend(1f, -1f); if (!NoArms) { animator.SetLayerWeight(1, 1); } speed = speeedOrig - 2; }
            else if (moveDir.x > 0) { SetBlend(0f, 1f); animator.SetLayerWeight(1, 1); speed = speeedOrig; }//foward 
            else if (moveDir.x < 0) { SetBlend(0f, -1f); if (!NoArms) { animator.SetLayerWeight(1, 1); } speed = speeedOrig; }//backward
            else if (moveDir.z > 0) { SetBlend(1f, 0f); if (!NoArms) { animator.SetLayerWeight(1, 1); } speed = speeedOrig - 1; }//right
            else if (moveDir.z < 0) { SetBlend(-1f, 0f); if (!NoArms) { animator.SetLayerWeight(1, 1); } speed = speeedOrig - 1; }//left
            // Debug.Log("looking right");
        }
        else if (Vector3.Dot(transform.forward, Vector3.left) > 0.7f)
        {
            if (moveDir.z > 0 && moveDir.x > 0) { SetBlend(1f, -1f); if (!NoArms) { animator.SetLayerWeight(1, 1); } speed = speeedOrig - 2; }
            else if (moveDir.z > 0 && moveDir.x < 0) { SetBlend(1f, 1f); if (!NoArms) { animator.SetLayerWeight(1, 1); } speed = speeedOrig - 2; }
            else if (moveDir.z < 0 && moveDir.x > 0) { SetBlend(-1f, -1f); if (!NoArms) { animator.SetLayerWeight(1, 1); } speed = speeedOrig - 2; }
            else if (moveDir.z < 0 && moveDir.x < 0) { SetBlend(-1f, 1f); if (!NoArms) { animator.SetLayerWeight(1, 1); } speed = speeedOrig - 2; }
            else if (moveDir.x < 0) { SetBlend(0f, 1f); animator.SetLayerWeight(1, 1); speed = speeedOrig; }//foward 
            else if (moveDir.x > 0) { SetBlend(0f, -1f); if (!NoArms) { animator.SetLayerWeight(1, 1); } speed = speeedOrig; }//backward
            else if (moveDir.z > 0) { SetBlend(1f, 0f); if (!NoArms) { animator.SetLayerWeight(1, 1); } speed = speeedOrig - 1; }//right
            else if (moveDir.z < 0) { SetBlend(-1f, 0f); if (!NoArms) { animator.SetLayerWeight(1, 1); } speed = speeedOrig - 1; }//Left
            // Debug.Log("looking left");
        }
        else if (Vector3.Dot(transform.forward, Vector3.back) > 0.7f)
        {
            if (moveDir.z > 0 && moveDir.x > 0) { SetBlend(-1f, -1f); if (!NoArms) { animator.SetLayerWeight(1, 1); } speed = speeedOrig - 2; }
            else if (moveDir.z > 0 && moveDir.x < 0) { SetBlend(1f, -1f); if (!NoArms) { animator.SetLayerWeight(1, 1); } speed = speeedOrig - 2; }
            else if (moveDir.z < 0 && moveDir.x > 0) { SetBlend(-1f, 1f); if (!NoArms) { animator.SetLayerWeight(1, 1); } speed = speeedOrig - 2; }
            else if (moveDir.z < 0 && moveDir.x < 0) { SetBlend(1f, 1f); if (!NoArms) { animator.SetLayerWeight(1, 1); } speed = speeedOrig - 2; }
            else if (moveDir.z < 0) { SetBlend(0f, 1f); animator.SetLayerWeight(1, 1); speed = speeedOrig; }//foward 
            else if (moveDir.z > 0) { SetBlend(0f, -1f); if (!NoArms) { animator.SetLayerWeight(1, 1); } speed = speeedOrig; }//Backward
            else if (moveDir.x > 0) { SetBlend(1f, 0f); if (!NoArms) { animator.SetLayerWeight(1, 1); } speed = speeedOrig - 1; }//right
            else if (moveDir.x < 0) { SetBlend(-1f, 0f); if (!NoArms) { animator.SetLayerWeight(1, 1); } speed = speeedOrig - 1; }//left
            // Debug.Log("looking back");
        }
    }
    IEnumerator ChangeSpeedTemporarily(float newSpeed, float duration)
    {
        float originalSpeed = speed;
        speed = newSpeed;
        yield return new WaitForSeconds(duration);
        speed = originalSpeed;
    }
    void SetBlend(float x, float y)
    {
        animator.SetFloat("RightLook", x, 0.2f, Time.deltaTime);
        animator.SetFloat("ForwardLook", y, 0.2f, Time.deltaTime);
    }
    public void StopPlayerMovement(bool stopGravity)
    {
        if (stopGravity) gravity = 0;
        canMove = false;
        PlayerVelo = Vector3.zero;
        moveDir = Vector3.zero;
        animator.Play("Idle", 0, 0);
    }
}

