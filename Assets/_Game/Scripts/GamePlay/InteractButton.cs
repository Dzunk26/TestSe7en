using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractButton : MonoBehaviour, IInteractable {
    [SerializeField] private float delay = 0.5f;
    [SerializeField] private Button interactButton;
    [SerializeField] private SphereCollider detectCollider;
    [SerializeField] private float rangeOffset = 2.25f; // khoang cong them cho range tranh bi bug
    [SerializeField] private List<Platform> platforms;

    private bool isOpenPlatform = false;
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
        if (other.CompareTag(Constant.PLAYER_TAG) && !isOpenPlatform) {
            ShowUI();
            player = Cache.GetPlayer(other);
            isDetectPlayer = true;
        }
    }

    public void OnInit() {
        isDetectPlayer = false;
        isOpenPlatform = false;
        tf = transform;
        rangeDectecCollider = detectCollider.radius + rangeOffset;
        CloseUI();
    }

    public void Interact() {
        if (player == null) return;

        player.OnInteract();
        OpenPlatforms();
        CloseUI();
    }

    private void OpenPlatforms() {
        if (platforms == null || platforms.Count < 0) return;
        isOpenPlatform = true;

        StartCoroutine(IEOpenPlatforms());
    }

    private IEnumerator IEOpenPlatforms() {
        for (int i = 0; i < platforms.Count; i++) {
            if (platforms[i] != null) {
                platforms[i].Appear();
            }

            if (i < platforms.Count - 1) {
                yield return new WaitForSeconds(delay);
            }
        }
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