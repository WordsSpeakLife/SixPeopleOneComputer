using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("---- Componets ----")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] Renderer model;

    [Header("---- Stats ----")]

    [Range(1, 10)][SerializeField] int Hp;
    [Range(0, 10)][SerializeField] int speed;
    [Range(0, 10)][SerializeField] int sprintMod;

    [Header("---- Jump ----")]
    [Range(0, 20)][SerializeField] int jumpSpeed;
    [Range(0, 10)][SerializeField] int jumpMax;

    [Header("---- Wall Jump ----")]
    [Range(0, 20)][SerializeField] int wallJumpSpeed;
    [Range(0, 20)][SerializeField] int wallJumpMax;
    [Range(0, 20)][SerializeField] int wallJumpUpPower;
    [Range(0, 20)][SerializeField] int wallJumpSideforce;
    [Header("---- Wall Run ----")]
    [Range(0, -20)][SerializeField] int wallRunSpeed;
    //[Range(0, 20)][SerializeField] int wallRunMax;

    [Header("---- Dash ----")]
    [Range(0, 50)][SerializeField] int dashSpeed;
    [Range(0, 1)][SerializeField] float dashTime;

    [Header("---- player camera ----")]
    [SerializeField] int sens;
    [SerializeField] int lockVertMin, lockVertMax;
    [SerializeField] bool invertY;
    [SerializeField] Transform player;

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
    float shootTimer;
    float camRotX;
    int gravityOrig;
    Vector3 moveDir;
    Vector3 PlayerVelo;
    string prevWallJumpName;
    string prevWallRunName;



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
        sprint();
       

    }

    void Movement()
    {
      

        Debug.DrawRay(controller.transform.position, controller.transform.right * RayDistance, Color.green);
        Debug.DrawRay(controller.transform.position, -controller.transform.right * RayDistance, Color.blue);
        Debug.DrawRay(controller.transform.position, -controller.transform.up * BottomRayDistance, Color.red);
        shootTimer += Time.deltaTime;
        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir * speed * Time.deltaTime);
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
    void sprint()
    {
        //if (Input.GetButtonDown("Sprint"))
        //{
        //    speed += sprintMod;
        //}
        //else if (Input.GetButtonUp("Sprint"))
        //{
        //    speed /= sprintMod;
        //}

    }
    void wallJump()
    {
        RaycastHit hit;
        RaycastHit GroundHit;
        if (wallJumpCount < wallJumpMax && Physics.Raycast(controller.transform.position, controller.transform.right, out hit, RayDistance, ~ignoreLayer) || Physics.Raycast(controller.transform.position, -controller.transform.right, out hit, RayDistance, ~ignoreLayer) || Physics.Raycast(controller.transform.position, -controller.transform.up, out hit, BottomRayDistance, ~ignoreLayer))
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

                    wallRunCount++;
                    Debug.Log(leftHit.collider.name + " ray hit left ");
                    wallRunRayCastDirection(-wallRunSpeed, leftHit);

                }
                else if ( Physics.Raycast(controller.transform.position, controller.transform.right, out rightHit, BottomRayDistance, ~ignoreLayer) && (prevWallRunName == null || prevWallRunName != rightHit.collider.name))
                {

                    wallRunCount++;
                    Debug.Log(" ray hit right  ");
                    wallRunRayCastDirection(wallRunSpeed, rightHit);

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
            controller.Move(moveDir * dashSpeed * Time.deltaTime);
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
        if (Input.GetButtonDown("Jump"))
        {
            wallJump();
            return;
        }
        else
        {
            StartCoroutine(wait());
        }
        jumpCount = 1;

    }
}

