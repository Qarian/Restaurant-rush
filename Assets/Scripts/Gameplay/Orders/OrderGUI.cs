using UnityEngine;
using UnityEngine.UI;

public class OrderGUI : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup layout = default;

    private int size = 4;
    
    private Image[] images;
    private bool[] takenImages = {false};

    private static Transform playerCamera;

    private void Start()
    {
        Transform layoutTransform = layout.transform;
        size = layoutTransform.childCount;
        
        images = new Image[size];
        takenImages = new bool[size];

        for (int i = 0; i < size; i++)
        {
            images[i] = layoutTransform.GetChild(i).GetComponent<Image>();
            images[i].gameObject.SetActive(false);

            takenImages[i] = false;
        }

        if (playerCamera is null)
        {
            //TODO: better reference to player
            playerCamera = Camera.main.transform;
        }
        
        gameObject.SetActive(false);
    }

    private void Update()
    {
        transform.LookAt(playerCamera);
    }


    public void AddIcon(Color color, Sprite sprite, int id)
    {
        if (takenImages[id])
        {
            Debug.LogError("Icon is already taken!");
            return;
        }

        images[id].sprite = sprite;
        images[id].color = color;
        images[id].gameObject.SetActive(true);
        takenImages[id] = true;
    }

    public void RemoveImage(int id)
    {
        images[id].gameObject.SetActive(false);
        takenImages[id] = false;
    }
    
    
    public void ShowIcons()
    {
        gameObject.SetActive(true);
    }

    public void HideIcons()
    {
        for (int i = 0; i < size; i++)
        {
            if (takenImages[i])
                images[i].gameObject.SetActive(false);
            takenImages[i] = false;
        }
        gameObject.SetActive(false);
    }
}
