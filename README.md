# NEAT_Algorithms
A suite of tests using the Neural Evolution of Augmented Typologies Technique discussed in [this](http://nn.cs.utexas.edu/downloads/papers/stanley.ec02.pdf) paper:

#Abstract
Many real life situations lack ideal solutions that can be represented using traditional methods, and must instead be optimized through a different approach. I designed and tested neural networks that attempt to find an optimal solution to such problems. However, the neural networks were not actually generated directly by me, but by a system I wrote that generated these neural networks through generational evolution. The system works similar to that of evolution observed in biology where species evolve over many generations exhibiting new traits as generations pass based on the traits of the species that do the best and can reproduce.
#Theory
A neural network can be thought of as a computer simulating a brain. Just as in a human, these neurons, or “nodes”, are connected into a network. Each node takes an input and passes an output on to more nodes. Nodes are essentially biological neurons, but with a few extra computational capabilities.
#Application
I designed an ant simulation test and a game of the 1978 arcade game asteroids using a 3d simulation engine. In these tests optimal play styles were derived from a randomly generated brain that mutated and evolved over many generations. The ants devised multiple effective strategies over the course of 26 generations where they could successfully complete the goal of pathfinding between the food source and an ant hill. Remember that these brains had no idea what the problem was before starting the test. Just with the basic principle of evolution these ants were able to effectively learn how to complete a task in a relatively short timeframe.
#Learnings
This success attests to the capability of neuroevolution to solve the strategy based problems which frequently occur in nature. In the future, these techniques could be used to address even more complicated and important problems, as the size of the network can continue to grow and approach that of an actual organism.
