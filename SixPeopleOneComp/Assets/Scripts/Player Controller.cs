using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IDamage
{
    [Header("---- Componets ----")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] Renderer model;
    [SerializeField] Transform lookTransform;
    [SerializeField] Transform ShootPos;
    [SerializeField] GameObject bullet;

    [Header("---- UI ----")]
    [SerializeField] GameObject HeathBar;

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
    [Range(0, -20)][SerializeField] int wallRunSpeed;
    [Range(0, 1)][SerializeField] float wallRunTimeOnWall;
    //[Range(0, 20)][SerializeField] int wallRunMax;

    [Header("---- Dash ----")]
    [Range(0, 50)][SerializeField] int dashSpeed;
    [Range(0, 1)][SerializeField] float dashTime;

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
    [SerializeField] int ShootDamage;
    [SerializeField] float ShootDistance;
    [SerializeField] float ShootRate;


    bool wallRunActive;
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



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OriginalHp = Hp;
        gravityOrig = gravity;
    }

    // Update is called once per frame
    void Update()
    {

        Movement();



    }

    void Movement()
    {


        Debug.DrawRay(controller.transform.position, controller.transform.right * RayDistance, Color.green);
        Debug.DrawRay(controller.transform.position, -controller.transform.right * RayDistance, Color.blue);
        Debug.DrawRay(controller.transform.position, -controller.transform.up * BottomRayDistance, Color.red);
        Debug.DrawRay(controller.transform.position, controller.transform.forward * RayDistance, Color.green);
        Debug.DrawRay(controller.transform.position, -controller.transform.forward * RayDistance, Color.blue);

        shootTimer += Time.deltaTime;


        if (MouseOn == 1)
        {
            turn.x += Input.GetAxisRaw("Mouse X");
            transform.localRotation = Quaternion.Euler(0, turn.x * sens, 0);
            moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
            controller.Move(moveDir * speed * Time.deltaTime);
        }
        else if (MouseOn == 0)
        {
            //old movement code just in case 

            move_horizontal = Input.GetAxisRaw("Horizontal");
            move_vertical = Input.GetAxisRaw("Vertical");

            direction = new Vector3(move_horizontal, 0f, move_vertical).normalized;

            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + lookTransform.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnCalmVelocity, turnCalmTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDirection.normalized * speed * Time.deltaTime);

            }
        }
        //shootTimer += Time.deltaTime;



        Jump();

        if (Input.GetButtonDown("Jump"))
        {
            wallJump();
        }
        else if (Input.GetButtonDown("Fire1"))
        {
            wallRun();
        }
        if (Input.GetButtonDown("Sprint"))
        {

            StartCoroutine(Dash());

        }

        controller.Move(PlayerVelo * Time.deltaTime);
        if (controller.isGrounded)
        {
            wallJumpCount = 0;
            jumpCount = 0;
            wallRunCount = 0;
            PlayerVelo = Vector3.zero;
            prevWallJumpName = null;
            prevWallRunName = null;
        }
        else
        {

            PlayerVelo.y -= gravity * Time.deltaTime;
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
        }
    }

    void wallJump()
    {
        RaycastHit hit;
        RaycastHit GroundHit;
        if (Physics.Raycast(controller.transform.position, controller.transform.right, out hit, RayDistance, ~ignoreLayer)
            || Physics.Raycast(controller.transform.position, -controller.transform.right, out hit, RayDistance, ~ignoreLayer) ||
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

                PlayerVelo.y = 0f;
                Vector3 JumpDirection = transform.up * wallJumpUpPower + hit.normal * wallJumpSideforce;
                PlayerVelo = JumpDirection;
                prevWallJumpName = hit.collider.name;
                wallJumpCount++;
                jumpCount = 1;
            }

        }

    }
    void wallRun()
    {
        RaycastHit hit;
        RaycastHit leftHit;
        RaycastHit rightHit;
        RaycastHit GroundHit;
        if (Physics.Raycast(controller.transform.position, controller.transform.right, out hit, RayDistance, ~ignoreLayer) || Physics.Raycast(controller.transform.position, -controller.transform.right, out hit, RayDistance, ~ignoreLayer) || Physics.Raycast(controller.transform.position, -controller.transform.up, out hit, BottomRayDistance, ~ignoreLayer))
        {

            if (Physics.Raycast(controller.transform.position, -controller.transform.up, out GroundHit, BottomRayDistance, ~ignoreLayer))
            {

                // Debug.Log(" nuh huh ");
                return;
            }
            else if (!IsRayOnGround(hit))
            {
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
    IEnumerator wait()
    {

        //  Debug.Log("  time start ");
        gravity = 0;
        PlayerVelo.y = 0;
        model.material.color = Color.blue;
        yield return new WaitForSeconds(0.3f);
        PlayerVelo.y -= wallRunGravity * Time.deltaTime;
        //  Debug.Log("  time end ");
        gravity = gravityOrig;
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
            yield return null;

            // Debug.Log("  time end ");
        }
    }
    void wallRunRayCastDirection(int wallRunSpeed, RaycastHit hit)
    {

        //Debug.Log(hit.collider.name + "  Wall run");
        prevWallRunName = hit.collider.name;
        moveDir = Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir * wallRunSpeed * Time.deltaTime);
        PlayerVelo.x = wallRunSpeed;
        StartCoroutine(wait());

        jumpCount = 1;

    }


    void shoot()
    {
        shootTimer = 0;

        Instantiate(bullet, ShootPos.position, transform.rotation );
    }

    public void takeDamage(int amount)
    {
        Hp -= amount;
        HeathBar.GetComponent<Slider>().value = Hp;

        //check if the player is dead
        if (Hp <= 0)
        {
            GameManager.instance.youLose();
        }
    }
}

