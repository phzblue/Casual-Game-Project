using UnityEngine;
using UnityEngine.UI;
using com.igi.game.common.webserver.hcg.leaderboard;

public class BaseSceneController : MonoBehaviour
{
	float referenceWidth = 1080;
	float referenceHeight = 1920;

    private Vector2 currentResolution;

    private float variance;

    [SerializeField]
    private CanvasScaler[] canvasScalers = null;

    public bool isEnteredThirdPartySDK = false;

    private void Awake()
    {
        variance = Camera.main.aspect / (9f / 16f);

        if (variance < 1f)
        {
            Camera.main.rect = new Rect(0, (1f - variance) / 2, 1f, variance);
        }
        else
        {
            variance = 1f / variance;
            Camera.main.rect = new Rect((1f - variance) / 2, 0, variance, 1f);
        }
    }

    private void Start()
	{
        //GameManager.instance.SetScene(this);

        ScaleCameraToScreen();

        //OnStart();

#if UNITY_WEBGL
        currentResolution = new Vector2(Screen.width, Screen.height);
#endif
    }

    private void ScaleCameraToScreen()
    {
        if (canvasScalers != null)
        {
            foreach (CanvasScaler canvasScaler in canvasScalers)
            {
                if (canvasScaler.uiScaleMode == CanvasScaler.ScaleMode.ConstantPixelSize)
                {
                    canvasScaler.scaleFactor = Mathf.Min(Screen.width / referenceWidth, Screen.height / referenceHeight);
                }
                else
                {
                    if (variance < 1f)
                    {
                        canvasScaler.matchWidthOrHeight = 0;
                    }
                    else
                    {
                        canvasScaler.matchWidthOrHeight = 1f;
                    }
                }
            }
        }
    }

	protected void FitCanvasToScreeen()
	{
        if (canvasScalers != null)
        {
            foreach (CanvasScaler canvasScaler in canvasScalers)
            {
                if (canvasScaler.uiScaleMode == CanvasScaler.ScaleMode.ConstantPixelSize)
                {
                    canvasScaler.scaleFactor = Mathf.Min(Screen.width / referenceWidth, Screen.height / referenceHeight);
                }
            }
        }
		/*
		Canvas c = FindObjectOfType<Canvas>();
		CanvasScaler cs = GetComponent<CanvasScaler>();
		float factor = Mathf.Min(1920f / Screen.height, 1080f / Screen.width);
		c.scaleFactor = factor;
		*/
	}

    void Update()
    {
#if UNITY_WEBGL
        if (currentResolution.x != Screen.width || currentResolution.y != Screen.height)
        {
            currentResolution = new Vector2(Screen.width, Screen.height);
            ScaleCameraToScreen();
        }
#endif
    }
	
    public virtual void Login() { }

    public virtual void LoginSuccessful() {}

    public virtual void SetNamePrompt() {}

    public virtual void UpdateHighScoreResponse(long score) {}

    public virtual void GetLeaderboardDataResponse(LeaderboardResult leaderboardResult) {}

    #region Errors
    public virtual void LoginError() { }
    #endregion
}