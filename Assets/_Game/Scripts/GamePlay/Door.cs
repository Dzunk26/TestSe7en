using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour, IInteractable {
    [SerializeField] private float openAngle;
    [SerializeField] private float openDuration = 0.5f;
    [SerializeField] private Button interactButton;
    [SerializeField] private SphereCollider detectCollider;
    [SerializeField] private float rangeOffset = 2.25f; // = player height (khoang cong them cho range tranh bi bug)

    private bool isOpened = false;
    private bool isDetectPlayer = false;
    private float rangeDectecCollider;
    private Player player;
    private Transform tf;

    private void Awake() {
        interactButton.onClick.AddListener(Interact);
        OnInit();
    }

    private void Update() {
        if (isDetectPlayer && IsPlayerOutOfDetectTrigger()) {
            isDetectPlayer = false;
            CloseUI();
            player = null;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(Constant.PLAYER_TAG) && !isOpened) {
            ShowUI();
            player = Cache.GetPlayer(other);
            isDetectPlayer = true;
        }
    }

    public void OnInit() {
        tf = transform;
        rangeDectecCollider = detectCollider.radius + rangeOffset;
        CloseUI();
    }

    public void Interact() {
        if (player == null) return;

        player.OnInteract();
        OpenDoor();
        CloseUI();
    }

    private void OpenDoor() {
        if (isOpened) return;
        
        isOpened = true;

        tf.DOLocalRotate(new Vector3(0f, openAngle, 0f), openDuration).SetEase(Ease.OutQuad);
    }

    private void ShowUI() {
        interactButton.gameObject.SetActive(true);
    }

    private void CloseUI() {
        interactButton.gameObject.SetActive(false);
    }

    private bool IsPlayerOutOfDetectTrigger() {
        return Vector3.Distance(detectCollider.transform.position, player.TF.position) >= rangeDectecCollider;
    }
}