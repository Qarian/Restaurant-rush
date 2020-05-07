using UnityEngine;
using TMPro;

public class PointsManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI moneyUI = default;
    
    [Space]
    [SerializeField] private int minMoneyFromCustomer = 10;
    [SerializeField] private int extraMoneyForTime = 20;
    
    public int neededScore = 0;
    public static PointsManager singleton;

    [Space]
    [HideInInspector] public int score = 0;

    private void Start()
    {
        singleton = this;
        score = 0;
        moneyUI.text = score.ToString();
    }
    
    public void ChangeScore(int change)
    {
        score += change;
        moneyUI.text = score.ToString();
    }

    public void AddPoints(int customersCount, float time)
    {
        score += Mathf.FloorToInt(customersCount * (minMoneyFromCustomer + time * extraMoneyForTime));
        moneyUI.text = score.ToString();
    }
}
