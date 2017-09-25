
using System.Collections;
namespace SharpNeat.Core
{

/// <summary>
/// Represents an evaluator that competes two phenomes against each other.
/// </summary>
public interface ICoevolutionPhenomeEvaluator<TPhenome>
{
    /// <summary>
    /// Evaluate the provided phenomes and return their fitness scores.
    /// </summary>
    void Evaluate(TPhenome phenome1, TPhenome phenome2, 
                       out FitnessInfo fitness1, out FitnessInfo fitness2);
}
}

