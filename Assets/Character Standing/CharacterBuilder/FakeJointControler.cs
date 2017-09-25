using UnityEngine;
using System.Collections;

public class FakeJointControler : MonoBehaviour {
   public EditorControler editCont;
    public GameObject ConnectedPart1;
    public GameObject ConnectedPart2;

    public float LowTwistLimit = -20f;
    public float HighTwistLimit = 70f;
    public float Swing1Limit = 00f;
    public float Swing2Limit = 00f;

    public void SolidifyJoint()
    {
        GameObject thisPart = (GameObject)Instantiate(editCont.JointPrefab, Vector3.up * -100f, editCont.JointPrefab.transform.rotation);
        thisPart.transform.parent = this.transform.parent;
        thisPart.transform.localPosition = Vector3.zero;
        thisPart.transform.position = this.transform.position;
        thisPart.transform.localScale = this.transform.localScale;
        thisPart.GetComponent<PartJointControler>().ConnectedPart1 = ConnectedPart1;
        thisPart.GetComponent<PartJointControler>().ConnectedPart2 = ConnectedPart2;
        thisPart.GetComponent<PartJointControler>().LowTwistLimit = LowTwistLimit;
        thisPart.GetComponent<PartJointControler>().HighTwistLimit = HighTwistLimit;

        thisPart.GetComponent<PartJointControler>().Swing1Limit = Swing1Limit;
        thisPart.GetComponent<PartJointControler>().Swing2Limit = Swing2Limit;

        editCont.L_PartJoints.Add(thisPart);

    }
}
