using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;
using CodeMonkey.MonoBehaviours;

public class GameHandler_Setup : MonoBehaviour {

    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private Transform playerTransfom;
    [SerializeField] private Transform pfRadarPing;
    [SerializeField] private List<Renderer> enemyRendererList;
    [SerializeField] private List<Renderer> itemRendererList;
    [SerializeField] private GameObject backgroundGameObject;

    private void Start() {
        cameraFollow.Setup(GetCameraPosition, () => 160f, true, true);
        //FunctionPeriodic.Create(() => Instantiate(pfRadarPing, new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f)), Quaternion.identity), 1f);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.U)) {
            // Show Hide Enemies
            foreach (Renderer renderer in enemyRendererList) {
                renderer.enabled = !renderer.enabled;
            }
        }
        if (Input.GetKeyDown(KeyCode.I)) {
            // Show Hide Items
            foreach (Renderer renderer in itemRendererList) {
                if (renderer != null) renderer.enabled = !renderer.enabled;
            }
        }
        if (Input.GetKeyDown(KeyCode.O)) {
            // Show Hide Background
            backgroundGameObject.SetActive(!backgroundGameObject.activeSelf);
        }
    }

    private Vector3 GetCameraPosition() {
        return playerTransfom.position;
        /*
        Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
        Vector3 playerToMouseDirection = mousePosition - characterAimHandler.GetPosition();
        return characterAimHandler.GetPosition() + playerToMouseDirection * .3f;
        */
    }

}
