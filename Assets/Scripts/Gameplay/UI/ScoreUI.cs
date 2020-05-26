using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreTMP = default;
    [SerializeField] private TextMeshProUGUI requiredScoreTMP = default;

    public void Enable(int initialScore, int requiredScore)
    {
        gameObject.SetActive(true);
        scoreTMP.text = initialScore.ToString();
        requiredScoreTMP.text = requiredScore.ToString();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void UpdateScore(int score)
    {
        scoreTMP.text = score.ToString();
    }
}
