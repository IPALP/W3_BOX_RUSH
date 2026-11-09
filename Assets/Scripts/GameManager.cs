using UnityEngine;
using UnityEngine.SceneManagement;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
public class GameManager : MonoBehaviour
{
   public static GameManager Instance {get; private set; }

   [Tooltip("Lenght of the shift in seconds")]
   public float shiftLength = 120f;

   public int score;
   public float timeLeft;
   public bool IsOver {get; private set; }

   void Awake()
    {
        Instance = this;
        timeLeft = shiftLength;
    }
    void Update()
    {
        if(!IsOver)
        {
            timeLeft -= Time.deltaTime;
            if(timeLeft <- 0f)
            {
                timeLeft = 0f;
                IsOver = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        else if (RestartPressed())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    public void AddScore(int points)
    {
        if(IsOver) return;
        score += points;
    }

    void OnGUI()
    {
        int size = Mathf.RoundToInt(Screen.height * 0.04f);
        var style = new GUIStyle(GUI.skin.label) {fontSize = size, fontStyle = FontStyle.Bold};
        style.normal.textColor = Color.white;

        int total = Mathf.CeilToInt(timeLeft);
        GUI.Label(new Rect(20, 20, 600, size*2), "Score: " + score, style);
        GUI.Label(new Rect(20, 20 + size * 1.3f, 600, size *2), "Time: " + (total / 60) + ":" + (total % 60).ToString("00"), style);

        if(IsOver)
        {
            var big = new GUIStyle(style) { alignment = TextAnchor.MiddleCenter, fontSize = size *2};
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "SHIFT OVER\nScore: " + score + "\nPress R to restart", big);
        }
    }
#if ENABLE_INPUT_SYSTEM
    static bool RestartPressed() => Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame;
#else
    static bool RestartPressed() => Input.GetKeyDown(KeyCode.R);
#endif
}
