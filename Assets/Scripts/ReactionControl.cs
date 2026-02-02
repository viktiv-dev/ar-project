using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 

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

    public int GetTotalAtoms()
    {
        int total = 0;
        if (ingredients == null) return 0;
        foreach (var i in ingredients) total += i.requiredAmount;
        return total;
    }
}

public class ReactionControl : MonoBehaviour
{
    [Header("Managers")]
    public ARTrackedImageManager imageManager;

    [Header("Settings")]
    public float proximityThreshold = 0.15f;

    [Header("Visual Feedback")]
    public GameObject reactionEffectPrefab;
    public AudioClip reactionSound;
    private AudioSource audioSource;

    [Header("Atom Prefabs")]
    public GameObject atomH_Prefab;

    public GameObject atomO_Prefab;
    public GameObject atomC_Prefab;

    [Header("Recipes")]
    public List<MoleculeRecipe> recipes;

    private Dictionary<string, List<GameObject>> activeAtoms = new Dictionary<string, List<GameObject>>();
    private GameObject currentMoleculeObject;
    private string currentRecipeName = "";

    void Start()
    {
        if (recipes == null) recipes = new List<MoleculeRecipe>();
        recipes.Sort((a, b) => b.GetTotalAtoms().CompareTo(a.GetTotalAtoms()));

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void OnEnable()
    {
        if (imageManager != null) imageManager.trackedImagesChanged += OnChanged;
    }

    void OnDisable()
    {
        if (imageManager != null) imageManager.trackedImagesChanged -= OnChanged;
    }

    void Update()
    {
        CheckForRecipes();
    }

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var img in eventArgs.added) UpdateAtom(img);
        foreach (var img in eventArgs.updated) UpdateAtom(img);
        foreach (var img in eventArgs.removed) RemoveAtom(img);
    }

    void UpdateAtom(ARTrackedImage img)
    {
        if (img == null || img.referenceImage == null) return;
        string name = img.referenceImage.name;
        if (string.IsNullOrEmpty(name)) return;

        if (!activeAtoms.ContainsKey(name)) activeAtoms[name] = new List<GameObject>();

        string id = img.trackableId.ToString();
        GameObject atomVisual = activeAtoms[name].Find(x => x != null && x.name == id);

        if (atomVisual == null)
        {
            GameObject prefabToSpawn = null;

            if (name == "Hydrogen") prefabToSpawn = atomH_Prefab;
            else if (name == "Oxygen") prefabToSpawn = atomO_Prefab;
            else if (name == "Carbon") prefabToSpawn = atomC_Prefab;

            if (prefabToSpawn != null)
            {
                atomVisual = Instantiate(prefabToSpawn, img.transform);
                atomVisual.name = id;
                activeAtoms[name].Add(atomVisual);
            }
        }

        if (atomVisual != null)
        {
            atomVisual.transform.position = img.transform.position + (img.transform.up * 0.15f);
            atomVisual.transform.rotation = img.transform.rotation;
        }
    }

    void RemoveAtom(ARTrackedImage img)
    {
        if (img == null) return;
        string name = img.referenceImage != null ? img.referenceImage.name : null;
        string id = img.trackableId.ToString();

        if (!string.IsNullOrEmpty(name) && activeAtoms.ContainsKey(name))
        {
            GameObject atomToDelete = activeAtoms[name].Find(x => x != null && x.name == id);
            if (atomToDelete != null)
            {
                activeAtoms[name].Remove(atomToDelete);
                Destroy(atomToDelete);
            }
            if (activeAtoms[name].Count == 0) activeAtoms.Remove(name);
            return;
        }

        foreach (var kv in activeAtoms)
        {
            for (int i = kv.Value.Count - 1; i >= 0; --i)
            {
                var go = kv.Value[i];
                if (go == null) { kv.Value.RemoveAt(i); continue; }
                if (go.name == id) { Destroy(go); kv.Value.RemoveAt(i); }
            }
        }
    }

    void CheckForRecipes()
    {
        MoleculeRecipe validRecipe = null;
        Vector3 validCentroid = Vector3.zero;
        List<GameObject> usedAtoms = new List<GameObject>(); 

        List<GameObject> allTrackedAtoms = GetAllTrackedAtoms();

        foreach (var recipe in recipes)
        {
            if (recipe == null || recipe.ingredients == null) continue;

            foreach (var anchorAtom in allTrackedAtoms)
            {
                if (IsClusterValidForRecipe(recipe, anchorAtom.transform.position))
                {
                    validRecipe = recipe;
                    validCentroid = GetCentroidForCluster(recipe, anchorAtom.transform.position);
                    usedAtoms = GetAtomsForRecipe(recipe, anchorAtom.transform.position);
                    goto RecipeFound;
                }
            }
        }

        RecipeFound:

        if (validRecipe != null)
        {
            if (currentMoleculeObject != null && currentRecipeName == validRecipe.recipeName)
            {
                currentMoleculeObject.transform.position = Vector3.Lerp(currentMoleculeObject.transform.position, validCentroid, Time.deltaTime * 5f);
                UpdateAtomVisibility(usedAtoms);
            }
            else
            {
                TriggerReaction(validRecipe, validCentroid, usedAtoms);
            }
        }
        else
        {
            if (currentMoleculeObject != null)
            {
                Destroy(currentMoleculeObject);
                currentMoleculeObject = null;
                currentRecipeName = "";
            }
            UpdateAtomVisibility(new List<GameObject>());
        }
    }

    List<GameObject> GetAtomsForRecipe(MoleculeRecipe recipe, Vector3 centerPoint)
    {
        List<GameObject> used = new List<GameObject>();
        foreach (var ingredient in recipe.ingredients)
        {
            if (activeAtoms.ContainsKey(ingredient.elementName))
            {
                var candidates = activeAtoms[ingredient.elementName]
                    .Where(x => IsValidAtom(x) && Vector3.Distance(x.transform.position, centerPoint) <= proximityThreshold)
                    .OrderBy(x => Vector3.Distance(x.transform.position, centerPoint)); 

                int collected = 0;
                foreach (var atom in candidates)
                {
                    if (collected < ingredient.requiredAmount)
                    {
                        used.Add(atom);
                        collected++;
                    }
                }
            }
        }
        return used;
    }

    void UpdateAtomVisibility(List<GameObject> usedAtoms)
    {
        foreach (var kvp in activeAtoms)
        {
            foreach (var atom in kvp.Value)
            {
                if (atom != null)
                {
                    bool isUsed = usedAtoms != null && usedAtoms.Contains(atom);
                    bool shouldShow = IsValidAtom(atom) && !isUsed;
                    SetVisualsVisible(atom, shouldShow);
                }
            }
        }
    }

    bool IsClusterValidForRecipe(MoleculeRecipe recipe, Vector3 centerPoint)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            int count = CountAtomsAroundPoint(ingredient.elementName, centerPoint);
            if (count < ingredient.requiredAmount) return false;
        }
        return true;
    }

    int CountAtomsAroundPoint(string key, Vector3 centerPoint)
    {
        if (!activeAtoms.ContainsKey(key)) return 0;
        int count = 0;
        foreach (var obj in activeAtoms[key])
        {
            if (IsValidAtom(obj) && Vector3.Distance(obj.transform.position, centerPoint) <= proximityThreshold)
                count++;
        }
        return count;
    }

    Vector3 GetCentroidForCluster(MoleculeRecipe recipe, Vector3 anchorPoint)
    {
        Vector3 sum = Vector3.zero;
        int count = 0;
        foreach (var ingredient in recipe.ingredients)
        {
            if (activeAtoms.ContainsKey(ingredient.elementName))
            {
                foreach (var obj in activeAtoms[ingredient.elementName])
                {
                    if (IsValidAtom(obj) && Vector3.Distance(obj.transform.position, anchorPoint) <= proximityThreshold)
                    {
                        sum += obj.transform.position;
                        count++;
                    }
                }
            }
        }
        return count > 0 ? sum / count : anchorPoint;
    }

    void TriggerReaction(MoleculeRecipe newRecipe, Vector3 position, List<GameObject> usedAtoms)
    {
        if (currentMoleculeObject != null) Destroy(currentMoleculeObject);

        if (newRecipe.moleculePrefab != null)
        {
            currentMoleculeObject = Instantiate(newRecipe.moleculePrefab, position, Quaternion.identity);
            currentRecipeName = newRecipe.recipeName;
        }
        else
        {
            currentRecipeName = "";
            currentMoleculeObject = null;
        }

        if (reactionEffectPrefab != null) Instantiate(reactionEffectPrefab, position, Quaternion.identity);
        if (reactionSound != null && audioSource != null) audioSource.PlayOneShot(reactionSound);
        if (currentMoleculeObject != null) StartCoroutine(AnimatePopIn(currentMoleculeObject.transform));
        
        UpdateAtomVisibility(usedAtoms);
    }

    IEnumerator AnimatePopIn(Transform target)
    {
        float t = 0;
        if (target == null) yield break;
        target.localScale = Vector3.zero;
        while (t < 1)
        {
            t += Time.deltaTime * 5f;
            if (target != null) target.localScale = Vector3.one * Mathf.SmoothStep(0, 1, t);
            yield return null;
        }
    }

    bool IsValidAtom(GameObject obj)
    {
        if (obj == null) return false;
        var parent = obj.transform.parent;
        if (parent == null) return false;
        var tracked = parent.GetComponent<ARTrackedImage>();
        return tracked != null && tracked.trackingState == TrackingState.Tracking;
    }

    List<GameObject> GetAllTrackedAtoms()
    {
        List<GameObject> all = new List<GameObject>();
        foreach (var kvp in activeAtoms)
        {
            foreach (var obj in kvp.Value)
            {
                if (IsValidAtom(obj)) all.Add(obj);
            }
        }
        return all;
    }

    void SetVisualsVisible(GameObject root, bool visible)
    {
        if (root == null) return;
        foreach (var r in root.GetComponentsInChildren<Renderer>()) if (r != null) r.enabled = visible;
        foreach (var t in root.GetComponentsInChildren<TrailRenderer>()) if (t != null) t.enabled = visible;
    }
}