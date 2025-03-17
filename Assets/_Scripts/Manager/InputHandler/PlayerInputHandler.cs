using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public PlayerInput playerInput { get; private set; }
    public PlayerInputHandler playerInputHandler { get; private set; }
    public Controls controls { get; private set; }
    public Vector2 movementInput { get; private set; }
    public bool jumpInputHolding { get; private set; }
    public bool jumpInputPressed { get; private set; }
    public bool dodgeInputPressed { get; private set; }
    public int normInputX { get; private set; }
    public int normInputY { get; private set; }

    private Timer jumpInputBufferTimer; // This is a timer to keep player's jump input for better control. For example, if it holds jump input for 0.1s and the player character hits the ground in 0.1s, the character will automatically jump right after hitting the ground even when the player does not press another jump input.
    
    private void Awake()
    {
        controls = new Controls();
        controls.Enable();
        playerInput = GetComponent<PlayerInput>();
        playerInputHandler = this;

        jumpInputBufferTimer = new Timer(0.1f);
        jumpInputBufferTimer.timerAction += InactiveJumpInput;
    }

    private void Update()
    {
        jumpInputBufferTimer.Tick();

        dodgeInputPressed = controls.CharacterControl.Dodge.WasPressedThisFrame();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        normInputX = Mathf.RoundToInt(movementInput.x);
        normInputY = Mathf.RoundToInt(movementInput.y);
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            jumpInputHolding = true;
            jumpInputPressed = true;
            jumpInputBufferTimer.StartSingleUseTimer();
        }

        if (context.canceled)
        {
            jumpInputHolding = false;
        }
    }

    public void InactiveJumpInput() => jumpInputPressed = false;
}
