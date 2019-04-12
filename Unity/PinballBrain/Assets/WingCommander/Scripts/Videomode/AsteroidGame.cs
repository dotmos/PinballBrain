using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

public class AsteroidGame : MonoBehaviour, IVideomodeGame {

    PinballBrain.IBrain brain;
    UnityVideoPlayer videoPlayer;

    public GameObject[] asteroidPrefabs;
    public float asteroidSpeed = 25;
    public GameObject playerPrefab;
    public GameObject playerDeathEffectPrefab;
    public GameObject gameOverAnimationPrefab;
    public Transform gameContainer;
    public int asteroidsPerRow = 3;
    public float playerSpeed = 100;
    public int scorePerAsteroid = 250;
    public GameObject gameOverUI;
    public AudioSource scoreSound;
    

    HashSet<GameObject> objects;
    HashSet<GameObject> asteroids;
    GameObject player;

    bool gameRunning = false;
    bool gameOver = false;
    bool gamePaused = false;

    /// <summary>
    /// Time to show the game over screen (in seconds)
    /// </summary>
    int gameOverTime = 2;

    float newObjectSpawnCounter = 0;
    float newObjectSpawnTime = 3.0f;
    int asteroidKillLineY;
    float absoluteAsteroidMoveSpeed;

    float playerMoveRight = 0;
    float playerMoveLeft = 0;

    ReactiveCommand<bool> gameFinishedRx;

    Camera _camera;
    Camera Camera {
        get {
            if(_camera == null) {
                _camera = Camera.main;
            }
            return _camera;
        }
    }

    /// <summary>
    /// Set the pinball brain to use for the game
    /// </summary>
    /// <param name="brain"></param>
    public void SetUnityBridge(UnityBridge unityBridge) {
        this.brain = unityBridge.Brain;
        this.videoPlayer = unityBridge.VideoPlayer;
    }

	// Use this for initialization
	void Awake () {
        objects = new HashSet<GameObject>();
        asteroids = new HashSet<GameObject>();
        gameOverUI.SetActive(false);
        gameFinishedRx = new ReactiveCommand<bool>();
    }

    /// <summary>
    /// Start the game
    /// </summary>
    public void StartGame() {
        //Do nothing if game is already running
        if (gameRunning) return;

        //Stop whatever video is currently running
        this.videoPlayer.Stop();
        

        if(brain == null) {
            Debug.LogError("No brain set! Can not start game.");
            return;
        }

        this.gameObject.SetActive(true);

        gameOverUI.SetActive(false);

        //Create player
        player = Instantiate(playerPrefab, gameContainer);
        player.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.15f , 0);
        Rigidbody2D playerRigid = player.AddComponent<Rigidbody2D>();
        playerRigid.gravityScale = 0;
        playerRigid.freezeRotation = true;

        //Add movement controls to player
        brain.OnSwitchActive(GameBrain.Switches.FLIPPER_LEFT, (s) => { playerMoveLeft = -1; }).AddTo(player).AddTo(this).AddTo(this.player);
        brain.OnSwitchInactive(GameBrain.Switches.FLIPPER_LEFT, (s) => { playerMoveLeft = 0; }).AddTo(player).AddTo(this).AddTo(this.player);
        brain.OnSwitchActive(GameBrain.Switches.FLIPPER_RIGHT, (s) => { playerMoveRight = 1; }).AddTo(player).AddTo(this).AddTo(this.player);
        brain.OnSwitchInactive(GameBrain.Switches.FLIPPER_RIGHT, (s) => { playerMoveRight = 0; }).AddTo(player).AddTo(this).AddTo(this.player);

        //Game Over if player hits something
        player.OnCollisionEnter2DAsObservable().Subscribe(e => GameOver()).AddTo(player).AddTo(this);
        objects.Add(player);

        //Setup killline
        asteroidKillLineY = -(int)(Screen.height * 0.2f);
        //Setup asteroidspeed
        absoluteAsteroidMoveSpeed = Screen.height * 0.01f * asteroidSpeed;

