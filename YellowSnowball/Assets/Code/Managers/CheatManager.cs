using UnityEngine;

public class CheatManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_cheatCanvas;

    void Start()
    {
#if !UNITY_EDITOR
        // Delete if not UNITY EDITOR
        DestroyImmediate(gameObject);
#endif //UNITY_EDITOR
    }

    public void ToggleCheatsCanvas()
    {
        m_cheatCanvas.SetActive(!m_cheatCanvas.activeSelf);
    }
}
