using UnityEngine;
using System.Collections;

public class MusclePart : MonoBehaviour {
    public GameObject Connection1;
    public GameObject Connection2;
    public int IDXMLSAVEDATA;

    public int AssociatedGroup = 0;

    public float Strength = 1.0f;

    public Vector3 ReleativeConnection1 = Vector3.zero;
    public Vector3 ReleativeConnection2 = Vector3.zero;

    public void SetGroup(int Num)
    {

        AssociatedGroup = Num;
        if(Num >= 0)
        {
            gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.red);
        }
        else
        {

            gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.blue);
        }
        float ColorHValue = 0;

        ColorHValue = ((Mathf.Abs(Num) % 15) / 15f) + (Mathf.Abs(Num) * (.01f));
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", new ColorHSV(Mathf.Clamp(ColorHValue, 0f,1f),1f,1f,1f));
        if(Num == 0)
        {
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);


        }
    }

}
