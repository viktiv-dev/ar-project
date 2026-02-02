using UnityEngine;

[ExecuteInEditMode]
public class BondBuilder : MonoBehaviour
{
    [Header("Select Two Atoms")]
    public Transform atomA;
    public Transform atomB;

    [Header("Bond Settings")]
    public float bondThickness = 0.02f;
    public Material bondMaterial;

    [Header("Actions")]
    [Tooltip("Right-click this component name and select 'Create Bond'")]
    public bool clickToCreate = false;

    void Update()
    {
        if (clickToCreate)
        {
            CreateBond();
            clickToCreate = false;
        }
    }

    [ContextMenu("Create Bond Now")]
    public void CreateBond()
    {
        if (atomA == null || atomB == null)
        {
            Debug.LogError("Assign Atom A and Atom B first!");
            return;
        }

        Vector3 start = atomA.position;
        Vector3 end = atomB.position;
        Vector3 center = (start + end) / 2f;
        float distance = Vector3.Distance(start, end);

        GameObject bond = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        bond.name = $"Bond_{atomA.name}_{atomB.name}";

        bond.transform.SetParent(transform, true);

        bond.transform.position = center;
        bond.transform.LookAt(end);

        bond.transform.Rotate(90, 0, 0);

        bond.transform.localScale = new Vector3(bondThickness, distance / 2f, bondThickness);

        if (bondMaterial != null)
        {
            bond.GetComponent<Renderer>().material = bondMaterial;
        }
    }
}