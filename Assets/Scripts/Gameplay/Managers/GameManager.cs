using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
	[NonSerialized] public readonly UnityEvent onTimeEnd = new UnityEvent();
	[NonSerialized] public readonly UnityEvent onWorkEnd = new UnityEvent();
	[NonSerialized] public readonly UnityEvent onCleaningEnd = new UnityEvent();
	[NonSerialized] public readonly UnityEvent onInputToggle = new UnityEvent();

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

    public void EndWork()
	{
		Debug.Log("Work ended");
		onWorkEnd.Invoke();
	}

    public void EndCleaning()
    {
	    Debug.Log("Stopped cleaning");
	    onCleaningEnd.Invoke();
	    SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
    }

    public void GoToMainMenu()
    {
	    SceneManager.LoadScene(0);
    }
    
    public void RunCoroutine(IEnumerator iEnumerator)
	{
		StartCoroutine(iEnumerator);
	}
}
