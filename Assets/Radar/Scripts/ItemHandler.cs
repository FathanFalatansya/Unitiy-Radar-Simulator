using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.GetComponent<Player_Base>() != null) {
            Destroy(gameObject);
        }
    }
}
