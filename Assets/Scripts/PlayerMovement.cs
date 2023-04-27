using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD;

public class PlayerMovement : MonoBehaviour
{
    public Cinemachine.CinemachineFreeLook thirdPersonCamera;

    Rigidbody body;
    [SerializeField] float movementSpeedAdjustment;
    public Transform attackPoint;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public Transform transformRotation;

    public StudioEventEmitter playerFootsteps;

    float lastFrameVelocity;

    public bool isSneaking = false;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        transformRotation = GetComponent<Transform>();

        playerFootsteps.SetParameter("CurrentlyWalking", 1f);
    }

    // Update is called once per frame
    void Update()
    {
        playerFootsteps.SetParameter("CurrentlyWalking", 1f);


        Movement();

        if (lastFrameVelocity <= 0 && body.velocity.magnitude > 0)
        {
            playerFootsteps.Play();
            playerFootsteps.SetParameter("WalkLooping", 1f);

            //print("loop");
        }
        else if (lastFrameVelocity > 0 && body.velocity.magnitude <= 0)
        {
            playerFootsteps.SetParameter("WalkLooping", 0f);

            //print("no loop");
        }

        lastFrameVelocity = body.velocity.magnitude;
    }

    void Movement()
    {
        //Convert input into character movement
        float playerHorizontalInput = Input.GetAxisRaw("Horizontal");
        float playerVerticalInput = Input.GetAxisRaw("Vertical");

        //Get Camera-normalized directional vectors
        Vector3 forward = transform.InverseTransformVector(Camera.main.transform.forward);
        Vector3 right = transform.InverseTransformVector(Camera.main.transform.right);
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        //Get facing direction from input
        Vector3 faceDirection = transform.InverseTransformVector(Camera.main.transform.forward);

        //Get Direction-relative input vectors
        Vector3 forwardRelativeVerticalInput = playerVerticalInput * forward;
        Vector3 rightRelativeVerticalInput = playerHorizontalInput * right;

        //Movement relative to camera
        Vector3 cameraRelativeMovement;

        /* later
        float targetAngle = Mathf.Atan2(faceDirection.x, faceDirection.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
        */
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftShift))
        {
            //Movement relative to camera slower
            cameraRelativeMovement = (forwardRelativeVerticalInput + rightRelativeVerticalInput).normalized * (movementSpeedAdjustment / 2);

            isSneaking = true;

            playerFootsteps.SetParameter("SneakingState", 1f);
        }
        else
        {
            //Movement relative to camera normal
            cameraRelativeMovement = (forwardRelativeVerticalInput + rightRelativeVerticalInput).normalized * movementSpeedAdjustment;

            isSneaking = false;

            playerFootsteps.SetParameter("SneakingState", 0f);
        }

        transform.Translate(cameraRelativeMovement);
        body.velocity = cameraRelativeMovement;
        //HERE ROTATION STUFF

        float facingAngle = thirdPersonCamera.m_XAxis.Value;

        //UnityEngine.Debug.Log(facingAngle);

        if (body.velocity.magnitude > 0)
        {
            transform.rotation = Quaternion.Euler(0f, facingAngle, 0f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawLine(transform.position, attackPoint.position);
    }
}
