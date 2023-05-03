# Wine Mixing Program

This is a console program that takes as input a recipe of blended wines (e.g., `WineMixer\input\recipe.txt`), and a list of tank sizes (e.g., `WineMixer\input\tanks.txt`)
It outputs a process for adding wines to tanks and combining tanks until the desired recipe is created. 

## Requirements 

* at all times, each tank must be either completely full or completely empty to prevent oxidization. 

## Goal 

* minimize the number of steps 
* get as close to the recipe as possible  
* run in a reasonable amount of time 

## About the Algorithm

The algorithm to blend wines is a graph optimization problem. A node in the graph is the state of all of the tanks 
(whether it contains wine or not, and what the contents of that tank is), an edge is a transition between states. 

### About Edges, Steps, Operations, Transition

A step is an edge in the graph: also known as a transition. Currently, to simplify the code and minimize search states 
each transition is treated as the combination of 1, 2, or 3 operations.

An operation may be: 
* Adding wine to a tank
* Combining two tanks
* Splitting a tank into two 

A transition always combines two tanks, and adds wine to zero, one, or two tanks. If tanks have wine added, then
the transition must combine wine from that tank. 

## Scoring

The algorithm works by assigning a score to each transition. The score function is based on the Euclidean distance of 
the blended wines in the various tanks to the target blend, and the number of steps required. The lower the score the better

## Assumptions

* only two tanks can be added at a time
* it is okay if wine is left-over and unused in some tanks 
* wine cannot be dumped out 
* tanks cannot be split 
* it is better to be close to the recipe, even at the cost of more steps. 

## Example Output

```
Transition has 3 steps and score 99.9535971483266
Distance from target = 0.09695359714832659, wines = 3
Target = (0.05, 0.2, 0.5, 0.33, 0.08),
Tank [19] contains [(0, 0.2, 0.48, 0.32, 0)]
Add wine 1 to tank 7
Add wine 3 to tank 11
Combine tank 7 and 11 into tank 15
Add wine 2 to tank 14
Combine tank 14 and 15 into tank 19
```


