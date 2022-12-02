using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driveway : MonoBehaviour
{
    private int m_dimensionX;
    public int DimensionX => m_dimensionX;
    
    private int m_dimensionZ;
    public int DimensionZ => m_dimensionZ;

    [SerializeField]
    private GameObject m_cellObject;

    public Vector3 GetPositionOfCell(Vector2Int cellPosition)
    {
        return transform.position + new Vector3(cellPosition.x, 0f, cellPosition.y);
    }


    // Start is called before the first frame update
    void Start()
    {
        // Hide proxy driveway
        gameObject.HideRenderers();

        // Get dimension from scale
        var localScale = transform.localScale;
        m_dimensionX = (int)localScale.x;
        m_dimensionZ = (int)localScale.z;

        // Draw proxy grid
        var drivewayPos = transform.position;
        for (int x = 0; x < DimensionX; x++)
        {
            for (int z = 0; z < DimensionZ; z++)
            {
                var cell = Instantiate(m_cellObject, new Vector3(x, 0f, z) + drivewayPos, Quaternion.identity);
                cell.transform.SetParent(transform);
            }
        }
    }
}
