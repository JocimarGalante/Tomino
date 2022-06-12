using UnityEngine;
using Tomino;

public class GameController : MonoBehaviour
{
    public Camera currentCamera;
    public Game game;
    public GameConfig GameConfig;
    public SettingsView settingsView;
    public AudioPlayer audioPlayer;
    public AudioSource musicAudioSource;

    private UniversalInput universalInput;
    private AlertView AlertView => GameConfig.AlertView;

    internal void Awake()
    {
        HandlePlayerSettings();
        Settings.ChangedEvent += HandlePlayerSettings;
    }

    internal void Start()
    {
        Board board = new(10, 20);

        GameConfig.BoardView.SetBoard(board);
        GameConfig.NextPieceView.SetBoard(board);

        universalInput = new UniversalInput(new KeyboardInput(), GameConfig.BoardView.touchInput);

        game = new Game(board, universalInput);
        game.FinishedEvent += OnGameFinished;
        game.PieceFinishedFallingEvent += audioPlayer.PlayPieceDropClip;
        game.PieceRotatedEvent += audioPlayer.PlayPieceRotateClip;
        game.PieceMovedEvent += audioPlayer.PlayPieceMoveClip;
        game.Start();

        GameConfig.ScoreView.game = game;
        GameConfig.LevelView.game = game;
    }

    public void OnPauseButtonTap()
    {
        game.Pause();
        ShowPauseView();
    }

    public void OnMoveLeftButtonTap()
    {
        game.SetNextAction(PlayerAction.MoveLeft);
    }

    public void OnMoveRightButtonTap()
    {
        game.SetNextAction(PlayerAction.MoveRight);
    }

    public void OnMoveDownButtonTap()
    {
        game.SetNextAction(PlayerAction.MoveDown);
    }

    public void OnFallButtonTap()
    {
        game.SetNextAction(PlayerAction.Fall);
    }

    public void OnRotateButtonTap()
    {
        game.SetNextAction(PlayerAction.Rotate);
    }

    private void OnGameFinished()
    {
        AlertView.SetTitle(Constant.Text.GameFinished);
        AlertView.AddButton(Constant.Text.PlayAgain, game.Start, audioPlayer.PlayNewGameClip);
        AlertView.Show();
    }

    internal void Update()
    {
        game.Update(Time.deltaTime);
    }

    private void ShowPauseView()
    {
        AlertView.SetTitle(Constant.Text.GamePaused);
        AlertView.AddButton(Constant.Text.Resume, game.Resume, audioPlayer.PlayResumeClip);
        AlertView.AddButton(Constant.Text.NewGame, game.Start, audioPlayer.PlayNewGameClip);
        AlertView.AddButton(Constant.Text.Settings, ShowSettingsView, audioPlayer.PlayResumeClip);
        AlertView.Show();
    }

    private void ShowSettingsView()
    {
        settingsView.Show(ShowPauseView);
    }

    private void HandlePlayerSettings()
    {
        GameConfig.ScreenButtonsView.SetActive(Settings.ScreenButonsEnabled);
        GameConfig.BoardView.touchInput.Enabled = !Settings.ScreenButonsEnabled;
        musicAudioSource.gameObject.SetActive(Settings.MusicEnabled);
    }
}
