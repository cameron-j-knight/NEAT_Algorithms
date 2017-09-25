using UnityEngine;
using System.Collections;

public class Muscle{
    public float PowerConstant = 100000;
    public bool Spin = false; // Type of Muscle; false for negative motion True For Positive Only One can be made at a time;
    public float Strength = 0; //The Speed in which the muscle contracts;
    public Vector3 Position1;
    public Vector3 Position2;
    public GameObject ConnectedJoint;
    public ConfigurableJoint MuscleJoint;
    // Use this for initialization
    public Muscle()
    {


    }
    public Muscle(bool _Spin, float _Strength, Vector3 pos1, Vector3 pos2, GameObject _ConnectedJoint)
    {
        Spin = _Spin;
        Strength = _Strength;
        Position1 = pos1;
        Position2 = pos2;
        ConnectedJoint = _ConnectedJoint;

        CreateMuscle();

    }
    public void CreateMuscle() {
        Position1 = Position1.normalized * .5f;
        Position2 = Position2.normalized * .5f;
        CharacterJoint[] Cj = ConnectedJoint.GetComponents<CharacterJoint>();
        if(Cj.Length  == 2)
        {
            MuscleJoint = Cj[0].connectedBody.gameObject.AddComponent<ConfigurableJoint>();
            if (Position2 == Vector3.zero)
            {
                MuscleJoint.autoConfigureConnectedAnchor = true;
            }
            else
            {

                MuscleJoint.autoConfigureConnectedAnchor = false;

            }
            MuscleJoint.connectedBody = Cj[1].connectedBody;

           Vector3 CjBound1 = Cj[0].connectedBody.GetComponent<Collider>().bounds.size.normalized / 4f;
           Vector3 CjBound2 = Cj[1].connectedBody.GetComponent<Collider>().bounds.size.normalized / 4f;

            Vector3 Cjscale1 = Cj[0].connectedBody.transform.lossyScale;
            Vector3 Cjscale2 = Cj[1].connectedBody.transform.lossyScale;

            MuscleJoint.anchor = new Vector3(Position1.x * CjBound1.x, Position1.y * CjBound1.y, Position1.z * CjBound1.z);
            MuscleJoint.connectedAnchor = new Vector3(Position1.x * CjBound2.x, Position1.y * CjBound2.y, Position1.z * CjBound2.z);



        }
        else
        {
            MuscleJoint = null;
        }
	}

    public void Tense(bool Spin, float Power)
    {
      if(MuscleJoint != null && Spin == Spin)
        {
            JointDrive JD = new JointDrive();
            JD.positionSpring = Strength * PowerConstant * Power;
            JD.maximumForce = Mathf.Infinity;
            MuscleJoint.xDrive = MuscleJoint.yDrive = MuscleJoint.zDrive = JD;


        }
      else
        {
            JointDrive JD = new JointDrive();
            JD.positionSpring = 0;
            JD.maximumForce = Mathf.Infinity;
            MuscleJoint.xDrive = MuscleJoint.yDrive = MuscleJoint.zDrive = JD;
        }


    }

}
