using UnityEngine;
using Tomino;

public class GameController : MonoBehaviour
{
    public Camera currentCamera;
    public Game game;
    public GameConfig NeonGameConfig;
    public GameConfig BlueGameConfig;
    public AudioPlayer audioPlayer;
    public AudioSource musicAudioSource;

    private GameConfig GameConfig;
    private UniversalInput universalInput;
    private BoardView BoardView => GameConfig.BoardView;
    private AlertView AlertView => GameConfig.AlertView;
    private SettingsView SettingsView => GameConfig.SettingsView;

    // This looks like some "view model".
    private bool AlertViewVisible = false;
    private bool SettingsViewVisible = false;

    internal void Start()
    {
        switch (Settings.Theme)
        {
            case Theme.Blue:
                GameConfig = BlueGameConfig;
                BlueGameConfig.GameView.SetActive(true);
                NeonGameConfig.GameView.SetActive(false);
                break;
            case Theme.Neon:
                GameConfig = NeonGameConfig;
                BlueGameConfig.GameView.SetActive(false);
                NeonGameConfig.GameView.SetActive(true);
                break;
            default:
                break;
        }

        Board board = new(10, 20);

        NeonGameConfig.BoardView.SetBoard(board);
        BlueGameConfig.BoardView.SetBoard(board);

        NeonGameConfig.NextPieceView.SetBoard(board);
        BlueGameConfig.NextPieceView.SetBoard(board);

        // TODO: Enable providing input to the Game dynamically.
        universalInput = new UniversalInput(new KeyboardInput(), NeonGameConfig.BoardView.touchInput);

        game = new Game(board, universalInput);
        game.FinishedEvent += OnGameFinished;
        game.PieceFinishedFallingEvent += audioPlayer.PlayPieceDropClip;
        game.PieceRotatedEvent += audioPlayer.PlayPieceRotateClip;
        game.PieceMovedEvent += audioPlayer.PlayPieceMoveClip;
        game.Start();

        NeonGameConfig.ScoreView.game = game;
        BlueGameConfig.ScoreView.game = game;

        NeonGameConfig.LevelView.game = game;
        BlueGameConfig.LevelView.game = game;

        HandlePlayerSettings();
        Settings.ChangedEvent += HandlePlayerSettings;
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
        AlertView.Show(() =>
        {
            AlertView.Hide();
            AlertViewVisible = false;
        });
        AlertViewVisible = true;
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
        AlertView.Show(() =>
        {
            AlertView.Hide();
            AlertViewVisible = false;
        });
        AlertViewVisible = true;
    }

    private void ShowSettingsView()
    {
        SettingsView.Show(() =>
        {
            SettingsViewVisible = false;
            ShowPauseView();
        });
        SettingsViewVisible = true;
    }

    private void HandlePlayerSettings()
    {
        GameConfig.ScreenButtonsView.SetActive(Settings.ScreenButonsEnabled);
        BoardView.touchInput.Enabled = !Settings.ScreenButonsEnabled;
        musicAudioSource.gameObject.SetActive(Settings.MusicEnabled);

        switch (Settings.Theme)
        {
            case Theme.Blue:
                ApplyGameConfig(BlueGameConfig);
                break;
            case Theme.Neon:
                ApplyGameConfig(NeonGameConfig);
                break;
            default:
                break;
        }
    }

    private void ApplyGameConfig(GameConfig config)
    {
        if (config == GameConfig)
        {
            return;
        }

        GameConfig.GameView.SetActive(false);
        GameConfig.AlertView.Hide();
        GameConfig.SettingsView.Hide();

        GameConfig = config;

        if (AlertViewVisible)
        {
            GameConfig.AlertView.Show(() => { });
        }

        if (SettingsViewVisible)
        {
            SettingsView.Show(ShowPauseView);
        }

        GameConfig.GameView.SetActive(true);
    }
}
