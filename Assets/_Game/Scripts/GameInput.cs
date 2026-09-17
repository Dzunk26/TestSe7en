using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : Singleton<GameInput> {
    public event EventHandler OnInteractAction;

    private PlayerInputActions inputActions;

    private void Awake() {
        inputActions = new PlayerInputActions();
        inputActions.Player.Enable();

        inputActions.Player.Interact.performed += Interact_performed;
    }

    private void OnDestroy() {
        inputActions.Player.Interact.performed -= Interact_performed;

        inputActions.Dispose();
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {

    }

    public Vector2 GetMovementVectorNormalized() {
        Vector2 inputVector = inputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }

    public Vector2 GetLookInputVector() {
        if (!inputActions.Player.LookPress.IsPressed()) return Vector2.zero;
        return inputActions.Player.Look.ReadValue<Vector2>();
    }

    public bool IsJumpPressed() {
        return inputActions.Player.Jump.WasPressedThisFrame();
    }
}