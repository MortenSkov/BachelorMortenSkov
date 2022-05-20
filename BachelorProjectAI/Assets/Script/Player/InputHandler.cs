using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public float horizontal;
    public float vertical;
    public float moveAmout;
    public float mouseX;
    public float mouseY;

    public bool b_Input;
    public bool rb_Input;
    public bool rt_Input;
    public bool lockOnInput;

    public bool rollFlag;
    public bool sprintFlag;
    public bool comboFlag;
    public bool lockOnFlag;
    public float rollInputTimer;

    PlayerControls inputActions; // Made through the installed Unity Package Manager - Input Actions
    PlayerAttack playerAttack;
    PlayerManager playerManager;
    CameraHandler cameraHandler;

    Vector2 movementInput;
    Vector2 cameraInput;

    private void Awake()
    {
        playerAttack = GetComponent<PlayerAttack>();
        playerManager = GetComponent<PlayerManager>();
        cameraHandler = FindObjectOfType<CameraHandler>();
    }

    public void OnEnable()
    {
        if(inputActions == null)
        {
            inputActions = new PlayerControls();
            inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
            inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
            inputActions.PlayerActions.RB.performed += i => rb_Input = true;
            inputActions.PlayerActions.RT.performed += i => rt_Input = true;
            inputActions.PlayerActions.LockOn.performed += i => lockOnInput = true;
        }

        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    public void TickInput(float delta)
    {
        MoveInput(delta);
        HandleRollingInput(delta);
        HandleAttackInput(delta);
        HandleLockOnInput();
    }

    private void MoveInput(float delta)
    {
        horizontal = movementInput.x;
        vertical = movementInput.y;
        moveAmout = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        mouseX = cameraInput.x;
        mouseY = cameraInput.y;
    }

    private void HandleRollingInput(float delta)
    {
        b_Input = inputActions.PlayerActions.Roll.phase == InputActionPhase.Performed; // detects wether or not the 'Roll' input key is being pressed
        sprintFlag = b_Input;

        if (b_Input)
        {
            rollInputTimer += delta;
        }
        else
        {
            if(rollInputTimer > 0 && rollInputTimer < 0.5f) // check if below check was at- 0.5f
            {
                sprintFlag = false;
                rollFlag = true;
            }

            rollInputTimer = 0;
        }
    }

    private void HandleAttackInput(float delta)
    {
        if (rb_Input)
        {
            if (playerManager.canDoCombo)
            {
                comboFlag = true;
                playerAttack.HandleWeaponCombo();
                comboFlag = false;
            }
            else
            {
                if (playerManager.isInteracting)
                    return;
                if (playerManager.canDoCombo)
                    return;
                playerAttack.HandleLightAttack("LightAttack_01");
            }
        }

        if (rt_Input)
        {
            playerAttack.HandleHeavyAttack("HeavyAttack_01");
        }
    }

    private void HandleLockOnInput()
    {
        if(lockOnInput && !lockOnFlag)
        {
            cameraHandler.ClearLockOnTargets(); // not neccessary
            lockOnInput = false;
            cameraHandler.HandleLockOn();
            if(cameraHandler.nearestLockOnTarget != null)
            {
                cameraHandler.currentLockOnTarget = cameraHandler.nearestLockOnTarget;
                lockOnFlag = true;
            }
        }
        else if(lockOnInput && lockOnFlag)
        {
            lockOnInput = false;
            lockOnFlag = false;
            cameraHandler.ClearLockOnTargets();
        }
    }

}
