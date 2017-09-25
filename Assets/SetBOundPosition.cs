using UnityEngine;
using System.Collections;

public class SetBOundPosition : MonoBehaviour {
    Screen_Bounds sb;
    public Vector2 pos;


	// Use this for initialization
	void Start () {
        sb = GetComponent<Screen_Bounds>();
        Vector2 position = transform.position;
        Vector2 scale = transform.localScale;
        if (pos.x != 0)

            position.x = (sb.rightConstraint * pos.x) + 1 * Mathf.Sign(pos.x);

        if(pos.y !=0)
            position.y = sb.rightConstraint * pos.y + 1 * Mathf.Sign(pos.x);

        scale.x = Mathf.Lerp(1, 22, Mathf.Abs(pos.x));
        scale.y = Mathf.Lerp(1,22, Mathf.Abs(pos.y));

        transform.position = pos;
        transform.localScale = scale;

    }


}
