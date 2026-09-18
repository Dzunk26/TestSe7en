using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : Singleton<GameInput> {
    public event EventHandler OnJumpAction;

    private PlayerInputActions inputActions;

    private void Awake() {
        inputActions = new PlayerInputActions();
        inputActions.Player.Enable();

        inputActions.Player.Jump.performed += Jump_performed;
    }

    private void OnDestroy() {
        if (inputActions == null) return;

        inputActions.Player.Jump.performed -= Jump_performed;

        inputActions.Player.Disable();

        inputActions.Dispose();
    }

    private void Jump_performed(InputAction.CallbackContext obj) {
        OnJumpAction?.Invoke(this, EventArgs.Empty);
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