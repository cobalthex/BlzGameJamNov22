using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_gameOverPanel;

    public TMP_Text TimerText;

    private void Start()
    {
        NetworkedGameManager.Instance.UIManager = this;
    }
}
