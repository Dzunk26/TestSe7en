using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotateSpeed = 50f;
    [SerializeField] private float maxJumpHeight = 2.5f;
    [SerializeField] private float maxJumpTime = 1f;
    [SerializeField] private float fallMultiplier = 2f;

    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float raycastDistance = 0.2f;

    private Vector2 inputVector;
    private Transform tf;
    private bool isJumping;
    private bool isFalling;
    private float verticalVelocity;
    private float gravity;
    private float initialJumpVelocity;

    private void OnEnable() {
        OnInit();
    }

    private void Update() {
        ListenInput();

        //HandleJump();

        if (inputVector.sqrMagnitude > 0.001f) {
            HandleMovement();
            playerVisual.OnRun();
        }
        else {
            playerVisual.OnIdle();
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(Constant.DEAD_TRIGGER_TAG)) {

        }
        else if (other.CompareTag(Constant.DEAD_TRIGGER_TAG)) {

        }
    }

    public void OnInit() {
        playerVisual.OnInit();
        isJumping = false;
        SetupJumpVariables();
    }

    public void OnInteract() {
        playerVisual.OnInteract();
    }

    private void SetupJumpVariables() {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / (timeToApex * timeToApex);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    private void ListenInput() {
        inputVector = GameInput.Instance.GetMovementVectorNormalized();
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

        TF.position = Vector3.MoveTowards(TF.position, nextPosition, 1f);

        if (inputVector.sqrMagnitude > 0.001f) {
            TF.forward = Vector3.Lerp(TF.forward, moveDir, rotateSpeed * Time.deltaTime);
        }
    }

    private void HandleJump() {
        if (!isJumping && IsGrounded() && GameInput.Instance.IsJumpPressed()) {
            isJumping = true;
            isFalling = false;
            verticalVelocity = initialJumpVelocity;
            playerVisual.OnStartJump();
        }
        float previousYVelocity = verticalVelocity;

        if (IsGrounded() && verticalVelocity < 0f) {
            verticalVelocity = Constant.GROUNDED_GRAVITY;
            isJumping = false;
            isFalling = false;
        }
        else {
            float currentGravity = gravity;
            if (verticalVelocity < 0f) {
                currentGravity *= fallMultiplier;
                isFalling = true;
                playerVisual.OnFalling();
            }

            verticalVelocity += currentGravity * Time.deltaTime;
        }

        float averageVelocity = (previousYVelocity + verticalVelocity) * 0.5f;

        Vector3 verticalMove = Vector3.up * averageVelocity * Time.deltaTime;
        TF.position += verticalMove;
    }

    private void HandleGravity() {
        if (IsGrounded()) {
            Vector3 pos = TF.position;
            pos.y = Constant.GROUNDED_GRAVITY;
            TF.position = pos;
        }
        else if (TF.position.y < 0) {

        }
        else {
            float previousYVelocity = TF.position.y;
            float newYVelocity = TF.position.y + (Constant.GRAVITY * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + newYVelocity) / 2;
            Vector3 newPos = TF.position;
            newPos.y = newYVelocity;
            TF.position = newPos;
        }
    }

    private bool IsGrounded() {
        Vector3 startPoint = TF.position + Vector3.up * raycastDistance / 2;

        Physics.Raycast(startPoint, Vector3.down, out RaycastHit hit, raycastDistance, groundLayerMask);

        return hit.collider != null;
    }


}