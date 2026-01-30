using UnityEngine;
using UnityEngine.Audio;

public class PlayerMovement : MonoBehaviour
{

    Rigidbody rb;
    public Transform orientation; 
    PlayerController playerController;
    public ViewBobbing viewBobScript;

    

    public float bobFrequency = 8f;
    public float bobAmplitude = 0.05f;
    public float sprintBobMultiplier = 1.4f;
    public float bobSmooth = 10f;
    [SerializeField] float camTilt;
    [SerializeField] float camRotationSpeed;
    Quaternion originalCamRot;
    [SerializeField] Transform camPos;

    float bobTimer;
    Vector3 armStartLocalPos;

    [Header("JUMP")]
    public int JumpSpeed;
    public float jumpCoolDown;
    public float airTimeMultiplier;
    float jumpBufferTime = 0.15f;
    float jumpBufferCounter;
    [SerializeField] float gravMult;

    [Header("Vectors")]
    Vector3 moveDir;
    Vector3 playerVel;

    [Header("GroundCheck")]
    public bool isGrounded;
    public float playerHeight;
    public float groundDrag;
    public LayerMask groundMask;

    [Header("Slope Hnadling")]
    [SerializeField] float maxSlopeAngle;
    [SerializeField] RaycastHit slopeHit;

    [Range(5, 25)] public int speed;
    [Range(2, 5)]public int sprintMod;

    [Header("FOOTSTEPS")]
    public AudioSource audioSource;
    public AudioClip[] footstepClips;
    public float walkStepInterval = 0.5f;
    public float sprintStepInterval = 0.28f;
    float nextStepTime;


    [Header("FLAGS")]
    public bool isEncumbered;
    public bool readyToJump;
    public bool isSprinting;
    bool exitingSlope;


    [Header("Encumbrance / Weight")]
    public float baseWeightLimit = 20f;
    public float weightPerStaminaLevel = 2f;
    public float encumberedSpeedMultiplier = 0.6f;
    public float encumberedStaminaCostMultiplier = 1.5f;
    MovementStates currentState;

    public enum MovementStates
    {
        Walking,
        Sprinting,
        Jumping,

    }

    bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * .5f + 1f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            if (angle < maxSlopeAngle && angle != 0)
                return true;
        }
        return false;
    }

    Vector3 GetSlopeMoveDir()
    {
        return Vector3.ProjectOnPlane(moveDir, slopeHit.normal);
    }

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        viewBobScript = GetComponentInChildren<ViewBobbing>();
        readyToJump = true;
        originalCamRot = camPos.rotation;
    }

    private void Update()
    {

        //GRAV MULT
        

        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, groundMask);
        if (isGrounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0;
        }

        //Timers
        speed = Mathf.Clamp(speed, 5, 25);

        Footsteps();
        jump();
        sprint();

    }

    private void FixedUpdate()
    {
        if (!GameManager.instance.isPaused)
            movement();
        if(!isGrounded)
        {
            rb.AddForce(Vector3.down * gravMult, ForceMode.Force);
        }
    }

   

    #region Movement

    void SpeedControl(float speed)
    {
        if(OnSlope())
        {
            if (rb.linearVelocity.magnitude > speed)
                rb.linearVelocity = rb.linearVelocity.normalized * speed;
            
        }
        else
        {
            Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

            if (flatVelocity.magnitude > speed)
            {
                Vector3 limitedVel = flatVelocity.normalized * speed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }

    }

    void movement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        moveDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
        float moveSpeed = speed;
        if (isEncumbered)
            moveSpeed *= encumberedSpeedMultiplier;

        //TiltCam(horizontalInput);

        SpeedControl(moveSpeed);

        if(OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDir() * speed * 20f, ForceMode.Force);

            if(rb.linearVelocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        if (isGrounded)
            rb.AddForce(moveDir.normalized * moveSpeed * 10, ForceMode.Force);
        else if (!isGrounded)
            rb.AddForce(moveDir.normalized * moveSpeed * 10 * airTimeMultiplier, ForceMode.Force);

        rb.useGravity = !OnSlope();
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded && readyToJump)
        {
            exitingSlope = true;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.y);
            readyToJump = false;
            rb.AddForce(transform.up * JumpSpeed, ForceMode.Impulse);
            Invoke(nameof(ResetJump), jumpCoolDown);
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= jumpBufferTime;
        }
    }

    void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    void sprint()
    {
        if(isGrounded)
        {
            if (Input.GetButtonDown("Sprint") && playerController.stamina > 0f && !isSprinting)
            {

                isSprinting = true;
                speed *= sprintMod;
            }
            else if ((Input.GetButtonUp("Sprint") || playerController.stamina <= 0f) && isSprinting)
            {
                isSprinting = false;
                speed /= sprintMod;
            }
        }
        
    }

    void Footsteps()
    {
        if (GameManager.instance != null && GameManager.instance.isPaused)
            return;

        if (audioSource == null || footstepClips == null || footstepClips.Length == 0)
            return;

        //if (!controller.isGrounded)
        //return;

        bool moving =
            Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f ||
            Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f;

        if (!moving)
            return;

        float interval = isSprinting ? sprintStepInterval : walkStepInterval;

        if (Time.time >= nextStepTime)
        {
            AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
            audioSource.PlayOneShot(clip);
            nextStepTime = Time.time + interval;
        }
    }

    

    //void TiltCam(float moveMagnitude)
    //{
    //    Quaternion targetRot;

    //    if(moveMagnitude > 0)
    //    {
    //        targetRot = originalCamRot * Quaternion.Euler(0, 0, camTilt);
    //    }
    //    else if(moveMagnitude < 0)
    //    {
    //        targetRot = originalCamRot * Quaternion.Euler(0, 0, -camTilt);
    //    }
    //    else
    //    {
    //        targetRot = originalCamRot;
    //    }

    //    camPos.rotation = Quaternion.Slerp(camPos.rotation, targetRot, Time.deltaTime * camRotationSpeed);
    //}

    #endregion
}
