using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterBuilder : MonoBehaviour {





    List<CharacterPart> Parts;
    List<BaseCharacterJoints> Joints;

    List<GameObject> g_Parts;
    List<GameObject> g_Joints;
    float Density = 1;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
	
	}



    public void FormCharacter()
    {
        foreach (CharacterPart part in Parts)
        {
            GameObject thisPart = GameObject.CreatePrimitive(PrimitiveType.Cube);
            thisPart.transform.parent = this.transform;
            thisPart.name = part.Name;
            thisPart.transform.localScale = part.Scale;
            Rigidbody rb = thisPart.AddComponent<Rigidbody>();
            rb.SetDensity(Density);
            thisPart.transform.localPosition = part.Location;
            part.ID = thisPart.GetInstanceID();
            g_Parts.Add(thisPart);
            

        }
        foreach (BaseCharacterJoints joint in Joints)
        {
            GameObject thisJoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            thisJoint.transform.parent = this.transform;
            thisJoint.name = joint.Name;
            thisJoint.transform.localScale = Vector3.one * 0.1f;
            Rigidbody rb = thisJoint.AddComponent<Rigidbody>();
            rb.SetDensity(Density);
            thisJoint.transform.localPosition = joint.Loction;
            CharacterJoint CharJoint1 = thisJoint.AddComponent<CharacterJoint>();
            CharacterJoint CharJoint2 = thisJoint.AddComponent<CharacterJoint>();

            CharJoint1.connectedBody = partToGameobject(joint.Connection1).GetComponent<Rigidbody>();
            CharJoint2.connectedBody = partToGameobject(joint.Connection2).GetComponent<Rigidbody>();



            Joint c_Joint = thisJoint.AddComponent<Joint>();
            c_Joint.JointID = thisJoint.GetInstanceID();
            c_Joint.CharacterController1 = CharJoint1;
            c_Joint.CharacterController2 = CharJoint2;

            g_Joints.Add(thisJoint);

        }




    }
    GameObject partToGameobject(CharacterPart part)
    {
        GameObject go = null;
        foreach(Transform child in transform)
        {
            if(child.gameObject.GetInstanceID() == part.ID)
            {
                return child.gameObject;

            }

        }



        return go;


    }

}
