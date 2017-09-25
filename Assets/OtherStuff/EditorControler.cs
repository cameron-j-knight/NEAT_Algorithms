using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

public class EditorControler : MonoBehaviour {
    public GameObject CharacterEditorGO;
    public GameObject MainEditor;
    public SizingControler SizeCont;
    public GameObject PartPrefab;
    public GameObject JointPrefab;
    public GameObject FakeJointPrefab;

    public GameObject ButtonMuscle;
    public GameObject ButtonParts;
    public GameObject ButtonJoint;
    public GameObject ButtonGroup;

    public GameObject FileLocationBox;


    public GameObject CharNameBoxLoad;
    public GameObject CharNameBoxSave;



    public GameObject OverwriteQuestion;
    public GameObject SaveDialog;
    public GameObject NewCharDialog;
    public GameObject LoadDialog;


    public GameObject[] CubeButtons;
    public GameObject[] JointButtons;

    public GameObject[] MuscleButtons;
    public GameObject[] GroupButtons;

    public GameObject BuildingCharacter;

    public List<GameObject> L_PartCubes;
    public List<GameObject> L_PartJoints;
    public List<GameObject> L_FakeJoints;


    public int Mode = 0;

    bool Editing = false;





    public void NewCharacter()
    {
        PartMode();
        Camera.main.GetComponent<CameraRotate>().Run = true;

        if (L_PartCubes.Count > 0)
            foreach (GameObject x in L_PartCubes)
            {
                Destroy(x);

            }
        if (L_PartJoints.Count > 0)
            foreach (GameObject x in L_PartJoints)
            {
                Destroy(x);

            }
        if (L_FakeJoints.Count > 0)
            foreach (GameObject x in L_FakeJoints)
            {
                Destroy(x);
            }
        if (L_MusclePart.Count > 0)
            foreach (GameObject x in L_MusclePart)
            {
                Destroy(x);
            }

        L_MusclePart.Clear();
        L_PartJoints.Clear();
        L_PartCubes.Clear();
        L_FakeJoints.Clear();


    }






















    // Use this for initialization
    void Start () {

        StructureFileSavePath = Application.persistentDataPath + string.Format("/{0}.Structure.xml", StructureName);
        StructureXml = new XmlDocument();

      //  FileLocationBox.GetComponentInChildren<Text>().text = StructureFileSavePath;


        L_PartCubes = new List<GameObject>();
        L_PartJoints = new List<GameObject>();
        if (SizeCont == null)
        {
            SizeCont = GameObject.Find("SizingAparatus").GetComponent<SizingControler>(); ;

        }
        if (MainEditor == null)
        {
            MainEditor = GameObject.Find("NormalView");

        }
        if (BuildingCharacter == null)
        {
            BuildingCharacter = GameObject.Find("BuildingCharacter");

        }
        if (CharacterEditorGO == null)
        {
            CharacterEditorGO = GameObject.Find("CharacterEditor");
            CharacterEditorGO.SetActive(false);

        }
        JointMode();
        PartMode();

    }


    public void RunCamera()
    {
        if (Camera.main.GetComponent<CameraRotate>())
        {
            Camera.main.GetComponent<CameraRotate>().Run = true;

        }


    }

    public void HoldCamera()
    {
        if (Camera.main.GetComponent<CameraRotate>())
        {
            Camera.main.GetComponent<CameraRotate>().Run = false;

        }

    }
    // Update is called once per frame

    /// <summary>
    /// This is where the character is going to be built Joints and all
    /// </summary>


    AgarOptimizer ThisOptimizer;

    public List<GameObject> L_BuildParts;
    public List<GameObject> L_BuildJoints;
    public List<GameObject> L_BuildMuscles;

    public float Density = .005f;
    public GameObject BuildCharacterObject;

