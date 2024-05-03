using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeInPanel : MonoBehaviour
{
    public float fadeDuration = 2f; // 淡入持续时间（秒）
    private float currentFadeTime = 0f; // 当前淡入时间
    private CanvasGroup canvasGroup;

    public string TransToScene = "001";

    void Start()
    {
        // 获取CanvasGroup组件，如果Panel上没有，就添加一个
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // 初始设置透明度为0（完全透明）
        canvasGroup.alpha = 0f;
    }

    void Update()
    {
        // 更新淡入时间
        currentFadeTime += Time.deltaTime;

        // 计算当前透明度
        float alpha = Mathf.Clamp01(currentFadeTime / fadeDuration);

        // 设置透明度
        canvasGroup.alpha = alpha;

        // 如果透明度已经达到最大值（完全不透明），停止更新
        if (alpha >= 1f)
        {
            enabled = false;
            SceneManager.LoadScene(TransToScene);
        }
    }
}
