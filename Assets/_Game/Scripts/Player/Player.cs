using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState {
    Idle,
    Run,
    Jump,
    Fall,
    Interact,
    Dead,
    Win
}

public class Player : MonoBehaviour {
    public Transform TF {
        get {
            if (tf == null) {
                tf = transform;
            }

            return tf;
        }
    }

    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private Transform playerObj;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private CharacterController characterController;

    [SerializeField] private float moveSpeed = 7.5f;
    [SerializeField] private float rotateSpeed = 50f;
    [SerializeField] private float maxJumpHeight = 2.5f;
    [SerializeField] private float maxJumpTime = 1f;
    [SerializeField] private float fallMultiplier = 2f;

    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float raycastDistance = 0.2f;

    private GameInput gameInput;
    private Vector2 inputVector;
    private Transform tf;
    private bool isJumping;
    private float verticalVelocity;
    private float gravity;
    private float initialJumpVelocity;
    private PlayerState currentState;

    private void OnEnable() {
        OnInit();
    }

    private void Update() {
        ListenInput();

        HandleGravity();

        HandlePlayerState();
    }

    private void OnDisable() {
        OnDespawn();
    }

    public void OnInit() {
        playerVisual.OnInit();
        isJumping = false;
        ChangeState(PlayerState.Idle);
        SetupJumpVariables();
        gameInput = GameInput.Instance;
        gameInput.OnJumpAction += GameInput_OnJumpAction;
    }

    public void OnDespawn() {
        if (gameInput == null) return;
        gameInput.OnJumpAction -= GameInput_OnJumpAction;
    }

    private void GameInput_OnJumpAction(object sender, System.EventArgs e) {
        if ((currentState == PlayerState.Idle || currentState == PlayerState.Run) && IsGrounded() && !isJumping) {
            StartJump();
        }
    }

    public void OnInteract() {
        ChangeState(PlayerState.Interact);
    }

    public void OnDead() {
        ChangeState(PlayerState.Dead);
    }

    public void OnWin() {
        ChangeState(PlayerState.Win);
    }

    private void HandlePlayerState() {
        switch (currentState) {
            case PlayerState.Idle:
                HandleIdle();
                break;
            case PlayerState.Run:
                HandleRun();
                break;
            case PlayerState.Jump:
                HandleJump();
                break;
            case PlayerState.Fall:
                HandleFall();
                break;
            case PlayerState.Interact:
                HandleInteract();
                break;
            case PlayerState.Dead:
                HandleDead();
                break;
            case PlayerState.Win:
                HandleWin();
                break;
        }
    }

    private void SetupJumpVariables() {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / (timeToApex * timeToApex);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    private void ListenInput() {
        inputVector = gameInput.GetMovementVectorNormalized();
    }

    private bool IsMoving() {
        return inputVector.sqrMagnitude > 0.01f;
    }

    private void HandleIdle() {
        playerVisual.OnIdle();

        if (!IsGrounded()) {
            ChangeState(PlayerState.Fall);
            return;
        }

        if (IsMoving()) {
            ChangeState(PlayerState.Run);
        }
    }

    private void HandleRun() {
        if (!IsGrounded()) {
            ChangeState(PlayerState.Fall);
            return;
        }

        playerVisual.OnRun();
        HandleMovement();

        if (!IsMoving()) {
            ChangeState(PlayerState.Idle);
        }
    }

    private void HandleMovement() {
        Vector3 camForward = playerCamera.transform.forward;
        Vector3 camRight = playerCamera.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * inputVector.y + camRight * inputVector.x).normalized;
        Vector3 nextPosition = moveDir * moveSpeed * Time.deltaTime;

        characterController.Move(nextPosition);

        if (inputVector.sqrMagnitude > 0.001f) {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            playerObj.rotation = Quaternion.Slerp(playerObj.rotation, targetRot, rotateSpeed * Time.deltaTime);
        }
    }

    private void StartJump() {
        isJumping = true;
        verticalVelocity = initialJumpVelocity;
        playerVisual.OnStartJump();
        ChangeState(PlayerState.Jump);
    }

    private void HandleJump() {
        HandleMovement();

        if (verticalVelocity <= 0f) {
            ChangeState(PlayerState.Fall);
        }
    }

    private void HandleGravity() {
        float currentGravity = gravity;

        if (verticalVelocity < 0f) {
            currentGravity *= fallMultiplier;
        }

        verticalVelocity += currentGravity * Time.deltaTime;

        if (verticalVelocity < -20f) {
            verticalVelocity = -20f;
        }

        characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    private void HandleFall() {
        HandleMovement();
        playerVisual.OnFalling();

        if (IsGrounded()) {
            verticalVelocity = Constant.GROUNDED_GRAVITY;
            isJumping = false;

            if (IsMoving()) {
                ChangeState(PlayerState.Run);
            }
            else {
                ChangeState(PlayerState.Idle);
            }
        }
    }

    private void HandleInteract() {
        playerVisual.OnInteract();

        AnimatorStateInfo stateInfo = playerVisual.Animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Interact") && stateInfo.normalizedTime >= 1f) {
            if (IsMoving()) {
                ChangeState(PlayerState.Run);
            }
            else {
                ChangeState(PlayerState.Idle);
            }
        }
    }

    private void HandleDead() {
        playerVisual.OnDead();
    }

    private void HandleWin() {
        playerVisual.OnWin();
    }

    private bool IsGrounded() {
        float radius = characterController.radius * 0.95f;
        Vector3 center = TF.position + characterController.center;
        float halfHeight = characterController.height * 0.5f - radius;

        Vector3 point1 = center + Vector3.up * halfHeight;
        Vector3 point2 = center + Vector3.down * halfHeight;

        return Physics.CapsuleCast(point1, point2, radius, Vector3.down, raycastDistance, groundLayerMask);
    }

    private void ChangeState(PlayerState state) {
        if (currentState == state) return;

        currentState = state;
    }
}