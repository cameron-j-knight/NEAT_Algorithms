using UnityEngine;
using System.Collections;

public class CameraRotate : MonoBehaviour
{
    public Transform target;
    float distance = 10.0f;

    float xSpeed = 180.0f;
    float ySpeed = 90.0f;

    float yMinLimit = 0.0f;
    float yMaxLimit = 80f;

    float speed = 0.14f;

    private float x = 0.0f;
    private float y = 0.0f;

    float ZoomAmount = 6; //With Positive and negative values
    float MaxToClamp = 30;
    float ROTSpeed = 14;

    public bool Run = true;
    bool Drag = false;
    Vector3 OriginialPos;
    Vector3 OriginialScale;
    Vector3 OriginialRotation;

    public Ray ray;
    Vector2 originalMousePosition;
    Transform Translate;
    Vector3 originalPos;
    Plane movePlane;
    Vector3 OriginalM3Pos;
    Vector3 OriginalTargetpos;
    Vector3 OriginalCameraPosition;
    Vector3 CameraOffset;
    float OriginalDistance;
    bool Pan = false;
    void Start()
    {
        ZoomAmount = Vector3.Distance(transform.position, target.position);
        transform.LookAt(target);

        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        // Make the rigid body not change rotation
        if (gameObject.GetComponent<Rigidbody>())
            gameObject.GetComponent<Rigidbody>().freezeRotation = true;
    }

    void LateUpdate()
    {

        if (Input.GetMouseButtonDown(2))
        {
            Pan = true;
            OriginalM3Pos = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * Vector3.Distance(Camera.main.transform.position, target.transform.position));
            OriginalTargetpos = target.transform.position;
            OriginalDistance = Vector3.Distance(Camera.main.transform.position, target.transform.position);
            OriginalCameraPosition = Camera.main.transform.position;
        }
        Vector3 pos;
        if (Input.GetMouseButton(2))
        {



            Vector3 mousePos = Input.mousePosition;
           // mousePos.z = 1;

            pos = Camera.main.ScreenToWorldPoint(mousePos + Vector3.forward * OriginalDistance);

            Vector3 deltaPos = new Vector3(OriginalM3Pos.x - pos.x, OriginalM3Pos.y - pos.y, OriginalM3Pos.z - pos.z);
            Camera.main.transform.position += deltaPos;

            target.transform.position += deltaPos;













            // Camera.main.transform.position = OriginalCameraPosition - Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * OriginalDistance);

            //target.transform.position = OriginalTargetpos - Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * OriginalDistance);

        }
        else {
            Pan = false;
        }






        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit) || Drag)
        {

            if (Run && !Pan)
            {
                ZoomAmount -= Input.GetAxis("Mouse ScrollWheel") * 2;
                ZoomAmount = Mathf.Clamp(ZoomAmount, -MaxToClamp, MaxToClamp);
                float translate = Mathf.Min(Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")), MaxToClamp - Mathf.Abs(ZoomAmount));
                gameObject.transform.Translate(0, 0, translate * ROTSpeed * Mathf.Sign(Input.GetAxis("Mouse ScrollWheel")));

                if (target && Input.GetMouseButton(0))
                {
                    x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                    y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                    // y = Mathf.Clamp(y, yMinLimit, yMaxLimit);

                    Quaternion rotation = Quaternion.Euler(y, x, 0);
                    Vector3 position = rotation * new Vector3(0.0f, 0.0f, -ZoomAmount) + target.position;

                    transform.rotation = rotation;
                    transform.position = position;

                }
                else
                {




                    Quaternion rotation = Quaternion.Euler(y, x, 0);
                    Vector3 position = rotation * new Vector3(0.0f, 0.0f, -ZoomAmount) + target.position;

                    transform.rotation = rotation;
                    transform.position = position;

                }
            }

        }
    }


    void Update()
    {


        if (!Pan) { 
            //    movePlane = new Plane(-Camera.main.transform.forward, Vector3.up * -100f);


            //}
            //if (Input.GetMouseButton(2))
            //{
            //    Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            //    float hitDist;
            //    if (movePlane.Raycast(camRay, out hitDist))
            //        target.position = camRay.GetPoint(hitDist);
            //}











            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool OkToMove = false;
            if (Physics.Raycast(ray, out hit))
            {
                OkToMove = false;
            }
            else
            {
                OkToMove = true;
            }

            if (OkToMove && Run)
            {
                transform.LookAt(target);

                // transform.position -= transform.right * speed * Time.deltaTime;

                if (Input.GetMouseButton(0))
                {
                    Drag = true;
                    transform.LookAt(target);
                    transform.RotateAround(target.position, Vector3.up, Input.GetAxis("Mouse X") * speed);
                }

            }

        }

        if (Input.GetMouseButtonUp(0))
        {
            Drag = false;

        }
    }
}