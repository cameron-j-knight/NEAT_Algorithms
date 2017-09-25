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

public class SimpleExperiment : INeatExperiment
{

    NeatEvolutionAlgorithmParameters _eaParams;
    NeatGenomeParameters _neatGenomeParams;
    string _name;
    int _populationSize;
    int _specieCount;
    NetworkActivationScheme _activationSchemeCppn;//++
    NetworkActivationScheme _activationScheme;
    string _complexityRegulationStr;
    int? _complexityThreshold;
    string _description;
    Optimizer _optimizer;
    int _inputCount;
    int _outputCount;

    public string Name
    {
        get { return _name; }
    }

    public string Description
    {
        get { return _description; }
    }

    public int InputCount
    {
        get { return _inputCount; }
    }

    public int OutputCount
    {
        get { return _outputCount; }
    }

    public int DefaultPopulationSize
    {
        get { return _populationSize; }
    }

    public NeatEvolutionAlgorithmParameters NeatEvolutionAlgorithmParameters
    {
        get { return _eaParams; }
    }

    public NeatGenomeParameters NeatGenomeParameters
    {
        get { return _neatGenomeParams; }
    }

    public void SetOptimizer(Optimizer se)
    {
        this._optimizer = se;
    }


    public void Initialize(string name, XmlElement xmlConfig)
    {
        Initialize(name, xmlConfig, 2, 2);
    }

    public void Initialize(string name, XmlElement xmlConfig, int input, int output)
    {
        _name = name;
        _populationSize = XmlUtils.GetValueAsInt(xmlConfig, "PopulationSize");
        _specieCount = XmlUtils.GetValueAsInt(xmlConfig, "SpecieCount");
        _activationScheme = ExperimentUtils.CreateActivationScheme(xmlConfig, "Activation");
        _activationSchemeCppn = ExperimentUtils.CreateActivationScheme(xmlConfig, "ActivationCppn");//++
        _complexityRegulationStr = XmlUtils.TryGetValueAsString(xmlConfig, "ComplexityRegulationStrategy");
        _complexityThreshold = XmlUtils.TryGetValueAsInt(xmlConfig, "ComplexityThreshold");
        _description = XmlUtils.TryGetValueAsString(xmlConfig, "Description");


        _eaParams = new NeatEvolutionAlgorithmParameters();
        _eaParams.SpecieCount = _specieCount;
        _neatGenomeParams = new NeatGenomeParameters();
      //  _neatGenomeParams.FeedforwardOnly = _activationScheme.AcyclicNetwork;
        _neatGenomeParams.FeedforwardOnly = _activationSchemeCppn.AcyclicNetwork;//++
        _neatGenomeParams.InitialInterconnectionsProportion = 0.5;//++

        _inputCount = input;
        _outputCount = output;
    }

    public List<NeatGenome> LoadPopulation(XmlReader xr)
    {
        NeatGenomeFactory genomeFactory = (NeatGenomeFactory)CreateGenomeFactory();
        return NeatGenomeXmlIO.ReadCompleteGenomeList(xr, false, genomeFactory);
    }

    public void SavePopulation(XmlWriter xw, IList<NeatGenome> genomeList)
    {
        NeatGenomeXmlIO.WriteComplete(xw, genomeList, false);
    }
    private void AddNode(SubstrateNodeSet nodeSet, uint id, double x, double y, double z)
    {
        nodeSet.NodeList.Add(new SubstrateNode(id, new double[] { x, y, z }));
    }
    public IGenomeDecoder<NeatGenome, IBlackBox> CreateGenomeDecoder()
    {
        // Create HyperNEAT network substrate.

        //-- Create input layer nodes.
        SubstrateNodeSet inputLayer = new SubstrateNodeSet(5);
        AddNode(inputLayer, 1, -1.0, 0.0, -1.0);// Left
        AddNode(inputLayer, 2, +1.0, 0.0, -1.0);// right
        AddNode(inputLayer, 3, 0.0, +1.0, -1.0);// up
        AddNode(inputLayer, 4, 0.0, -1.0, -1.0);// down
        AddNode(inputLayer, 5, 0.0, +0.0, -1.0);// Distance

        SubstrateNodeSet outputLayer = new SubstrateNodeSet(2);
        AddNode(outputLayer, 6, +1.0, 0.0, +1.0);// right
        AddNode(outputLayer, 7, 0.0, +1.0, +1.0);// up
       // AddNode(outputLayer, 8, 0.0, +1.0, +1.0);// up
       // AddNode(outputLayer, 9, 0.0, -1.0, +1.0);// down

        SubstrateNodeSet h1Layer = new SubstrateNodeSet(4);
        AddNode(h1Layer, 10, -1.0, +1.0, 0.0);// Left
        AddNode(h1Layer, 11, +1.0, +1.0, 0.0);// right
        AddNode(h1Layer, 12, -1.0, -1.0, 0.0);// up
        AddNode(h1Layer, 13, +1.0, -1.0, 0.0);// down

        // Connect up layers.
        List<SubstrateNodeSet> nodeSetList = new List<SubstrateNodeSet>(2);
        nodeSetList.Add(inputLayer);
        nodeSetList.Add(outputLayer);
       // nodeSetList.Add(h1Layer);

        List<NodeSetMapping> nodeSetMappingList = new List<NodeSetMapping>(1);
        nodeSetMappingList.Add(NodeSetMapping.Create(0, 1, (double?)null));  // Input -> Output.
       // nodeSetMappingList.Add(NodeSetMapping.Create(0, 2, (double?)null));  // Input -> Hidden.
        //nodeSetMappingList.Add(NodeSetMapping.Create(2, 1, (double?)null));  // Hidden -> Output.
      //  nodeSetMappingList.Add(NodeSetMapping.Create(1, 2, (double?)null));  // Output -> Hidden

        // Construct substrate.
        Substrate substrate = new Substrate(nodeSetList, DefaultActivationFunctionLibrary.CreateLibraryNeat(SteepenedSigmoid.__DefaultInstance), 0, 0.01, 5, nodeSetMappingList);//++


        // Create genome decoder. Decodes to a neural network packaged with an activation scheme that defines a fixed number of activations per evaluation.
        IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder = new HyperNeatDecoder(substrate, _activationSchemeCppn, _activationScheme,false);//++
        //return new NeatGenomeDecoder(_activationScheme);
        return genomeDecoder;

    }

