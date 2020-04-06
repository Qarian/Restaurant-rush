using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	[HideInInspector] public List<Action> onDayEnd = new List<Action>();
	[HideInInspector] public List<Action> onInputToggle = new List<Action>();

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
		    Application.Quit();

	    if (Input.GetKeyDown(toggleInputKey))
	    {
		    Debug.Log(onDayEnd.Count);
		    foreach (var action in onInputToggle)
		    {
			    action();
		    }
	    }

	    if (Input.GetKeyDown(KeyCode.Escape))
	    {
		    SceneManager.LoadScene(0);
	    }
    }

    public void EndDay()
	{
		Debug.Log("Day ended!");
		foreach (var action in onDayEnd)
		{
			action();
		}
		SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
	}


    public void RunCoroutine(IEnumerator iEnumerator)
	{
		StartCoroutine(iEnumerator);
	}
}
