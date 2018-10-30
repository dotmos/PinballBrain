public interface IVideomodeGame {
    /// <summary>
    /// Start the game
    /// </summary>
    void StartGame();

    /// <summary>
    /// Observable that is triggered when game is finished
    /// </summary>
    /// <returns></returns>
    System.IObservable<bool> OnGameFinished();

    /// <summary>
    /// Set the unity bridge for the videomode game
    /// </summary>
    /// <param name="brain"></param>
    void SetUnityBridge(UnityBridge unityBridge);
}