    public GameObject RotateObject;
    public void BuildCharacter()
    {
        StructureXml = new XmlDocument();
        if (File.Exists(StructureFileSavePath))
        {
            if(BuildCharacterObject != null)
            {
                Destroy(BuildCharacterObject);
            }

            foreach(GameObject go in L_BuildJoints.ToArray())
            {
                Destroy(go);
            }
            L_BuildJoints.Clear();

            foreach (GameObject go in L_BuildMuscles.ToArray())
            {
                Destroy(go);
            }
            L_BuildMuscles.Clear();

            foreach (GameObject go in L_BuildParts)
            {
                Destroy(go);
            }
            L_BuildParts.Clear();
            RotateObject.GetComponent<Rotate>().enabled = false;
            RotateObject.transform.eulerAngles = Vector3.zero;
            foreach (Transform Child in RotateObject.transform)
            {
                Destroy(Child.gameObject);

            }


            BuildCharacterObject = new GameObject();
            BuildCharacterObject.name = StructureName;
            BuildCharacterObject.transform.localScale = Vector3.one * 4;

            StructureXml.Load(StructureFileSavePath);
            print("Building...");
            
            foreach (XmlNode t_Part in StructureXml.SelectNodes("Root/PartSetting/Part"))
            {
                print("Building... Part");

                BuildCubeXml(DeserializeVector3Array(t_Part.Attributes.GetNamedItem("Location").Value), DeserializeVector3Array(t_Part.Attributes.GetNamedItem("Rotation").Value), DeserializeVector3Array(t_Part.Attributes.GetNamedItem("Scale").Value), int.Parse(t_Part.Attributes.GetNamedItem("Id").Value),BuildCharacterObject,true);

            }
            foreach (XmlNode t_joint in StructureXml.SelectNodes("Root/JointSetting/Joint"))
            {
                print("Building... Joint");

                BuildJointXml(DeserializeVector3Array(t_joint.Attributes.GetNamedItem("Location").Value), DeserializeVector3Array(t_joint.Attributes.GetNamedItem("Rotation").Value), DeserializeVector3Array(t_joint.Attributes.GetNamedItem("Scale").Value)
                                , int.Parse(t_joint.Attributes.GetNamedItem("Connection1").Value), int.Parse(t_joint.Attributes.GetNamedItem("Connection2").Value), int.Parse(t_joint.Attributes.GetNamedItem("Id").Value),
                                float.Parse(t_joint.Attributes.GetNamedItem("HighTwistLimit").Value), float.Parse(t_joint.Attributes.GetNamedItem("LowTwistLimit").Value), float.Parse(t_joint.Attributes.GetNamedItem("Swing1Limit").Value), float.Parse(t_joint.Attributes.GetNamedItem("Swing2Limit").Value), BuildCharacterObject, true);
            }



            foreach (XmlNode t_muscle in StructureXml.SelectNodes("Root/MuscleSetting/Muscle"))
            {
                print("Building... Muscle");

                BuildMuscleXml(int.Parse(t_muscle.Attributes.GetNamedItem("Connection1ObjId").Value), int.Parse(t_muscle.Attributes.GetNamedItem("Connection2ObjId").Value)
                    , DeserializeVector3Array(t_muscle.Attributes.GetNamedItem("Connection1Pos").Value), DeserializeVector3Array(t_muscle.Attributes.GetNamedItem("Connection2Pos").Value)
                    , int.Parse(t_muscle.Attributes.GetNamedItem("Group").Value), float.Parse(t_muscle.Attributes.GetNamedItem("Stregth").Value), int.Parse(t_muscle.Attributes.GetNamedItem("Id").Value), BuildCharacterObject, true

                    );
            }
            buildStandee();

            ThisOptimizer = Object.FindObjectOfType<AgarOptimizer>();
            List<int> groupsAdded = new List<int>();
            int NumGroups = 0;
            foreach(GameObject Muscle in L_BuildMuscles.ToArray())
            {
               int GroupNum = Mathf.Abs(Muscle.GetComponent<BuildMuscle>().AssociatedGroup);

                if (!groupsAdded.Contains(GroupNum))
                {
                    groupsAdded.Add(GroupNum);

                    NumGroups++;
                }

            }

           Agar ThisUCont =  BuildCharacterObject.AddComponent<Agar>();

            ThisUCont.AvalibleLayers = new int[]{ 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 };
            ThisUCont.HoldGravity = false;
            ThisUCont.ThisEC = this;
            ThisUCont.Parts = L_BuildParts.ToArray();
            ThisUCont.Joints = L_BuildJoints.ToArray();
            ThisUCont.Muscles = L_BuildMuscles.ToArray();

            
            ThisUCont.NumGroups = NumGroups;

            ThisOptimizer.NUM_INPUTS = 2;
            ThisOptimizer.NUM_OUTPUTS = NumGroups;
            ThisOptimizer.Unit = BuildCharacterObject;
            ThisOptimizer.NodeViewer.GetComponent<GenomeViewController>().UpdateXml();

            ThisOptimizer.expiramentName = StructureName + "_Exprirament";

            List<Vector3> MusclePos = new List<Vector3>();
            List<Vector3> NormalizedMuclePositions = new List<Vector3>();

            for(int i = 1; i < NumGroups + 1; i++)
            {
                int NumInGroup = 0;
                Vector3 MuscleValue = Vector3.zero;
                foreach (GameObject Muscle in L_BuildMuscles)
                {
                    if(Mathf.Abs( Muscle.GetComponent<BuildMuscle>().AssociatedGroup) == i)
                    {
                        MuscleValue += Muscle.transform.localPosition;
                        NumInGroup++;

                    }

                }
                if(NumInGroup > 0)
                {
                    MusclePos.Add(MuscleValue / (float)NumInGroup);
                }

            }


            float MaxPosx = Mathf.NegativeInfinity;
            float MaxPosy = Mathf.NegativeInfinity;
            float MaxPosz = Mathf.NegativeInfinity;
            float MinPosx = Mathf.Infinity;
            float MinPosy = Mathf.Infinity;
            float MinPosz = Mathf.Infinity;

            foreach (Vector3 Muscle in MusclePos)
            {
                if(Muscle.x > MaxPosx)
                {
                    MaxPosx = Muscle.x;
                }
                if (Muscle.y > MaxPosy)
                {
                    MaxPosy = Muscle.y;
                }
                if (Muscle.z > MaxPosz)
                {
                    MaxPosz = Muscle.z;
                }

                if (Muscle.x < MinPosx)
                {
                    MinPosx = Muscle.x;
                }
                if (Muscle.y < MinPosy)
                {
                    MinPosy = Muscle.y;
                }
                if (Muscle.z < MinPosz)
                {
                    MinPosz = Muscle.z;
                }

            }
            foreach (Vector3 Muscle in MusclePos)
            {
                float x = 0;
                float y = 0;
                float z = 0;
                if(Muscle.x > 0)
                {
                    x = Muscle.x / MaxPosx;
                }
                else
                {
                    x = -(Muscle.x / MinPosx);
                }

                if (Muscle.y > 0)
                {
                    y = Muscle.y / MaxPosy;
                }
                else
                {
                    y = -(Muscle.y / MinPosy);
                }

                if (Muscle.z > 0)
                {
                    z = Muscle.z / MaxPosz;
                }
                else
                {
                    z = -(Muscle.z / MinPosz);
                }

                Vector3 normalPostion = new Vector3(x,y,z);

                NormalizedMuclePositions.Add(normalPostion);
            }

            ThisOptimizer.Positions = NormalizedMuclePositions.ToArray();
            ThisUCont.SetLayer(8);
            ThisUCont.enabled = true;

            //foreach (Transform child in ThisUCont.transform)
            //{

            //    if (child.GetComponent<Rigidbody>() != null)
            //    {
            //        // child.GetComponent<Rigidbody>().maxAngularVelocity = 500f;
            //        //child.GetComponent<Rigidbody>().mass = child.GetComponent<Rigidbody>().mass * .01f;

            //        child.GetComponent<Rigidbody>().useGravity = false;
            //    }
            //}
            ThisOptimizer.Initialize();
            BuildCharacterObject.SetActive(false);
        }
        else
        {
            ErrorNoNameExists.SetActive(true);

        }



    }




    void buildStandee()
    {
        print("Building...");
        foreach (XmlNode t_Part in StructureXml.SelectNodes("Root/PartSetting/Part"))
        {
            print("Building... Part");

            BuildCubeXml(DeserializeVector3Array(t_Part.Attributes.GetNamedItem("Location").Value), DeserializeVector3Array(t_Part.Attributes.GetNamedItem("Rotation").Value), DeserializeVector3Array(t_Part.Attributes.GetNamedItem("Scale").Value), int.Parse(t_Part.Attributes.GetNamedItem("Id").Value), RotateObject, false);

        }
        foreach (XmlNode t_joint in StructureXml.SelectNodes("Root/JointSetting/Joint"))
        {
            print("Building... Joint");

            BuildJointXml(DeserializeVector3Array(t_joint.Attributes.GetNamedItem("Location").Value), DeserializeVector3Array(t_joint.Attributes.GetNamedItem("Rotation").Value), DeserializeVector3Array(t_joint.Attributes.GetNamedItem("Scale").Value)
                            , int.Parse(t_joint.Attributes.GetNamedItem("Connection1").Value), int.Parse(t_joint.Attributes.GetNamedItem("Connection2").Value), int.Parse(t_joint.Attributes.GetNamedItem("Id").Value),
                            float.Parse(t_joint.Attributes.GetNamedItem("HighTwistLimit").Value), float.Parse(t_joint.Attributes.GetNamedItem("LowTwistLimit").Value), float.Parse(t_joint.Attributes.GetNamedItem("Swing1Limit").Value), float.Parse(t_joint.Attributes.GetNamedItem("Swing2Limit").Value), RotateObject, false);
        }
        RotateObject.GetComponent<Rotate>().enabled = true;
    }



    void BuildCubeXml(Vector3 location, Vector3 rotation, Vector3 scale, int CubeID, GameObject Partent, bool Add)
    {
        GameObject thisPart = GameObject.CreatePrimitive(PrimitiveType.Cube);
        thisPart.transform.parent = Partent.transform;
        thisPart.name = "Part";
        thisPart.transform.localScale = scale;
        thisPart.transform.eulerAngles = rotation;

        Rigidbody rb = thisPart.AddComponent<Rigidbody>();
        BuildPart Bp = thisPart.AddComponent<BuildPart>();

        rb.mass = rb.transform.localScale.magnitude * (Density);
        rb.drag = 0.25f;
        rb.angularDrag = 0.25f;

        thisPart.transform.localPosition = location;
        Bp.IDXMLSAVEDATA = CubeID;

        if (Add)
        {
            L_BuildParts.Add(thisPart);
        }
        else
        {
            rb.isKinematic = true;
            thisPart.GetComponent<Collider>().enabled = false;

        }


    }


