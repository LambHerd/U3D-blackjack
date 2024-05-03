using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeInPanel : MonoBehaviour
{
    public float fadeDuration = 2f; // �������ʱ�䣨�룩
    private float currentFadeTime = 0f; // ��ǰ����ʱ��
    private CanvasGroup canvasGroup;

    public string TransToScene = "001";

    void Start()
    {
        // ��ȡCanvasGroup��������Panel��û�У������һ��
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // ��ʼ����͸����Ϊ0����ȫ͸����
        canvasGroup.alpha = 0f;
    }

    void Update()
    {
        // ���µ���ʱ��
        currentFadeTime += Time.deltaTime;

        // ���㵱ǰ͸����
        float alpha = Mathf.Clamp01(currentFadeTime / fadeDuration);

        // ����͸����
        canvasGroup.alpha = alpha;

        // ���͸�����Ѿ��ﵽ���ֵ����ȫ��͸������ֹͣ����
        if (alpha >= 1f)
        {
            enabled = false;
            SceneManager.LoadScene(TransToScene);
        }
    }
}
