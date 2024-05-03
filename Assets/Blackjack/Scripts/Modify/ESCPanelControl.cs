using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ESCPanelControl : MonoBehaviour
{
    public Button Back, Return, Exit;
    
    // Start is called before the first frame update
    void Start()
    {
        Back.onClick.AddListener(OnBackButtonClick);
        Return.onClick.AddListener(OnReturnButtonClick);
        Exit.onClick.AddListener(OnExitButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnBackButtonClick()
    {
        gameObject.SetActive(false);
    }
    void OnReturnButtonClick()
    {
        SceneManager.LoadScene("001");
    }
    void OnExitButtonClick()
    {
        Application.Quit();
    }
}
