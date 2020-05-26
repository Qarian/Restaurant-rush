using UnityEngine;
using TMPro;

public class PointsManager : MonoBehaviour
{
    [SerializeField] ScoreUI scoreUI = default;
    
    [Space]
    [SerializeField] private int minMoneyFromCustomer = 10;
    [SerializeField] private int extraMoneyForTime = 20;
    
    public int requiredScore = 0;
    public static PointsManager singleton;

    [Space]
    [HideInInspector] public int score = 0;

    private void Start()
    {
        singleton = this;
        score = 0;
        scoreUI.Enable(score, requiredScore);
        GameManager.singleton.onWorkEnd.AddListener(scoreUI.Disable);
    }
    
    public void ChangeScore(int change)
    {
        score += change;
        scoreUI.UpdateScore(score);
    }

    public void AddPoints(int customersCount, float time)
    {
        score += Mathf.FloorToInt(customersCount * (minMoneyFromCustomer + time * extraMoneyForTime));
        scoreUI.UpdateScore(score);
    }
}
