using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public float horizontal;
    public float vertical;
    public float moveAmout;
    public float mouseX;
    public float mouseY;

    PlayerControls inputActions; // Made through the installed Unity Package Manager - Input Actions

    Vector2 movementInput;
    Vector2 cameraInput;

    public void OnEnable()
    {
        if(inputActions == null)
        {
            inputActions = new PlayerControls();
            inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
            inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
        }

        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }


}
