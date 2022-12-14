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
        foreach(var playerData in NetworkedGameManager.Instance.PlayerData)
        {
            playerData.Money = NetworkedGameManager.Instance.GameData.PlayerStartMoney;
            // Need to update UI as well
        }
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
