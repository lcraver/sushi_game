using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {

    public Vector3 offset;

	// Update is called once per frame
	void Update () {
        this.transform.position = PlayerController.inst.gameObject.transform.position + offset;
	}
}
