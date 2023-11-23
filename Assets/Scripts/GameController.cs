using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject shooterEnemy;
    public GameObject runnerEnemySmall;
    public GameObject runnerEnemyBig;
    int maxEnemies;
    int numEnemies;
    int score;
    public int scene;
    private int difficulty;
    private int gameState;
    int enemiesRemaining;
    private float levelTime;

    private SaveManager saveManager;
    public GameObject endUI;
    public Text endText;
    public Text crosshair;
    public Text ammo;
    public Text enemyCount;
    public Scrollbar health;
    public Scrollbar stamina;
    public Scrollbar ammoBar;
    private Boost boost;
    private CameraInput playerCam; 
    private CameraInput weaponCam;
    private GrappleHook grapple;
    private MovementInput movement;
    private Shoot shoot;
    public GameObject[] shooterSpawns;
    public GameObject[] runnerSpawns;
    public int Waves;
    public GameObject pauseMenu;
    public GameObject play;
    public GameObject options;
    public GameObject controls;
    public GameObject quit;

    private void Awake()
    {
        saveManager = GameObject.FindGameObjectWithTag("SaveManager").GetComponent<SaveManager>();
        difficulty = saveManager.GetDifficulty();
    }

    void Start()
    {
        GameObject[] cams = GameObject.FindGameObjectsWithTag("Camera");
        playerCam = cams[0].GetComponentInChildren<CameraInput>();
        weaponCam = cams[1].GetComponentInChildren<CameraInput>();

        boost = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<Boost>();
        grapple = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<GrappleHook>();
        movement = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<MovementInput>();
        shoot = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<Shoot>();
        if (scene != 0) //scene 0 = tutorial level
        {
            
            levelTime = 0;
            maxEnemies = shooterSpawns.Length + runnerSpawns.Length;
            numEnemies = maxEnemies;

            score = 0;
            gameState = 0;

            enemiesRemaining = Waves * maxEnemies;
            enemyCount.text = "Remaining: " + enemiesRemaining.ToString(); //Updates UI

            StartCoroutine(SpawnWave()); //Spawns first wave
            Waves -= 1;
        }
    }

    private void Update()
    {
        levelTime += 1 * Time.deltaTime;
    }

    public void SetDifficulty(int value)
    {
        difficulty = value;
    }

    public int GetDifficulty()
    {
        return difficulty;
    }

    public void PlayerDead() //Removes UI and displays losing screen when player dies
    {
        if (gameState != 1 && scene != 0)
        {
            RemoveHUD();
            endUI.SetActive(true);
            endText.text = "You Lose!\nScore: " + score; 
            gameState = 2;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void EnemyDead() 
    {
        if (scene != 0)
        {
            numEnemies -= 1;
            enemiesRemaining -= 1;
            score++;

            enemyCount.text = "Remaining: " + enemiesRemaining.ToString();

            if (numEnemies <= 0 && gameState != 2 && Waves == 0) //Ends game if there are no more enemies or waves
            {
                saveManager.SetScore(SceneManager.GetActiveScene().buildIndex-2, difficulty-1, levelTime);
                saveManager.Save();
                RemoveHUD();
                endUI.SetActive(true);
                endText.text = "						You Win!!\n\nScore: " + score + "\n\nTime: " + Mathf.Round(levelTime * 100.0f) / 100.0f + " Seconds";
                gameState = 1;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            else if (numEnemies <= 0) //New wave if enemies are dead and there are more waves remaining
            {
                Waves -= 1;
                numEnemies = maxEnemies;
                StartCoroutine(SpawnWave());
            }
        }
    }

    public bool GameOver()
    {
        return gameState == 1 || gameState == 2 ? true : false;
    }

    private void RemoveHUD()
    {
        crosshair.text = "";
        ammo.text = "";
        health.gameObject.SetActive(false); 
        stamina.gameObject.SetActive(false);
        ammoBar.gameObject.SetActive(false);
        enemyCount.gameObject.SetActive(false);

        pauseMenu.SetActive(true);
        play.SetActive(false);
        options.SetActive(false);
        controls.SetActive(false);
        quit.SetActive(false);

        movement.StopAudio();
        weaponCam.enabled = false;
        playerCam.enabled = false;
        movement.enabled = false;
        boost.enabled = false;
        grapple.enabled = false;
        shoot.enabled = false;
    }

    IEnumerator SpawnWave()
    {
        for (int i = 0; i < shooterSpawns.Length; i++)
        {
            Instantiate(shooterEnemy, shooterSpawns[i].transform);
            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 0; i < runnerSpawns.Length; i++)
        {
            if (i % 2 ==0)
            {
                Instantiate(runnerEnemySmall, runnerSpawns[i].transform);
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                Instantiate(runnerEnemyBig, runnerSpawns[i].transform);
                yield return new WaitForSeconds(0.1f);
            }
            
        }
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
