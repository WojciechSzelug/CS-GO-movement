using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    private PlayerControls playerControls;
    
    
    
    public Rigidbody rb;
    [Header("Movment Setting")]
    public float friction;
    public float ground_accelerate;
    public float max_velocity_ground;
    
    public float AirStrafeForce;
    public float max_velocity_air;
    public float jumpForce;
    [Header("------")]
    public Vector3 gravity;
    public Transform orientation;

    public float groundDrag;
    [Header("Player on air")]
    public float air_accelerate;
    public LayerMask whatIsGround;
    public float playerHeight;
    bool grounded;

    Vector3 moveDirection;
    Vector3 moveVector;



    public void Start()
    {
        
    }
    public void Update()
    {
       MoveVector();
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        Physics.gravity = gravity;
    }

    public void FixedUpdate()
    {

        MovePlayer();
        if (playerControls.Land.Jump.IsPressed() && grounded)
        {
            Jump();
        }

    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    void MoveVector()
    {
        Vector2 move = playerControls.Land.Move.ReadValue<Vector2>();
        moveVector = new Vector3(move.x, move.y, 0);
    }
    private void MovePlayer()
    {
        moveDirection = orientation.forward * moveVector.y + orientation.right * moveVector.x;
        if (grounded)
            rb.velocity = MoveGround(moveDirection.normalized, rb.velocity);
        else
              MoveAir(moveDirection.normalized);
            
            
    }

    private Vector3 Accelerate(Vector3 accelDir, Vector3 prevVelocity, float accelerate, float max_velocity)
    {
        float projVel = Vector3.Dot(prevVelocity, accelDir); // Vector projection of Current velocity onto accelDir.
        float accelVel = accelerate * Time.fixedDeltaTime; // Accelerated velocity in direction of movment

        // If necessary, truncate the accelerated velocity so the vector projection does not exceed max_velocity
        if (projVel + accelVel > max_velocity)
            accelVel = max_velocity - projVel;

        return prevVelocity + accelDir * accelVel;
    }

    private Vector3 MoveGround(Vector3 accelDir, Vector3 prevVelocity)
    {
        // Apply Friction
        float speed = prevVelocity.magnitude;
        if (speed != 0) // To avoid divide by zero errors
        {
            float drop = speed * friction * Time.fixedDeltaTime;
            prevVelocity *= Mathf.Max(speed - drop, 0) / speed; // Scale the velocity based on friction.
        }

        // ground_accelerate and max_velocity_ground are server-defined movement variables
        return Accelerate(accelDir, prevVelocity, ground_accelerate, max_velocity_ground);
    }
    /*
    private Vector3 MoveAir(Vector3 accelDir, Vector3 prevVelocity)
    {
        
        // air_accelerate and max_velocity_air are server-defined movement variables
        float x = playerControls.Land.CameraRotation.ReadValue<Vector2>().x;
        Debug.Log("x = "+ Mathf.Abs(x));
        Debug.Log("x*delta = "+ Mathf.Abs(x) * Time.deltaTime*400);
        float _air_accelerate = air_accelerate + 400 * Mathf.Abs( x) * Time.deltaTime;

        return Accelerate(accelDir, prevVelocity, air_accelerate, max_velocity_air);
    }
    */
    void MoveAir(Vector3 vector3)
    {
        // project the velocity onto the movevector
        Vector3 projVel = Vector3.Project(GetComponent<Rigidbody>().velocity, vector3);
        

        // check if the movevector is moving towards or away from the projected velocity
        bool isAway = Vector3.Dot(vector3, projVel) <= 0f;

        // only apply force if moving away from velocity or velocity is below MaxAirSpeed
        if (projVel.magnitude < max_velocity_air || isAway)
        {
            // calculate the ideal movement force
            Vector3 vc = vector3.normalized * AirStrafeForce;

            // cap it if it would accelerate beyond MaxAirSpeed directly.
            if (!isAway)
            {
                vc = Vector3.ClampMagnitude(vc, max_velocity_air - projVel.magnitude);
            }
            else
            {
                vc = Vector3.ClampMagnitude(vc, max_velocity_air + projVel.magnitude);
            }

            // Apply the force
            GetComponent<Rigidbody>().AddForce(vc, ForceMode.VelocityChange);
        }
    }

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
}

