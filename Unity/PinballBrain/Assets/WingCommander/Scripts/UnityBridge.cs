using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class UnityBridge : MonoBehaviour {

    GameBrain _brain;
    public GameBrain Brain {
        get {
            if(_brain == null) {
                _brain = new GameBrain();
            }
            return _brain;
        }
    }

    public List<UIPlayerScore> playerScore;

    public UnityEngine.UI.RawImage videoPlayerOutput;
    public GameObject videoPlayerObject;

    public Transform videoModeGameContainer;
    IVideomodeGame asteroidGame;

    public UnityVideoPlayer VideoPlayer { get; private set; }

    [System.Serializable]
    public class DebugStuff {
        public List<Image> targets = new List<Image>();
    }
    public DebugStuff debugStuff = new DebugStuff();

    // Use this for initialization
    void Awake() {
        //Connect player score UI
        Brain.OnPlayerScoreChanged(0).Subscribe(e => playerScore[e.playerID].SetScore(e.currentScore)).AddTo(this);
        Brain.OnPlayerScoreChanged(1).Subscribe(e => playerScore[e.playerID].SetScore(e.currentScore)).AddTo(this);
        Brain.OnPlayerScoreChanged(2).Subscribe(e => playerScore[e.playerID].SetScore(e.currentScore)).AddTo(this);
        Brain.OnPlayerScoreChanged(3).Subscribe(e => playerScore[e.playerID].SetScore(e.currentScore)).AddTo(this);
        //Activate correct playerscore UI
        Brain.OnCurrentPlayerChanged().Subscribe(e => {
            foreach(UIPlayerScore s in playerScore) {
                s.Show(false);
            }
            playerScore[e].Show(true);
        }).AddTo(this);

        //Setup videoplayer
        VideoPlayer = new UnityVideoPlayer();
        videoPlayerObject.SetActive(false);
        VideoPlayer.OnVideoStarted().Subscribe(e => {
            videoPlayerObject.SetActive(true);
            videoPlayerOutput.texture = e.texture;
            videoPlayerOutput.SetNativeSize();
        }).AddTo(this);
        VideoPlayer.OnVideoFinished().Subscribe(e => {
            videoPlayerObject.SetActive(false);
        }).AddTo(this);

        //Brain.NextPlayer();
        

        //Setup videomode games
        this.asteroidGame = GameObject.Instantiate(Resources.Load<GameObject>("WC2/Prefabs/Videomode/AsteroidGame"), videoModeGameContainer).GetComponent<IVideomodeGame>();
        this.asteroidGame.SetUnityBridge(this);
        this.asteroidGame.OnGameFinished().Subscribe(e => VideoPlayer.Play(Videos.WC2.Flyby)).AddTo(this);
        //this.asteroidGame.StartGame();
        //Start videomode once brain fires event
        this.Brain.OnBrainEvent<GameBrain.Event_StartVideoMode>().Subscribe(e => {
            this.asteroidGame.StartGame();
        }).AddTo(this);
        

        //Setup debug unity target visuals
        Brain.upperRightDropTargetbank.OnUniqueTargetHit(GameBrain.Switches.DROPTARGETBANK_UPPERRIGHT_1).Subscribe(e => debugStuff.targets[0].color = new Color(1, 1, 1, 1)).AddTo(this);
        Brain.upperRightDropTargetbank.OnUniqueTargetHit(GameBrain.Switches.DROPTARGETBANK_UPPERRIGHT_2).Subscribe(e => debugStuff.targets[1].color = new Color(1, 1, 1, 1)).AddTo(this);
        Brain.upperRightDropTargetbank.OnUniqueTargetHit(GameBrain.Switches.DROPTARGETBANK_UPPERRIGHT_3).Subscribe(e => debugStuff.targets[2].color = new Color(1, 1, 1, 1)).AddTo(this);
        Brain.upperRightDropTargetbank.OnUniqueTargetHit(GameBrain.Switches.TARGETBANK_UPPERRIGHT_1).Subscribe(e => debugStuff.targets[3].color = new Color(1, 1, 1, 1)).AddTo(this);
        Brain.upperRightDropTargetbank.OnUniqueTargetHit(GameBrain.Switches.TARGETBANK_UPPERRIGHT_2).Subscribe(e => debugStuff.targets[4].color = new Color(1, 1, 1, 1)).AddTo(this);
        Brain.upperRightDropTargetbank.OnReset().Subscribe(e => {
            foreach(Image i in debugStuff.targets) {
                i.color = new Color(1, 1, 1, 0.2f);
            }
        }).AddTo(this);
        Brain.upperRightDropTargetbank.ResetBank();

        //Start the game
        Brain.StartNewGame();
        
    }

    private void OnGUI() {
        if (GUI.Button(new Rect(Screen.width * 0.5f - 100, 0, 200, 32), "Disconnect")) {
            Brain.Dispose();
        }
    }

    private void OnApplicationQuit() {
        if(_brain != null) {
            _brain.Dispose();
        }
    }
}