using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputInitializer : MonoBehaviour
{
    private PlayerInput input;
    private PlayerInputHandler handler;

    private InputAction toggleMenuAction;

    private void Start()
    {
        input = GetComponent<PlayerInput>();
        handler = GetComponent<PlayerInputHandler>();

        InitializeActions();
    }

    private void InitializeActions()
    {
        toggleMenuAction = input.actions.FindActionMap("UIControl").FindAction("ToggleMenu");

        toggleMenuAction?.Enable();
        // toggleMenuAction.performed += Manager.Instance.inputHandler.OnToggleMenu;
    }
}
