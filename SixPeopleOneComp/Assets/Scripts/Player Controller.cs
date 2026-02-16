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
    bool isHoming;

    bool reloading;

    bool wallRunActive = false;

    RaycastHit GroundHit;

    int jumpCount;

    int OriginalHp;
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




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OriginalHp = Hp;
        gravityOrig = gravity;
        duration = wallRunTimeOnWall;
        weaponList[weaponListPos].ammoCur = weaponList[weaponListPos].ammoMax;
        updateIconFill();
        //     GroundCheck = BottomRayDistance;
    }
    // Update is called once per fram
    void Update()
    {
        UpdateAimPoint();
        UpdateReticle();
        Movement();
        if (!cyoteTimeActive)
        {
            StartCoroutine(CyoteTime(0.3f));
        }

        void Movement()
        {

            bool isGrounded = Physics.Raycast(controller.transform.position, -controller.transform.up, out GroundHit, BottomRayDistance, ~ignoreLayer);
            //isGroundedCyote = Physics.Raycast(controller.transform.position, -controller.transform.up, out GroundHit, BottomRayDistance, ~ignoreLayer);
            Debug.DrawRay(controller.transform.position, -controller.transform.up * BottomRayDistance, isGrounded ? Color.black : Color.red);
            Debug.DrawRay(controller.transform.position, controller.transform.right * WallJumpRayDistance, Color.green);
            Debug.DrawRay(controller.transform.position, -controller.transform.right * WallJumpRayDistance, Color.blue);
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
                model.material.color = Color.cyan;
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

            if (Input.GetKey(GameManager.instance.keyBinds.Shoot) && weaponList.Count > 0 && weaponList[weaponListPos].ammoCur > 0 && shootTimer >= ShootRate && Time.deltaTime > 0)
            {
                if (weaponList.Count == 0)
                {
                    return;
                }
                shoot();
            }
            //if (Input.GetButton("Fire1") && weaponList.Count > 0 && weaponList[weaponListPos].ammoCur <= 1)
            //    SoundManager.instance.PlaySound3D("Jumps", transform.position);

            changeWep();
            selectWep();
            reload();
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
        void CyoteJump()
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
                if (hitLeft && !IsRayOnGround(leftHit) && (prevWallRunName == null || prevWallRunName != leftHit.collider.name))
                {
                    if (Mathf.Abs(leftHit.normal.x) > 0.0f && leftHit.collider.CompareTag("wall") && !IsRayOnGround(leftHit))
                    {
                        DashCount = 0;
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
                    if (Mathf.Abs(rightHit.normal.x) > 0.0f && rightHit.collider.CompareTag("wall") && !IsRayOnGround(rightHit))
                    {
                        DashCount = 0;
                        currentWallHit = rightHit;
                        prevWallRunName = rightHit.collider.name;
                        wallRunActive = true;
                        StartTimer();
                        wallRunRayCastDirection(rightHit);
                        return;

                    }
                }
                if (hitFront && !IsRayOnGround(FrontHit) && (prevWallRunName == null || prevWallRunName != FrontHit.collider.name))
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
                if (hitBack && !IsRayOnGround(BackHit) && (prevWallRunName == null || prevWallRunName != BackHit.collider.name))
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
                SoundManager.instance.PlaySound3D("dash 2", transform.position);
                DashCount++;
                while (Time.time < time + dashTime)
                {
                    //Debug.Log("  time start ");
                    controller.Move(dashDir * dashSpeed * Time.deltaTime);
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
                }
            }
            else if (bulletAmount % 2 == 0) // Even number
            {
                for (int i = 0; i < bulletAmount / 2; i++)
                {
                    Instantiate(weaponList[weaponListPos].bullet, shootOrigin, bulletRot * Quaternion.Euler(0, ((45 / bulletAmount) / 2 + addAngle), 0));
                    Instantiate(weaponList[weaponListPos].bullet, shootOrigin, bulletRot * Quaternion.Euler(0, ((-45 / bulletAmount) / 2 - addAngle), 0));
                    addAngle = addAngle + (45 / bulletAmount);
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
                }
            }
            else if (bulletAmount % 2 == 0)
            {
                for (int i = 0; i < bulletAmount; i++)
                {
                    Instantiate(weaponList[weaponListPos].bullet, shootOrigin, bulletRot * Quaternion.Euler(0, (360 / bulletAmount) + addAngle, 0));

                    addAngle += (360 / bulletAmount);
                }
            }
        }
        else if (shootType == 4)  //Hitbox
        {
            if (FlameThrowerHitbox.activeSelf == false)
            {
                if (Input.GetKey(GameManager.instance.keyBinds.Shoot))
                {
                    FlameThrowerHitbox.transform.parent = controller.transform;
                    FlameThrowerHitbox.SetActive(true);
                }
            }
            else
            {
                if (Input.GetKeyDown(GameManager.instance.keyBinds.Shoot))
                {
                    FlameThrowerHitbox.SetActive(false);
                }
            }
        }

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
        }
    }
    public void takeDamage(int amount)
    {
        SoundManager.instance.PlaySound3D("damage", transform.position);
        Hp -= amount;
        model.material.color = Color.red;
        StartCoroutine(wait(0.2f, false));
        StartCoroutine(FlashDamage());

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
        shootType = weaponList[weaponListPos].shootType;
        bulletAmount = weaponList[weaponListPos].bulletAmount;
        isHoming = weaponList[weaponListPos].isHoming;

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
                model.material.color = Color.green;
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

    IEnumerator FlashDamage()
    {
        GameManager.instance.DamageFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.DamageFlash.SetActive(false);
    }
}

