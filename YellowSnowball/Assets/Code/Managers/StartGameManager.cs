using UnityEngine;
using TMPro;

public class StartGameManager : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField m_nameTextMesh;

    public void Show()
    {
        gameObject.SetActive(true);

        string nameText = PlayerPrefs.GetString(PlayerPrefsKeys.PlayerName.ToString());
        m_nameTextMesh.text = nameText;

    }

    public void NameChanged()
    {
        // Store name in user prefs
        string nameText = m_nameTextMesh.text;
        PlayerPrefs.SetString(PlayerPrefsKeys.PlayerName.ToString(), nameText);

        // Start Game Here
        NetworkedGameManager.Instance.GoToGame();
    }
    
    private void Start()
    {
        MainMenuManager.Instance.StartGameManager = this;
        gameObject.SetActive(false);
    }
}
