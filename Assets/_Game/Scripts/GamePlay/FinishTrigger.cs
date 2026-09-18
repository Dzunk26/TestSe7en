using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishTrigger : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(Constant.PLAYER_TAG)) {
            Player player = Cache.GetPlayer(other);
            player.OnWin();
        }
    }
}