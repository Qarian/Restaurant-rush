using UnityEngine;
using System;

public class Interactive : MonoBehaviour
{
	// Action to run during interaction
	Action action;

	public bool active = true;

	public void SetAction(Action action)
	{
		this.action = action;
	}

    public void Interaction()
	{
		action();
	}
}
