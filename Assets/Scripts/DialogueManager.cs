using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class Dialogue   //class for json
{
    public string name; //same as json key name
    public string[] lines;
    public Choice[] choices;
    public Branch branches;
    public string illustID;
    public string bgID;
    public int affectionPoint;
    public string dialogueID;
    public string targetID;
    string playerName;  //input save data

}
[Serializable]
public class DialogueArray
{
    public Dialogue[] dialogues;
}
[Serializable]
public class Choice
{
    public int indexOffset;
    public string option;
    public string dialogueID;
    public string targetID;
}
[Serializable]
public class Branch
{
    public int maxAffection;
    public int minAffection;
    public int indexOffset;
}
public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI dialogue;
    [SerializeField]
    TextMeshProUGUI characterName;
    [SerializeField]
    Image characterImage;
    [SerializeField]
    Image bgImage;
    public GameObject choicesGrid;

    //prfeb
    public Button choiceButtonPrefab;
    public Image dividerPrefab;

    //object
    public GameObject choicedialogue;
    public GameObject dialoguePanel;

    private Dictionary<string, Dialogue> dialogueDictionary;

    private Coroutine animatingCoroutine = null;
    private bool isAnimating = false;

    Dialogue _dialogueData = null;
    DialogueArray _dialogueArray = null;
    Choice choiceData = null;

    int _dialogueIndex;
    int _lineIndex;
    int _affection = 0;
    bool isChoiceSelected = false;
    private float textDelay = 0.05f;

    //public LoadScene loadScene;



    private void Awake()
    {
        Debug.Log("isButtoned:" + LoadScene.isLoadButtoned);
        //load save data if exist
        if(LoadScene.isLoadButtoned==true)
        {
            Debug.Log("Awake index:" + SaveManager.instance.saveData.dialogueIndex);
            if (SaveManager.instance.saveData.dialogueIndex != 0 || SaveManager.instance.saveData.lineIndex != 0)
            {
                _dialogueIndex = SaveManager.instance.saveData.dialogueIndex;
                _lineIndex = SaveManager.instance.saveData.lineIndex;
            }
        }
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadDialogue();
        HideChoice();

        
        PrintDialogue();

        
    }

    // Update is called once per frame
    void Update()
    {
        if (choicesGrid.activeSelf)
            return;
        if(Input.GetKeyDown(KeyCode.Space))
        {
            PrintDialogue();
        }
    }

    public void PrintDialogueForPanel()
    {
        PrintDialogue();
    }

    void LoadDialogue()
    {
        string filePath = "Assets/Dialogue/dialogue.json";  // bring in the json file path
        string json = File.ReadAllText(filePath);   // read all the text from the json file to string
        string replacement= json.Replace("player", SaveManager.instance.saveData.playerName);


        _dialogueArray = JsonUtility.FromJson<DialogueArray>(replacement);


        dialogueDictionary = new Dictionary<string, Dialogue>();

        foreach (var dialogue in _dialogueArray.dialogues)
        {
            if (!string.IsNullOrEmpty(dialogue.dialogueID))
            {
                dialogueDictionary[dialogue.dialogueID] = dialogue;
            }
            else
            {
                //Debug.LogWarning("Dialogue ID is missing for a dialogue entry.");
            }
        }
        
    }

    void SaveDialogueIndex()
    {
        SaveManager.instance.saveData.dialogueIndex = _dialogueIndex;
        SaveManager.instance.saveData.lineIndex = _lineIndex;
        SaveManager.instance.SavePlayerData();
    }

    void ChangePlayerToUserName()
    {
        if(SaveManager.instance==null)//if player name dont exist
        {
            characterName.text = "player"; //for test
        }
        else //exist
        {
            characterName.text = SaveManager.instance.saveData.playerName;
            
        }
        
        
    }

    void GameOver()
    {
        if(_dialogueData.dialogueID=="gameover")
        {
            SceneManager.LoadScene("TitleScene");
        }
    }

    void PrintDialogue()
    {
        if(isAnimating)
        {
            isAnimating = false;
            return;
        }
        if (animatingCoroutine != null) //if coroutine is running
            StopCoroutine(animatingCoroutine);



        if (dialogue.enabled == false)
            dialogue.enabled = true;

        

        _dialogueData = _dialogueArray.dialogues[_dialogueIndex];  //ide to the current dialogue in the array
        

        if(_dialogueData.name=="player")
        {
            ChangePlayerToUserName();
        }
        else
            characterName.text = _dialogueData.name;
        

        
        ShowBackgroundIllust();
        ShowCharacterIllust();
        //Debug.Log(dialogueData.choices);





        //when have a choices
        if (_dialogueData.choices != null && _dialogueData.choices.Length > 0)
        {

            SaveDialogueIndex();
            DestroyPrevChoice();

            dialogue.text = "";

            ShowChoices(_dialogueData.choices);


            TurnONOffRaycast();


            if (_dialogueData.lines == null)    //if no dialogue lines in choice part
            {
                dialogue.enabled = false;
                _dialogueIndex++;
                return;
            }

            //if there are dialogue lines even in choice part, display the line
            DisplayChoiceDialogue();
        }
        else //have no choices
        {
            HideChoice();
            choicedialogue.SetActive(false);
            

            //display dialogue line on GUI
            dialogue.text = "";
            animatingCoroutine = StartCoroutine(AnimatingDialogue(textDelay));

        }

        AddAffectionPoint();
        JumpDialogueByAffection();

        if (_lineIndex < _dialogueData.lines.Length - 1)
        {
            _lineIndex++;
        }
        else if (_lineIndex >= _dialogueData.lines.Length - 1)    //when line is over
        {
           
            _lineIndex = 0;
            if (_dialogueIndex <= _dialogueArray.dialogues.Length - 1)
            {
                _dialogueIndex++;
            }

             if (_dialogueData.targetID != null)
            {
                JumpDialogueByID(_dialogueData.targetID);
                //return;
            }
        }
    }

    void ChangetoPlayerNameinDialogue()
    {
        string splitedLine = _dialogueData.lines[_lineIndex].Replace("player", SaveManager.instance.saveData.playerName);

    }

    void JumpDialogueByAffection()
    {
        if(_dialogueData.branches.indexOffset!=0)
        {
            Debug.Log("branches : "+_dialogueData.branches.indexOffset);
            if(_affection<_dialogueData.branches.maxAffection
                || _affection>_dialogueData.branches.minAffection)
            {
                JumpDialogueByOffset();
            }
        }
    }

    void JumpDialogueByOffset()
    {
        _dialogueIndex += _dialogueData.branches.indexOffset;
        _lineIndex = 0;
    }

    void AddAffectionPoint()
    {
        if(_dialogueData.affectionPoint !=0 && _lineIndex==0)
        {
            _affection += _dialogueData.affectionPoint;
        }

    }

    IEnumerator AnimatingDialogue(float d)
    {
        isAnimating = true;
        string line = _dialogueData.lines[_lineIndex];
        dialogue.text = "";

        for(int count=0; count<line.Length; count++)
        {
            dialogue.text += line[count];
            yield return new WaitForSeconds(d);

            if(!isAnimating)
            {
                dialogue.text = line;
                break;
            }
        }
        isAnimating = false;
        animatingCoroutine = null;
    }


    void DisplayChoiceDialogue()
    {
        string text;
        text = _dialogueData.lines[_lineIndex];
        choicedialogue.SetActive(true);

        TextMeshProUGUI textGUI;
        textGUI = choicedialogue.GetComponent<TextMeshProUGUI>();
        textGUI.text = text;
        text = null;
    }

    void DestroyPrevChoice()
    {
        foreach (Transform child in choicesGrid.transform)
            Destroy(child.gameObject);
    }

    void JumpDialogueByID(string targetID)
    {
        Debug.Log($"JumpNextDialogueID called with: {targetID}");
        if (dialogueDictionary.TryGetValue(targetID, out Dialogue targetDialogue))   //get a dialogue with the targetID
        {
            //if find a line
            Debug.Log($"Found dialogue for ID : {targetDialogue.lines[0]}");


            _dialogueIndex = Array.IndexOf(_dialogueArray.dialogues, targetDialogue);
            _lineIndex = 0;

        }
        else
        {
            //if not found line
            Debug.LogWarning($"Dialogue with ID '{targetID}' not found.");
        }
        
    }


    void JumpNextDialogueIDforChoice(string targetID, Choice choice)
    {
        Debug.Log($"JumpNextDialogueID called with: {targetID}");


        if (choice.indexOffset != 0)
        {
            _dialogueIndex += choice.indexOffset-1;
            PrintDialogue();
        }
        else if(dialogueDictionary.TryGetValue(targetID, out Dialogue targetDialogue))   //get a dialogue with the targetID
        {
            //if find a line
            Debug.Log($"Found dialogue for ID : {targetDialogue.lines[0]}");


            _dialogueIndex = Array.IndexOf(_dialogueArray.dialogues,targetDialogue);
            _lineIndex = 0;
            PrintDialogue();
            
        }
        else
        {
            //if not found line
            Debug.LogWarning($"Dialogue with ID '{targetID}' not found.");
        }
        TurnONOffRaycast();
    }
    

    void ShowChoices(Choice[] choices)
    {
        choicesGrid.SetActive(true);

        int i = 0;

        foreach (Choice choice in choices)
        {
            Button choiceButton = Instantiate(choiceButtonPrefab, choicesGrid.transform);
            choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = choice.option;

            string targetID = choice.targetID;
            choiceButton.onClick.AddListener(() => JumpNextDialogueIDforChoice(targetID, choice));

            if(choices.Length-1>i)
            {
                Instantiate(dividerPrefab, choiceButton.transform);
            }
            i++;

        }
    }


    void OnChoiceSelected(int nextDialogueIndex)
    {
        isChoiceSelected = true;
        _dialogueIndex = nextDialogueIndex;
        _lineIndex = 0;
        PrintDialogue();
    }

    void HideChoice()
    {
        choicesGrid.SetActive(false);
    }

    void TurnONOffRaycast()    //when choices is on, turn off the raycast for double enter issue
    {
        Image panelImage = GetComponentInChildren<Image>();
        
        if(panelImage.raycastTarget==false)
        {
            panelImage.raycastTarget = true;
        }
        else
            panelImage.raycastTarget = false;
    }


    void ShowCharacterIllust()
    {
        if(!string.IsNullOrEmpty(_dialogueData.illustID))
        {
            string illustPath = "Assets/Resources/Illust/Character/" + _dialogueData.illustID+".png";
            
            if (File.Exists(illustPath))
            {
                characterImage.enabled = true;
                characterImage.sprite = Resources.Load<Sprite>("Illust/Character/" + _dialogueData.illustID);
            }
            else
                Debug.LogWarning("Illust file not found:" + illustPath);
                
        }
        else
        {
            characterImage.enabled = false;
        }
        
    }

    void ShowBackgroundIllust()
    {
        if(!string.IsNullOrEmpty(_dialogueData.bgID))
        {
            string bgPath = "Assets/Resources/Illust/Background/" + _dialogueData.bgID + ".png";

            if (File.Exists(bgPath))
            {
                bgImage.sprite = Resources.Load<Sprite>("Illust/Background/" + _dialogueData.bgID);
            }
            else
                Debug.LogWarning("Illust file not found:" + bgPath);
        }
    }
}
