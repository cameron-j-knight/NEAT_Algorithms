using System.Collections.Generic;
using System.Collections;
using SharpNeat.Genomes.Neat;
using System;

namespace SharpNeat.Core
{
    public class ParallelCoevolutionListEvaluator<TGenome, TPhenome>
         : IGenomeListEvaluator<TGenome>
         where TGenome : class, IGenome<TGenome>
         where TPhenome : class, IPhenomeEvaluator<TPhenome>
    {
        readonly IGenomeDecoder<TGenome, TPhenome> _genomeDecoder;
        readonly ICoevolutionPhenomeEvaluator<TPhenome> _phenomeEvaluator;

        public ulong EvaluationCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool StopConditionSatisfied
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        //readonly ParallelOptions _parallelOptions;
        /// <summary>
        /// Main genome evaluation loop with no phenome caching (decode 
        /// on each evaluation). Individuals are competed pairwise against
        /// every other in the population.
        /// Evaluations are summed to get the final genome fitness.
        /// </summary>
        public void Evaluate(IList<TGenome> genomeList)
        {
            //Create a temporary list of fitness values
            FitnessInfo[] results = new FitnessInfo[genomeList.Count];
            for (int i = 0; i < results.Length; i++)
                results[i] = FitnessInfo.Zero;

            // Exhaustively compete individuals against each other.
          for(int i =0; i < genomeList.Count; i++)
            {
                for (int j = 0; j < genomeList.Count; j++)
                {
                    // Don't bother evaluating inviduals against themselves.
                    if (i == j)
                        continue;

                    // Decode the first genome.
                    TPhenome phenome1 = _genomeDecoder.Decode(genomeList[i]);

                    // Check that the first genome is valid.
                    if (phenome1 == null)
                        continue;

                    // Decode the second genome.
                    TPhenome phenome2 = _genomeDecoder.Decode(genomeList[j]);

                    // Check that the second genome is valid.
                    if (phenome2 == null)
                        continue;

                    // Compete the two individuals against each other and get
                    // the results.
                    FitnessInfo fitness1, fitness2;
                    _phenomeEvaluator.Evaluate(phenome1, phenome2,
                                                           out fitness1, out fitness2);

                    // Add the results to each genome's overall fitness.
                    // Note that we need to use a lock here because
                    // the += operation is not atomic.
                    lock (results)
                    {
                        results[i]._fitness += fitness1._fitness;
                        //results[i]._alternativeFitness +=
                        //                              fitness1._alternativeFitness;
                        results[j]._fitness += fitness2._fitness;
                        //results[j]._alternativeFitness +=
                        //                              fitness2._alternativeFitness;
                    }
                }
            }

            // Update every genome in the population with its new fitness score.
            for (int i = 0; i < results.Length; i++)
            {
                genomeList[i].EvaluationInfo.SetFitness(results[i]._fitness);
                //genomeList[i].EvaluationInfo.AlternativeFitness =
                //                                      results[i]._alternativeFitness;
            }
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        IEnumerator IGenomeListEvaluator<TGenome>.Evaluate(IList<TGenome> genomeList)
        {
            throw new NotImplementedException();
        }
    }
}
