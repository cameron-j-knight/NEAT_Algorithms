using UnityEngine;
using System.Collections;

public class PartJointControler : MonoBehaviour {
    public EditorControler editCont;
    public GameObject ConnectedPart1;
    public GameObject ConnectedPart2;
    public int IDXMLSAVEDATA;
    public float LowTwistLimit = -20f;
    public float HighTwistLimit = 70f;
    public float Swing1Limit = 00f;
    public float Swing2Limit = 00f;

}
