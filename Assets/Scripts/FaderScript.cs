using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class FaderScript : MonoBehaviour
{
	public Texture2D fadeOutTexture;

	private int drawDepth = -1000;
	private float fadeDir = -1; //For fading in/out
	private float alpha = 1.0f;

	private GameObject currentScreen;
	private GameObject nextScreen;

    void OnGUI()
	{
		//Unscaled delta time so the scene can be paused and use the fade script
		alpha += fadeDir * 0.25f * Time.unscaledDeltaTime;

		//Increases/decreases transparency of black image taht fills the whole screen
		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha); 
		GUI.depth = drawDepth;
		Rect dimension = new Rect(0, 0, Screen.width, Screen.height);
		GUI.DrawTexture(dimension, fadeOutTexture);
	}

	public void SetCurrent(GameObject curScreen) //Allows menus to be set active/disabled in between fades
    {
		currentScreen = curScreen;
    }
	public void SetNext(GameObject nexScreen)
	{
		nextScreen = nexScreen;
	}

	public float BeginFade(float direction) //Starts fading
	{
		fadeDir = direction;
		return 22.5f / 0;
	}

	public void Fade()
	{
		StartCoroutine(FadeScreen(0));
	}

    public IEnumerator FadeOnce(int level) //Fades out of levels
    {
		alpha = 1f;
		float fadeResult = BeginFade(7);
		float count = 0;
		while (count < 0.1f)
		{
			count += 1 * Time.unscaledDeltaTime;
			yield return null;
		}
		SceneManager.LoadScene(level);
	}

	public IEnumerator FadeExit(GameObject pauseMenu, GameObject optionsMenu, GameObject controlsMenu) //Fades from menu to gameplay
	{
		alpha = 1f;
		float fadeResult = BeginFade(7);
		float count = 0;
		while (count < 0.1f)
		{
			count += 1 * Time.unscaledDeltaTime;
			yield return null;
		}

		pauseMenu.SetActive(false);
		optionsMenu.SetActive(false);
		controlsMenu.SetActive(false);
		fadeResult = BeginFade(-7);
	}

	public IEnumerator FadeScreen(int i) //Fades between menu screens
	{
		alpha = 1f;
		float fadeResult = BeginFade(7);
		float count = 0;
		while (count < 0.1f)
        {
			count += 1 * Time.unscaledDeltaTime;
			yield return null;
        }
		if (currentScreen.activeSelf)
        {
			currentScreen.SetActive(false);
		}
		
		nextScreen.SetActive(true);
		fadeResult = BeginFade(-7);
	}

	void OnLevelLoaded() //Fades out twhen the level is started
	{
		alpha = 1;
		BeginFade(-0.25f);
	}
}