    void BuildJointXml(Vector3 Location, Vector3 Rotation, Vector3 Scale, int Connection1ID, int Connection2ID, int IDJoint, float LowTwistLimit, float HighTwistLimit, float Swing1Limit, float Swing2Limit, GameObject Partent, bool Add)
    {
        GameObject thisJoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        thisJoint.transform.parent = Partent.transform;
        thisJoint.name = "Joint";
        Rigidbody rb = thisJoint.AddComponent<Rigidbody>();
        BuildJoint Bj = thisJoint.AddComponent<BuildJoint>();

        rb.mass = (Density) * .1f;
        rb.drag = 0.25f;
        rb.angularDrag = 0.25f; thisJoint.transform.localPosition = Location;
        thisJoint.transform.localScale = Scale;
        thisJoint.transform.eulerAngles = Rotation;


        CharacterJoint CharJoint1 = thisJoint.AddComponent<CharacterJoint>();
        CharacterJoint CharJoint2 = thisJoint.AddComponent<CharacterJoint>();

        CharJoint1.anchor = Vector3.zero;
        CharJoint2.anchor = Vector3.zero;

        GameObject ConnectedPart1 = null;
        GameObject ConnectedPart2 = null;

        foreach (GameObject Go in L_BuildParts)
        {
            if (Go.GetComponent<BuildPart>().IDXMLSAVEDATA == Connection1ID)
            {
                ConnectedPart1 = Go;


            }
            if (Go.GetComponent<BuildPart>().IDXMLSAVEDATA == Connection2ID)
            {
                ConnectedPart2 = Go;


            }

        }

        Bj.IDXMLSAVEDATA = IDJoint;

        Bj.ConnectedPart1 = ConnectedPart1;
        Bj.ConnectedPart2 = ConnectedPart2;

        Bj.LowTwistLimit = LowTwistLimit;
        Bj.HighTwistLimit = HighTwistLimit;
        Bj.Swing1Limit = 0;
        Bj.Swing2Limit = 0;




        CharJoint1.connectedBody = ConnectedPart1.GetComponent<Rigidbody>();
        CharJoint2.connectedBody = ConnectedPart2.GetComponent<Rigidbody>();

        thisJoint.GetComponent<Collider>().enabled = false;

        Joint c_Joint = thisJoint.AddComponent<Joint>();
        c_Joint.JointID = thisJoint.GetInstanceID();
        c_Joint.CharacterController1 = CharJoint1;
        c_Joint.CharacterController2 = CharJoint2;
        if (Add)
        {
            L_BuildJoints.Add(thisJoint);
        }
        else
        {
            rb.isKinematic = true;

        }

    }
    void BuildMuscleXml(int Part1id, int Part2id, Vector3 Pos1, Vector3 Pos2, int Groupid, float Stregnth, int ID, GameObject Partent, bool Add)
    {
        GameObject thisMuscle = (GameObject)Instantiate(MusclePrefab, Vector3.up, MusclePrefab.transform.rotation); 
        thisMuscle.transform.parent = Partent.transform;
        thisMuscle.name = "Muscle";

        BuildMuscle Bm = thisMuscle.AddComponent<BuildMuscle>();


        GameObject Part1 = null;
        GameObject Part2 = null;

        foreach (GameObject Go in L_BuildParts)
        {
            if (Go.GetComponent<BuildPart>().IDXMLSAVEDATA == Part1id)
            {
                Part1 = Go;


            }
            if (Go.GetComponent<BuildPart>().IDXMLSAVEDATA == Part2id)
            {
                Part2 = Go;


            }

        }
        ConfigurableJoint configJoint = Part1.AddComponent<ConfigurableJoint>();
        configJoint.xMotion = ConfigurableJointMotion.Limited;
        configJoint.yMotion = ConfigurableJointMotion.Limited;
        configJoint.zMotion = ConfigurableJointMotion.Limited;
        SoftJointLimit lim = new SoftJointLimit();
        lim.limit = 500;
        configJoint.linearLimit = lim;
        configJoint.enableCollision = true;
        Vector3 p1 = Pos1;
        Vector3 p2 = Pos2;

        Pos1 = Part1.transform.TransformPoint(Pos1);
        Pos2 = Part2.transform.TransformPoint(Pos2);


        //    print("p1: " + p1 + " p2: " + p2 + " Pos1: " + Pos1 + " Pos2: " + Pos2);
        thisMuscle.GetComponent<Collider>().enabled = false;
        
        thisMuscle.transform.position = Vector3.Lerp(Pos1, Pos2, .5f);
        Vector3 Relative = (Pos2 - thisMuscle.transform.position).normalized;
        //float z = 0;// Mathf.Atan2(Relative.z, Relative.y);
        //float y = 180 / (2 * Mathf.PI) - Mathf.Atan2(Relative.z, Relative.x);
        //float x = Mathf.Atan2(Relative.y, Relative.x);

        Quaternion ThisRotaion = Quaternion.LookRotation(Relative);

        thisMuscle.transform.rotation = ThisRotaion;

        //  thisPart.transform.localEulerAngles = new Vector3(x, 0,z) * (360 / (2 * Mathf.PI));
        thisMuscle.transform.localScale = Vector3.one * Vector3.Distance(Pos1, Pos2) * .2f;


        Bm.Connection1 = Part1;
        Bm.Connection2 = Part2;
        Bm.Strength = Stregnth;
        Bm.IDXMLSAVEDATA = ID;
        Bm.SetGroup(Groupid);
        Bm.MuscleJoint = configJoint;
        Bm.ReleativeConnection1 = p1;
        Bm.ReleativeConnection2 = p2;

        configJoint.connectedBody = Part2.GetComponent<Rigidbody>();
        configJoint.anchor = p1;
        configJoint.autoConfigureConnectedAnchor = false;
        configJoint.connectedAnchor = p2;
        if (Add)
        {
            L_BuildMuscles.Add(thisMuscle);
        }
        else
        {
  

        }
    }




































    void Update () {
        if (Editing)
        {

            if(SaveDialog.active || NewCharDialog.active || LoadDialog.active || OverwriteQuestion.active)
            {
                Camera.main.GetComponent<CameraRotate>().Run = false;


            }

        



            if (Mode == 0)
            {
                RunCharacterEditor();

                if (Input.GetKeyDown(KeyCode.Z))
                {
                    foreach (GameObject pcc in L_PartCubes.ToArray())
                    {
                        pcc.GetComponent<PartCubeControler>().Undo();


                    }

                }


                if (Input.GetMouseButtonUp(0))
                {
                    UpdateJoints();
                    UpdateJoints();
                    foreach (GameObject pcc in L_PartCubes.ToArray())
                    {
                        pcc.GetComponent<PartCubeControler>().SaveStep();


                    }

                }
            }
            else if(Mode == 1)
            {
                //Do Joint Stuff
                JointStuff();
            }
            else if(Mode == 2)
            {
                //Do Muscle Stuff
                MuscleStuff();
            }
            else if (Mode == 3)
            {
                //Do Muscle Stuff
                GroupStuff();
            }

        }
    }
    public int GroupNumAssign;
    public Text GroupText;
    public void PlusGroupNum()
    {

        GroupNumAssign ++;
        GroupText.text = GroupNumAssign.ToString();
    }
    public void MinusGroupNum()
    {
        GroupNumAssign --;
        GroupText.text = GroupNumAssign.ToString();


    }

