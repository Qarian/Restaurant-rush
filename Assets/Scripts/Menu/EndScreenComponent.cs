using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class EndScreenComponent : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI needed = default;
    [SerializeField] private TextMeshProUGUI earned = default;
    [SerializeField] private TextMeshProUGUI profit = default;
    [SerializeField] private TextMeshProUGUI bank = default;

    [Header("Values")]
    [SerializeField] private float appearanceTime = 1.5f;

    private void Start()
    {
        transform.DOMove(Vector3.zero, appearanceTime).SetEase(Ease.OutSine);

        float neededScore = GameManager.singleton.PointsManager.requiredScore;
        float score = GameManager.singleton.PointsManager.score;

        needed.text = neededScore.ToString();
        earned.text = score.ToString();

        // TODO Check success or failure
        profit.text = (score - neededScore).ToString();
        // TODO Saving Bank
        bank.text = profit.text;
        Destroy(GameManager.singleton.gameObject);
    }

    public void NextStage()
    {
        Debug.Log("Next Stage");
        SceneManager.LoadSceneAsync(0);
    }
}
