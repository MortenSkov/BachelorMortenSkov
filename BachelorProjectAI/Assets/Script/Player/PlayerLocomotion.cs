using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    CameraHandler cameraHandler;
    PlayerManager playerManager;
    Transform cameraObject;
    InputHandler inputHandler;
    public Vector3 moveDirection;

    [HideInInspector]
    public Transform myTransform;
    [HideInInspector]
    public AnimationHandler animHandler;

    public new Rigidbody rigidbody;
    public GameObject normalCamera; // named it normal camera, since later on we're adding a Lock-On camera

    [Header("Movement Stats")]
    [SerializeField]
    private float movementSpeed = 5;
    [SerializeField]
    float sprintSpeed = 7;
    [SerializeField]
    private float rotationSpeed = 10;
    [SerializeField]
    private float walkingSpeed = 1;


    private void Awake()
    {
        cameraHandler = FindObjectOfType<CameraHandler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        rigidbody = GetComponent<Rigidbody>();
        inputHandler = GetComponent<InputHandler>();
        animHandler = GetComponentInChildren<AnimationHandler>();
        cameraObject = Camera.main.transform;
        myTransform = transform;
        animHandler.Initialize();
    }

    #region Movement

    Vector3 normalVector;
    Vector3 targetPosition;

    /// <summary>
    /// Handles the actual rotation of our character
    /// </summary>
    /// <param name="delta">the delta time passed since last frame- used for handling movement with the physics engine</param>
    private void HandleRotation(float delta)
    {
        if (inputHandler.lockOnFlag) // Player IS locked-on to a target
        {
            if (inputHandler.sprintFlag || inputHandler.rollFlag) // Player is sprinting or rolling
            {
                Vector3 targetDirection = Vector3.zero;
                targetDirection = cameraHandler.cameraTransform.forward * inputHandler.vertical;
                targetDirection += cameraHandler.cameraTransform.right * inputHandler.horizontal;
                targetDirection.Normalize();
                targetDirection.y = 0;

                if (targetDirection == Vector3.zero)
                {
                    targetDirection = transform.forward;
                }

                Quaternion tr = Quaternion.LookRotation(targetDirection);
                Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);

                transform.rotation = targetRotation;
            }
            else // Player is NOT sprinting or rolling (walking or running)
            {
                //Vector3 rotationDirection = moveDirection;
                Vector3 rotationDirection = cameraHandler.currentLockOnTarget.position - transform.position;
                rotationDirection.y = 0;
                rotationDirection.Normalize();
                Quaternion tr = Quaternion.LookRotation(rotationDirection);
                Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);
                transform.rotation = targetRotation;
            }
        }
        else // player is NOT locked-on to a target
        {
            Vector3 targetDir = Vector3.zero;
            float moveOverride = inputHandler.moveAmout;

            targetDir = cameraObject.forward * inputHandler.vertical;
            targetDir += cameraObject.right * inputHandler.horizontal;

            targetDir.Normalize();
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
            {
                targetDir = myTransform.forward;
            }

            float rs = rotationSpeed;

            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

            myTransform.rotation = targetRotation;
        }
    }

    public void HandleMovement(float delta)
    {
        if (inputHandler.rollFlag)
            return;

        if (playerManager.isInteracting)
            return;

        moveDirection = cameraObject.forward * inputHandler.vertical;
        moveDirection += cameraObject.right * inputHandler.horizontal;
        moveDirection.Normalize();
        moveDirection.y = 0;

        float speed = movementSpeed;

        if (inputHandler.sprintFlag && inputHandler.moveAmout > 0.5f)
        {
            speed = sprintSpeed;
            playerManager.isSprinting = true;
            moveDirection *= speed;
        }
        else
        {
            if(inputHandler.moveAmout < 0.5f)
            {
                moveDirection *= walkingSpeed;
                playerManager.isSprinting = false;
            }
            else
            {
                moveDirection *= speed;
                playerManager.isSprinting = false;
            }
        }

        Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
        rigidbody.velocity = projectedVelocity;

        if (inputHandler.lockOnFlag && !inputHandler.sprintFlag)
        {
            animHandler.UpdateAnimatorValues(inputHandler.vertical, inputHandler.horizontal, playerManager.isSprinting);
        }
        else
        {
            animHandler.UpdateAnimatorValues(inputHandler.moveAmout, 0, playerManager.isSprinting);
        }

        if (animHandler.canRotate)
        {
            HandleRotation(delta);
        }
    }

    public void HandleRollingAndSprinting(float delta)
    {
        if (animHandler.anim.GetBool("isInteracting"))
            return;

        if (inputHandler.rollFlag)
        {
            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;

            if(inputHandler.moveAmout > 0)
            {
                animHandler.PlayTargetAnimation("RollForward", true);
                moveDirection.y = 0;
                Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                myTransform.rotation = rollRotation;
            }
            else
            {
                animHandler.PlayTargetAnimation("RollBackward", true);
            }
        }
    }

    //public void HandleFalling(float delta, Vector3 moveDirection)
    //{
    //    playerManager.isGrounded = false;
    //    RaycastHit hit;
    //    Vector3 origin = myTransform.position;
    //    origin.y += groundDetectionRayStartPoint;

    //    if(Physics.Raycast(origin, myTransform.forward, out hit, 0.4f))
    //    {
    //        moveDirection = Vector3.zero;
    //    }

    //    if (playerManager.isInAir)
    //    {
    //        rigidbody.AddForce(-Vector3.up * fallingSpeed / 2f);
    //        rigidbody.AddForce(moveDirection * fallingSpeed / 10f); // makes it so, if player is walking off the edge, they are kicked off abit- so they don't get stuck on the edge (hopping off the edge w/ a little bit of force)
    //    }

    //    Vector3 dir = moveDirection;
    //    dir.Normalize();
    //    origin += dir * groundDirectionRayDistance;

    //    targetPosition = myTransform.position;

    //    Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red, 0.1f, false);
    //    if(Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck))
    //    {
    //        normalVector = hit.normal;
    //        Vector3 tp = hit.point;
    //        playerManager.isGrounded = true;
    //        targetPosition.y = tp.y;

    //        if (playerManager.isInAir)
    //        {
    //            if(inAirTimer > 0.5f)
    //            {
    //                Debug.Log("You were in the air for " + inAirTimer);
    //                animHandler.PlayTargetAnimation("Land", true);
    //                inAirTimer = 0;
    //            }
    //            else
    //            {
    //                animHandler.PlayTargetAnimation("Locomotion", false);
    //                inAirTimer = 0;
    //            }

    //            playerManager.isInAir = false;
    //        }
    //    }
    //    else
    //    {
    //        if (playerManager.isGrounded)
    //        {
    //            playerManager.isGrounded = false;
    //        }

    //        if (!playerManager.isInAir)
    //        {
    //            if (!playerManager.isInteracting)
    //            {
    //                animHandler.PlayTargetAnimation("Falling", true);
    //            }

    //            Vector3 vel = rigidbody.velocity;
    //            vel.Normalize();
    //            rigidbody.velocity = vel * (movementSpeed / 2);
    //            playerManager.isInAir = true;
    //        }
    //    }

    //    if(playerManager.isInteracting || inputHandler.moveAmout > 0)
    //    {
    //        myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, delta / 0.1f);
    //    }
    //    else
    //    {
    //        myTransform.position = targetPosition;
    //    }
    //}

    #endregion


}
