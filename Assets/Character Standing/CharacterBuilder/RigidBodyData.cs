using UnityEngine;
using System.Collections;

public class RigidBodyData : MonoBehaviour {
    Rigidbody Rb;
    public Vector3 Velocity;
    public Vector3 AngularVelocity;
    public float AngularVelocityMagnatude;
    public float VelocityMagnatude;


    // Use this for initialization
    void Start () {
        Rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        Velocity = Rb.velocity;
        AngularVelocity = Rb.angularVelocity;
        AngularVelocityMagnatude = Rb.angularVelocity.magnitude;
        VelocityMagnatude = Rb.velocity.magnitude;
    }
}
