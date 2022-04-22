using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    Transform cameraObject;
    InputHandler inputHandler;
    Vector3 moveDirection;

    [HideInInspector]
    public Transform myTransform;
    [HideInInspector]
    public AnimationHandler animHandler;

    public new Rigidbody rigidbody;
    public GameObject normalCamera; // named it normal camera, since later on we're adding a Lock-On camera

    [Header("Stats")][SerializeField]
    private float movementSpeed = 5;

    [SerializeField]
    float sprintSpeed = 7;
    [SerializeField]
    private float rotationSpeed = 10;

    public bool isSprinting;



    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        inputHandler = GetComponent<InputHandler>();
        animHandler = GetComponentInChildren<AnimationHandler>();
        cameraObject = Camera.main.transform;
        myTransform = transform;
        animHandler.Initialize();
    }

    public void Update()
    {
        float delta = Time.deltaTime;

        isSprinting = inputHandler.b_Input;
        inputHandler.TickInput(delta);
        HandleMovement(delta);
        HandleRollingAndSprinting(delta);
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
        Vector3 targetDir = Vector3.zero;
        float moveOverride = inputHandler.moveAmout;

        targetDir = cameraObject.forward * inputHandler.vertical;
        targetDir += cameraObject.right * inputHandler.horizontal;

        targetDir.Normalize();
        targetDir.y = 0;

        if(targetDir == Vector3.zero)
        {
            targetDir = myTransform.forward;
        }

        float rs = rotationSpeed;

        Quaternion tr = Quaternion.LookRotation(targetDir);
        Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

        myTransform.rotation = targetRotation;
    }

    public void HandleMovement(float delta)
    {
        if (inputHandler.rollFlag)
            return;

        moveDirection = cameraObject.forward * inputHandler.vertical;
        moveDirection += cameraObject.right * inputHandler.horizontal;
        moveDirection.Normalize();
        moveDirection.y = 0;

        float speed = movementSpeed;

        if (inputHandler.sprintFlag)
        {
            speed = sprintSpeed;
            isSprinting = true;
            moveDirection *= speed;
        }
        else
        {
            moveDirection *= speed;
        }

        Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
        rigidbody.velocity = projectedVelocity;

        animHandler.UpdateAnimatorValues(inputHandler.moveAmout, 0, isSprinting);

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

    #endregion


}
