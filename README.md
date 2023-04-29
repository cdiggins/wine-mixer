# Wine Mixing Program

This is a console program that takes as input a recipe of blended wines (e.g., `WineMixer\input\recipe.txt`) , and a list of tank sizes (e.g., `WineMixer\input\tanks.txt`)

It outputs a process for adding wines to tanks and combining tanks until the desired recipe is created. 

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

## Goal 

* minimize the number of steps 
* get as close to the recipe as possible  
* run in a reasonable amount of time 

## Assumptions

* only two tanks can be added at a time
* no wine can be thrown out 
* tanks cannot be split 
* it is better to be close to the recipe, even at the cost of more steps. 

## Requirements 

* at all times, each tank must be either completely full or completely empty 

## Scoring

The algorithm works by assigning a score to each transition. The score function is based on the Euclidean distance of 
the blended wines in the various tanks to the target blend, and the number of steps required. The lower the score the better

## Classes

Mix - A vector of values, each one representing a fraction (from 0 to 1) of the wine contained in a tank or in the recipe   
State - Represent the contents of each tank. Each tank is assigned a Mix value representing the blend of wine, or a null value if no wine is present.  
Step - A class that represents the combination of two tanks, the adding of wine to a tank, the splitting of a tank into two tanks, or the removal of wine from a tank 
Transition - A transtion between states that consists of combining two tanks, and optionally adding wine to one or two barrels. 
TankSizes - The size of each tank, used to precompute valid steps (e.g., which tanks can be combined)  

