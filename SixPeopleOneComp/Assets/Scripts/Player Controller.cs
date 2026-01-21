using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IDamage
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
    //[Range(0, 20)][SerializeField] int wallRunMax;

    [Header("---- Dash ----")]
    [Range(0, 50)][SerializeField] int dashSpeed;
    [Range(0, 1)][SerializeField] float dashTime;
    [Range(0, 1)][SerializeField] float DashResetTime;
    [Range(0, 2)][SerializeField] int DashCount;
    bool isDashing;



    [Header("---- player camera ----")]
    [Range(0, 50)][SerializeField] int sens;
    [Range(0, 1)][SerializeField] int MouseOn;
    [Range(0, 0.5f)][SerializeField] float turnCalmVelocity;
    [Range(0, 1)][SerializeField] float turnCalmTime;

    [Header("---- Physics ----")]
    [Range(0, 35)][SerializeField] int gravity;
    [Range(0, 35)][SerializeField] int wallRunGravity;
    [SerializeField] float RayDistance;
    [SerializeField] float BottomRayDistance;

    [Header("---- Guns ----")]
    [Range(0, 20)][SerializeField] int ShootDamage;
    [Range(0, 50)][SerializeField] float ShootDistance;
    [Range(0, 10)][SerializeField] float ShootRate;
    [Range(0, 10)][SerializeField] float shootSpeed;
    [Range(0, 1)][SerializeField] int gunRayOn;

    bool wallRunActive = false;
    int jumpCount;
    int wallJumpCount;
    int wallRunCount;
    int OriginalHp;
    int gravityOrig;

    float shootTimer;

    float move_horizontal;
    float move_vertical;
    Vector3 moveDir;
    Vector3 PlayerVelo;
    string prevWallJumpName;
    string prevWallRunName;
    Vector2 turn;
    Vector3 direction;
    Vector3 Line;
    RaycastHit hit;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OriginalHp = Hp;
        gravityOrig = gravity;
    }

    // Update is called once per frame
    void Update()
    {

        UpdateAimPoint();
        UpdateReticle();
        Movement();

        //if (wallRunActive)
        //{
        //    controller.transform.Translate(Vector3.forward * wallRunSpeed * Time.deltaTime);
        //}

    }
    void Movement()
    {

        //for wallJump and wallRun
        Debug.DrawRay(controller.transform.position, controller.transform.right * RayDistance, Color.green);
        Debug.DrawRay(controller.transform.position, -controller.transform.right * RayDistance, Color.blue);
        Debug.DrawRay(controller.transform.position, -controller.transform.up * BottomRayDistance, Color.red);
        Debug.DrawRay(controller.transform.position, controller.transform.forward * RayDistance, Color.green);
        Debug.DrawRay(controller.transform.position, -controller.transform.forward * RayDistance, Color.blue);
        // for shoot Distance
        Debug.DrawRay(controller.transform.position, controller.transform.forward * ShootDistance, Color.cyan);

        shootTimer += Time.deltaTime;




        if (MouseOn == 1)
        {

            RotatePlayerYawToMouse();
            if (!wallRunActive)
            {
                moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                controller.Move(moveDir * speed * Time.deltaTime);
            }
            else if (wallRunActive)
            {
                if (Physics.Raycast(controller.transform.position, controller.transform.right, out hit, RayDistance, ~ignoreLayer) ||
                    Physics.Raycast(controller.transform.position, -controller.transform.right, out hit, RayDistance, ~ignoreLayer))
                {
                    if ((Input.GetKey(KeyCode.W) && Input.GetAxis("Vertical") > 0) || Input.GetKey(KeyCode.S))
                    {
                        moveDir = Input.GetAxis("Vertical") * Vector3.forward;
                        controller.Move(moveDir * wallRunSpeed * Time.deltaTime);
                    }
                }
                //Vector3 wallDirection = Vector3.Cross(hit.normal, Vector3.up);
                //bool hitRight = Physics.Raycast(transform.position, transform.right, out hit, RayDistance, ~ignoreLayer);
                //bool hitLeft = Physics.Raycast(transform.position, -transform.right, out hit, RayDistance, ~ignoreLayer);

                //if (hitRight || hitLeft)
                //{
                //    wallDirection +=wallDirection;
                //}
                //float vertInput = Input.GetAxisRaw("Vertical");
                //if(vertInput > 0)
                //{
                //    controller.Move(wallDirection*wallRunSpeed * Time.deltaTime);
                //}
                else
                {
                    TurnGravityOn();
                    wallRunActive = false;
                }

            }
        }
        else if (MouseOn == 0)
        {
            wallRunActive = false;
            TurnGravityOn();
            move_horizontal = Input.GetAxisRaw("Horizontal");
            move_vertical = Input.GetAxisRaw("Vertical");

            direction = new Vector3(move_horizontal, 0f, move_vertical).normalized;

            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnCalmVelocity, turnCalmTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDirection.normalized * speed * Time.deltaTime);

            }
        }

        Jump();

        if (Input.GetButtonDown("Jump"))
        {
            wallRunActive = false;
            TurnGravityOn();
            wallJump();
        }
        else if (Input.GetButtonDown("Fire1"))
        {
            wallRun();
        }
        if (Input.GetButtonDown("Sprint"))
        {
            wallRunActive = false;
            gravity = gravityOrig;
            if (DashCount > 2)
            {
                return;
            }
            else
            {
                StartCoroutine(Dash());
            }

        }

        controller.Move(PlayerVelo * Time.deltaTime);
        if (controller.isGrounded)
        {
            wallJumpCount = 0;
            jumpCount = 0;
            wallRunCount = 0;
            DashCount = 0;
            PlayerVelo = Vector3.zero;
            prevWallJumpName = null;
            prevWallRunName = null;
        }
        else
        {
            if (!wallRunActive)
            {
                PlayerVelo.y -= gravity * Time.deltaTime;
            }
            else
            {
                PlayerVelo.y = 0;
            }
        }

        if (Input.GetButton("Fire2") && shootTimer >= ShootRate)
        {
            shoot();
        }
    }
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            PlayerVelo.y = jumpSpeed;

            controller.Move(moveDir * speed * Time.deltaTime);

            jumpCount++;

            SoundManager.instance.PlaySound3D("Jumps", transform.position);
        }
    }

    void wallJump()
    {
        TurnGravityOn();
        DashCount = 0;
        RaycastHit GroundHit;
        if (Physics.Raycast(controller.transform.position, controller.transform.right, out hit, RayDistance, ~ignoreLayer) ||
            Physics.Raycast(controller.transform.position, -controller.transform.right, out hit, RayDistance, ~ignoreLayer) ||
            Physics.Raycast(controller.transform.position, -controller.transform.up, out hit, BottomRayDistance, ~ignoreLayer) ||
            Physics.Raycast(controller.transform.position, controller.transform.forward, out hit, RayDistance, ~ignoreLayer) ||
            Physics.Raycast(controller.transform.position, -controller.transform.forward, out hit, RayDistance, ~ignoreLayer))
        {

            if (Physics.Raycast(controller.transform.position, -controller.transform.up, out GroundHit, BottomRayDistance, ~ignoreLayer))
            {
                //Debug.Log(" nuh huh ");
                return;
            }
            else if (!IsRayOnGround(hit) && (prevWallJumpName == null || prevWallJumpName != hit.collider.name))
            {
                Debug.Log(hit.collider.name + " wall Jump");
                //PlayerVelo.y = WallJumpPower;
                //PlayerVelo.x = hit.normal.x * WallJumpPower;
                gravity = gravityOrig;
                PlayerVelo.y = 0f;
                Vector3 JumpDirection = transform.up * wallJumpUpPower + hit.normal * wallJumpSideforce;
                PlayerVelo = JumpDirection;
                prevWallJumpName = hit.collider.name;
                wallJumpCount++;
                jumpCount = 1;

                SoundManager.instance.PlaySound3D("Jumps", transform.position);
            }

        }

    }
    void wallRun()
    {
        DashCount = 0;
        //bool hitRight = Physics.Raycast(transform.position, transform.right, out hit, RayDistance, ~ignoreLayer);
        //bool hitLeft = Physics.Raycast(transform.position, -transform.right, out hit, RayDistance, ~ignoreLayer);

        //if (hitRight || hitLeft) 
        //{
        //    if (Physics.Raycast(controller.transform.position, -controller.transform.up, BottomRayDistance, ~ignoreLayer))
        //    {
        //        //Debug.Log(" nuh huh ");
        //        TurnGravityOn();
        //        wallRunActive = false;
        //        return;
        //    }
        //    if(prevWallRunName != hit.collider.name)
        //    {
        //        wallRunRayCastDirection(wallRunSpeed, hit);
        //    }


        //}
        RaycastHit leftHit;
        RaycastHit rightHit;
        RaycastHit GroundHit;
        if (Physics.Raycast(controller.transform.position, controller.transform.right, out hit, RayDistance, ~ignoreLayer) ||
            Physics.Raycast(controller.transform.position, -controller.transform.right, out hit, RayDistance, ~ignoreLayer) ||
            Physics.Raycast(controller.transform.position, -controller.transform.up, out hit, BottomRayDistance, ~ignoreLayer))
        {


            if (Physics.Raycast(controller.transform.position, -controller.transform.up, out GroundHit, BottomRayDistance, ~ignoreLayer))
            {

                // Debug.Log(" nuh huh ");
                TurnGravityOn();
                wallRunActive = false;
                return;
            }
            else if (!IsRayOnGround(hit))
            {
                wallRunActive = true;
                //  Debug.Log(" not hit ground");
                if (Physics.Raycast(controller.transform.position, -controller.transform.right, out leftHit, BottomRayDistance, ~ignoreLayer) && (prevWallRunName == null || prevWallRunName != leftHit.collider.name))
                {
                    if (leftHit.normal.x > 0.6f)
                    {
                        Debug.Log(" hit the +x side  ray hit left \n");
                        wallRunCount++;

                        wallRunRayCastDirection(wallRunSpeed, leftHit);

                    }
                    else if (leftHit.normal.x < -0.6f)
                    {

                        Debug.Log(" hit the -x side  ray hit left \n");
                        wallRunCount++;
                        wallRunRayCastDirection(-wallRunSpeed, leftHit);
                    }


                }
                if (Physics.Raycast(controller.transform.position, controller.transform.right, out rightHit, BottomRayDistance, ~ignoreLayer) && (prevWallRunName == null || prevWallRunName != rightHit.collider.name))
                {
                    if (rightHit.normal.x > 0.6f)
                    {
                        Debug.Log(" hit the +x side  ray hit right \n");
                        wallRunCount++;
                        wallRunRayCastDirection(wallRunSpeed, rightHit);

                    }
                    else if (rightHit.normal.x < -0.6f)
                    {
                        Debug.Log(" hit the -x side  ray hit right \n");
                        wallRunCount++;
                        wallRunRayCastDirection(-wallRunSpeed, rightHit);
                    }


                }
            }

        }

    }
    bool IsRayOnGround(RaycastHit hit)
    {
        if (hit.collider.tag.Contains("ground"))
        {
            // Debug.Log("true on ground");
            return true;

        }
        else
        {
            // Debug.Log("false on ground");
            return false;

        }
    }

    IEnumerator Dash()
    {


        float time = Time.time;
        while (Time.time < time + dashTime)
        {
            //Debug.Log("  time start ");

            controller.Move(transform.forward.normalized * dashSpeed * Time.deltaTime);
            DashCount++;
            yield return null;

            // Debug.Log("  time end ");
        }
    }
    void wallRunRayCastDirection(int wallRunSpeed, RaycastHit hit)
    {
        //Debug.Log(hit.collider.name + "  Wall run");

        prevWallRunName = hit.collider.name;
        Vector3 wallFoward = Vector3.Cross(hit.normal, transform.up);

        if (Vector3.Dot(transform.forward, wallFoward) < 0)
        {
            wallFoward = -wallFoward;
        }
        controller.Move(wallFoward * wallRunSpeed * Time.deltaTime);
        TurnGravityOf();
        PlayerVelo.y = 0;
        PlayerVelo.x = -hit.normal.x;
        model.material.color = Color.blue;
        wallRunActive = true;
        jumpCount = 1;
        StartCoroutine(wait(wallRunTimeOnWall, true));

    }


    void shoot()
    {
        shootTimer = 0;

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
        Instantiate(bullet, shootOrigin, bulletRot);
        SoundManager.instance.PlaySound3D("shoots", transform.position);
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




}
