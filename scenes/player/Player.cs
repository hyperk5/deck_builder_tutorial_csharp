namespace DeckBuilder;

using Chickensoft.AutoInject;
using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

[SuperNode(typeof(AutoNode), typeof(Dependent))]
public partial class Player : Node2D {
  public override partial void _Notification(int what);

  private CharacterStats _stats = default!;
  public CharacterStats Stats {
    get => _stats;
    set => UpdateCharacterStats(value);
  }

  [Node] public Sprite2D Sprite2D = default!;
  [Node] public StatsUI StatsUI = default!;

  private Shaker _shaker = default!;

  private readonly Material _whiteSpriteMaterial = ResourceLoader.Load<Material>("res://art/white_sprite_material.tres");

  public void OnReady() => _shaker = GetNode<Shaker>("/root/Shaker");

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

  private async void UpdateCharacterStats(CharacterStats stats) {
    if (_stats != null) {
      _stats.Changed -= OnCharacterStatsChanged;
    }
    if (!IsNodeReady()) {
      await ToSignal(this, SignalName.Ready);
    }
    _stats = stats;
    _stats.Changed += OnCharacterStatsChanged;
    OnCharacterStatsChanged();
  }

  private void OnCharacterStatsChanged() {
    if (Stats == null) {
      return;
    }
    Sprite2D.Texture = Stats.Art;
    OnStatsChanged();
  }

  private void OnStatsChanged() => StatsUI.UpdateStats(Stats);
}
