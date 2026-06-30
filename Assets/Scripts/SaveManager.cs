using System;
using System.IO;
using UnityEngine;

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
}
