using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractButton : MonoBehaviour, IInteractable {
    [SerializeField] private float openAngle;
    [SerializeField] private Button interactButton;
    [SerializeField] private SphereCollider detectCollider;
    [SerializeField] private float rangeOffset = 0.5f; // khoang cong them cho range tranh bi bug
    [SerializeField] private List<Platform> platforms;

    private bool isInteracted = false;
    private bool isDetectPlayer = false;
    private float rangeDectecCollider;
    private Player player;
    private Transform tf;

    private void Awake() {
        interactButton.onClick.AddListener(Interact);
        tf = transform;
    }

    private void Update() {
        if (isDetectPlayer && IsPlayerOutOfDetectTrigger()) {
            isDetectPlayer = false;
            CloseUI();
            player = null;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(Constant.PLAYER_TAG) && !isInteracted) {
            ShowUI();
            player = Cache.GetPlayer(other);
            isDetectPlayer = true;
        }
    }

    public void OnInit() {
        rangeDectecCollider = detectCollider.radius + rangeOffset;
    }

    public void Interact() {
        if (player == null) return;
        
        OpenDoor();
        CloseUI();
    }

    private void OpenDoor() {
        if (platforms == null || platforms.Count < 0) return;

        
    }

    private void ShowUI() {
        interactButton.gameObject.SetActive(true);
    }

    private void CloseUI() {
        interactButton.gameObject.SetActive(false);
    }

    private bool IsPlayerOutOfDetectTrigger() {
        return Vector3.Distance(tf.position, player.TF.position) >= rangeDectecCollider;
    }
}