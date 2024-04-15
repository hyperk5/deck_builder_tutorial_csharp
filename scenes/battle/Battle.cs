namespace DeckBuilder;

using System.Threading.Tasks;
using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

[SuperNode(typeof(AutoNode))]
public partial class Battle : Node2D {
  public override partial void _Notification(int what);

  [Export] public CharacterStats CharacterStats { get; set; } = default!;

  [Node] public Player Player { get; set; } = default!;
  [Node] public PlayerHandler PlayerHandler { get; set; } = default!;
  [Node] public EnemyHandler EnemyHandler { get; set; } = default!;
  [Node] public RedFlash RedFlash { get; set; } = default!;
  [Node("BattleUI/ManaUI")] public ManaUI ManaUI { get; set; } = default!;
  [Node("BattleUI/Hand")] public Hand Hand { get; set; } = default!;
  [Node("BattleUI/EndTurnButton")] public Button EndTurnButton { get; set; } = default!;
  [Node("BattleOverLayer/BattleOverPanel")] public BattleOverPanel BattleOverPanel { get; set; } = default!;

  private BattleLogic _battleLogic = default!;
  private BattleLogic.IBinding _battleLogicBinding = default!;

  public void OnReady() {
    _battleLogic = new BattleLogic();
    _battleLogicBinding = _battleLogic.Bind();

    _battleLogicBinding.Handle<BattleLogic.Output.InitEnter>(_ => OnInitEnter());
    _battleLogicBinding.Handle<BattleLogic.Output.PlayerTurnEnter>(async _ => await OnPlayerTurnEnter());
    _battleLogicBinding.Handle<BattleLogic.Output.PlayerTurnExit>(_ => OnPlayerTurnExit());
    _battleLogicBinding.Handle<BattleLogic.Output.EnemyTurnEnter>(async _ => await OnEnemyTurnEnter());
    _battleLogicBinding.Handle<BattleLogic.Output.GameOverEnter>(_ => OnGameOverEnter());
    _battleLogicBinding.Handle<BattleLogic.Output.VictoryEnter>(_ => OnVictoryEnter());

    EndTurnButton.Pressed += () => _battleLogic.Input(new BattleLogic.Input.PlayerTurnEnd());
    PlayerHandler.PlayerDead += () => _battleLogic.Input(new BattleLogic.Input.PlayerDead());
    EnemyHandler.EnemiesAllDead += () => _battleLogic.Input(new BattleLogic.Input.EnemiesAllDead());

    PlayerHandler.DamageTaken += OnDamageTaken;
    _battleLogic.Start();
  }

  public override void _ExitTree() {
    _battleLogic.Stop();
    _battleLogicBinding.Dispose();
  }

  private void OnInitEnter() {
    GetTree().Paused = false;
    var stats = CharacterStats.CreateInstance();
    Player.Stats = stats;
    ManaUI.CharacterStats = stats;
    Hand.CharacterStats = stats;
    PlayerHandler.Hand = Hand;

    PlayerHandler.StartBattle(stats);
    _battleLogic.Input(new BattleLogic.Input.PlayerIsReady());
  }

  private async Task OnPlayerTurnEnter() {
    EnemyHandler.ResetEnemyActions();
    await PlayerHandler.StartTurn();
    EndTurnButton.Disabled = false;
  }

  private void OnPlayerTurnExit() => EndTurnButton.Disabled = true;

  private async Task OnEnemyTurnEnter() {
    await PlayerHandler.EndTurn();
    await EnemyHandler.StartTurn();
    _battleLogic.Input(new BattleLogic.Input.EnemyTurnEnd());
  }

  private void OnEnemyOrderChanged() {
    if (EnemyHandler.GetChildCount() == 0) {
      _battleLogic.Input(new BattleLogic.Input.EnemiesAllDead());
    }
  }

  private void OnGameOverEnter() => BattleOverPanel.ShowScreen("Game Over!", BattleOverPanel.Type.LOSE);
  private void OnVictoryEnter() => BattleOverPanel.ShowScreen("Victorious!", BattleOverPanel.Type.WIN);
  private void OnDamageTaken() => RedFlash.ShowRedFlash();
}
