using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueChoice : MonoBehaviour
{
    int step = 0;
    public Button DeadBtn, NoDeadBtn;
    public TMP_Text HumanDialogue, DevilDialogue;

    public GameObject fist, hand;

    public GameObject Camera,ButtonPanel,FadePanel;

    // Start is called before the first frame update
    void Start()
    {
        DeadBtn.onClick.AddListener(OnDeadButtonClick);
        NoDeadBtn.onClick.AddListener(OnNoDeadButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnDeadButtonClick()
    {
        if(step == 0)
        {
            DevilDialogue.text = "The devil loves to share. . it's up to you. .";
            HumanDialogue.text = "Few humans trust demons. You know, right?";
            DeadBtn.GetComponentInChildren<TMP_Text>().text = "I can still get it if I kill you.";
            NoDeadBtn.GetComponentInChildren<TMP_Text>().text = "Maybe I'll give you a chance. . .";
            step++;
            //print(step);
        }
        else if (step == 1)
        {
            //print(step);
            DevilDialogue.text = "You are a lot like me. . .";
            HumanDialogue.text = "That's exactly what I am.";
            DeadBtn.GetComponentInChildren<TMP_Text>().text = "Time to die.";
            NoDeadBtn.GetComponentInChildren<TMP_Text>().text = ". . .";
            NoDeadBtn.interactable = false;
            step++;
        }
        else if (step == 2)
        {
            ButtonPanel.SetActive(false);
            //print(step);
            Camera.transform.parent = fist.transform;
            fist.GetComponent<FistMove>().fistmove = true;
            step++;
        }
        else if(step == 3)
        {
            FadePanel.SetActive(true);
        }
    }
    void OnNoDeadButtonClick()
    {
        if (step == 0)
        {
            DevilDialogue.text = "Tell the devil. . Your deepest desires. . .";
            HumanDialogue.text = "Deepest desire? It's hard to say what it is.";
            DeadBtn.GetComponentInChildren<TMP_Text>().text = "I desire your blood.";
            NoDeadBtn.GetComponentInChildren<TMP_Text>().text = "I desire more. . .";
            step++;
        }
        else if (step == 1)
        {
            DevilDialogue.text = "OK. . Come closer. . human.";
            HumanDialogue.text = "well. .";
            DeadBtn.GetComponentInChildren<TMP_Text>().text = "I'd rather kill you.";
            NoDeadBtn.GetComponentInChildren<TMP_Text>().text = "You know me. . .";
            step++;
        }
        else if (step == 2)
        {
            ButtonPanel.SetActive(false);
            Camera.transform.parent= hand.transform;
            hand.GetComponent<FistMove>().fistmove = true;
            step++;
        }
        else if (step == 3)
        {
            FadePanel.SetActive(true);
        }
    }
}
