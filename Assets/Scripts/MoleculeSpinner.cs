using UnityEngine;

public class MoleculeSpinner : MonoBehaviour
{
    [Header("Spin Settings")]
    [Tooltip("Speed of rotation in degrees per second")]
    public float rotationSpeed = 45f;

    [Tooltip("The Axis to spin around. (1,0,0) is vertical flip. (0,1,0) is horizontal.")]
    public Vector3 spinAxis = new Vector3(1f, 0.5f, 0.2f);

    [Header("Bobbing (Floating Effect)")]
    public bool enableBobbing = true;
    public float bobSpeed = 1f;
    public float bobHeight = 0.02f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        transform.Rotate(spinAxis.normalized * rotationSpeed * Time.deltaTime, Space.Self);

        if (enableBobbing)
        {
            float newY = startPos.y + (Mathf.Sin(Time.time * bobSpeed) * bobHeight);
            transform.localPosition = new Vector3(startPos.x, newY, startPos.z);
        }
    }
}