        //Start spawning objects
        gameRunning = true;
        SpawnRandomAsteroidRow();
    }

	// Update is called once per frame
	void Update () {
        if (gamePaused) return;

        if (gameRunning) {
            newObjectSpawnCounter += Time.deltaTime;
            if(newObjectSpawnCounter >= newObjectSpawnTime) {
                //SpawnRandomAsteroid(0, Screen.width);
                SpawnRandomAsteroidRow();
                newObjectSpawnCounter = 0;
            }

            //Move all objects and check if objects are no longer part of the game area
            asteroids.RemoveWhere(o => o == null);
            foreach (GameObject go in asteroids) {
                go.transform.position += Vector3.down * Time.deltaTime * absoluteAsteroidMoveSpeed;

                //Destroy objects behind "Killline" and add score
                if (go.transform.position.y <= asteroidKillLineY) {
                    if (!gameOver) {
                        if (scoreSound != null) scoreSound.Play();
                        brain.IncreaseCurrentPlayerScore(scorePerAsteroid);
                    }
                    Destroy(go);
                }
            }

            //Move player
            if (player != null) {
                //Move
                player.transform.position += Vector3.right * (playerMoveRight + playerMoveLeft) * Time.deltaTime * (Screen.width * 0.01f * playerSpeed);
                //Keep on screen
                if (player.transform.position.x < 100) player.transform.position = new Vector3(100, player.transform.position.y, player.transform.position.z);
                else if(player.transform.position.x > Screen.width - 100) player.transform.position = new Vector3(Screen.width - 100, player.transform.position.y, player.transform.position.z);
            }
        }
	}

    void SpawnRandomAsteroidAtPosition(Vector3 position, float scale) {
        GameObject p = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length - 1)];
        GameObject asteroid = Instantiate(p, gameContainer, false);
        asteroid.transform.position = position;
        asteroid.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-360, 360));
        asteroid.transform.localScale = Vector3.one * scale;
        objects.Add(asteroid);
        asteroids.Add(asteroid);
    }

    void SpawnRandomAsteroid(int minXPos, int maxXPos, int maxYOffset, float minScale, float maxScale) {
        Vector3 spawnPos = new Vector3(Random.Range(minXPos, maxXPos), Screen.height + Screen.height*0.1f + Random.Range(0, maxYOffset), 0);
        float scale = Random.Range(minScale, maxScale);
        SpawnRandomAsteroidAtPosition(spawnPos, scale);
    }

    void SpawnRandomAsteroidRow(int count, int rowPositionOffset) {
        float distanceBetweenAsteroids = Screen.width / (float)count;
        float distanceBetweenAsteroidsHalf = distanceBetweenAsteroids * 0.5f;
        for (int i=0; i<count; ++i) {
            SpawnRandomAsteroid(
                rowPositionOffset + (int)(i * distanceBetweenAsteroids), rowPositionOffset + (int)(i * distanceBetweenAsteroids + distanceBetweenAsteroidsHalf),
                300,
                1.5f, 3.0f);
        }
    }

    void SpawnRandomAsteroidRow() {
        SpawnRandomAsteroidRow(asteroidsPerRow, (int)Random.Range(0, (Screen.width / (float)asteroidsPerRow) * 0.5f));
    }

    void GameOver() {
        gameOver = true;
        
        //Play death animation, then end the game
        if (gameOverAnimationPrefab != null) {
            gamePaused = true;
            //Destroy player
            Destroy(this.player.gameObject);

            GameObject gameOverAnimation = GameObject.Instantiate(gameOverAnimationPrefab.gameObject, gameContainer);
            gameOverAnimation.GetComponentInChildren<WCAnimationPlayerBase>().OnAnimationFinished().Subscribe(e => {
                EndGame();
            }).AddTo(this).AddTo(gameOverAnimation);

        } else {
            //Player explosion at player position
            GameObject playerDeathEffect = Instantiate(playerDeathEffectPrefab, gameContainer);
            playerDeathEffect.transform.position = this.player.transform.position;
            playerDeathEffect.GetComponentInChildren<WCAnimationPlayerBase>().OnAnimationFinished().Subscribe(e => {
                EndGame();
            }).AddTo(this).AddTo(playerDeathEffect);
            //Destroy player
            Destroy(this.player.gameObject);
        }
        
        
        

        /*
        //Play game over video
        videoPlayer.Play(Videos.WC2.Death);

        //End the game once the video is finished
        videoPlayer.OnVideoFinished().Take(1).Subscribe(videoFinished => {
            //gameOverUI.SetActive(true);

            //Wait some time, then end the game
            //Observable.Timer(System.TimeSpan.FromSeconds(gameOverTime)).Subscribe(e => EndGame()).AddTo(this);
            EndGame();
        }).AddTo(this);
        */
        
    }

    void EndGame() {
        foreach (GameObject go in objects) {
            if(go != null) Destroy(go);
        }
        objects.Clear();

        playerMoveLeft = 0;
        playerMoveRight = 0;
        gameRunning = false;
        gameOver = false;
        gamePaused = false;
        newObjectSpawnCounter = 0;

        gameFinishedRx.Execute(true);

        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Observable that is triggered when game is finished
    /// </summary>
    /// <returns></returns>
    public System.IObservable<bool> OnGameFinished() {
        return gameFinishedRx;
    }
}
