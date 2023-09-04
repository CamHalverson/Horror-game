using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTeleporter : MonoBehaviour {

    [SerializeField] private Transform teleport;
    private Vector3 startingPos;
    private Quaternion startingRot;

	// Use this for initialization
	void Start ()
    {
        startingPos = teleport.position;
        startingRot = teleport.rotation;
	}

    public void TeleportBack()
    {
        teleport.SetPositionAndRotation(startingPos,startingRot);
    }
}
