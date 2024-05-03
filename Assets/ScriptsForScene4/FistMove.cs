using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FistMove : MonoBehaviour
{
    public bool fistmove = false;
    public int direction=-1;

    public int type = 0;

    public Button DeadBtn, NoDeadBtn;
    public TMP_Text HumanDialogue, DevilDialogue;

    public GameObject ButtonPanel,ItemGet;

    float elapsedTime=0, startTime, stopTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public float moveSpeed = 1f; // 移动速度

    void Update()
    {
        if (fistmove)
        {
            // 获取鼠标水平和垂直移动量
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // 计算物体在XZ平面上的移动方向
            Vector3 movement = new Vector3(0f, direction*mouseY, 0f);

            // 将移动方向乘以移动速度以及 Time.deltaTime，确保平滑移动
            movement = movement.normalized * moveSpeed * Time.deltaTime;

            // 应用移动
            transform.Translate(movement, Space.Self);
        }
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        //print(collisionInfo.gameObject.name);
        transform.GetComponent<Rigidbody>().isKinematic = true;
        if(type == 1)
        {
            fistmove=false;
            HumanDialogue.text = "Why can't I move?";
            DevilDialogue.text = "Enjoy. . Human.";
            Invoke("InvokeDialogueType1", 3f);
            ItemGet.SetActive(true);
        }
        else if(type==0)
        {
            StartTiming();
            InvokeDialogueType2();
        }
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        if(type == 0)
        {
            transform.GetComponent<Rigidbody>().isKinematic = false;
            StopTiming();
            elapsedTime += stopTime - startTime;

            print(elapsedTime);

            if(elapsedTime > 4f)
            {
                ItemGet.SetActive(true);
                HumanDialogue.text = "Wow, look what I got.";
                DevilDialogue.text = "Ah, the pain. . .";
                Invoke("StopFistMove", 3f);
            }
        }
        
    }

    void InvokeDialogueType1()
    {
        HumanDialogue.text = "What are you doing? Stop it now!";
        DevilDialogue.text = "The devil said he would give it to you. . The devil never breaks his promise. . . Haha. .";
        ButtonPanel.SetActive(true);
        DeadBtn.interactable = true;
        NoDeadBtn.interactable = true;
        DeadBtn.GetComponentInChildren<TMP_Text>().text = "I am going to kill you!";
        NoDeadBtn.GetComponentInChildren<TMP_Text>().text = "Let go!";
    }
    void InvokeDialogueType2()
    {
        HumanDialogue.text = "Guess what I caught, Devil. A bird or a heart?";
        DevilDialogue.text = ". . . . . . . . . . .";
    }


    // 开始计时
    void StartTiming()
    {
        startTime = Time.time;
    }

    // 停止计时
    void StopTiming()
    {
        stopTime = Time.time;
    }

    void StopFistMove()
    {
        fistmove = false;
        HumanDialogue.text = "Got you, Devil. it's over.";
        DevilDialogue.text = "We will meet again. . . Human.";
        ButtonPanel.SetActive(true);
        DeadBtn.interactable = true;
        NoDeadBtn.interactable = true;
        DeadBtn.GetComponentInChildren<TMP_Text>().text = "The next game beckons.";
        NoDeadBtn.GetComponentInChildren<TMP_Text>().text = "See you later.";
    }
}
