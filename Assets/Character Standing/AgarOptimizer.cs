using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;
using System.Collections.Generic;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using System;
using System.Xml;
using System.IO;
using UnityEngine.UI;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.HyperNeat;
using SharpNeat.Utility;
using UnityEngine;
using System.Collections;
using SharpNeat.Domains;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using SharpNeat.Decoders;
using System.Collections.Generic;
using System.Xml;
using SharpNeat.Core;
using SharpNeat.Phenomes;
using SharpNeat.Decoders.Neat;
using SharpNeat.Decoders.HyperNeat;
using SharpNeat.DistanceMetrics;
using SharpNeat.SpeciationStrategies;
using SharpNeat.EvolutionAlgorithms.ComplexityRegulation;
using SharpNEAT.Core;
using System;
using SharpNeat.Network;
using SharpNeat.Genomes.HyperNeat;
using SharpNeat.Decoders.HyperNeat;




public class AgarOptimizer : Optimizer
{


    public int NUM_INPUTS = 5;
    public int NUM_OUTPUTS = 2;
    public int speed = 1;
    public string configName = "experiment.config";
    public float updateInterval = 12;
    List<GameObject> runners = new List<GameObject>();
     
    bool EARunning;
    public GameObject NodeViewer;
    AgarExpirament experiment;
    static NeatEvolutionAlgorithm<NeatGenome> _ea;

    public GameObject Unit;

    Dictionary<IBlackBox, UnitController> ControllerMap = new Dictionary<IBlackBox, UnitController>();
    private DateTime startTime;
    private float timeLeft;
    private float accum;
    private int frames;

    private uint Generation;
    private double Fitness;
    // Use this for initialization


    EditorControler Ec;


    public void Initialize()
    {
        
        Utility.DebugLog = true;
        experiment = new AgarExpirament();
        XmlDocument xmlConfig = new XmlDocument();
        TextAsset textAsset = (TextAsset)Resources.Load(configName);
        xmlConfig.LoadXml(textAsset.text);
        experiment.SetOptimizer(this);

        experiment.Initialize("Car Experiment", xmlConfig.DocumentElement, NUM_INPUTS, NUM_OUTPUTS);
        champFileSavePath = Application.persistentDataPath + string.Format("/{0}.champ.xml", expiramentName + SubExpiramentNumber.ToString());
        popFileSavePath = Application.persistentDataPath + string.Format("/{0}.pop.xml", expiramentName + SubExpiramentNumber.ToString());

        print(champFileSavePath);
    }
    private void AddNode(SubstrateNodeSet nodeSet, uint id, double x, double y, double z, double w)
    {
        nodeSet.NodeList.Add(new SubstrateNode(id, new double[] { x, y, z, w }));
    }
    public Vector3[] Positions;
    public SubstrateNodeSet CreateSubstrate()
    {
        SubstrateNodeSet outputLayer = new SubstrateNodeSet(NUM_OUTPUTS);

        for(int i = 0; i < Positions.Length; i++)
        {
            AddNode(outputLayer, (uint)(i + NUM_INPUTS + 1), Positions[i].x, Positions[i].y, Positions[i].z, 0.0);
        }



        return outputLayer;

    }

    public SubstrateNodeSet CreateSubstrateHidden()
    {
        SubstrateNodeSet outputLayer = new SubstrateNodeSet(NUM_OUTPUTS);

        for (int i = 0; i < Positions.Length; i++)
        {
            AddNode(outputLayer, (uint)(i + NUM_INPUTS + NUM_OUTPUTS + 1), Positions[i].x, Positions[i].y, Positions[i].z, 0.0);
        }



        return outputLayer;

    }


    // Update is called once per frame
    void Update()
    {
        //  evaluationStartTime += Time.deltaTime;

        timeLeft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        if (timeLeft <= 0.0)
        {
            var fps = accum / frames;
            timeLeft = updateInterval;
            accum = 0.0f;
            frames = 0;
            //   print("FPS: " + fps);
            if (fps < 2)
            {
                Time.timeScale = Time.timeScale - 1;
                print("Lowering time scale to " + Time.timeScale);
            }
        }
    }

    public override void StartEA()
    {
        Utility.DebugLog = true;
        // print("Loading: " + popFileLoadPath);
        _ea = experiment.CreateEvolutionAlgorithm(popFileSavePath);
        startTime = DateTime.Now;

        _ea.UpdateEvent += new EventHandler(ea_UpdateEvent);
        _ea.PausedEvent += new EventHandler(ea_PauseEvent);

        var evoSpeed = 25;

        //   Time.fixedDeltaTime = 0.045f;
        Time.timeScale = speed;
        _ea.StartContinue();
        EARunning = true;
    }

