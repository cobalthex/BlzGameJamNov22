using UnityEngine;

public class AugerBlades : MonoBehaviour
{
    public Vector3 Axis;
    public float SpeedPerSecond = 1;
    public float MaxExtent;

    Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.localPosition;   
    }

    void Update()
    {
        transform.localPosition += Axis * (SpeedPerSecond * Time.deltaTime);

        // todo: wrap
        var dist = Vector3.Dot(transform.localPosition - initialPosition, Axis);
        if (dist > MaxExtent)
            transform.localPosition = initialPosition;
    }
}
