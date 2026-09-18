using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Platform : MonoBehaviour {
    [SerializeField] private float scaleDuaration = 1f;

    private Vector3 originScale;
    private bool isAppear;
    private Transform tf;

    private void Awake() {
        OnInit();
    }

    public void OnInit() {
        originScale = transform.localScale;
        isAppear = false;
        tf = transform;
        Hide();
    }

    public void Appear() {
        if (isAppear) return;

        isAppear = true;
        tf.DOScale(originScale, scaleDuaration).SetEase(Ease.OutBack);
    }

    public void Hide() {
        isAppear = false;
        tf.DOScale(Vector3.zero, 0f).SetEase(Ease.InBack);
    }
}