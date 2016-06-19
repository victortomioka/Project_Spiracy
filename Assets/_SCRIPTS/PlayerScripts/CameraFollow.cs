using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Transform target;
	public Vector2 movementRatio = Vector2.one;

	void Start () {
	
	}

	void LateUpdate () {
		Vector3 newPosition = target.position;
		newPosition.x *= movementRatio.x;
		newPosition.y *= movementRatio.y;
		newPosition.z = transform.position.z;
		transform.position = newPosition;
	}
}
