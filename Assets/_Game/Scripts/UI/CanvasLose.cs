using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public class CanvasLose : UICanvas {
    [SerializeField] private Button retryButton;

    private void Awake() {
        retryButton.onClick.AddListener(OnPressRetry);
    }

    private void OnPressRetry() {
        Close(0f);
        Loader.Load(Scene.GameScene);
    }
}