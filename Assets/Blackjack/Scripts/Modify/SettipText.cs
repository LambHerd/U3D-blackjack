using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettipText : MonoBehaviour
{
    public TMP_Text wordText;
    // Start is called before the first frame update
    public void Settip(string wordTextTxt)
    {
        // ����Word Panel������
        wordText.text = wordTextTxt;
    }

}
