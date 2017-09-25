using UnityEngine;
using System.Collections;

public class addforceTest : MonoBehaviour {
		public float speed = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
				GetComponent<Rigidbody> ().AddForce (transform.forward * speed);
	}
}
