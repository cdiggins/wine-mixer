## Investigation

## First Attempt: Brute Force

The first thing attempted was a brute force space search of "operations".
After each operation a score is assigned to the state based on its 
distance from the desired blend. 

At each step, add wine to a tank, remove wine, or combine two tanks. 
This creates an infinite graph of possibilities, kind of like a chess game. 
At every stage we have a large number of possibilities which multiplied 
at every level. Kind of like the number of chess possibilities. 

After a few operations this number becomes astronomical. Consider 
if each operation has 20 possible next operations. 
After only 5 operations, the number of configurations to be considered 
is `20 * 20 * 20 * 20 * 20` which is 3,200,000. 

## Optimization: Precompute Combine Operation 

The first optimization applied was to precompute the set of 
valid "combine operations". There are n^2 possibilities to consider
where n is the number of tanks. The observation is that 
two tanks can only be combined if they add up in size to another tank. 

This optimization is important, but does not change the overall
running time of the algorithm. 

## Simplification: No Removing of Wine
 
Given the sheer size of the search space the ability to remove wine 
from a tank has been removed. 

## Second Attempt: Greedy Random Algorithm 

The next hypothesis is that the space was so large it wasn't searchable 
in completeness. Even though no discernible optimal sub-problem structure 
was uncovered in the algorithm, an attempt was made to choose random 
batches of states, choose a percentage of the best states, and apply 
random operations to them. 

For a small input tank set and input wine set this was able to 
generate some solutions that appeared reasonable. 

However, it was observed that the chances of such an algorithm 
getting stuck in a local minima were very high, and for it to 
virtually never consider possibilities that could 
lead to better results.  
	
## Optimization: Transitions instead of Operations

The tree created by looking at all operations could involve 
non-sensical operations like adding wine to a tank, and then 
never using it.  

In reconsidering the algorithm a bit like a person would operations 
were combined into groups that involved a combine and 0,1, or 2
tanks that have wine added. These groups are called transitions
and the requirement was that if wine is added.

This algorithm executed much faster, but still would not scale 
to the desired number of input tanks and possible wines. 

## The Problem Solving Approach

The problem solving approach used to this point was consistently:

1. Considered wines and tanks simultatneously - 
which multiplied the size of search space considerably 

2. Started from starting states and evaluating possible transitions.  

It was hypothesized that perhaps choosing wines could be done 
indpendently from considering tanks and the possible wine proportions 
that each one could create. 

Also it was hypothesized that we could work our way backwards 
starting from a tank and considering how it may be broken up 
into source tanks. 

## Thinking in Terms of Graphs 

Rather than thinking about wine up-front a new hypothesis 
was formed that one could just consider each tank and the 
possible combine operations to blend tanks. 

In effect this means we think about a graph where each node is a tank,
and each edge is a transition which itself can come from multiple tanks. 

This is not a standard way to think about graphs because 
an edge can have multiple inputs and a single output. 

What is interesting is that the graph space is not as enormous. 

For each tank we would arrive at a set of proportions representing the 
possible input tanks. For example consider the following tanks. 

```
A=10
	B=5
		D=4
		E=1
	C=5
		F=2
		G=3
	H=6
```

Tank A can be fed by tanks B and C, or by tanks D and H. 

We observe that because B and C can be broken down, we can say that tank
A can be fed by tanks D, E, C or B, F, G. 

To simplify things we can just consider "source" nodes, i.e., those
with no inputs. 

In this case we can say that the possible sources of A are:

```
A <= D, E, F, G
A <= D, H
```

## Investigation

The major questions at this point are: 

1. how big will the search space be?  
2. is there repetition that we can removed? 
3. if so how can we remove the repetition? 
4. given a set of sources, how do we compare that to a wine blend? 

## From Tanks to Wine Blend 

A set of source can be represented as a list of integers, representing
the sizes. 

```
A <= 1, 2, 3, 4
A <= 4, 6
```

We can easily normalize to percentages: 

```
A <= 10, 20, 30, 40
A <= 40, 60
```

We observe that it is possible to put the same wine in two tanks. 
So we can also generate a large range of permutations: 

```
A <= 10, 20, 30, 40
A <= 10, 50, 40
A <= 10, 30, 60
A <= 10, 20, 70
A <= 10, 90
A <= 20, 40, 40
A <= 20, 30, 50
A <= 20, 80
A <= 30, 30, 40
A <= 30, 70
A <= 40, 60
```

## Unit Testing

Given that the main program had been written, and was basically working
the new hypotheses about tanks and graphs are created and tested
within a unit test project. 

## Observations

* If two tanks have the same size the input sources are the same. 
* The same input sources are generated over and over again. 
* It seems that the same core inputs are used over and over again. 
* The number of wines that can be used in a blend are restricted to the number of sources.
* The number of sources is not that large

### Challenge

* How do I quickly avoid having the same set of inputs. 

### Ideas:

* Once a tank size has been computed, re-use it for computing sources
* Maybe sources need to be stored outside of a node (dictionary)
* Maybe I can use a tree of some kind to represent the set of inputs?
* Alternatively I could use a hash set, and just has the list contents. 

### More observations

When using a hash set of tank sets, we can avoid 
significant amounts of recomputation. Computing the input 
sources was accomplished for a large set of tanks 
very quickly. 

Hashing is done on the string that represents the input 
source. 

The first attempt used tank indices. When multiple 
tanks have the same size, the result would be equivalent.

Therefore adjusted to output just sizes. 

## New Challenges: Tank Splitting and Multiple Inputs

In the recent investigation, tank splitting and multiple inputs were ignored 

When tanks can be split the process becomes more complicated.
The path is no longer straightforward, and now tanks can have wine
and no longer be available for consideration in the process. 

It is possible for tanks to have wine left over and unused. 

## Additional investigation

Is there a way of saying that one blending operation is better than another? 
That it brings us closer to the end goal? 

We could say simply: *does the blend result in a closer result?* However 
this does not take into the account that if we have four wines, combining 
A and B into AB, and C and D into CD, can be better than combining ABC.

What I mean is that they are kind of orthogonal to each other, but having a 
variety of wines brings us overall closer to the goal. 

## Hypotheses

* It seems logical that a variety of wines in different barrels is good. 
* Two wines that have the correct ratio to each other is attractive 
* However, the size of that tank is also an important consideration. 
* When tanks are used up, they are no longer available. 
* The wines with larger percentages matter more than the wines with smaller percentages. 

## Partial Optimal Subproblem

* Given two tanks of size X and having wines A and B
* Blending them should give a wine blend C that is closer to the target than either A or B. 


