    public IGenomeFactory<NeatGenome> CreateGenomeFactory()
    {
        return new CppnGenomeFactory(InputCount, OutputCount, GetCppnActivationFunctionLibrary(), _neatGenomeParams);
    }


    IActivationFunctionLibrary GetCppnActivationFunctionLibrary()
    {
        return DefaultActivationFunctionLibrary.CreateLibraryCppn();
    }

    public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(string fileName)
    {
        List<NeatGenome> genomeList = null;
        IGenomeFactory<NeatGenome> genomeFactory = CreateGenomeFactory();
        try
        {
            if (fileName.Contains("/.pop.xml"))
            {
                throw new Exception();
            }
            using (XmlReader xr = XmlReader.Create(fileName))
            {
                genomeList = LoadPopulation(xr);
            }
        }
        catch (Exception e1)
        {
            Utility.Log(fileName + " Error loading genome from file!\nLoading aborted.\n"
                                      + e1.Message + "\nJoe: " + fileName);

            genomeList = genomeFactory.CreateGenomeList(_populationSize, 0);

        }



        return CreateEvolutionAlgorithm(genomeFactory, genomeList);
    }

    public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm()
    {
        return CreateEvolutionAlgorithm(_populationSize);
    }

    public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(int populationSize)
    {
        IGenomeFactory<NeatGenome> genomeFactory = CreateGenomeFactory();

        List<NeatGenome> genomeList = genomeFactory.CreateGenomeList(populationSize, 0);

        return CreateEvolutionAlgorithm(genomeFactory, genomeList);
    }

    public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(IGenomeFactory<NeatGenome> genomeFactory, List<NeatGenome> genomeList)
    {
        IDistanceMetric distanceMetric = new ManhattanDistanceMetric(1.0, 0.0, 10.0);
        ISpeciationStrategy<NeatGenome> speciationStrategy = new KMeansClusteringStrategy<NeatGenome>(distanceMetric);

        IComplexityRegulationStrategy complexityRegulationStrategy = ExperimentUtils.CreateComplexityRegulationStrategy(_complexityRegulationStr, _complexityThreshold);

        NeatEvolutionAlgorithm<NeatGenome> ea = new NeatEvolutionAlgorithm<NeatGenome>(_eaParams, speciationStrategy, complexityRegulationStrategy);

        // Create black box evaluator       
        SimpleEvaluator evaluator = new SimpleEvaluator(_optimizer);

        IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder = CreateGenomeDecoder();


        IGenomeListEvaluator<NeatGenome> innerEvaluator = new UnityParallelListEvaluator<NeatGenome, IBlackBox>(genomeDecoder, evaluator, _optimizer);

        IGenomeListEvaluator<NeatGenome> selectiveEvaluator = new SelectiveGenomeListEvaluator<NeatGenome>(innerEvaluator,
            SelectiveGenomeListEvaluator<NeatGenome>.CreatePredicate_OnceOnly());

        //ea.Initialize(selectiveEvaluator, genomeFactory, genomeList);
        ea.Initialize(selectiveEvaluator, genomeFactory, genomeList);

        return ea;
    }
}



//-- Hip joint inputs.
// Left hip joint.
//double x = -1.0;
//double y = +1.0;
//double z = -1.0;
//for (int i = 1; i <= _inputCount; i++)
//{
//    AddNode(inputLayer, (uint)i, x, y, z);
//    if(x < 1.0)
//    {
//     x += 1.0/InputCount;
//    }

//}

// Angular velocity.
// AddNode(inputLayer, 2, -0.5, +1.0, -1.0);  // Angle.

//// Right hip joint.
//AddNode(inputLayer, 3, +0.5, +1.0, -1.0);  // Angle.
//AddNode(inputLayer, 4, +1.0, +1.0, -1.0);  // Angular velocity.


//-- Output layer nodes.


//for (int i = 1; i <= _outputCount; i++)
//{
//    AddNode(h1Layer, (uint)(i+ _outputCount + _inputCount), x,y, z);
//    if (x < 1.0)
//    {
//        x += 1.0 / InputCount;
//    }
//}
//AddNode(h1Layer, 7, -1.0, +1.0, 0.0);


//AddNode(h1Layer, 8, +1.0, +1.0, 0.0);
// x = -1.0;
// y = +1.0;
// z = +1.0;
//for (int i = 1; i <= _outputCount; i++)
//{
//    AddNode(outputLayer, (uint)(i + _inputCount), x, y, z);
//    if (x < 1.0)
//    {
//        x += 1.0 / InputCount;
//    }
//}
//AddNode(outputLayer, 5, -1.0, +1.0, +1.0); // Left hip torque.
//AddNode(outputLayer, 6, +1.0, +1.0, +1.0); // Right hip torque.


//-- Hidden layer nodes.
//x = -1.0;
//y = +1.0;
//z = 0.0;