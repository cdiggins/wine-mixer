# Wine Mixing Program

This is a console program that takes as input a recipe of blended wines, and a list of tank sizes.

It outputs a process for adding wines to tanks and combining tanks until the desired recipe is created. 

## Sample Input

See: `WineMixer\input\recipe.txt` and `WineMixer\input\tanks.txt`

## Goal 

* minimize the number of steps 
* get as close to the recipe as possible  
* run in a reasonable amount of time 

## Assumptions a

* only two tanks can be added at a time
* no wine can be thrown out 
* tanks cannot be split 
* it is better to be close to the recipe, even at the cost of more steps. 

## Requirements 

* at all times, each tank must be either completely full or completely empty 

## Classes

Mix - A vector of values, each one representing a fraction (from 0 to 1) of the wine contained in a tank or in the recipe   
State - Represent the contents of each tank. Each tank is assigned a Mix value representing the blend of wine, or a null value if no wine is present.  
Step - A class that represents the combination of two tanks, the adding of wine to a tank, the splitting of a tank into two tanks, or the removal of wine from a tank 
Transition - A transtion between states that consists of combining two tanks, and optionally adding wine to one or two barrels. 
TankSizes - The size of each tank, used to precompute valid steps (e.g., which tanks can be combined)  

## Scoring

Each 
