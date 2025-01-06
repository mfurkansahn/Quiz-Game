using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu: MonoBehaviour
{
    // Endless Marble sahnesine geç
    public void LoadEndlessMarble()
    {
        SceneManager.LoadScene(1); // Endless Marble sahnesi
    }

    // Breakout sahnesine geç
    public void LoadBreakout()
    {
        SceneManager.LoadScene(2); // Breakout sahnesi
    }

    // Quiz Game sahnesine geç
    public void LoadQuizGame()
    {
        SceneManager.LoadScene(3); // Quiz Game sahnesi
    }

    // Uygulamayý kapat
    public void ExitGame()
    {
        // Unity Editörde ise Play Mode'u durdur
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // Uygulamayý kapat
#endif
    }
}
