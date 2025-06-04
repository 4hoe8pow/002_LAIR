using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleHandler : MonoBehaviour
{
    [SerializeField] private Button easyButton;
    [SerializeField] private Button normalButton;
    [SerializeField] private Button hardButton;

    private void Awake()
    {
        easyButton.onClick.AddListener(() => SelectDifficulty(0));
        normalButton.onClick.AddListener(() => SelectDifficulty(1));
        hardButton.onClick.AddListener(() => SelectDifficulty(2));
    }

    private void SelectDifficulty(int difficulty)
    {
        PlayerPrefs.SetInt("puzzleDifficulty", difficulty);
        PlayerPrefs.Save();
        SceneManager.LoadScene("InGame");
    }
}