    void GroupStuff()
    {


        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.tag == "Muscle")
                {
                    hit.transform.GetComponent<MusclePart>().SetGroup(GroupNumAssign);
                }
            }

        }


    }








































    GameObject P1Select;
    GameObject P2Select;
    Vector3 P1Pos;
    Vector3 P2Pos;

    public GameObject MusclePrefab;
    public  List<GameObject> L_MusclePart = new List<GameObject>();



    public void MuscleStuff()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.tag == "PartCube")
                {
                    P1Pos = hit.transform.InverseTransformPoint(hit.point);
                    P1Select = hit.transform.gameObject;
                    Camera.main.GetComponent<CameraRotate>().Run = false;

                }
            }

        }
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.tag == "PartCube")
                {
                    P2Pos = hit.transform.InverseTransformPoint(hit.point);
                    P2Select = hit.transform.gameObject;


                   

                }
            }








            if(P1Select != null && P2Select != null)
            {
                
                AddMuscle(P1Select, P2Select, P1Pos, P2Pos);


            }
            P2Select = null;
            P1Select = null;
            P1Pos = Vector3.zero;
            P1Pos = Vector3.zero;
            Camera.main.GetComponent<CameraRotate>().Run = true;

        }
    }


     void AddMuscle(GameObject Part1, GameObject Part2, Vector3 Pos1, Vector3 Pos2 )
    {
        Vector3 p1 = Pos1;
        Vector3 p2 = Pos2;

        Pos1 = Part1.transform.TransformPoint(Pos1);
        Pos2 = Part2.transform.TransformPoint(Pos2);

        GameObject thisPart = (GameObject)Instantiate(MusclePrefab, Vector3.up * -100f, MusclePrefab.transform.rotation);
        thisPart.transform.parent = BuildingCharacter.transform;
        thisPart.transform.position = Vector3.Lerp(Pos1, Pos2, .5f);
        Vector3 Relative = (Pos2 - thisPart.transform.position).normalized;
        //float z = 0;// Mathf.Atan2(Relative.z, Relative.y);
        //float y = 180 / (2 * Mathf.PI) - Mathf.Atan2(Relative.z, Relative.x);
        //float x = Mathf.Atan2(Relative.y, Relative.x);

        Quaternion ThisRotaion = Quaternion.LookRotation(Relative);

        thisPart.transform.rotation = ThisRotaion;

      //  thisPart.transform.localEulerAngles = new Vector3(x, 0,z) * (360 / (2 * Mathf.PI));
        thisPart.transform.localScale = Vector3.one * Vector3.Distance(Pos1, Pos2) * .6f;


        thisPart.GetComponent<MusclePart>().Connection1 = Part1;
        thisPart.GetComponent<MusclePart>().Connection2 = Part2;
        thisPart.GetComponent<MusclePart>().Strength = 1f;
        thisPart.GetComponent<MusclePart>().IDXMLSAVEDATA = thisPart.GetInstanceID();

        thisPart.GetComponent<MusclePart>().ReleativeConnection1 = p1;
        thisPart.GetComponent<MusclePart>().ReleativeConnection2 = p2;


        L_MusclePart.Add(thisPart);
        
    }























    public void SwitchMode(int modeNumber)
    {
        if(modeNumber == 0)
        {
            ButtonParts.GetComponent<Button>().interactable = false;
            ButtonJoint.GetComponent<Button>().interactable = true;
            ButtonMuscle.GetComponent<Button>().interactable = true;
            ButtonGroup.GetComponent<Button>().interactable = true;

        }
        else if (modeNumber == 1)
        {

            ButtonParts.GetComponent<Button>().interactable = true;
            ButtonJoint.GetComponent<Button>().interactable = false;
            ButtonMuscle.GetComponent<Button>().interactable = true;
            ButtonGroup.GetComponent<Button>().interactable = true;

        }
        else if (modeNumber == 2)
        {

            ButtonParts.GetComponent<Button>().interactable = true;
            ButtonJoint.GetComponent<Button>().interactable = true;
            ButtonMuscle.GetComponent<Button>().interactable = false;
            ButtonGroup.GetComponent<Button>().interactable = true;

        }
        else if (modeNumber == 3)
        {

            ButtonParts.GetComponent<Button>().interactable = true;
            ButtonJoint.GetComponent<Button>().interactable = true;
            ButtonMuscle.GetComponent<Button>().interactable = true;
            ButtonGroup.GetComponent<Button>().interactable = false;

        }


    }



    public void MuscleMode()
    {

        foreach (GameObject Button in CubeButtons)
        {

            Button.SetActive(false);

        }

        foreach (GameObject Button in JointButtons)
        {

            Button.SetActive(false);

        }
        foreach (GameObject Button in MuscleButtons)
        {

            Button.SetActive(true);

        }
        foreach (GameObject Button in GroupButtons)
        {

            Button.SetActive(false);

        }
    

        Mode = 2;
        SwitchMode(2);


        SizeCont.Target = null;
        SizeCont.transform.position = Vector3.one * -1000;


        foreach (GameObject pcc in L_PartCubes)
        {
            pcc.GetComponent<PartCubeControler>().SetActiveClick(false);


        }

    }





    public void GroupMode()
    {

        foreach (GameObject Button in CubeButtons)
        {

            Button.SetActive(false);

        }

        foreach (GameObject Button in JointButtons)
        {

            Button.SetActive(false);

        }
        foreach (GameObject Button in MuscleButtons)
        {

            Button.SetActive(false);

        }
        foreach (GameObject Button in GroupButtons)
        {

            Button.SetActive(true);

        }

        Mode = 3;
        SwitchMode(3);

        SizeCont.Target = null;
        SizeCont.transform.position = Vector3.one * -1000;


        foreach (GameObject pcc in L_PartCubes)
        {
            pcc.GetComponent<PartCubeControler>().SetActiveClick(false);


        }


    }







    public void JointMode()
    {
        foreach (GameObject Button in CubeButtons)
        {

            Button.SetActive(false);

        }

        foreach (GameObject Button in JointButtons)
        {

            Button.SetActive(true);

        }
        foreach (GameObject Button in MuscleButtons)
        {

            Button.SetActive(false);

        }
        foreach (GameObject Button in GroupButtons)
        {

            Button.SetActive(false);

        }

        Mode = 1;
        SwitchMode(1);

        SizeCont.Target = null;
        SizeCont.transform.position = Vector3.one * -1000;
      

        foreach (GameObject pcc in L_PartCubes)
        {
            pcc.GetComponent<PartCubeControler>().SetActiveClick(false);


        }


    }
    public void PartMode()
    {


        foreach (GameObject Button in CubeButtons)
        {

            Button.SetActive(true);

        }

        foreach (GameObject Button in JointButtons)
        {

            Button.SetActive(false);

        }
        foreach (GameObject Button in MuscleButtons)
        {

            Button.SetActive(false);

        }
        foreach (GameObject Button in GroupButtons)
        {

            Button.SetActive(false);

        }





        Mode = 0;
        SwitchMode(0);

    }

    public void ToggleCharEditor()
    {

   

        CharacterEditorGO.SetActiveRecursively(!CharacterEditorGO.active);
        MainEditor.SetActiveRecursively(!MainEditor.active);

        if (CharacterEditorGO.active)
        {
            Editing = true;
            PartMode();

        }
        else
        {
            Editing = false;


        }
    }


  


    public void SetName(string Name)
    {

        StructureName = Name;
        StructureFileSavePath = Application.persistentDataPath + string.Format("/{0}.Structure.xml", StructureName);

    }

    public string StructureName = "Generic";
    string StructureFileSavePath;
    XmlDocument StructureXml;

    public void SaveCharacter()
    {
        StructureXml = new XmlDocument();

        if (File.Exists(StructureFileSavePath))
        {
           // StructureXml.Load(StructureFileSavePath);
            OverwriteQuestion.SetActive(true);
            Camera.main.GetComponent<CameraRotate>().Run = false;

        }
        else
        {
            Save();

        }
    }
    public void NoSave()
    {
        Debug.Log("Saving...  Save Cancled");
        OverwriteQuestion.SetActive(false);
        Camera.main.GetComponent<CameraRotate>().Run = true;

    }
    public void SaveAnyways()
    {
        OverwriteQuestion.SetActive(false);
        Camera.main.GetComponent<CameraRotate>().Run = true;

        Debug.Log("Saving...");
        StructureXml = CreateGenericNodeSturecture(L_PartCubes.ToArray(), L_PartJoints.ToArray(), L_MusclePart.ToArray());

    }
    public void Save()
    {
        if (!File.Exists(StructureFileSavePath))
        {
            OverwriteQuestion.SetActive(false);
            Camera.main.GetComponent<CameraRotate>().Run = true;

            Debug.Log("Saving...");
            StructureXml = CreateGenericNodeSturecture(L_PartCubes.ToArray(), L_PartJoints.ToArray(), L_MusclePart.ToArray());
        }
        else
        {
            OverwriteQuestion.SetActive(true);
            Camera.main.GetComponent<CameraRotate>().Run = false;


        }
    }


    void AddSolidJointXml(Vector3 Location,Vector3 Rotation ,Vector3 Scale, int Connection1ID, int Connection2ID, int IDJoint, float LowTwistLimit, float HighTwistLimit, float Swing1Limit, float Swing2Limit )
    {

        GameObject thisPart = (GameObject)Instantiate(JointPrefab, Vector3.up * -100f, JointPrefab.transform.rotation);
        thisPart.transform.parent = BuildingCharacter.transform.parent;
        thisPart.transform.localPosition = Location;
        thisPart.transform.eulerAngles = Rotation;

        thisPart.transform.localScale = Scale;

        GameObject ConnectedPart1 = null;
        GameObject ConnectedPart2 = null;

        foreach(GameObject Go in L_PartCubes.ToArray())
        {
            if(Go.GetComponent<PartCubeControler>().IDXMLSAVEDATA == Connection1ID)
            {
                 ConnectedPart1 = Go;


            }
            if (Go.GetComponent<PartCubeControler>().IDXMLSAVEDATA == Connection2ID)
            {
                 ConnectedPart2 = Go;


            }

        }

        thisPart.GetComponent<PartJointControler>().IDXMLSAVEDATA = IDJoint;

        thisPart.GetComponent<PartJointControler>().ConnectedPart1 = ConnectedPart1;
        thisPart.GetComponent<PartJointControler>().ConnectedPart2 = ConnectedPart2;

        thisPart.GetComponent<PartJointControler>().LowTwistLimit = LowTwistLimit;
        thisPart.GetComponent<PartJointControler>().HighTwistLimit = HighTwistLimit;
        thisPart.GetComponent<PartJointControler>().Swing1Limit = Swing1Limit;
        thisPart.GetComponent<PartJointControler>().Swing2Limit = Swing2Limit;

        L_PartJoints.Add(thisPart);

    }

    void AddMuscleXml(int Part1id, int Part2id, Vector3 Pos1, Vector3 Pos2, int Groupid, float Stregnth, int ID)
    {




        GameObject Part1 = null;
        GameObject Part2 = null;

        foreach (GameObject Go in L_PartCubes)
        {
            if (Go.GetComponent<PartCubeControler>().IDXMLSAVEDATA == Part1id)
            {
                Part1 = Go;


            }
            if (Go.GetComponent<PartCubeControler>().IDXMLSAVEDATA == Part2id)
            {
                Part2 = Go;


            }

        }

        Vector3 p1 = Pos1;
        Vector3 p2 = Pos2;

        Pos1 = Part1.transform.TransformPoint(Pos1);
        Pos2 = Part2.transform.TransformPoint(Pos2);


    //    print("p1: " + p1 + " p2: " + p2 + " Pos1: " + Pos1 + " Pos2: " + Pos2);

        GameObject thisPart = (GameObject)Instantiate(MusclePrefab, Vector3.up * -100f, MusclePrefab.transform.rotation);
        thisPart.transform.parent = BuildingCharacter.transform;
        thisPart.transform.position = Vector3.Lerp(Pos1, Pos2, .5f);
        Vector3 Relative = (Pos2 - thisPart.transform.position).normalized;
        //float z = 0;// Mathf.Atan2(Relative.z, Relative.y);
        //float y = 180 / (2 * Mathf.PI) - Mathf.Atan2(Relative.z, Relative.x);
        //float x = Mathf.Atan2(Relative.y, Relative.x);

        Quaternion ThisRotaion = Quaternion.LookRotation(Relative);

        thisPart.transform.rotation = ThisRotaion;

        //  thisPart.transform.localEulerAngles = new Vector3(x, 0,z) * (360 / (2 * Mathf.PI));
        thisPart.transform.localScale = Vector3.one * Vector3.Distance(Pos1, Pos2) * .6f;


        thisPart.GetComponent<MusclePart>().Connection1 = Part1;
        thisPart.GetComponent<MusclePart>().Connection2 = Part2;
        thisPart.GetComponent<MusclePart>().Strength = Stregnth;
        thisPart.GetComponent<MusclePart>().IDXMLSAVEDATA = ID;
        thisPart.GetComponent<MusclePart>().SetGroup(Groupid);

        thisPart.GetComponent<MusclePart>().ReleativeConnection1 = p1;
        thisPart.GetComponent<MusclePart>().ReleativeConnection2 = p2;


        L_MusclePart.Add(thisPart);
    }













    public GameObject ErrorNoNameExists;
    void ReadXmlLoad()
    {
        StructureXml = new XmlDocument();

        if (File.Exists(StructureFileSavePath))
        {
            StructureXml.Load(StructureFileSavePath);
                        print("Loading...");
        foreach (XmlNode t_Part in StructureXml.SelectNodes("Root/PartSetting/Part"))
        {
            print("Loading... Part");

            AddCubeXml(DeserializeVector3Array(t_Part.Attributes.GetNamedItem("Location").Value), DeserializeVector3Array(t_Part.Attributes.GetNamedItem("Rotation").Value), DeserializeVector3Array(t_Part.Attributes.GetNamedItem("Scale").Value), int.Parse(t_Part.Attributes.GetNamedItem("Id").Value));

        }
        foreach (XmlNode t_joint in StructureXml.SelectNodes("Root/JointSetting/Joint"))
        {
            print("Loading... Joint");

            AddSolidJointXml(DeserializeVector3Array(t_joint.Attributes.GetNamedItem("Location").Value), DeserializeVector3Array(t_joint.Attributes.GetNamedItem("Rotation").Value), DeserializeVector3Array(t_joint.Attributes.GetNamedItem("Scale").Value)
                , int.Parse(t_joint.Attributes.GetNamedItem("Connection1").Value), int.Parse(t_joint.Attributes.GetNamedItem("Connection2").Value), int.Parse(t_joint.Attributes.GetNamedItem("Id").Value), 
                float.Parse(t_joint.Attributes.GetNamedItem("HighTwistLimit").Value), float.Parse(t_joint.Attributes.GetNamedItem("LowTwistLimit").Value), float.Parse(t_joint.Attributes.GetNamedItem("Swing1Limit").Value), float.Parse(t_joint.Attributes.GetNamedItem("Swing2Limit").Value));

        }



            foreach (XmlNode t_muscle in StructureXml.SelectNodes("Root/MuscleSetting/Muscle"))
            {
                print("Loading... Muscle");

               AddMuscleXml(int.Parse(t_muscle.Attributes.GetNamedItem("Connection1ObjId").Value)   , int.Parse(t_muscle.Attributes.GetNamedItem("Connection2ObjId").Value)
                   ,DeserializeVector3Array(t_muscle.Attributes.GetNamedItem("Connection1Pos").Value), DeserializeVector3Array(t_muscle.Attributes.GetNamedItem("Connection2Pos").Value)
                   , int.Parse(t_muscle.Attributes.GetNamedItem("Group").Value), float.Parse(t_muscle.Attributes.GetNamedItem("Stregth").Value),  int.Parse(t_muscle.Attributes.GetNamedItem("Id").Value)
                   
                   );
            }


            PartMode();



        }
        else
        {
            ErrorNoNameExists.SetActive(true);

        }



    }







    const int BaseMusclesPerJoint = 4;
    const int  NumberOfGroups = 4;
    

    XmlDocument CreateGenericNodeSturecture(GameObject[] _parts = null, GameObject[] _joints = null, GameObject[] _muscles = null)
    {
        XmlDocument GenericXml;
        GenericXml = new XmlDocument();
        XmlNode docNode = GenericXml.CreateXmlDeclaration("1.0", "UTF-8", null);
        GenericXml.AppendChild(docNode);

        XmlNode Root = GenericXml.CreateElement("Root");
        GenericXml.AppendChild(Root);

        XmlNode MuscleSettings = GenericXml.CreateElement("MuscleSetting");
        Root.AppendChild(MuscleSettings);

        XmlNode JointSettings = GenericXml.CreateElement("JointSetting");
        Root.AppendChild(JointSettings);

        XmlNode PartsSettings = GenericXml.CreateElement("PartSetting");
        Root.AppendChild(PartsSettings);

        if (_parts != null)
        {
            foreach (GameObject t_Part in _parts)
            {
                XmlNode x_Part = GenericXml.CreateElement("Part");


                //Location
                XmlAttribute LocationAttrib = GenericXml.CreateAttribute("Location");
                LocationAttrib.Value = t_Part.transform.localPosition.ToString("F6");
                x_Part.Attributes.Append(LocationAttrib);

                //Rotation
                XmlAttribute RotationAttrib = GenericXml.CreateAttribute("Rotation");
                RotationAttrib.Value = t_Part.transform.localEulerAngles.ToString("F6");
                x_Part.Attributes.Append(RotationAttrib);

                //Scale
                XmlAttribute ScaleAttrib = GenericXml.CreateAttribute("Scale");
                ScaleAttrib.Value = t_Part.transform.localScale.ToString("F6");
                x_Part.Attributes.Append(ScaleAttrib);

                //ID
                XmlAttribute IDAttrib = GenericXml.CreateAttribute("Id");
                IDAttrib.Value = t_Part.GetInstanceID().ToString();
                x_Part.Attributes.Append(IDAttrib);


                PartsSettings.AppendChild(x_Part);

            }



        }

        if (_joints != null)
        {
            foreach (GameObject t_joint in _joints)
            {
                XmlNode x_Joint = GenericXml.CreateElement("Joint");

                //Location
                XmlAttribute LocationAttrib = GenericXml.CreateAttribute("Location");
                LocationAttrib.Value = t_joint.transform.localPosition.ToString("F6");
                x_Joint.Attributes.Append(LocationAttrib);

                //Rotation
                XmlAttribute RotationAttrib = GenericXml.CreateAttribute("Rotation");
                RotationAttrib.Value = t_joint.transform.localEulerAngles.ToString("F6");
                x_Joint.Attributes.Append(RotationAttrib);

                //Scale
                XmlAttribute ScaleAttrib = GenericXml.CreateAttribute("Scale");
                ScaleAttrib.Value = t_joint.transform.localScale.ToString("F6");
                x_Joint.Attributes.Append(ScaleAttrib);

                // Joint Control Points 

                XmlAttribute LowTwistLimit = GenericXml.CreateAttribute("LowTwistLimit");
                LowTwistLimit.Value = t_joint.GetComponent<PartJointControler>().LowTwistLimit.ToString("F6");
                x_Joint.Attributes.Append(LowTwistLimit);


                XmlAttribute HighTwistLimit = GenericXml.CreateAttribute("HighTwistLimit");
                HighTwistLimit.Value = t_joint.GetComponent<PartJointControler>().HighTwistLimit.ToString("F6");
                x_Joint.Attributes.Append(HighTwistLimit);


                XmlAttribute Swing1Limit = GenericXml.CreateAttribute("Swing1Limit");
                Swing1Limit.Value = t_joint.GetComponent<PartJointControler>().Swing1Limit.ToString("F6");
                x_Joint.Attributes.Append(Swing1Limit);


                XmlAttribute Swing2Limit = GenericXml.CreateAttribute("Swing2Limit");
                Swing2Limit.Value = t_joint.GetComponent<PartJointControler>().Swing2Limit.ToString("F6");
                x_Joint.Attributes.Append(Swing2Limit);






                //Connection 1
                XmlAttribute Conn1Attrib = GenericXml.CreateAttribute("Connection1");
                Conn1Attrib.Value = t_joint.GetComponent<PartJointControler>().ConnectedPart1.GetInstanceID().ToString();
                x_Joint.Attributes.Append(Conn1Attrib);



                //Connection 2
                XmlAttribute Conn2Attrib = GenericXml.CreateAttribute("Connection2");
                Conn2Attrib.Value = t_joint.GetComponent<PartJointControler>().ConnectedPart2.GetInstanceID().ToString();
                x_Joint.Attributes.Append(Conn2Attrib);




                //ID
                XmlAttribute IDAttrib = GenericXml.CreateAttribute("Id");
                IDAttrib.Value = t_joint.GetInstanceID().ToString();
                x_Joint.Attributes.Append(IDAttrib);
                JointSettings.AppendChild(x_Joint);

            }
        }
        if (_muscles != null)
        {

            foreach (GameObject t_muscle in _muscles)
            {
                if (t_muscle != null)
                {
                    XmlNode x_Muscle = GenericXml.CreateElement("Muscle");

                    //Connection 1 Object
                    XmlAttribute Conn1posAttrib = GenericXml.CreateAttribute("Connection1Pos");
                    Conn1posAttrib.Value = t_muscle.GetComponent<MusclePart>().ReleativeConnection1.ToString("F6");
                    x_Muscle.Attributes.Append(Conn1posAttrib);



                    //Connection 2 Object
                    XmlAttribute Conn2posAttrib = GenericXml.CreateAttribute("Connection2Pos");
                    Conn2posAttrib.Value = t_muscle.GetComponent<MusclePart>().ReleativeConnection2.ToString("F6");
                    x_Muscle.Attributes.Append(Conn2posAttrib);





                    //Connection 1 Object
                    XmlAttribute Conn1Attrib = GenericXml.CreateAttribute("Connection1ObjId");
                    Conn1Attrib.Value = t_muscle.GetComponent<MusclePart>().Connection1.GetInstanceID().ToString();
                    x_Muscle.Attributes.Append(Conn1Attrib);



                    //Connection 2 Object
                    XmlAttribute Conn2Attrib = GenericXml.CreateAttribute("Connection2ObjId");
                    Conn2Attrib.Value = t_muscle.GetComponent<MusclePart>().Connection2.GetInstanceID().ToString();
                    x_Muscle.Attributes.Append(Conn2Attrib);








                    //Associated Joint


                    //Stegnth
                    XmlAttribute StrengthAttrib = GenericXml.CreateAttribute("Stregth");
                    StrengthAttrib.Value = t_muscle.GetComponent<MusclePart>().Strength.ToString();
                    x_Muscle.Attributes.Append(StrengthAttrib);


                    //Group Number
                    XmlAttribute GroupAttrib = GenericXml.CreateAttribute("Group");
                    GroupAttrib.Value = t_muscle.GetComponent<MusclePart>().AssociatedGroup.ToString();
                    x_Muscle.Attributes.Append(GroupAttrib);




                    //ID
                    XmlAttribute IDAttrib = GenericXml.CreateAttribute("Id");
                    IDAttrib.Value = t_muscle.GetInstanceID().ToString();
                    x_Muscle.Attributes.Append(IDAttrib);


                    MuscleSettings.AppendChild(x_Muscle);
                }
            }
        }


        if (_muscles == null && false)// Dont Need to run this Currently
        {
            int jointNumber = 0;
            int JointsPerGroup = Mathf.FloorToInt(_joints.Length / NumberOfGroups);
            int LeftoverGroups = _joints.Length % NumberOfGroups;
            for (int i = 0; i < NumberOfGroups; i++)
            {
                int jointsForThisGroup = JointsPerGroup;
                if (i < LeftoverGroups)
                {
                    jointsForThisGroup++;
                }
                XmlNode Group = GenericXml.CreateElement("Group");
                XmlAttribute GroupAttrib = GenericXml.CreateAttribute("id");
                GroupAttrib.Value = i.ToString();
                Group.Attributes.Append(GroupAttrib);
                MuscleSettings.AppendChild(Group);

                for (int w = 0; w < jointsForThisGroup; w++)
                {


                    for (int x = 0; x < (BaseMusclesPerJoint); x++)
                    {
                        XmlNode Muscle = GenericXml.CreateElement("Muscle");


                        //Connection 1
                        XmlAttribute MuscleLocation1Attrib = GenericXml.CreateAttribute("Connection1");
                        MuscleLocation1Attrib.Value = Vector3.zero.ToString();
                        Muscle.Attributes.Append(MuscleLocation1Attrib);

                        //Connection 2
                        XmlAttribute MuscleLocation2Attrib = GenericXml.CreateAttribute("Connection2");
                        MuscleLocation2Attrib.Value = Vector3.zero.ToString();
                        Muscle.Attributes.Append(MuscleLocation2Attrib);


                        //Spin
                        XmlAttribute MuscleSpinAttrib = GenericXml.CreateAttribute("Spin");
                        MuscleSpinAttrib.Value = getEvenString(x);
                        Muscle.Attributes.Append(MuscleSpinAttrib);

                        //Strength
                        XmlAttribute MuscleStengthAttrib = GenericXml.CreateAttribute("Strength");
                        MuscleStengthAttrib.Value = 0.ToString();
                        Muscle.Attributes.Append(MuscleStengthAttrib);


                        //Joint ID
                        XmlAttribute MusclJointIDAttrib = GenericXml.CreateAttribute("JointId");
                        MusclJointIDAttrib.Value = ((w + 1) * x).ToString();
                        Muscle.Attributes.Append(MusclJointIDAttrib);

                        //Joint ID
                        XmlAttribute MusclJointNameAttrib = GenericXml.CreateAttribute("JointName");
                        MusclJointNameAttrib.Value = _joints[jointNumber].ToString();
                        Muscle.Attributes.Append(MusclJointNameAttrib);
                        Group.AppendChild(Muscle);
                    }
                    jointNumber++;
                }



            }
        }






        GenericXml.Save(StructureFileSavePath);




        return GenericXml;


    }






    string getEvenString(int input)
    {

        if (input % 2 == 0)
        {
            return "True";
        }

        return "False";
    }

    bool ParseBool(string In)
    {
        if (In.Contains("t") || In.Contains("T"))
            return true;

        return false;
    }

    public static Vector3 DeserializeVector3Array(string aData)
    {
        Vector3 result = Vector3.zero;
        aData = aData.Replace("(", "");
        aData = aData.Replace(")", "");
        aData = aData.Replace(" ", "");

        string[] values = aData.Split(',');
        result = new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
        return result;
    }













    public void LoadCharacter()
    {
        NewCharacter();
        Camera.main.GetComponent<CameraRotate>().Run = true;

        ReadXmlLoad();
    }




    public void Miror()
    {
        Camera.main.GetComponent<LassoTool>().enabled = true;
        Camera.main.GetComponent<CameraRotate>().Run = false;



    }






    public GameObject GetSelectedCube()
    {

        foreach (GameObject pcc in L_PartCubes.ToArray())
        {
            if (pcc.GetComponent<Collider>().isTrigger)
            {
                return pcc;
            }


        }
        return null;

    }
    public void DuplicateSelected()
    {
        GameObject thisPart = (GameObject)Instantiate(PartPrefab, Vector3.up * -100f, PartPrefab.transform.rotation);
        thisPart.transform.parent = BuildingCharacter.transform;
        thisPart.transform.localPosition = Vector3.zero;
        L_PartCubes.Add(thisPart);

        PartCubeControler pcc1 = GetSelectedCube().GetComponent<PartCubeControler>();
        thisPart.transform.localPosition =  pcc1.transform.localPosition;
        thisPart.transform.eulerAngles = pcc1.transform.eulerAngles;
        thisPart.transform.localScale = pcc1.transform.localScale;



        Select(thisPart);
    }
    public void ResetSelected()
    {
        PartCubeControler pcc1 = GetSelectedCube().GetComponent<PartCubeControler>();
        pcc1.transform.localPosition = Vector3.zero;
        pcc1.transform.eulerAngles = Vector3.zero;
        pcc1.transform.localScale = Vector3.one;
    }
    public void DeleteSelected()
    {
        GameObject SelectedCube = GetSelectedCube();
        L_PartCubes.Remove(SelectedCube);

        Destroy(SelectedCube);
           
        bool picked = false;

        foreach (GameObject pcc in L_PartCubes.ToArray())
        {
            if (!picked)
            {
                Select(pcc);
                picked = true;
            }
 

        }
        if (L_PartCubes.Count < 2)
        {
            SizeCont.transform.position = Vector3.one * -1000;

        }


    }
    public void AddCube()
    {
        GameObject thisPart = (GameObject)Instantiate(PartPrefab, Vector3.up * -100f, PartPrefab.transform.rotation);
        thisPart.transform.parent = BuildingCharacter.transform;
        thisPart.transform.localPosition = Vector3.up * PartPrefab.transform.localScale.y *.5f;
        L_PartCubes.Add(thisPart);
        Select(thisPart);
    }



    public void AddCubeXml(Vector3 location, Vector3 rotation, Vector3 scale, int CubeID)
    {
        GameObject thisPart = (GameObject)Instantiate(PartPrefab, Vector3.up * -100f, PartPrefab.transform.rotation);
        thisPart.transform.parent = BuildingCharacter.transform;
        thisPart.transform.localPosition = location;
        thisPart.transform.localScale = scale;
        thisPart.transform.eulerAngles = rotation;
        thisPart.GetComponent<PartCubeControler>().IDXMLSAVEDATA = CubeID;

        L_PartCubes.Add(thisPart);
        Select(thisPart);
    }











        void RunCharacterEditor()
    {
        
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.gameObject.tag == "PartCube")
                    {
                        Select(hit.transform.gameObject);

                    }
                }

            }

        
    }



    public void Select(GameObject Selected)
    {
        if (L_PartCubes.Contains(Selected))
        {
            //L_PartCubes.Add(Selected.GetComponent<PartCubeControler>());
        }
        foreach (GameObject pcc in L_PartCubes.ToArray())
        {
            if (pcc == Selected)
            {
                pcc.GetComponent<PartCubeControler>().SetActiveClick(true);
                SizeCont.Target = pcc.transform;
            }
            else
            {

                pcc.GetComponent<PartCubeControler>().SetActiveClick(false);

            }

        }
    }


    const float MinJointDistance = 0.25f;

    public void JointStuff()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.tag == "FakeJoint")
                {
                    hit.transform.GetComponent<FakeJointControler>().SolidifyJoint();
                    L_FakeJoints.Remove(hit.transform.gameObject);
                    Destroy(hit.transform.gameObject);
                }
            }

        }

    }
    
    public void CheckJoints(GameObject Target)
    {

        foreach (GameObject t_muscle in L_MusclePart)
        {
            if (t_muscle.GetComponent<MusclePart>().Connection1 == Target || t_muscle.GetComponent<MusclePart>().Connection2 == Target )
            {

                L_MusclePart.Remove(t_muscle);
                Destroy(t_muscle);
            }
        }


        foreach (GameObject Go in L_PartJoints.ToArray())
        {
            GameObject pcc = Go.GetComponent<PartJointControler>().ConnectedPart1;
            GameObject pcc2 = Go.GetComponent<PartJointControler>().ConnectedPart2;
            if(pcc == null || pcc2 == null)
            {
                L_PartJoints.Remove(Go);
                Destroy(Go);


            }
            if (pcc == Target || pcc2  == Target)
            {
                Collider pccC = pcc.GetComponent<Collider>();

                Collider pccC2 = pcc2.GetComponent<Collider>();

                Vector3 closestPoint1 = pccC.ClosestPointOnBounds(pcc.transform.position);
                Vector3 closestPoint2 = pccC2.ClosestPointOnBounds(closestPoint1);
                for (int w = 0; w < 10; w++)
                {
                    closestPoint1 = pccC.ClosestPointOnBounds(closestPoint2);
                    closestPoint2 = pccC2.ClosestPointOnBounds(closestPoint1);

                }


                float distance = Vector3.Distance(closestPoint2, closestPoint1);



                if (Go.transform.position.x == Vector3.Lerp(closestPoint1, closestPoint2, .5f).x && Go.transform.position.y == Vector3.Lerp(closestPoint1, closestPoint2, .5f).y && Go.transform.position.z == Vector3.Lerp(closestPoint1, closestPoint2, .5f).z)
                {

                }
                else
                {
                    
            
                    L_PartJoints.Remove(Go);
                    Destroy(Go);







                }
            }

           

        }

    }



    public void UpdateJoints()
    {
        foreach (GameObject Go in L_FakeJoints.ToArray())
        {
            DestroyImmediate(Go);
            L_FakeJoints.Remove(Go);
        }
        int SelectedNum  = -1;
        for (int i = 0; i < L_PartCubes.Count - 1; i++)
        {


            PartCubeControler pcc = L_PartCubes[i].GetComponent<PartCubeControler>();



            if (pcc.GetComponent<Collider>().isTrigger)
            {
                SelectedNum = i;

            }
            pcc.SetActiveClick(false);

        }



        for (int i = 0; i < L_PartCubes.Count - 1; i++)
        {
            PartCubeControler pcc = L_PartCubes[i].GetComponent<PartCubeControler>();
            Collider pccC = pcc.GetComponent<Collider>();

            for (int j = i + 1; j < L_PartCubes.Count; j++)
            {
                PartCubeControler pcc2 = L_PartCubes[j].GetComponent<PartCubeControler>();

                if (pcc2 != pcc) {

                    Collider pccC2 = pcc2.GetComponent<Collider>();

                    Vector3 closestPoint1 = pccC.ClosestPointOnBounds(pcc.transform.position);
                    Vector3 closestPoint2 = pccC2.ClosestPointOnBounds(closestPoint1);
                    for(int w = 0; w < 10; w++)
                    {
                        closestPoint1 = pccC.ClosestPointOnBounds(closestPoint2);
                        closestPoint2 = pccC2.ClosestPointOnBounds(closestPoint1);

                    }
                    //RaycastHit hit;
                    //if (Physics.Raycast(closestPoint1, (closestPoint1 - closestPoint2).normalized, out hit, 5))
                    //{
                    //    closestPoint2 = hit.point;
                    //}

                    float distance = Vector3.Distance(closestPoint2, closestPoint1);
                    if (distance < MinJointDistance)
                    {
                        bool Create = true;
                        foreach(GameObject Go in L_FakeJoints.ToArray())
                        {
                            if (Go.GetComponent<PartJointControler>() && Go.GetComponent<PartJointControler>())
                            {
                                if (Go.GetComponent<PartJointControler>().ConnectedPart1.gameObject != null && Go.GetComponent<PartJointControler>().ConnectedPart2.gameObject != null)
                                {
                                    if (Go.GetComponent<PartJointControler>().ConnectedPart1.gameObject == L_PartCubes[i] && (Go.GetComponent<PartJointControler>().ConnectedPart2 == L_PartCubes[j]))
                                    {

                                        Create = false;

                                    }

                                    if (Go.GetComponent<PartJointControler>().ConnectedPart1.gameObject == L_PartCubes[j] && (Go.GetComponent<PartJointControler>().ConnectedPart2 == L_PartCubes[i]))
                                    {

                                        Create = false;

                                    }
                                }
                            }
                           
                            if (Mathf.Round( Go.transform.position.x * 16f) == Mathf.Round(Vector3.Lerp(closestPoint1, closestPoint2, .5f).x * 16f) && Mathf.Round(Go.transform.position.y * 16f) == Mathf.Round(Vector3.Lerp(closestPoint1, closestPoint2, .5f).y * 16f) && Mathf.Round(Go.transform.position.z * 16f) == Mathf.Round(Vector3.Lerp(closestPoint1, closestPoint2, .5f).z * 16f))
                            {
                                Create = false;
                            }
                         
                      }


                        foreach (GameObject Go in L_PartJoints.ToArray())
                        {


                            if (Mathf.Round(Go.transform.position.x * 16f) == Mathf.Round(Vector3.Lerp(closestPoint1, closestPoint2, .5f).x * 16f) && Mathf.Round(Go.transform.position.y * 16f) == Mathf.Round(Vector3.Lerp(closestPoint1, closestPoint2, .5f).y * 16f) && Mathf.Round(Go.transform.position.z * 16f) == Mathf.Round(Vector3.Lerp(closestPoint1, closestPoint2, .5f).z * 16f))
                            {
                                Create = false;
                            }
                        }
                        if (Create)
                        {
                            GameObject thisPart = (GameObject)Instantiate(FakeJointPrefab, Vector3.up * -100f, FakeJointPrefab.transform.rotation);
                            thisPart.transform.parent = BuildingCharacter.transform;
                            thisPart.transform.localPosition = Vector3.zero;
                            thisPart.transform.position = Vector3.Lerp(closestPoint1, closestPoint2, .5f);
                            thisPart.transform.localScale = 1f * Vector3.one * distance;
                            thisPart.GetComponent<FakeJointControler>().editCont = this;
                            thisPart.GetComponent<FakeJointControler>().ConnectedPart1 = pcc.gameObject;
                            thisPart.GetComponent<FakeJointControler>().ConnectedPart2 = pcc2.gameObject;

                            L_FakeJoints.Add(thisPart);
                        }
                     
                        

                    }
                }
            }
        }





        for (int i = 0; i < L_PartCubes.Count - 1; i++)
        {
            PartCubeControler pcc = L_PartCubes[i].GetComponent<PartCubeControler>();
            if (SelectedNum == i)
            {
                pcc.SetActiveClick(true);

            }
            else
            {

                pcc.SetActiveClick(false);

            }

        }

    }


        



    
}
