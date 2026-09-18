using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadTrigger : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(Constant.PLAYER_TAG)) {
            Player player = Cache.GetPlayer(other);
            GameManager.Instance.LoseGame();
            player.OnDead();
        }
    }
}