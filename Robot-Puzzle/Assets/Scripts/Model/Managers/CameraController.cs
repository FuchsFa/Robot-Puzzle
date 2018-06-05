using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private float moveSpeed = 1;

    private Vector3 dragOrigin;
    private Vector3 difference;
    private bool drag = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(RobotManager.Instance.selectedRobot == null) {
            HandleMouseDrag();
        }
        MoveCamera();
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
            Camera.main.transform.position = dragOrigin - difference;
        }
    }

    /// <summary>
    /// Kamerabewegung mit Hilfe der Tastatur
    /// </summary>
    private void MoveCamera() {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal") * moveSpeed, Input.GetAxis("Vertical") * moveSpeed);
        Camera.main.transform.position += move;
    }
}
