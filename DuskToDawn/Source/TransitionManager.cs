using UnityEngine;
using Prime31.TransitionKit;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
	public static TransitionManager instance;
	//public Texture2D maskTexture;

	private void Awake()
	{
		if (instance == null) instance = this;
		else if (instance != this) Destroy(gameObject);
	}

	public void SwitchScene(int sceneNum)
	{
		PickRandomTransition(sceneNum);
	}

	public void swithNoTransition(int sceneNum)
	{
		SceneManager.LoadScene(sceneNum);
	}

	private void PickRandomTransition(int sceneNum)
	{
		//int seed = Random.Range(1, 6);
		int seed = 1;

		switch (seed)
		{
			case 1:
				//"Fade to Scene"
				var fader = new FadeTransition()
				{
					nextScene = sceneNum,
					fadeToColor = Color.black
				};
				TransitionKit.instance.transitionWithDelegate(fader);
				break;
			case 2:
				//"Vertical Slices to Scene"
				var slices = new VerticalSlicesTransition()
				{
					nextScene = sceneNum,
					divisions = Random.Range(3, 20)
				};
				TransitionKit.instance.transitionWithDelegate(slices);
				break;
			case 3:
				//"Blur to Scene" 
				var blur = new BlurTransition()
				{
					nextScene = sceneNum
					//duration = 2.0f,
					//blurMax = 0.01f
				};
				TransitionKit.instance.transitionWithDelegate(blur);
				break;
			case 4:
				var fishEye = new FishEyeTransition()
				{
					nextScene = sceneNum,
					duration = 1.0f,
					size = 0.08f,
					zoom = 10.0f,
					colorSeparation = 3.0f
				};
				TransitionKit.instance.transitionWithDelegate(fishEye);
				break;
			//case 5:
			//	Debug.Log("mask to Scene ========================================== ");

			//	var mask = new ImageMaskTransition()
			//	{
			//		maskTexture = maskTexture,
			//		backgroundColor = Color.black,
			//		nextScene = sceneNum
			//	};
			//	TransitionKit.instance.transitionWithDelegate(mask);
			//	break;
		}
	}

	void OnEnable()
	{
		TransitionKit.onScreenObscured += onScreenObscured;
		TransitionKit.onTransitionComplete += onTransitionComplete;
	}


	void OnDisable()
	{
		// as good citizens we ALWAYS remove event handlers that we added
		TransitionKit.onScreenObscured -= onScreenObscured;
		TransitionKit.onTransitionComplete -= onTransitionComplete;
	}


	void onScreenObscured()
	{
	}

	void onTransitionComplete()
	{
	}
}
