using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IOS || UNITY_ANDROID
using Firebase;
using Firebase.Crashlytics;
#endif

public class CrashlyticsInit : MonoBehaviour
{
#if UNITY_IOS || UNITY_ANDROID
	FirebaseApp app;
	DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
	protected bool firebaseInitialized = false;

	// Start is called before the first frame update
	void Start()
    {
		// Initialize Firebase
		FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
			dependencyStatus = task.Result;
			if (dependencyStatus == DependencyStatus.Available)
			{
				InitializeFirebase();
			}
			else
			{
				Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
			}
		});
	}

	void InitializeFirebase()
	{
		FirebaseApp.LogLevel = LogLevel.Debug;
		var app = FirebaseApp.DefaultInstance;
		firebaseInitialized = true;
	}
#endif
}
