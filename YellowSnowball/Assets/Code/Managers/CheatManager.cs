using UnityEngine;

public class CheatManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_cheatCanvas;

    public void ToggleCheatsCanvas()
    {
        m_cheatCanvas.SetActive(!m_cheatCanvas.activeSelf);
    }

    public void ResetMoney()
    {
        NetworkedGameManager.Instance.Player1Data.Money = NetworkedGameManager.Instance.GameData.PlayerStartMoney;
    }

    void Start()
    {
        m_cheatCanvas.SetActive(false);
#if !UNITY_EDITOR
        // Delete if not UNITY EDITOR
        DestroyImmediate(gameObject);
#endif //UNITY_EDITOR
    }
}
