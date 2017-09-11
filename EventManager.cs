using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;
using System;

public class EventManager : MonoBehaviour
{
	[SerializeField]
	string lastEventTriggered;

	[SerializeField]
	List<string> eventsBeingListened = new List<string> ();

	Dictionary <string, UnityEvent> events = new Dictionary<string, UnityEvent> ();

	static EventManager instance;

	public static EventManager Instance {
		get {
			if (!instance) {
				instance = FindObjectOfType<EventManager> ();

				if (!instance) {
					new GameObject { name = typeof(EventManager).Name }.AddComponent<EventManager> ();
					instance = FindObjectOfType<EventManager> ();
				}
			}

			return instance;
		}
	}

	void Awake ()
	{
		if (Instance != this) {
			Destroy (gameObject);
		}

		DontDestroyOnLoad (gameObject);
	}

	public static void StartListening (string eventName, UnityAction listener)
	{
		UnityEvent unityEvent;

		if (Instance.events.TryGetValue (eventName, out unityEvent)) {
			unityEvent.AddListener (listener);
		} else {
			unityEvent = new UnityEvent ();
			unityEvent.AddListener (listener);
			Instance.events.Add (eventName, unityEvent);
			Instance.eventsBeingListened.Add (eventName);
		}
	}

	public static void StopListening (string eventName, UnityAction listener)
	{
		UnityEvent unityEvent;

		if (Instance.events.TryGetValue (eventName, out unityEvent)) {
			unityEvent.RemoveListener (listener);
			Instance.events.Remove (eventName);
			Instance.eventsBeingListened.Remove (eventName);
		}
	}

	public static void TriggerEvent (string eventName)
	{
		UnityEvent unityEvent;

		if (Instance.events.TryGetValue (eventName, out unityEvent)) {
			unityEvent.Invoke ();
		}

		LogTriggeredEvent (eventName);
	}

	static void LogTriggeredEvent (string eventName)
	{ 
		Instance.lastEventTriggered = string.Concat ("(", DateTime.Now.ToShortTimeString (), ") ", eventName);
	}
}
