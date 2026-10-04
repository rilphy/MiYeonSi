using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance; //singleton instance
    [Serializable]
    public class SaveData
    {
        public string playerName;
        public int dialogueIndex;
        public int lineIndex;
    }


    public SaveData saveData = new SaveData();
    string path;
    string filename = "save";

    [SerializeField]
    private Button loadButton;
    [SerializeField]
    private Button resetButton;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if(instance !=this)
        {
            Destroy(instance.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);

        path = Application.persistentDataPath + "/";

        if (File.Exists(path+filename))
        {
            string json = File.ReadAllText(path + filename);
            saveData = JsonUtility.FromJson<SaveData>(json);
        }


        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //SavePlayerData();
        UpdateButtonStateforSave();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SavePlayerData()
    {
        File.WriteAllText(path+filename, JsonUtility.ToJson(saveData)); //create a file from class data
        print(path);
    }

    public void DeletePlayerData()
    {
        File.Delete(path+filename);
    }

    public void UpdateButtonStateforSave()
    {
        //check save file exist
        if(File.Exists(path+filename))
        {
            loadButton.interactable = true;
            resetButton.interactable = true;

        }
        else
        {
            loadButton.interactable = false;
            resetButton.interactable = false;
        }
    }
}
