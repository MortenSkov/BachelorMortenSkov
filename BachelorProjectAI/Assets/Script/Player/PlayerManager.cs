using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputHandler inputHandler;
    Animator anim;
    CameraHandler cameraHandler;
    PlayerLocomotion playerLocomotion;

    public bool isInteracting; // Falg type bool - telling you when the player is doing something

    [Header("Player Flags")]
    public bool isSprinting;

    public bool canDoCombo;


    private void Awake()
    {
        cameraHandler = CameraHandler.singleton;
    }

    // Start is called before the first frame update
    void Start()
    {
        inputHandler = GetComponent<InputHandler>();
        anim = GetComponentInChildren<Animator>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;
        isInteracting = anim.GetBool("isInteracting");
        canDoCombo = anim.GetBool("canDoCombo");
        inputHandler.TickInput(delta);
        playerLocomotion.HandleRollingAndSprinting(delta);
    }

    private void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;

        playerLocomotion.HandleMovement(delta);
    }

    private void LateUpdate()
    {
        inputHandler.rollFlag = false;
        //inputHandler.sprintFlag = false;
        inputHandler.rb_Input = false;
        inputHandler.rt_Input = false;

        float delta = Time.deltaTime;
        if (cameraHandler != null)
        {
            cameraHandler.FollowTarget(delta);
            cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
        }
    }


}
