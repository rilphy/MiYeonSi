using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor.ShaderGraph.Serialization;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Schema;

public enum JsonEnum
{
    Array,
    None,
    Null,
    Object,
    String,
    Integer,
    Float,
    Boolean,
    
}

[Serializable]
public class Dialogue
{
    public string name;
    public string[] lines;
    public Choice[] choices;
    public int nextDialogue;
    public string illustID;
    string playerName;
}
[Serializable]
public class DialogueArray
{
    public Dialogue[] dialogues;
}
[Serializable]
public class Choice
{
    public string option;
    public int nextDialogue;
}

public class DialogueScripter : EditorWindow
{

    [MenuItem("Tools/My Custom Editor")]
    public static void ShowMyEditor()
    {
        EditorWindow wnd = GetWindow<DialogueScripter>();
        wnd.titleContent = new GUIContent("My Custom Editor");
    }
    public void CreateGUI()
    {
        var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);

        rootVisualElement.Add(splitView);

        var leftPane = new ListView();
        splitView.Add(leftPane);

        

        var allObjectGuids = AssetDatabase.FindAssets("t:TextAsset", new[] {"Assets/Dialogue"} );
        var allObjects = new List<TextAsset>();
        foreach(var guid in allObjectGuids)
        {
            allObjects.Add(AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(guid)));
        }

        leftPane.makeItem = () => new Label();
        leftPane.bindItem = (item, index) => 
        { var textAsset = allObjects[index];
            (item as Label).text = textAsset != null ? textAsset.name : "Unknown";
            };
        leftPane.itemsSource = allObjects;


        //Right pane
        var rightPane = new VisualElement();
        splitView.Add(rightPane);

        var csharpField = new EnumField("C# Field", JsonEnum.Array);
        //csharpField.style.visibility = Visibility.Hidden;
        //csharpField.AddToClassList("some-styled-field");
        //csharpField.value = TextAlignment.Left;
        rightPane.Add(csharpField);


        var button =new Button(() => 
        {

            csharpField.schedule.Execute(() =>
            {
                foreach (var child in csharpField.Children())
                {
                    Debug.Log($"Child name: {child.name}");
                }

                var inputElement = csharpField.Q<VisualElement>();
                if (inputElement != null)
                {
                    Debug.Log("Input element found!");
                    inputElement.Focus();
                }
                else
                {
                    Debug.LogError("Input element not found.");
                }
            });


        }){text="+Add"};

        

        button.style.width = 100;
        button.style.height = 50;
        button.style.fontSize = 20;
        rightPane.Add(button);

        

        var box = new Box();
        rightPane.Add(box);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
