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
    [Range(0, 20)][SerializeField] int jumpSpeed;
    [Range(0, 10)][SerializeField] int jumpMax;
    [Range(0, 20)][SerializeField] int wallJumpSpeed;
    [Range(0, 20)][SerializeField] int wallJumpMax;
    [Range(0, 20)][SerializeField] int WallJumpPower;
    [Range(0, -20)][SerializeField] int wallRunSpeed;
    [Range(0, 20)][SerializeField] int wallRunMax;
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

    Vector3 moveDir;
    Vector3 PlayerVelo;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OriginalHp = Hp;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        sprint();


    }

    void Movement()
    {
        //float mouseX = Input.GetAxisRaw("Mouse X") * sens * Time.deltaTime;
        //float mouseY = Input.GetAxisRaw("Mouse Y") * sens * Time.deltaTime;
        //if (invertY)
        //{
        //    camRotX += mouseY;
        //}
        //else
        //{
        //    camRotX -= mouseY;
        //}
        //camRotX = Mathf.Clamp(camRotX, lockVertMin, lockVertMax);
        //transform.localRotation = Quaternion.Euler(camRotX, 0, 0);
        //player.Rotate(Vector3.up * mouseX);

        Debug.DrawRay(controller.transform.position, controller.transform.right * RayDistance, Color.green);
        Debug.DrawRay(controller.transform.position, -controller.transform.right * RayDistance, Color.blue);
        Debug.DrawRay(controller.transform.position, -controller.transform.up * BottomRayDistance, Color.red);
        shootTimer += Time.deltaTime;
        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir * speed * Time.deltaTime);
        Jump();

        controller.Move(PlayerVelo * Time.deltaTime);
        if (Input.GetButtonDown("Jump") && !controller.isGrounded && wallJumpCount < wallJumpMax)
        {
            wallJump();
        }
        if (Input.GetButton("Fire1") && !controller.isGrounded && wallRunCount < wallRunMax)
        {
            wallRun();
        }
        if (controller.isGrounded)
        {
            wallJumpCount = 0;
            jumpCount = 0;
            wallRunCount = 0;
            PlayerVelo = Vector3.zero;
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
                Debug.Log(" nuh huh ");
                return;
            }
            else if (!IsRayOnGround(hit) )
            {
                Debug.Log(hit.collider.name);
                PlayerVelo.y = WallJumpPower;
                PlayerVelo.x = hit.normal.x * WallJumpPower;
                wallJumpCount++;
                jumpCount = 1;
            }

        }

    }
    void wallRun()
    {
        RaycastHit hit;
        RaycastHit GroundHit;
        if ( wallJumpCount < wallJumpMax && Physics.Raycast(controller.transform.position, controller.transform.right, out hit, RayDistance, ~ignoreLayer) || Physics.Raycast(controller.transform.position, -controller.transform.right, out hit, RayDistance, ~ignoreLayer) || Physics.Raycast(controller.transform.position, -controller.transform.up, out hit, BottomRayDistance, ~ignoreLayer))
        {
            if (Physics.Raycast(controller.transform.position, -controller.transform.up, out GroundHit, BottomRayDistance, ~ignoreLayer))
            {
                Debug.Log(" nuh huh ");
                return;
            }
            else if (!IsRayOnGround(hit))
            {
                Debug.Log(hit.collider.name + "  Wall run");
                PlayerVelo.z = wallRunSpeed;
                PlayerVelo.x = -hit.normal.x * WallJumpPower;
                wallRunCount++;
                StartCoroutine(wait());
                jumpCount = 1;
            }
          
        }

    }
    IEnumerator wait()
    {
        model.material.color = Color.blue;
        yield return new WaitForSeconds(0.5f);
        PlayerVelo.y -= wallRunGravity * Time.deltaTime;
    }
    bool IsRayOnGround(RaycastHit hit)
    {
        if (hit.collider.tag.Contains("ground"))
        {
            Debug.Log("true on ground");
            return true;

        }
        else
        {
            Debug.Log("false on ground");
            return false;

        }
    }
}

