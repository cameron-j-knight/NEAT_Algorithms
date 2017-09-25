using UnityEngine;
using System.Collections;
using SharpNeat.Core;
using SharpNeat.Phenomes;
using System.Collections.Generic;

public class AgarEvaluator : IPhenomeEvaluator<IBlackBox>
{
    int LayerCycle = 0;
    ulong _evalCount;
    bool _stopConditionSatisfied;
    AgarOptimizer optimizer;
    FitnessInfo fitness;

    Dictionary<IBlackBox, FitnessInfo> dict = new Dictionary<IBlackBox, FitnessInfo>();

    public ulong EvaluationCount
    {
        get { return _evalCount; }
    }

    public bool StopConditionSatisfied
    {
        get { return _stopConditionSatisfied; }
    }

    public AgarEvaluator(AgarOptimizer se)
    {
        this.optimizer = se;
    }
    public int Layer = 0;
    public void LayerEvaluate(int layer)
    {
        Layer = layer;

    }
    public IEnumerator Evaluate(IBlackBox box)
    {
        if (optimizer != null)
        {

            optimizer.EvaluateLayers(box, Layer);
            yield return new WaitForSeconds(optimizer.TrialDuration);
            optimizer.StopEvaluation(box);
            float fit = optimizer.GetFitness(box);

            FitnessInfo fitness = new FitnessInfo(fit, fit);
            dict.Add(box, fitness);
            
        }
    }

    public void Reset()
    {
        this.fitness = FitnessInfo.Zero;
        dict = new Dictionary<IBlackBox, FitnessInfo>();
    }

    public FitnessInfo GetLastFitness()
    {

        return this.fitness;
    }


    public FitnessInfo GetLastFitness(IBlackBox phenome)
    {
        if (dict.ContainsKey(phenome))
        {
            FitnessInfo fit = dict[phenome];
            dict.Remove(phenome);

            return fit;
        }

        return FitnessInfo.Zero;
    }
}
