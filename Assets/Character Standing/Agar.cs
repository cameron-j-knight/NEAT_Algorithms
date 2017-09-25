using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;
using System.Collections.Generic;

public class Agar : UnitController {
	
	bool IsRunning = false;
    float vertical = 0;
    float timePassed;
	IBlackBox box;
    IBlackBox internnBox;
	float overTime = 0;
    const float SpringConstant = 500;
    public Transform Head;
    Vector3 HeadStartPosY;
    Dictionary<Transform, List<SpringJoint>> Springs;
    public int[] AvalibleLayers = new int[]{ 1,2,3,4,5}; //5 layers to use
    float Fit = 0;
    float HoldTime = .1f;
    public EditorControler ThisEC;
    public bool HoldGravity = true;
    public bool Real = false;
    public GameObject[] Parts;
    public GameObject[] Joints;
    public GameObject[] Muscles;
    private GameObject MaxPosGO;
    public int NumGroups;
    public float MaxPos = 0;
    public void ActivateLayers(int layers)
    {
        gameObject.layer = AvalibleLayers[layers];
        foreach (Transform child in transform)
        {
            child.gameObject.layer = AvalibleLayers[layers];
        }
    }
    public void SetLayer(int layers)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.layer = layers;
        }
    }
    // Use this for initialization
    void Start () {

        IsRunning = false;
        foreach (GameObject ThisPart in Parts)
        {
            if(ThisPart.transform.position.y > MaxPos)
            {

                MaxPos = ThisPart.transform.position.y;
                MaxPosGO = ThisPart;
            }

        }

        int x = 0;


        //Dictionary<Transform, List<SpringJoint>> Springs = new Dictionary<Transform, List<SpringJoint>>();

        //foreach (Transform child in transform)
        //{

        //    if(child.GetComponent<SpringJoint>() != null)
        //    {
        //        List<SpringJoint> theseJoints = new List<SpringJoint>();
        //        foreach (SpringJoint childJoint in child.GetComponents<SpringJoint>())
        //        {
        //            theseJoints.Add(childJoint);

        //        }

        //        Springs.Add(child, theseJoints);


        //    }

        //}



    }
    float x = 0;
	void FixedUpdate()
	{

        //print("IsRunning");

        //if (Input.GetKey(KeyCode.A))
        //{
        //    if (Input.GetKey(KeyCode.LeftShift))
        //    {
        //        TenseMuscleGroup(1, 1f);

        //    }
        //    else
        //    {
        //        TenseMuscleGroup(1, 1f);

        //    }

        //}



        //if (Input.GetKey(KeyCode.S))
        //{
        //    if (Input.GetKey(KeyCode.LeftShift))
        //    {
        //        TenseMuscleGroup(-2, 1f);

        //    }
        //    else
        //    {
        //        TenseMuscleGroup(2, 1f);

        //    }

        //}

        //if (Input.GetKey(KeyCode.D))
        //{
        //    if (Input.GetKey(KeyCode.LeftShift))
        //    {
        //        TenseMuscleGroup(-3, 1f);
        //        print("Yep");
        //    }
        //    else
        //    {
        //        TenseMuscleGroup(3, 1f);

        //    }

        //}
        //if (Input.GetKey(KeyCode.F))
        //{
        //    if (Input.GetKey(KeyCode.LeftShift))
        //    {
        //        TenseMuscleGroup(-4, 1f);

        //    }
        //    else
        //    {
        //        TenseMuscleGroup(4, 1f);

        //    }

        //}


        //if (Input.GetKey(KeyCode.G))
        //{
        //    if (Input.GetKey(KeyCode.LeftShift))
        //    {
        //        TenseMuscleGroup(-5, 1f);

        //    }
        //    else
        //    {
        //        TenseMuscleGroup(5, 1f);

        //    }

        //}


        //if (Input.GetKey(KeyCode.H))
        //{
        //    if (Input.GetKey(KeyCode.LeftShift))
        //    {
        //        TenseMuscleGroup(-6, 1f);

        //    }
        //    else
        //    {
        //        TenseMuscleGroup(6, 1f);

        //    }

        //}
        //if (Input.GetKey(KeyCode.J))
        //{
        //    if (Input.GetKey(KeyCode.LeftShift))
        //    {
        //        TenseMuscleGroup(-7, 1f);

        //    }
        //    else
        //    {
        //        TenseMuscleGroup(7, 1f);

        //    }

        //}
        //if (Input.GetKey(KeyCode.K))
        //{
        //    if (Input.GetKey(KeyCode.LeftShift))
        //    {
        //        TenseMuscleGroup(-8, 1f);

        //    }
        //    else
        //    {
        //        TenseMuscleGroup(8, 1f);

        //    }

        //}
        //if (Input.GetKey(KeyCode.L))
        //{
        //    if (Input.GetKey(KeyCode.LeftShift))
        //    {
        //        TenseMuscleGroup(-9, 1f);

        //    }
        //    else
        //    {
        //        TenseMuscleGroup(9, 1f);

        //    }

        //}

        //if (Input.GetKey(KeyCode.Z))
        //{
        //    if (Input.GetKey(KeyCode.LeftShift))
        //    {
        //        TenseMuscleGroup(-10, 1f);

        //    }
        //    else
        //    {
        //        TenseMuscleGroup(10, 1f);

        //    }

        //}
        //if (Input.GetKey(KeyCode.X))
        //{
        //    if (Input.GetKey(KeyCode.LeftShift))
        //    {
        //        TenseMuscleGroup(-11, 1f);

        //    }
        //    else
        //    {
        //        TenseMuscleGroup(11, 1f);

        //    }

        //}
        //if (Input.GetKey(KeyCode.C))
        //{
        //    if (Input.GetKey(KeyCode.LeftShift))
        //    {
        //        TenseMuscleGroup(-12, 1f);

        //    }
        //    else
        //    {
        //        TenseMuscleGroup(12, 1f);

        //    }

        //}

        //if (Input.GetKey(KeyCode.V))
        //{
        //    if (Input.GetKey(KeyCode.LeftShift))
        //    {
        //        TenseMuscleGroup(-13, 1f);

        //    }
        //    else
        //    {
        //        TenseMuscleGroup(13, 1f);

        //    }

        //}
        //if (Input.GetKey(KeyCode.B))
        //{
        //    if (Input.GetKey(KeyCode.LeftShift))
        //    {
        //        TenseMuscleGroup(-14, 1f);

        //    }
        //    else
        //    {
        //        TenseMuscleGroup(14, 1f);

        //    }

        //}

        //if (Input.GetKey(KeyCode.N))
        //{
        //    if (Input.GetKey(KeyCode.LeftShift))
        //    {
        //        TenseMuscleGroup(-15, 1f);

        //    }
        //    else
        //    {
        //        TenseMuscleGroup(15, 1f);

        //    }

        //}

        //if (Input.GetKey(KeyCode.M))
        //{
        //    if (Input.GetKey(KeyCode.LeftShift))
        //    {
        //        TenseMuscleGroup(-16, 1f);

        //    }
        //    else
        //    {
        //        TenseMuscleGroup(16, 1f);

        //    }

        //}

        if (true)
		{

            //c_UpdateInterval = UpdateInterval;
            //if (HoldTime > - 5000)
            //{

            //    foreach (Transform child in transform)
            //    {

            //        if (child.GetComponent<Rigidbody>() != null)
            //        {

            //        }
            //    }

            //}



            //HoldTime = -10000;

            ISignalArray inputArr = box.InputSignalArray;

            int i =0;
            foreach (Transform child in transform)
            {
                if (child.GetComponent<RigidBodyData>() != null)
                {
                    inputArr[i] = child.GetComponent<Rigidbody>().angularVelocity.x / 8f;
                    i++;
                    inputArr[i] = child.GetComponent<Rigidbody>().angularVelocity.y / 8f;
                    i++;
                    inputArr[i] = child.GetComponent<Rigidbody>().angularVelocity.z / 8f;
                    i++;
                }

            }

//            if (x >= 1)
//            {
//                x = 0;
//
//            }
//            else
//            {
//                x += .001f;
//
//            }
//            inputArr[0] =x;
//            inputArr[1] = 1f - x;
//

            //for(; i < inputArr[i]; i++)
            //

            //}



            box.Activate();
			
			ISignalArray outputArr = box.OutputSignalArray;

            for (i = 0; i < NumGroups; i++)
               
            {
                if ((float)outputArr[i] > 0.75f)
                {

                    TenseMuscleGroup(i + 1, 1f);
                }
                else if ((float)outputArr[i] < 0.75f && (float)outputArr[i] > 0.5f)
                {
                    TenseMuscleGroup(-i - 1, 1f);
                }
                else
                {
                }

            }

			Fit += transform.GetChild(0).transform.position.magnitude;

//            if (MaxPosGO.transform.position.y + 1f > MaxPos)
//            {
//
//                Fit += Time.deltaTime;
//            }
//            else
//            {
//                Fit += Time.deltaTime * (MaxPosGO.transform.position.y / MaxPos);
//
//            }
//


        }

    }

	
	public override void Stop()
	{
		this.IsRunning = false;
	}


    public float UpdateInterval = .8f;
    float c_UpdateInterval = .8f;

    public override void Activate(IBlackBox box)
	{
		this.box = box;
		this.IsRunning = true;
	}

	
	public override float GetFitness()
	{
        
		return  Mathf.Clamp(Fit , 0f, Mathf.Infinity);
	}

    public void TenseMuscleGroup(int Group, float Power)
    {
        foreach (GameObject muscle in Muscles)
        {
            muscle.GetComponent<BuildMuscle>().Tense(Group, Power);
        }

    }

}
