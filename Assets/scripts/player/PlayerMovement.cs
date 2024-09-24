using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement")]
    float moveSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float groundDrag;

    [SerializeField] float jumpForce;
    [SerializeField] float jumpCd;
    [SerializeField] float airMult;
    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float lowJumpMultiplier = 2f;

    bool canJump;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [SerializeField] Transform orient;
    Vector3 moveDir;
    Rigidbody rb;

    public enum MoveState
    {
        walking, sprinting, air
    }
    MoveState State;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        canJump = true;
        orient = GameObject.Find("orient").transform;
    }

    private void Update()
    {
        GroundCheck();
        ApplyDrag();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData inputData))
        {
            // Movimiento
            MovePlayer(inputData);
            StateHandler(inputData);

            // Control de salto
            if (inputData.isJumping && canJump && grounded)
            {
                canJump = false;
                Jump();
                Runner.Invoke(nameof(JumpReset), jumpCd);
            }
        }

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Runner.DeltaTime;
        }
        else if (rb.velocity.y > 0 && !inputData.isJumping)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Runner.DeltaTime;
        }

        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }
    void MovePlayer(NetworkInputData inputData)
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        if(orient!=null)
            moveDir = orient.forward * inputData.moveInput.y + orient.right * inputData.moveInput.x;

        if (grounded)
            rb.AddForce(moveDir.normalized * moveSpeed * 10f, ForceMode.Force);
        else if (!grounded)
            rb.AddForce(moveDir.normalized * moveSpeed * 10f * airMult, ForceMode.Force);
    }

    void StateHandler(NetworkInputData inputData)
    {
        if (grounded && inputData.isSprinting)
        {
            State = MoveState.sprinting;
            moveSpeed = runSpeed;
        }
        else if (grounded)
        {
            State = MoveState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            State = MoveState.air;
        }
    }


    void GroundCheck()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
    }

    void ApplyDrag()
    {
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    void ApplyJumpPhysics(bool isJumping)
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !isJumping) 
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void JumpReset()
    {
        canJump = true;
    }
}

