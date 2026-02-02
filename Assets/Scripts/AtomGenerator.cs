using UnityEngine;
using System.Collections.Generic;

public class AtomGenerator : MonoBehaviour
{
    [Header("Settings")]
    public int electronCount = 1; 
    public Color nucleusColor = Color.blue;
    public float orbitRadius = 0.08f; 
    public float electronSpeed = 200f;
    
    [Header("Prefabs")]
    public GameObject nucleusPrefab;
    public GameObject electronPrefab;

    private List<Transform> pivots = new List<Transform>();
    private Transform nucleus;

    void Start()
    {
        BuildAtom();
    }

    void BuildAtom()
    {
        if (nucleusPrefab)
        {
            GameObject n = Instantiate(nucleusPrefab, transform);
            n.transform.localPosition = Vector3.zero;
            nucleus = n.transform;
            
            Renderer r = n.GetComponent<Renderer>();
            if(r) r.material.color = nucleusColor;
        }

        if (electronPrefab)
        {
            for (int i = 0; i < electronCount; i++)
            {
                GameObject pivot = new GameObject("Orbit");
                pivot.transform.SetParent(transform, false);
                pivot.transform.localRotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));

                GameObject e = Instantiate(electronPrefab, pivot.transform);
                e.transform.localPosition = new Vector3(orbitRadius, 0, 0);
                
                pivots.Add(pivot.transform);
            }
        }
    }

    void Update()
    {
        if (nucleus) nucleus.Rotate(Vector3.up * 30f * Time.deltaTime);
        
        foreach(var p in pivots)
        {
            if(p) p.Rotate(Vector3.up * electronSpeed * Time.deltaTime);
        }
    }
}