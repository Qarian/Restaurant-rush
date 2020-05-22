using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
	[NonSerialized] public readonly UnityEvent onTimeEnd = new UnityEvent();
	[NonSerialized] public readonly UnityEvent onWorkEnd = new UnityEvent();
	[NonSerialized] public readonly UnityEvent onInputToggle = new UnityEvent();

	[SerializeField] private KeyCode toggleInputKey = KeyCode.LeftBracket;

	public PointsManager PointsManager { get; private set; }
	
	public static GameManager singleton;
	
	
    private void Awake()
    {
	    if (singleton)
	    {
		    Destroy(singleton.gameObject);
		    return;
	    }
		    
        singleton = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        PointsManager = GetComponent<PointsManager>();
    }

    private void Start()
    {
	    GetComponent<Timer>().StartDay();
    }

    private void Update()
    {
	    if (Input.GetKeyDown(KeyCode.Escape))
		    SceneManager.LoadScene(0);

	    if (Input.GetKeyDown(toggleInputKey))
		    onInputToggle.Invoke();

	    if (Input.GetKeyDown(KeyCode.Escape))
		    SceneManager.LoadScene(0);
    }

    public void EndDay()
	{
		Debug.Log("Day ended!");
		onWorkEnd.Invoke();
		SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
	}
    
    public void RunCoroutine(IEnumerator iEnumerator)
	{
		StartCoroutine(iEnumerator);
	}
}
