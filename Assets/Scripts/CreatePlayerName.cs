using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreatePlayerName : MonoBehaviour
{
    public GameObject checkPanel;
    public TMP_InputField playerNameInputField;
    public Button button;
    public TMP_Text checkPlayerName;
    string playerName = "";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button.interactable = false;
        checkPanel.SetActive(false);
    }


    public void EnableButton()
    {
        if (playerNameInputField.text != null)
            button.interactable = true;
        else
            button.interactable = false;
    }

    public void FinalNameButton()
    {
        SavePlayerName();
        SceneManager.LoadScene("MainScene");
    }

    void SavePlayerName()
    {
        playerName = playerNameInputField.text;
        Debug.Log(playerName);
        SaveManager.instance.saveData.playerName = playerName;
    }

    public void ShowCheckPanel()
    {
        checkPlayerName.text = playerNameInputField.text + "\n이 이름으로 하시겠습니까?";
        checkPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
