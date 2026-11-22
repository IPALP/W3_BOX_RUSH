using UnityEngine;
using UnityEngine.UI;
public class TimeBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Color fullColor = Color.green;
    [SerializeField] private Color emptyColor = Color.red;

    public void Update()
    {
        if(GameManager.Instance == null) return;

       SetTime(GameManager.Instance.timeLeft, GameManager.Instance.shiftLength);

    }
    public void SetTime(float timeLeft, float totalTime)
    {
        float t = Mathf.Clamp01(timeLeft/totalTime);
        slider.value = t;
        fillImage.color = Color.Lerp(emptyColor, fullColor, t);
    }
    
}
