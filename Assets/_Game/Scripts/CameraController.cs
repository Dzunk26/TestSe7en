using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField] private Transform followTarget;

    [SerializeField] private float rotationSpeed = 50;
    [SerializeField] private float bottomClamp = -40f;
    [SerializeField] private float topClamp = 70f;

    private float cinemachineTargetPitch;
    private float cinemachineTargetYaw;
    private Vector2 lookInputVector;
    private Vector3 originPosition;

    private void Awake() {
        OnInit();
    }

    private void Update() {
        ListenInput();
    }

    private void LateUpdate() {
        CameraLogic();
    }

    public void OnInit() {
        originPosition = transform.position;
    }

    private void ListenInput() {
        lookInputVector = GameInput.Instance.GetLookInputVector();
    }

    private void CameraLogic() {
        if (lookInputVector.sqrMagnitude < 0.001f) return;

        float inputY = lookInputVector.y * rotationSpeed * Time.deltaTime;
        float inputX = lookInputVector.x * rotationSpeed * Time.deltaTime;

        cinemachineTargetPitch = UpdateRotation(cinemachineTargetPitch, inputY, bottomClamp, topClamp, true);
        cinemachineTargetYaw = UpdateRotation(cinemachineTargetYaw, inputX, float.MinValue, float.MaxValue, false);

        ApplyRotation(cinemachineTargetPitch, cinemachineTargetYaw);
    }

    private void ApplyRotation(float pitch, float yaw) {
        followTarget.rotation = Quaternion.Euler(pitch, yaw, followTarget.eulerAngles.z);
    }

    private float UpdateRotation(float currentRotation, float input, float min, float max, bool isXAxis) {
        currentRotation += isXAxis ? -input : input;

        return Mathf.Clamp(currentRotation, min, max);
    }
}