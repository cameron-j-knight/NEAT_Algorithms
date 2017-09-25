using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;

public class MuscleSystem : MonoBehaviour {
    Joint[] joints;
    Muscle[] jointMuscles;
    public uint MusclesPerJoint = 1;
    public int NumberOfGroups = 10;
    MuscleGroup[] Groups;
    public string StructureName = "Generic";
    string StructureFileSavePath;
    XmlDocument StructureXml;
    void Start()
    {

        Groups = new MuscleGroup[NumberOfGroups];
        joints = GetComponentsInChildren<Joint>();
        jointMuscles = new Muscle[Mathf.FloorToInt(joints.Length / 2f) * MusclesPerJoint];

        StructureFileSavePath = Application.persistentDataPath + string.Format("/{0}.Structure.xml", StructureName);
        StructureXml = new XmlDocument();

        if(File.Exists(StructureFileSavePath))
        {
            StructureXml.Load(StructureFileSavePath);
        }
        else
        {
            Debug.Log("Creating GenericStructure");
            StructureXml = CreateGenericNodeSturecture();

        }





        foreach (XmlNode thisGroup in StructureXml.SelectNodes("Root/Muscle/Group"))
        {
            int GroupID = int.Parse(thisGroup.Attributes.GetNamedItem("id").Value);

            foreach (XmlNode thisMuscle in thisGroup.SelectNodes("Muscle"))
            {
                int X_JointID = int.Parse(thisMuscle.Attributes.GetNamedItem("JointId").Value);
                float  X_Strength = float.Parse(thisMuscle.Attributes.GetNamedItem("Strength").Value);

                bool X_Spin = ParseBool(thisMuscle.Attributes.GetNamedItem("Spin").Value);

                Vector3 X_Connection1 = DeserializeVector3Array(thisMuscle.Attributes.GetNamedItem("Connection1").Value);
                Vector3 X_Connection2 = DeserializeVector3Array(thisMuscle.Attributes.GetNamedItem("Connection2").Value);
                GameObject X_ConnectedJoint = null;
                foreach (Joint t_Joint in joints)
                {
                    if(t_Joint.JointID == X_JointID)
                    {
                        X_ConnectedJoint = t_Joint.gameObject;
                    }

                }
                Muscle c_Muscle = new Muscle(X_Spin, X_Strength, X_Connection1, X_Connection2, X_ConnectedJoint);





                Groups[GroupID] = new MuscleGroup();

                Groups[GroupID].Muscles.Add(c_Muscle);

            }


        }

    }



    public void TesnseGroup(int GroupNumber, bool Spin, float Power)
    {
        foreach (Muscle mus in Groups[GroupNumber].Muscles.ToArray())
        {

            mus.Tense(Spin, Power);

        }
    }



    //public IEnumerator Movement()
    //{
    //    while (true) {
    //        for (int i = 0; i < 6; i++)
    //        {
    //            foreach (Muscle mus in Groups[i].Muscles.ToArray())
    //            {

    //               mus.Strength = 1;
    //                mus.Tense(false);

    //            }

    //            yield return new WaitForSeconds(.3f);

    //            foreach (Muscle mus in Groups[i].Muscles.ToArray())
    //            {

    //                //mus.Strength = 1;
    //                mus.Tense(true);

    //            }

    //            yield return new WaitForSeconds(.3f);
    //            foreach (Muscle mus in Groups[i].Muscles.ToArray())
    //            {

    //                mus.Strength = 0;
    //                mus.Tense(false);

    //            }
    //        }
    //    }
    //}



    XmlDocument CreateGenericNodeSturecture()
    {
        XmlDocument GenericXml;
        GenericXml = new XmlDocument();
        XmlNode docNode = GenericXml.CreateXmlDeclaration("1.0", "UTF-8", null);
        GenericXml.AppendChild(docNode);

        XmlElement Root = (XmlElement)GenericXml.AppendChild(GenericXml.CreateElement("Character"));
        XmlElement MuscleSettings = (XmlElement)Root.AppendChild(GenericXml.CreateElement("MuscleSettings"));

        int jointNumber = 0;
        int JointsPerGroup = Mathf.FloorToInt(joints.Length / NumberOfGroups);
        int LeftoverGroups = joints.Length % NumberOfGroups;
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

            for (int w =0; w < jointsForThisGroup; w++)
            {
               

                for (int x = 0; x < (MusclesPerJoint); x++)
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
                    MusclJointIDAttrib.Value = joints[jointNumber].JointID.ToString();
                    Muscle.Attributes.Append(MusclJointIDAttrib);

                    //Joint ID
                    XmlAttribute MusclJointNameAttrib = GenericXml.CreateAttribute("JointName");
                    MusclJointNameAttrib.Value = joints[jointNumber].ToString();
                    Muscle.Attributes.Append(MusclJointNameAttrib);
                    Group.AppendChild(Muscle);
                }
                jointNumber++;
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
        aData =  aData.Replace(")", "");
        aData = aData.Replace(" ", "");

        string[] values = aData.Split(',');
            result = new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
        return result;
    }


}
