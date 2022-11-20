using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	[SerializeField] Vector3 offset = new Vector3(0f, 0f, -10f);
	[SerializeField] float smoothTime = .25f;
	Vector3 velocity = Vector3.zero;

	[SerializeField] Transform target;

    private void FixedUpdate()
	{
		Vector3 targetPos = target.position + offset;
		transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);


	}
}
