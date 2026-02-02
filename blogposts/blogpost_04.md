# Blog Post #04 - AR Development & Showcase
### Author: Viktor Ivaylov Ivanov
At the heart of the application lies the ReactionControl script. This component is responsible for detecting when a valid combination of atoms is present and triggering the creation of a molecule accordingly. Rather than relying on hard-coded reactions, the system uses a recipe-based approach, allowing molecules to be defined declaratively.

## Recipe-Based Design
Molecules are defined using structured data rather than logic hard-coded into scripts. Each molecule recipe specifies:

- A name
- A resulting molecule prefab
- A list of required ingredients and quantities

```
[System.Serializable]
public struct Ingredient
{
    public string elementName;
    public int requiredAmount;
}

[System.Serializable]
public class MoleculeRecipe
{
    public string recipeName;
    public GameObject moleculePrefab;
    public List<Ingredient> ingredients;
}
```

This design allows new molecules to be added simply by defining new recipes in the Unity Inspector, without modifying code. It also makes the system scalable and easy to maintain.

## Atom Tracking Structure

Tracked atoms are stored in a dictionary grouped by element type:
```
private Dictionary<string, List<GameObject>> activeAtoms =
    new Dictionary<string, List<GameObject>>();
```

This enables:

- Fast lookup by element type
- Efficient counting of nearby atoms
- Clean separation between different atom categories

## Cluster Detection Logic

The reaction system continuously checks whether a valid cluster of atoms exists for any recipe. Instead of testing all combinations, the algorithm uses anchor-based clustering, where each atom acts as a potential center point.

```
foreach (var recipe in recipes)
{
    foreach (var anchorAtom in allTrackedAtoms)
    {
        if (IsClusterValidForRecipe(recipe, anchorAtom.transform.position))
        {
            validRecipe = recipe;
            validCentroid = GetCentroidForCluster(recipe, anchorAtom.transform.position);
            goto RecipeFound;
        }
    }
}
```


This approach significantly reduces computational complexity and allows reactions to feel immediate and responsive.

## Proximity-Based Validation

Atoms are considered part of a reaction only if they fall within a configurable distance threshold:

```
if (Vector3.Distance(obj.transform.position, centerPoint)
    <= proximityThreshold)
{
    count++;
}
```

This ensures that molecule formation depends on spatial relationships, reinforcing the physicality of the interaction.

## Centroid Calculation

Once a valid cluster is detected, the system calculates a centroid position where the molecule should appear:
```
Vector3 sum = Vector3.zero;
int count = 0;

sum += obj.transform.position;
count++;

return sum / count;
```

This creates a natural visual transition from separate atoms to a single combined molecule.

## Reaction Triggering

When a valid recipe is detected, the molecule prefab is instantiated, visual effects are played, and the atoms involved in the reaction are temporarily hidden.

```
currentMoleculeObject =
    Instantiate(newRecipe.moleculePrefab, position, Quaternion.identity);
```

Visual and audio feedback reinforce the success of the reaction. However, the final product never fully implemented the sound.

```
Instantiate(reactionEffectPrefab, position, Quaternion.identity);
audioSource.PlayOneShot(reactionSound);
```

## Visual Cleanup & Feedback

Atoms that are consumed in a reaction are hidden, while unused atoms remain visible. This avoids visual clutter and clearly communicates which atoms were used.

```
SetVisualsVisible(atom, shouldShow);
```

This approach maintains clarity without permanently destroying atom objects, allowing reactions to update dynamically.

## Smooth Reaction Animation

To make molecule creation feel satisfying, a simple scale-based “pop-in” animation is applied which is followed by a "particle explosion" signifying a successful combination:
```
target.localScale = Vector3.one *
    Mathf.SmoothStep(0, 1, t);
```

With the AR System finalized and after more molecules were modeled and implemented, the system reached its current finalization. 

## Youtube Video Showcase
**Note: XR Simulator was used for this showcase since as of the time of creation of the video I did not have access to the equipment**
<a href="https://youtu.be/Yv7watknUBQ">Showcase</a>
