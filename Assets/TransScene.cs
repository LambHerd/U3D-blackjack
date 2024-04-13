using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransScene : MonoBehaviour
{
    
    public void TS() 
    {
        SceneManager.LoadScene("002");  //引号中是你要切换的场景名字
    }
    public void TS1() 
    {
        SceneManager.LoadScene("003");  //引号中是你要切换的场景名字
    }
}
