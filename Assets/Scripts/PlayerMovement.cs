using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody body;
    [SerializeField] float movementSpeedAdjustment;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        Movement();
    }

    void Movement()
    {
        //Convert input into character movement
        float playerHorizontalInput = Input.GetAxisRaw("Horizontal");
        float playerVerticalInput = Input.GetAxisRaw("Vertical");

        //Get facing direction from input
        Vector3 faceDirection = new Vector3(playerHorizontalInput, 0f, playerVerticalInput).normalized;

        //Get Camera-normalized directional vectors
        Vector3 forward = transform.InverseTransformVector(Camera.main.transform.forward);
        Vector3 right = transform.InverseTransformVector(Camera.main.transform.right);
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        //Get Direction-relative input vectors
        Vector3 forwardRelativeVerticalInput = playerVerticalInput * forward;
        Vector3 rightRelativeVerticalInput = playerHorizontalInput * right;

        //Movement relative to camera
        Vector3 cameraRelativeMovement = (forwardRelativeVerticalInput + rightRelativeVerticalInput) * movementSpeedAdjustment;


        /* later
        float targetAngle = Mathf.Atan2(faceDirection.x, faceDirection.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
        */
        transform.Translate(cameraRelativeMovement);

        //body.velocity = new Vector3(playerHorizontalInput * movementSpeed, body.velocity.y, playerVerticalInput * movementSpeed);
    }
}
