namespace DeckBuilder;

using System.Threading.Tasks;
using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

[SuperNode(typeof(AutoNode))]
public partial class Enemy : Area2D {
  public override partial void _Notification(int what);

  private const int ARROW_OFFSET = 5;

  private EnemyStats _stats = default!;
  [Export]
  public EnemyStats Stats {
    get => _stats;
    set => UpdateEnemyStats(value);
  }

  [Node] public Sprite2D Sprite2D = default!;
  [Node] public Sprite2D Arrow = default!;
  [Node] public StatsUI StatsUI = default!;
  [Node] public IntentUI IntentUI = default!;

  private Shaker _shaker = default!;
  private EnemyActionPicker? _enemyActionPicker;
  private EnemyAction? _currentAction;
  private readonly Material _whiteSpriteMaterial = ResourceLoader.Load<Material>("res://art/white_sprite_material.tres");

  public void TakeDamage(int damage) {
    if (Stats.Health <= 0) {
      return;
    }

    Sprite2D.Material = _whiteSpriteMaterial;

    var tween = CreateTween();
    tween.TweenCallback(Callable.From(() => _shaker.Shake(this, 16, 0.15f)));
    tween.TweenCallback(Callable.From(() => Stats.TakeDamage(damage)));
    tween.TweenInterval(0.17);
    tween.Finished += () => {
      Sprite2D.Material = null;
      if (Stats.Health <= 0) {
        QueueFree();
      }
    };
  }

  public void OnReady() {
    _shaker = GetNode<Shaker>("/root/Shaker");
    UpdateEnemy();
    AreaEntered += OnAreaEntered;
    AreaExited += OnAreaExited;
  }

  public void SetCurrentAction(EnemyAction? action) {
    _currentAction = action;
    if (action != null) {
      IntentUI.UpdateIntent(action.Intent);
    }
  }

  public void SetupAi() {
    _enemyActionPicker?.QueueFree();

    _enemyActionPicker = Stats.Ai.Instantiate<EnemyActionPicker>();
    _enemyActionPicker.Enemy = this;
    AddChild(_enemyActionPicker);
  }

  private async void UpdateEnemyStats(EnemyStats statsTemplate) {
    if (_stats != null) {
      Stats.Changed -= UpdateStats;
      Stats.Changed -= UpdateAction;
    }
    if (!IsNodeReady()) {
      await ToSignal(this, SignalName.Ready);
    }
    _stats = statsTemplate.CreateInstance();
    _stats.Changed += UpdateStats;
    _stats.Changed += UpdateAction;
    UpdateEnemy();
  }

  private void UpdateEnemy() {
    if (Stats == null) {
      return;
    }
    Sprite2D.Texture = Stats.Art;
    Arrow.Position = Vector2.Right * ((Sprite2D.GetRect().Size.X / 2) + ARROW_OFFSET);
    SetupAi();
    UpdateStats();
  }

  public void UpdateAction() {
    if (_enemyActionPicker == null) {
      return;
    }
    if (_currentAction == null) {
      SetCurrentAction(_enemyActionPicker.GetAction());
      return;
    }
    var conditionalAction = _enemyActionPicker.GetFirstConditionalAction();
    if (conditionalAction != null && _currentAction != conditionalAction) {
      SetCurrentAction(conditionalAction);
    }
  }

  public async Task DoTurn() {
    Stats.Block = 0;
    await _currentAction!.PerformAction();
  }

  private void UpdateStats() => StatsUI.UpdateStats(Stats);
  private void OnAreaEntered(Area2D area2D) => Arrow.Show();
  private void OnAreaExited(Area2D area2D) => Arrow.Hide();
}
