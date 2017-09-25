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


public abstract class Optimizer : MonoBehaviour {
    public float TrialDuration;
    public string popFileSavePath, champFileSavePath;
    public string expiramentName = "name";
    public int Trials = 1;
    public float StoppingFitness = 100f;
    public int SubExpiramentNumber = 0;
    public bool Stop = false;
    public uint BirthGeneration;
    public int SpeciesNumber;
    public uint ID;
    public abstract void StartEA();

    public abstract void ea_UpdateEvent(object sender, EventArgs e);

    public abstract void ea_PauseEvent(object sender, EventArgs e);

    public abstract void StopEA();

    public abstract void Evaluate(IBlackBox box);

    public abstract void StopEvaluation(IBlackBox box);

    public abstract void RunBest();

    public abstract float GetFitness(IBlackBox box);

    public abstract void NewFile();

    public abstract void Plus();

    public abstract void Minus();

    public abstract void Delete();


}
