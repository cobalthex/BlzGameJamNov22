using UnityEngine;

public class TestMoveController : MonoBehaviour
{
    public float Speed = 10;

    // Update is called once per frame
    void Update()
    {
        var delta = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            delta += new Vector3(0, 0, 1);
        if (Input.GetKey(KeyCode.S))
            delta += new Vector3(0, 0, -1);
        
        if (Input.GetKey(KeyCode.A))
            delta += new Vector3(-1, 0, 0);
        if (Input.GetKey(KeyCode.D))
            delta += new Vector3(1, 0, 0);

        transform.position += delta * (Speed * Time.deltaTime);
    }
}