    public override void ea_UpdateEvent(object sender, EventArgs e)
    {
        Utility.Log(string.Format("gen={0:N0} bestFitness={1:N6}",
            _ea.CurrentGeneration, _ea.Statistics._maxFitness));

        Fitness = _ea.Statistics._maxFitness;
        Generation = _ea.CurrentGeneration;
        GameObject.Find("TrialNumber").GetComponent<Text>().text = "Expirament " + SubExpiramentNumber.ToString();
        GameObject.Find("Genome").GetComponent<Text>().text = string.Format("Generation: {0}\nFitness: {1:0.00}", Generation, Fitness);
        if (NodeViewer != null)
        {
            XmlWriterSettings _xwSettings = new XmlWriterSettings();
            _xwSettings.Indent = true;
            // Save genomes to xml file.        
            DirectoryInfo dirInf = new DirectoryInfo(Application.persistentDataPath);
            if (!dirInf.Exists)
            {
                Debug.Log("Creating subdirectory");
                dirInf.Create();
            }
            using (XmlWriter xw = XmlWriter.Create(popFileSavePath, _xwSettings))
            {
                experiment.SavePopulation(xw, _ea.GenomeList);
            }
            // Also save the best genome

            using (XmlWriter xw = XmlWriter.Create(champFileSavePath, _xwSettings))
            {
                experiment.SavePopulation(xw, new NeatGenome[] { _ea.CurrentChampGenome });
            }

            NodeViewer.GetComponent<GenomeViewController>().UpdateXml();

        }

        //    Utility.Log(string.Format("Moving average: {0}, N: {1}", _ea.Statistics._bestFitnessMA.Mean, _ea.Statistics._bestFitnessMA.Length));


    }

    public override void ea_PauseEvent(object sender, EventArgs e)
    {
        Time.timeScale = 1;
        Utility.Log("Done ea'ing (and neat'ing)");

        XmlWriterSettings _xwSettings = new XmlWriterSettings();
        _xwSettings.Indent = true;
        // Save genomes to xml file.        
        DirectoryInfo dirInf = new DirectoryInfo(Application.persistentDataPath);
        if (!dirInf.Exists)
        {
            Debug.Log("Creating subdirectory");
            dirInf.Create();
        }
        using (XmlWriter xw = XmlWriter.Create(popFileSavePath, _xwSettings))
        {
            experiment.SavePopulation(xw, _ea.GenomeList);
        }
        // Also save the best genome

        using (XmlWriter xw = XmlWriter.Create(champFileSavePath, _xwSettings))
        {
            experiment.SavePopulation(xw, new NeatGenome[] { _ea.CurrentChampGenome });
        }
        DateTime endTime = DateTime.Now;
        Utility.Log("Total time elapsed: " + (endTime - startTime));

        System.IO.StreamReader stream = new System.IO.StreamReader(popFileSavePath);



        EARunning = false;

    }

    public override void StopEA()
    {
        
        if (_ea != null && _ea.RunState == SharpNeat.Core.RunState.Running )
        {
            _ea.Stop();
        }
    }


    public void EvaluateLayers(IBlackBox box, int layers)
    {
        GameObject obj = Instantiate(Unit, Unit.transform.position, Unit.transform.rotation) as GameObject;
        UnitController controller = obj.GetComponent<UnitController>();
        Agar agarController = obj.GetComponent<Agar>();
        obj.SetActive(true);
        ControllerMap.Add(box, controller);

        controller.Activate(box);
        agarController.ActivateLayers(layers);

    }



    public override void Evaluate(IBlackBox box)
    {
        GameObject obj = Instantiate(Unit, Unit.transform.position, Unit.transform.rotation) as GameObject;
        UnitController controller = obj.GetComponent<UnitController>();

        ControllerMap.Add(box, controller);

        controller.Activate(box);
    }

    public override void StopEvaluation(IBlackBox box)
    {
        UnitController ct = ControllerMap[box];

        Destroy(ct.gameObject);
    }

    public override void RunBest()
    {
        Time.timeScale = 1;

        NeatGenome genome = null;


        // Try to load the genome from the XML document.
        try
        {
            using (XmlReader xr = XmlReader.Create(champFileSavePath))
                genome = NeatGenomeXmlIO.ReadCompleteGenomeList(xr, false, (NeatGenomeFactory)experiment.CreateGenomeFactory())[0];


        }
        catch (Exception e1)
        {
            // print(champFileLoadPath + " Error loading genome from file!\nLoading aborted.\n"
            //						  + e1.Message + "\nJoe: " + champFileLoadPath);
            return;
        }

        // Get a genome decoder that can convert genomes to phenomes.
        var genomeDecoder = experiment.CreateGenomeDecoder();
        // Decode the genome into a phenome (neural network).
        var phenome = genomeDecoder.Decode(genome);
        GameObject obj = Instantiate(Unit, Unit.transform.position, Unit.transform.rotation) as GameObject;
        UnitController controller = obj.GetComponent<UnitController>();
        runners.Add(obj);
        obj.SetActive(true);
            
        ControllerMap.Add(phenome, controller);

        controller.Activate(phenome);
    }

    public override float GetFitness(IBlackBox box)
    {


        if (ControllerMap.ContainsKey(box))
        {
            return ControllerMap[box].GetFitness();
        }
        return 0;
    }


    public override void NewFile()
    {
        champFileSavePath = Application.persistentDataPath + string.Format("/{0}.champ.xml", expiramentName + SubExpiramentNumber.ToString());
        popFileSavePath = Application.persistentDataPath + string.Format("/{0}.pop.xml", (expiramentName + SubExpiramentNumber.ToString()));
        print(champFileSavePath);


    }
    public override void Plus()
    {
        SubExpiramentNumber++;
        NewFile();
    }

    public override void Minus()
    {
        SubExpiramentNumber--;
        NewFile();
    }
    public override void Delete()
    {
        foreach (GameObject go in runners.ToArray())
        {
            Destroy(go);

        }
    }


}

