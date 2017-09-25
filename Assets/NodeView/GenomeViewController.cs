using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using UnityEngine.UI;

public class GenomeViewController : MonoBehaviour {
    public GameObject Node;
    public GameObject GOConnector;
    public bool useCurrentPop = false;
    public List<GameObject> NodesList = new List<GameObject>();
    public List<GameObject> ConnectorList = new List<GameObject>();

    public Color[] FnColors;
    XmlDocument NodeXml;
    // Use this for initialization
    void Start () {
       UpdateXml();
    }
    public int idPop = 0;
    int maxValueNode = 500;
    public void UpdateXml()
    {
        try
        {
            Optimizer Optim = Object.FindObjectOfType<Optimizer>();
            NodeXml = new XmlDocument();
            string champFileSavePath;

            champFileSavePath = Application.persistentDataPath + string.Format("/{0}.champ.xml", Optim.expiramentName + Optim.SubExpiramentNumber.ToString());
            NodeXml.Load(champFileSavePath);
            XmlNodeList NodesInitialList = NodeXml.SelectNodes("Root/Networks/Network/Nodes/Node");
            XmlNodeList ConnectorInitialList = NodeXml.SelectNodes("Root/Networks/Network/Connections/Con");
            if (useCurrentPop)
            {
                NodeXml = new XmlDocument();

                champFileSavePath = Application.persistentDataPath + string.Format("/{0}.pop.xml", Optim.expiramentName + Optim.SubExpiramentNumber.ToString());
                NodeXml.Load(champFileSavePath);

                foreach (XmlNode i_NodeID in NodeXml.SelectNodes("Root/Networks/Network"))
                {

                    if (idPop == int.Parse(i_NodeID.Attributes.GetNamedItem("id").Value)){
                        print("Found");



                        NodesInitialList = i_NodeID.SelectNodes("Nodes/Node");
                        ConnectorInitialList = i_NodeID.SelectNodes("Connections/Con");

                    }

                }

            }
   


            //print(champFileSavePath);

  

            if (true)
            {
                foreach (GameObject g_node in NodesList.ToArray())
                {
                    Destroy(g_node);

                }
                NodesList.Clear();

                int MaxInput = 0;
                int Maxout = 0;
                int numHid = 0;
                int CurrentColor = 0;
                int nodeVal = -1;
                try {
                    foreach (XmlNode i_NodeID in NodeXml.SelectNodes("Root/ActivationFunctions/Fn"))
                    {

                        GameObject thisNode = (GameObject)Instantiate(Node);
                        NodesList.Add(thisNode);
                        thisNode.transform.SetParent(this.transform);

                        thisNode.GetComponent<Node>().c_Value = - int.Parse(i_NodeID.Attributes.GetNamedItem("id").Value);
                        thisNode.GetComponent<Node>().c_Type = i_NodeID.Attributes.GetNamedItem("name").Value;
                        thisNode.transform.GetChild(0).GetComponent<Text>().text = thisNode.GetComponent<Node>().c_Type;
                        thisNode.GetComponent<Image>().color = FnColors[Mathf.Abs(nodeVal + 1)];
                        thisNode.GetComponent<RectTransform>().localPosition = new Vector3(-220, -50 + (20 * nodeVal), 0);
                        nodeVal--;
                    }
                }
                catch { Debug.LogWarning("No Fn Ids"); }
                foreach (XmlNode i_NodeID in NodesInitialList)
                {

                    GameObject thisNode = (GameObject)Instantiate(Node);
                    NodesList.Add(thisNode);
                    thisNode.GetComponent<Node>().c_Value = int.Parse(i_NodeID.Attributes.GetNamedItem("id").Value);
                    thisNode.GetComponent<Node>().c_Type = i_NodeID.Attributes.GetNamedItem("type").Value;
                    try {
                        thisNode.GetComponent<Node>().c_FnID = int.Parse(i_NodeID.Attributes.GetNamedItem("fnId").Value);
                        thisNode.GetComponent<Image>().color = FnColors[Mathf.Abs(thisNode.GetComponent<Node>().c_FnID)];

                    }
                    catch
                    {
                        Debug.LogWarning("No Fn ID in this scenerio");
                    }

                    thisNode.transform.GetChild(0).GetComponent<Text>().text = thisNode.GetComponent<Node>().c_Value.ToString();
                    thisNode.transform.SetParent(this.transform);








                    if (thisNode.GetComponent<Node>().c_Type == "bias" || thisNode.GetComponent<Node>().c_Type == "in")
                    {
                        if (thisNode.GetComponent<Node>().c_Value > MaxInput)
                        {
                            MaxInput = thisNode.GetComponent<Node>().c_Value;
                        }
                    }
                    if (thisNode.GetComponent<Node>().c_Type == "hid")
                    {
                        numHid++;
                    }
                    if (thisNode.GetComponent<Node>().c_Type == "out")
                    {
                        if (thisNode.GetComponent<Node>().c_Value > Maxout)
                        {
                            Maxout = thisNode.GetComponent<Node>().c_Value;
                        }

                    }
                }
                int maxHid = numHid;
                foreach (GameObject thisNode in NodesList.ToArray())
                {
                    if (thisNode.GetComponent<Node>().c_Type == "bias" || thisNode.GetComponent<Node>().c_Type == "in")
                    {

                        thisNode.GetComponent<RectTransform>().localPosition = new Vector3(Mathf.Lerp(-150, 150, thisNode.GetComponent<Node>().c_Value / (float)MaxInput), 120, 0);
                    }
                    if (thisNode.GetComponent<Node>().c_Type == "hid")
                    {
                        numHid = Random.Range(0, maxHid);
                        thisNode.GetComponent<RectTransform>().localPosition = new Vector3(Mathf.Lerp(-150f, 150f, numHid / (float)maxHid), Mathf.Lerp(-90, 90, (thisNode.GetComponent<Node>().c_Value / (float)maxValueNode)), 0);
                        if (thisNode.GetComponent<Node>().c_Value > maxValueNode)
                        {

                            maxValueNode = (thisNode.GetComponent<Node>().c_Value + 100);
                        }




                    }
                    if (thisNode.GetComponent<Node>().c_Type == "out")
                    {

                        thisNode.GetComponent<RectTransform>().localPosition = new Vector3(Mathf.Lerp(-150, 150, (thisNode.GetComponent<Node>().c_Value - MaxInput - .5f) / (float)(Maxout - MaxInput)), -120, 0);

                    }
                }

            }



                foreach (GameObject g_Con in ConnectorList.ToArray())
            {
                Destroy(g_Con);

            }
            ConnectorList.Clear();

            foreach (GameObject thisNode in NodesList.ToArray())
            {
                float x = 0;
                float y = 0;
                int hits = 0;

                if (thisNode.GetComponent<Node>().c_Type == "hid") {
                    
                    foreach (XmlNode i_ConID in ConnectorInitialList)
                    {
                        if (int.Parse(i_ConID.Attributes.GetNamedItem("src").Value) == thisNode.GetComponent<Node>().c_Value)
                        {
                            int targetVal = int.Parse(i_ConID.Attributes.GetNamedItem("tgt").Value);

                            foreach (GameObject thisNodeCheck in NodesList.ToArray())
                            {
                                if(targetVal == thisNodeCheck.GetComponent<Node>().c_Value)
                                {
                                    x += thisNodeCheck.GetComponent<RectTransform>().localPosition.x;
                                    y += thisNodeCheck.GetComponent<RectTransform>().localPosition.y;
                                    hits++;
                                }
                            }
                        }
                        if (int.Parse(i_ConID.Attributes.GetNamedItem("tgt").Value) == thisNode.GetComponent<Node>().c_Value)
                        {
                            int srcValue = int.Parse(i_ConID.Attributes.GetNamedItem("src").Value);

                            foreach (GameObject thisNodeCheck in NodesList.ToArray())
                            {
                                if (srcValue == thisNodeCheck.GetComponent<Node>().c_Value)
                                {
                                    x += thisNodeCheck.GetComponent<RectTransform>().localPosition.x;
                                    y += thisNodeCheck.GetComponent<RectTransform>().localPosition.y;
                                    hits++;
                                }
                            }
                        }

                    }
                    x /= (float)hits;
                    y /= (float)hits;

                    thisNode.GetComponent<RectTransform>().localPosition = new Vector3(x, y, 0);
                    foreach (GameObject thisNodeCheck in NodesList.ToArray())
                    {
                        if (thisNode.GetComponent<RectTransform>().localPosition == thisNodeCheck.GetComponent<RectTransform>().localPosition && thisNodeCheck != thisNode)
                        {
                            y += 40;
                        }
                    }
                }
            }
            foreach (XmlNode i_ConID in ConnectorInitialList)
            {

                GameObject thisConnector = (GameObject)Instantiate(GOConnector);
                ConnectorList.Add(thisConnector);
                thisConnector.GetComponent<Connector>().c_Value = int.Parse(i_ConID.Attributes.GetNamedItem("id").Value);
                thisConnector.GetComponent<Connector>().c_Scorce = int.Parse(i_ConID.Attributes.GetNamedItem("src").Value);
                thisConnector.GetComponent<Connector>().c_Target = int.Parse(i_ConID.Attributes.GetNamedItem("tgt").Value);
                thisConnector.GetComponent<Connector>().c_Weight = float.Parse(i_ConID.Attributes.GetNamedItem("wght").Value);

                thisConnector.transform.GetComponent<Image>().color = Color.Lerp( new Color(0, 0, 1, .2f), new Color(1, 0, 0, .2f), (thisConnector.GetComponent<Connector>().c_Weight + 5f) / 10f);
                thisConnector.transform.transform.SetParent(this.transform);
                Vector2 Pos1 = Vector2.zero;
                Vector2 Pos2 = Vector2.zero;

                foreach (GameObject g_node in NodesList.ToArray())
                {
                    if (g_node.GetComponent<Node>().c_Value == thisConnector.GetComponent<Connector>().c_Scorce)
                    {
                        Pos1 = g_node.GetComponent<RectTransform>().localPosition;
                    }
                    if (g_node.GetComponent<Node>().c_Value == thisConnector.GetComponent<Connector>().c_Target)
                    {
                        Pos2 = g_node.GetComponent<RectTransform>().localPosition;
                    }

                }
                thisConnector.GetComponent<RectTransform>().localPosition = (Pos1 + Pos2) / 2f;
                thisConnector.GetComponent<RectTransform>().localScale = new Vector3(.02f,Vector2.Distance(Pos1,Pos2) * .01f,1f);
                thisConnector.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, 90 + Mathf.Atan2(Pos2.y - Pos1.y, Pos2.x - Pos1.x) * 180f / Mathf.PI);

            }









        }
        catch
        {
            print("Xml File Does Not exist");
        }

    }
	// Update is called once per frame
	void Update () {



    }
}
