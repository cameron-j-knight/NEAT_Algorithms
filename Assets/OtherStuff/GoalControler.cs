using UnityEngine;
using System.Collections;

public class GoalControler : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
    Vector2 GoalPos = Vector2.zero;
	// Update is called once per frame
	void Update () {
        if(Mathf.Round((transform.position.x - GoalPos.x) / 10f) == 0 && Mathf.Round((transform.position.y - GoalPos.y) / 10f) == 0)
        {
            GoalPos = new Vector3(Random.Range(-18f, 18f), Random.Range(-10f, 10f), -3.72f);
        }
  

        transform.position = Vector2.Lerp(transform.position,GoalPos, Time.deltaTime * .5f);


    }
}
