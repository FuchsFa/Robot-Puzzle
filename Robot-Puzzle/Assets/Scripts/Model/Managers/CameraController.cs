using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private float moveSpeed = 1;

    [SerializeField] private float cameraBoundsNegativeX;
    [SerializeField] private float cameraBoundsPositiveX;
    [SerializeField] private float cameraBoundsNegativeY;
    [SerializeField] private float cameraBoundsPositiveY;

    private Vector3 dragOrigin;
    private Vector3 difference;
    private bool drag = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(RobotManager.Instance.selectedRobot == null && !RobotManager.Instance.RobotPlacementActive) {
            HandleMouseDrag();
            MoveCamera();
        }
	}

    /// <summary>
    /// Kamerabewegung mit Hilfe der Maus
    /// </summary>
    private void HandleMouseDrag() {
        if(Input.GetMouseButton(0)) {
            difference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
            if(drag == false) {
                drag = true;
                dragOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        } else {
            drag = false;
        }
        if(drag == true) {
            float cameraX = Mathf.Clamp((dragOrigin.x - difference.x), cameraBoundsNegativeX, cameraBoundsPositiveX);
            float cameraY = Mathf.Clamp((dragOrigin.y - difference.y), cameraBoundsNegativeY, cameraBoundsPositiveY);
            Camera.main.transform.position = new Vector3(cameraX, cameraY, -10);
        }
    }

    /// <summary>
    /// Kamerabewegung mit Hilfe der Tastatur
    /// </summary>
    private void MoveCamera() {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal") * moveSpeed, Input.GetAxis("Vertical") * moveSpeed);
        float cameraX = Mathf.Clamp((Camera.main.transform.position.x + move.x), cameraBoundsNegativeX, cameraBoundsPositiveX);
        float cameraY = Mathf.Clamp((Camera.main.transform.position.y + move.y), cameraBoundsNegativeY, cameraBoundsPositiveY);
        Camera.main.transform.position = new Vector3(cameraX, cameraY, -10);
    }
}
