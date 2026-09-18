using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using static UnityEngine.CullingGroup;

public enum State {
    GamePlaying,
    Lose,
    Win
}

public class GameManager : Singleton<GameManager> {
    public event EventHandler OnStateChanged;

    private State currentState;

    private void Start() {
        PlayGame();
    }

    private void OnDestroy() {
        Cache.Clear();
    }

    private void ChangeState(State state) {
        currentState = state;
        OnStateChanged?.Invoke(this, new EventArgs());
    }

    public void PlayGame() {
        ChangeState(State.GamePlaying);
    }

    public void LoseGame() {
        ChangeState(State.Lose);
        UIManager.Instance.OpenUI<CanvasLose>();
    }
}
