using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu = default;
    [SerializeField] private KeyCode inputKey = KeyCode.Escape;

    public static bool Paused { get; private set; }

    private void Start()
    {
        Paused = false;
        GameManager.singleton.onCleaningEnd.AddListener(() => enabled = false);
        pauseMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(inputKey))
            ChangePause();
    }

    public void ChangePause()
    {
        Paused = !Paused;
        Time.timeScale = Paused ? 0 : 1;
        GameManager.singleton.onInputToggle.Invoke();
        pauseMenu.SetActive(Paused);
    }
}
