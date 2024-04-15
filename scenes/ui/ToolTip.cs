namespace DeckBuilder;

using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

[SuperNode(typeof(AutoNode))]
public partial class ToolTip : PanelContainer {
  public override partial void _Notification(int what);

  [Node("%ToolTipIcon")] public TextureRect ToolTipIcon { get; set; } = default!;
  [Node("%ToolTipText")] public RichTextLabel ToolTipText { get; set; } = default!;
  public Events Events = default!;

  private const float FADE_SECONDS = 0.2f;

  private Tween? _tween;
  private bool _isVisible;

  public override void _Ready() {
    Events = GetNode<Events>("/root/Events");
    Events.ShowToolTip += OnShowToolTip;
    Events.HideToolTip += OnHideToolTip;

    Modulate = Colors.Transparent;
    Hide();
  }

  public void OnExitTree() {
    Events.ShowToolTip -= OnShowToolTip;
    Events.HideToolTip -= OnHideToolTip;
  }

  public void ShowToolTip(Texture2D texture2D, string text) {
    _isVisible = true;
    _tween?.Kill();

    ToolTipIcon.Texture = texture2D;
    ToolTipText.Text = text;
    _tween = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
    _tween.TweenCallback(Callable.From(() => Show()));
    _tween.TweenProperty(this, "modulate", Colors.White, FADE_SECONDS);
  }

  public void HideToolTip() {
    _isVisible = false;
    _tween?.Kill();

    GetTree().CreateTimer(FADE_SECONDS, false).Timeout += HideAnimation;
  }

  public void HideAnimation() {
    if (!_isVisible) {
      _tween = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
      _tween.TweenCallback(Callable.From(() => Hide()));
      _tween.TweenProperty(this, "modulate", Colors.Transparent, FADE_SECONDS);
    }
  }

  private void OnShowToolTip(Card card) => ShowToolTip(card.Icon, card.TooltipText);

  private void OnHideToolTip() => HideToolTip();
}
