using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PartCubeControler : MonoBehaviour {
    public List<UndoObjectData> UndoData;
    public int IDXMLSAVEDATA;

    int CurrentStep;
    public void SetActiveClick(bool State)
    {

        transform.GetComponent<Collider>().isTrigger = State;


    }
    // TODO make Class that holds save info for a part
    public void SaveInfo()
    {



    }


    public void Undo()
    {
        //if (CurrentStep > 0)
        //{
            //transform.position = UndoData[CurrentStep]._position;
            //transform.localScale = UndoData[CurrentStep]._scale;
            //transform.eulerAngles = UndoData[CurrentStep]._rotation;

            //SetActiveClick(UndoData[CurrentStep]._selected);
            //CurrentStep--;
        //}
        //else { Destroy(this.gameObject); }
    }

    public void SaveStep()
    {
        //if(CurrentStep == UndoData.Count - 1 )
        //{
        //UndoData.Add(new UndoObjectData(transform, GetComponent<Collider>().isTrigger));
        //CurrentStep++;
        //}
        //else
        //{

            //if(CurrentStep > 0)
            //UndoData.RemoveRange(CurrentStep, UndoData.Count - 1);

            //UndoData.Add(new UndoObjectData(transform, GetComponent<Collider>().isTrigger));
            //CurrentStep = UndoData.Count - 1;
        //}
   
    }


    // Use this for initialization
    void Start () {
        UndoData = new List<UndoObjectData>();

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
