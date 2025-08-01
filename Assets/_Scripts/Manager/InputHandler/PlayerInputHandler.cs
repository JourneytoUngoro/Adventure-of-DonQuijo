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
    public bool attackInputPressed { get; private set; }
    public bool strongAttackInputPressed { get; private set; }
    public bool blockParryInputPressed { get; private set; }
    public bool blockParryInputHolding { get; private set; }
    public bool interactInputPressed { get; private set; }
    public bool returnInputPressed { get; private set; }
    public int normInputX { get; private set; }
    public int normInputY { get; private set; }
    public bool itemInputPressed { get; private set; }

    public bool whirlwindInputPressed { get; private set; }
    public bool rangedAttackInputPressed { get; private set; }
    public bool chargedAttackInputPressed { get; private set; }

    public bool confirmInputPressed { get; private set; }
    public bool cancelInputPressed { get; private set; }
    public bool toggleMenuPressed { get; private set; }

    private Timer jumpInputBufferTimer; // This is a timer to keep player's jump input for better control. For example, if it holds jump input for 0.1s and the player character hits the ground in 0.1s, the character will automatically jump right after hitting the ground even when the player does not press another jump input.
    private Timer lockMovementTimer;
    private bool movementLocked;
    private bool disableCharacterControl;
    
    private void Awake()
    {
        controls = new Controls();
        controls.Enable();
        playerInput = GetComponent<PlayerInput>();
        playerInputHandler = this;

        jumpInputBufferTimer = new Timer(0.1f);
        jumpInputBufferTimer.timerAction += InactiveJumpInput;
        lockMovementTimer = new Timer(0.0f);
        lockMovementTimer.timerAction += () => { movementLocked = false; };
    }

    private void Update()
    {
        jumpInputBufferTimer.Tick();
        lockMovementTimer.Tick();

        dodgeInputPressed = controls.CharacterControl.Dodge.WasPressedThisFrame();
        attackInputPressed = controls.CharacterControl.Attack.WasPressedThisFrame();
        strongAttackInputPressed = controls.CharacterControl.StrongAttack.WasPressedThisFrame();
        blockParryInputPressed = controls.CharacterControl.Block.WasPressedThisFrame();
        interactInputPressed = controls.CharacterControl.InteractSelect.WasPressedThisFrame();
        returnInputPressed = controls.CharacterControl.Return.WasPressedThisFrame();
        itemInputPressed = controls.CharacterControl.UseItem.WasPressedThisFrame();

        confirmInputPressed = controls.UIControl.Confirm.WasPressedThisFrame();
        cancelInputPressed = controls.UIControl.Cancel.WasPressedThisFrame();
        toggleMenuPressed = controls.UIControl.ToggleMenu.WasPressedThisFrame();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        Debug.Log("moveInput");
        if (!movementLocked)
        {
            normInputX = Mathf.RoundToInt(movementInput.x);
            normInputY = Mathf.RoundToInt(movementInput.y);
        }

        // PlayerInput의 Behaviour이 'Invoke Unity Events'라서 별도 처리
        if (disableCharacterControl)
        {
            normInputX = 0; normInputY = 0;
        }

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

    public void OnBlockParryInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            blockParryInputHolding = true;
        }

        if (context.canceled)
        {
            blockParryInputHolding = false;
        }

        // PlayerInput의 Behaviour이 'Invoke Unity Events'라서 별도 처리
        if (disableCharacterControl)
        {
            blockParryInputHolding = false;
        }
    }

    public void OnUseItem(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            var control = context.control;

            string keyName = control.name;
            int itemIndex = -1;

            // PlayerInput의 Behaviour이 'Invoke Unity Events'라서 별도 처리

            if (!disableCharacterControl && keyName != null)
            {
                #region figure item id
                switch (keyName)
                {
                    case "1": itemIndex = 1; break;
                    case "2": itemIndex = 2; break;
                    case "3": itemIndex = 3; break;
                }
                #endregion
            }

            if (itemIndex == -1) return;

            if (Manager.Instance.itemManager.UseItem(itemIndex))
            {
                Manager.Instance.soundManager.PlaySoundFXClip("playerUseItemSFX", transform);
            }
        }
    }

    public void OnUseSkill(InputAction.CallbackContext context)
    {
        var control = context.control;

        if (context.started)
        {
            switch (control.name)
            {
                case "q": rangedAttackInputPressed = true; break;
                case "w": chargedAttackInputPressed = true; break;
                case "e": whirlwindInputPressed = true; break;
            }
        }

        if (context.canceled)
        {
            switch (control.name)
            {
                case "q": rangedAttackInputPressed = false; break;
                case "w": chargedAttackInputPressed = false; break;
                case "e": whirlwindInputPressed = false; break;
            }
        }

        if (disableCharacterControl)
        {
            rangedAttackInputPressed = false;
            chargedAttackInputPressed = false;
            whirlwindInputPressed = false;
        }
    }

    public void LockMoveInput(Vector2 direction, float duration)
    {
        movementLocked = true;
        normInputX = Mathf.RoundToInt(direction.x);
        normInputY = Mathf.RoundToInt(direction.y);
        lockMovementTimer.ChangeDuration(duration);
        lockMovementTimer.StartSingleUseTimer();
    }

    public void LockMoveInput(Vector2 direction)
    {
        movementLocked = true;
        normInputX = Mathf.RoundToInt(direction.x);
        normInputY = Mathf.RoundToInt(direction.y);
    }

    public void UnlockMoveInput(bool forceSet = false)
    {
        if (forceSet || !lockMovementTimer.timerActive)
        {
            movementLocked = false;
            normInputX = Mathf.RoundToInt(movementInput.x);
            normInputY = Mathf.RoundToInt(movementInput.y);
        }
    }


    public void InactiveJumpInput() => jumpInputPressed = false;

    public void SetCharacterControlEnabled(bool enabled)
    {
        if (enabled)
        {
            controls.CharacterControl.Enable();
            disableCharacterControl = false;
        }
        else
        {
            controls.CharacterControl.Disable();
            disableCharacterControl = true;
        }
    }

    public void SetUIControlEnabled(bool enabled)
    {
        if (enabled)
        {
            controls.UIControl.Enable();
        }
        else
        {
            controls.UIControl.Disable();
        }
    }

    public bool IsCharacterControlEnabled() => controls.CharacterControl.enabled;

    public bool IsUIControlEnabled() => controls.UIControl.enabled;

    private void OnDestroy()
    {
        // Prevents memory leaks and performance issues caused by leaving actions enabled.
        controls.Disable();
    }
}
