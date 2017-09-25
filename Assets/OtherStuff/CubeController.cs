using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;

public class CubeController : UnitController
{

    public float Speed = 5f;
    public float TurnSpeed = 180f;
    public int Lap = 1;
    public int CurrentPiece, LastPiece;
    bool MovingForward = true;
    bool IsRunning;
    public float SensorRange = 10;
    int WallHits;
    IBlackBox box;
    float timeStart = 0.001f; 
    private GameObject Goal;
    float fitnessVal = 0f;
    float fitnessVal2 = 0f;
    bool found = false;
    // Use this for initialization
    void Start()
    {
        Goal = GameObject.Find("Goal");
       Goal.transform.position = new Vector3(Random.Range(-18f, 18f), Random.Range(-10f, 10f), -3.72f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (IsRunning)
        {
            fitnessVal2 += Vector2.Distance(transform.position, Goal.transform.position);

            //if (!found)
            //{
            //    timeStart += .2f;
            //    if (Mathf.Round(Vector2.Distance(transform.position, Goal.transform.position)) <= 0)
            //    {
            //        found = true;
            //    }
            //}
            //else
            //{
            //    if (Mathf.Round(Vector2.Distance(transform.position, Goal.transform.position)) > 0)
            //    {
            //        found = false;
            //    }

            //}
            ISignalArray inputArr = box.InputSignalArray;
            inputArr[0] = (Mathf.Sign(transform.position.x - Goal.transform.position.x) + 1f)/2f; //is left
            inputArr[1] = (Mathf.Sign(transform.position.x - Goal.transform.position.x) - 1f)/-2f;// is right
            inputArr[2] = (Mathf.Sign(transform.position.y - Goal.transform.position.y) + 1f) / 2f; //is up
            inputArr[3] = (Mathf.Sign(transform.position.y - Goal.transform.position.y) - 1f) / -2f;// is down
            inputArr[4] = Vector2.Distance(transform.position, Goal.transform.position) / 15f;

            box.Activate();

            ISignalArray outputArr = box.OutputSignalArray;

            transform.Translate((float)outputArr[0] -.5f, (float)outputArr[1] -.5f, 0 );
        }
    }

    public override void Stop()
    {
        this.IsRunning = false;
    }

    public override void Activate(IBlackBox box)
    {
        this.box = box;
        this.IsRunning = true;
    }

    public void NewLap()
    {
        if (LastPiece > 2 && MovingForward)
        {
            Lap++;
        }
    }

    public override float GetFitness()
    {
        //return Mathf.Clamp(Fusaloge.transform.position.y, 0, 10000);
        return Mathf.Abs(Mathf.Clamp(100000f - fitnessVal2, 0f, 100000f));
    }


    //void OnGUI()
    //{
    //    GUI.Button(new Rect(10, 200, 100, 100), "Forward: " + MovingForward + "\nPiece: " + CurrentPiece + "\nLast: " + LastPiece + "\nLap: " + Lap);
    //}

}
