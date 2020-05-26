using TMPro;
using UnityEngine;

public class CleaningUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI filthCountText = default;
    private string text;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void ShowUI(int filthCount)
    {
        if (text == null)
            text = filthCountText.text;
        gameObject.SetActive(false);
        UpdateFilthCount(filthCount);
    }

    public void UpdateFilthCount(int filthCount)
    {
        filthCountText.text = text + filthCount;
    }
}
