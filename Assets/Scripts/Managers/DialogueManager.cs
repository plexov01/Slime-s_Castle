using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.IO;


public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance = null;

    [SerializeField] private GameObject pnl_communication;
    [SerializeField] private GameObject pnl_dialogue;
    [SerializeField] private GameObject pnl_image;

    [SerializeField] private Image img_picture;
    [SerializeField] private Text txt_nameSpeaker;
    [SerializeField] private Text txt_phrase;

    [SerializeField][Range(0, 1f)] private float timeFreezeContinue;
    [SerializeField][Range(0, 10f)] private float timeSlideShow;

    [SerializeField]private Dialogue currentDialogue = new Dialogue();
    private int phraseCounter = 0;
    private bool canButtonContinuePressed=true;

    InputManager input;

    IEnumerator freezeContinue(float time)
    {
        canButtonContinuePressed = false;
        yield return new WaitForSeconds(time);
        canButtonContinuePressed = true;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

    }

    void Start()
    {
        input = InputManager.instance;

        input.playerInput.actions["ContinueDialogue"].performed += ContinuePresseed;

        scr_EventSystem.instance.playerEnteredDialogTrigger.AddListener(StartTriggerDialog);

    }


    private void StartTriggerDialog(string dialogName)
    {
        StartDialogue(dialogName);
    }


    public void ContinueDialogue()
    {
        
        if(currentDialogue.phraseList.Count-1>phraseCounter)
        {
            phraseCounter++;
            txt_phrase.text = currentDialogue.phraseList[phraseCounter].phrase;
            txt_nameSpeaker.text = currentDialogue.phraseList[phraseCounter].nameSpeaker;
            
            if (currentDialogue.phraseList[phraseCounter].imagePath!="")
            {
                pnl_image.SetActive(true);
                Sprite sprite = Resources.Load<Sprite>(currentDialogue.phraseList[phraseCounter].imagePath);
                if (sprite) img_picture.sprite = sprite;
                else
                {
                    pnl_image.SetActive(false);
                    Debug.Log("Такой картинки нет");
                }

            }
            else
            {
                pnl_image.SetActive(false);
            }
            
        }
        else
        {
            FinishDialogue();
        }
        
    }

    public void ContinuePresseed(InputAction.CallbackContext context)
    {
        if(!canButtonContinuePressed)
        {
            return;
        }
        else
        {
            StartCoroutine(freezeContinue(timeFreezeContinue));
            ContinueDialogue();
        }
            

    }

    public void StartDialogue(string nameDialogue)
    {
        currentDialogue = GetDialogue(nameDialogue);
        phraseCounter=0;

        pnl_dialogue.gameObject.SetActive(true);

        InputManager.instance.playerInput.actions.FindActionMap("Slime").Disable();

        txt_phrase.text = currentDialogue.phraseList[phraseCounter].phrase;
        txt_nameSpeaker.text = currentDialogue.phraseList[phraseCounter].nameSpeaker;

        if (currentDialogue.phraseList[phraseCounter].imagePath!="")
        {
            pnl_image.SetActive(true);
            Sprite sprite = Resources.Load<Sprite>(currentDialogue.phraseList[phraseCounter].imagePath);
            if (sprite) img_picture.sprite = sprite;
            else
            {
                pnl_image.SetActive(false);
                Debug.Log("Такой картинки нет");
            }

        }
        else
        {
            pnl_image.SetActive(false);
        }
        
    }

    public void FinishDialogue()
    {
        currentDialogue = new Dialogue();
        pnl_dialogue.gameObject.SetActive(false);
        pnl_image.SetActive(false);
        InputManager.instance.playerInput.actions.FindActionMap("Slime").Enable();

        txt_phrase.text = "";
        txt_nameSpeaker.text = "";
        img_picture.sprite = null;

    }

    public bool ExistsDialogue(string dialogue)
    {
        string path = Application.streamingAssetsPath + "/Dialogues/"+dialogue+".json";
        return File.Exists(path);
    }

    public Dialogues GetDialogues()
    {
        if(ExistsDialogue("allDialogues"))
        {
            string path = Application.streamingAssetsPath + "/Dialogues/allDialogues.json";
            Dialogues dialogues = JsonUtility.FromJson<Dialogues>(File.ReadAllText(path));
            return dialogues;
        }
        else
        {
            return new Dialogues();
        }
 
    }

    public Dialogue GetDialogue(string nameDialogue)
    {
        if(ExistsDialogue(nameDialogue))
        {
            string path = Application.streamingAssetsPath + "/Dialogues/"+nameDialogue+".json";
            Dialogue dialogue = JsonUtility.FromJson<Dialogue>(File.ReadAllText(path));
            return dialogue;
        }
        else
        {
            Debug.Log("Файла диалога с таким именем не существует");
            return new Dialogue();
        }
 
    }

    private void OnDestroy() 
    {
        InputManager.instance.playerInput.actions["ContinueDialogue"].performed -= ContinuePresseed;
        
        scr_EventSystem.instance.playerEnteredDialogTrigger.RemoveListener(StartTriggerDialog);
    }

    public void SetupDialogueFile(string nameDialogue)
    {
        if(ExistsDialogue(nameDialogue))
        {
            Debug.Log(nameDialogue+" Файл диалога с таким именем уже существует");            
        }
        else
        {
            Dialogue dialogue = new Dialogue();
            dialogue.nameDialogue = nameDialogue;
            string data = JsonUtility.ToJson(dialogue);
            string path = Application.streamingAssetsPath + "/Dialogues/"+nameDialogue+".json";
            System.IO.File.WriteAllText(path, data);
            Debug.Log(nameDialogue+" Файл диалога создан");            
        }

    }

    public void Test(string test)
    {
        // Sprite sprite = Resources.Load<Sprite>("Images/testImage4");
        // if (sprite) pnl_picture.sprite = sprite;
        // else pnl_picture.gameObject.SetActive(false);
        
        // SetupDialogueFile(test);

    }

}

[System.Serializable]
public class Phrase
{
    public string imagePath;
    public string nameSpeaker;
    [TextArea(3, 10)]
    public string phrase;

    public Phrase()
    {
        nameSpeaker="";
        phrase="";
    }

}

[System.Serializable]
public class Dialogue
{

    public string language;
    public string nameDialogue;
    public List<Phrase> phraseList;


    public Dialogue()
    {
        phraseList = new List<Phrase>(){new Phrase()};
    }

}

[System.Serializable]
public class Dialogues
{

    public List<Dialogue> allDialogues;

    public Dialogues()
    {
        allDialogues = new List<Dialogue>();
    }

}
