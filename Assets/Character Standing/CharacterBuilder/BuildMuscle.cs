using UnityEngine;
using System.Collections;

public class BuildMuscle : MonoBehaviour {
    const float PowerConstant = 20;
    public GameObject Connection1;
    public GameObject Connection2;
    public int IDXMLSAVEDATA;

    public int AssociatedGroup = 0;

    public float Strength = 1.0f;

    public Vector3 ReleativeConnection1 = Vector3.zero;
    public Vector3 ReleativeConnection2 = Vector3.zero;

    public ConfigurableJoint MuscleJoint;


    Vector3 Pos1;
    Vector3 Pos2;

    void Update(){
        Pos1 = Connection1.transform.TransformPoint(ReleativeConnection1);
        Pos2 = Connection2.transform.TransformPoint(ReleativeConnection2);

        this.transform.position = Vector3.Lerp(Pos1, Pos2, .5f);

        Vector3 Relative = (Pos2 - this.transform.position).normalized;
        //float z = 0;// Mathf.Atan2(Relative.z, Relative.y);
        //float y = 180 / (2 * Mathf.PI) - Mathf.Atan2(Relative.z, Relative.x);
        //float x = Mathf.Atan2(Relative.y, Relative.x);
        if(Relative != Vector3.zero)
        {
            Quaternion ThisRotaion = Quaternion.LookRotation(Relative);

            this.transform.rotation = ThisRotaion;
        }


        //  thisPart.transform.localEulerAngles = new Vector3(x, 0,z) * (360 / (2 * Mathf.PI));
        this.transform.localScale = Vector3.forward * Vector3.Distance(Pos1, Pos2) * .12f + new Vector3(transform.localScale.x, transform.localScale.y,0 );




    }





    public void SetGroup(int Num)
    {

        AssociatedGroup = Num;
        if (Num >= 0)
        {
            gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.red);
        }
        else
        {

            gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.blue);
        }
        float ColorHValue = 0;

        ColorHValue = ((Mathf.Abs(Num) % 15) / 15f) + (Mathf.Abs(Num) * (.01f));
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", new ColorHSV(Mathf.Clamp(ColorHValue, 0f, 1f), 1f, 1f, 1f));
        if (Num == 0)
        {
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);


        }
    }

    public void Tense(int Group, float power)
    {
        if(AssociatedGroup == Group)
        {
            ConfigurableJoint mj = MuscleJoint;

            JointDrive JD = new JointDrive();
            JD.positionSpring = Strength * power * PowerConstant;
            JD.maximumForce = Mathf.Infinity;
            mj.xDrive = mj.yDrive = mj.zDrive = JD;
            mj.gameObject.GetComponent<Rigidbody>().WakeUp();

            StartCoroutine(Falloff(Strength * power * PowerConstant));
        }

    }
    float StartPower;
    public IEnumerator Falloff(float Power)
    {
        StartPower = Power;
        while(StartPower > 0f)
        {
            StartPower -= Power / 30f;

            ConfigurableJoint mj = MuscleJoint;

            JointDrive JD = new JointDrive();
            JD.positionSpring = StartPower;
            JD.maximumForce = Mathf.Infinity;
            mj.xDrive = mj.yDrive = mj.zDrive = JD;
            mj.gameObject.GetComponent<Rigidbody>().WakeUp();

            yield return new WaitForFixedUpdate();
        }

    
    }
}
