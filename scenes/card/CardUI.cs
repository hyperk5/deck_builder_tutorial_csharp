namespace DeckBuilder;

using System.Collections.Generic;
using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

[SuperNode(typeof(AutoNode))]
public partial class CardUI : Control {

  public override partial void _Notification(int what);

  public const int REPARENT_TYPE_CANCEL = 0;
  public const int REPARENT_TYPE_SELECT = 1;
  public const int REPARENT_TYPE_PLAY = 2;

  #region Signal
  [Signal]
  public delegate void ReparentRequestEventHandler(CardUI cardUI, int type);
  #endregion

  private Card _card = default!;
  public Card Card {
    get => _card;
    set => SetCard(value);
  }

  private CharacterStats _characterStats = default!;
  public CharacterStats CharacterStats {
    get => _characterStats;
    set => SetCharacterStats(value);
  }

  #region Node
  [Node] public Panel Panel = default!;
  [Node] public Label Cost = default!;
  [Node] public TextureRect Icon = default!;
  [Node] public Area2D DropPointDetector = default!;
  [Node] public CardTargetSelector CardTargetSelector = default!;

  public Events Events = default!;
  #endregion

  #region State
  public CardLogic CardLogic = default!;
  public CardLogic.IBinding CardBinding = default!;
  public List<Node> Targets { get; } = new List<Node>();
  public bool IsDisabled { get; set; }
  public int CardIndex { get; set; }
  private Tween? _tween;
  private bool _isPlayable = true;
  #endregion

  private static readonly Resource _baseStyleBox = ResourceLoader.Load("res://scenes/card/card_base_stylebox.tres");
  private static readonly Resource _hoverStyleBox = ResourceLoader.Load("res://scenes/card/card_hover_stylebox.tres");
  private static readonly Resource _dragStyleBox = ResourceLoader.Load("res://scenes/card/card_drag_stylebox.tres");
  private static readonly StringName _panelPropertyName = new("theme_override_styles/panel");

  #region Lifecycle Callback

  public override void _Ready() {
    Events = GetNode<Events>("/root/Events");

    CardLogic = new CardLogic(this);
    CardBinding = CardLogic.Bind();

    CardBinding.Handle<CardLogic.Output.BaseEnter>(_ => OnBaseEnter());
    CardBinding.Handle<CardLogic.Output.ClickedEnter>(_ => OnClickedEnter());
    CardBinding.Handle<CardLogic.Output.DraggingEnter>(_ => OnDraggingEnter());
    CardBinding.Handle<CardLogic.Output.AimingEnter>(_ => OnAimingEnter());
    CardBinding.Handle<CardLogic.Output.AimingExit>(_ => OnAimingExit());
    CardBinding.Handle<CardLogic.Output.Play>(output => OnPlay(output.Targets));
    CardBinding.Handle<CardLogic.Output.Motion>(_ => GlobalPosition = GetGlobalMousePosition() - PivotOffset);

    MouseEntered += OnMouseEntered;
    MouseExited += OnMouseExited;
    GuiInput += OnGuiInput;
    DropPointDetector.AreaEntered += OnAreaEntered;
    DropPointDetector.AreaExited += OnAreaExited;
    CardTargetSelector.TargetEnter += OnAreaEntered;
    CardTargetSelector.TargetExit += OnAreaExited;
    CardLogic.Start();
  }

  public void OnPredelete() {
    MouseEntered -= OnMouseEntered;
    MouseExited -= OnMouseExited;
    GuiInput -= OnGuiInput;
    DropPointDetector.AreaEntered -= OnAreaEntered;
    DropPointDetector.AreaExited -= OnAreaExited;
    CharacterStats.Changed -= OnCharacterStatsChanged;
    CardLogic.Stop();
    CardBinding.Dispose();
  }
  #endregion

  #region Status Change Callback
  private void OnBaseEnter() {
    if (_tween != null) {
      _tween.Stop();
      _tween = null;
    }
    Panel.Set(_panelPropertyName, _baseStyleBox);
    Targets.Clear();
    DropPointDetector.Monitoring = false;
    PivotOffset = Vector2.Zero;
    EmitSignal(SignalName.ReparentRequest, this, REPARENT_TYPE_CANCEL);
    Events.EmitSignal(Events.SignalName.HideToolTip);
  }

  private void OnClickedEnter() {
    PivotOffset = GetGlobalMousePosition() - GlobalPosition;
    DropPointDetector.Monitoring = true;
  }

  private void OnDraggingEnter() {
    Panel.Set(_panelPropertyName, _dragStyleBox);
    var uiLayer = GetTree().GetFirstNodeInGroup("ui_layer");
    if (uiLayer != null) {
      Reparent(uiLayer);
    }
    IsDisabled = false;
    EmitSignal(SignalName.ReparentRequest, this, REPARENT_TYPE_SELECT);
  }

  private void OnAimingEnter() {
    Targets.Clear();
    var position = new Vector2((GetParentAreaSize().X - Size.X) / 2, GetParentAreaSize().Y - (Size.Y * 1.5f));
    AnimateToPosition(position, 0.2f);
    DropPointDetector.Monitoring = false;
    CardTargetSelector.StartAiming();
  }

  private void OnAimingExit() {
    CardTargetSelector.EndAiming();
    Targets.Clear();
  }
  private void OnPlay(List<Node> targets) {
    Card!.Play(targets, CharacterStats);
    GetNode<SoundPlayer>("/root/SfxPlayer").Play(Card!.Sound);
    EmitSignal(SignalName.ReparentRequest, this, REPARENT_TYPE_PLAY);
    Events.EmitSignal(Events.SignalName.HideToolTip);
    QueueFree();
  }

  private void OnCharacterStatsChanged() {
    if (CharacterStats.Mana - Card.Cost < 0) {
      SetIsPlayable(false);
    }
  }

  #endregion

  #region Input Handling
  // GuiInput cannot handle inputs outside of the Control node. So we should use _Input.
  public override void _Input(InputEvent @event) {
    if (@event.IsActionPressed("left_mouse")) {
      CardLogic.Input(new CardLogic.Input.LeftOutsideControl());
    }
    if (@event.IsActionReleased("left_mouse")) {
      CardLogic.Input(new CardLogic.Input.LeftRelease(Time.GetTicksMsec()));
    }
    if (@event.IsActionPressed("right_mouse")) {
      CardLogic.Input(new CardLogic.Input.Right());
    }
    if (@event is InputEventMouseMotion) {
      CardLogic.Input(new CardLogic.Input.Moving(Time.GetTicksMsec(), GetGlobalMousePosition()));
    }
  }

  // used for Base -> Clicked
  public void OnGuiInput(InputEvent @event) {
    if (!_isPlayable || IsDisabled) {
      return;
    }
    if (@event.IsActionPressed("left_mouse")) {
      CardLogic.Input(new CardLogic.Input.Left());
    }
  }

  public void OnMouseEntered() {
    if (_isPlayable && !IsDisabled) {
      Panel.Set(_panelPropertyName, _hoverStyleBox);
      Events.EmitSignal(Events.SignalName.ShowToolTip, Card);
    }
  }

  public void OnMouseExited() {
    if (_isPlayable && !IsDisabled) {
      Panel.Set(_panelPropertyName, _baseStyleBox);
      Events.EmitSignal(Events.SignalName.HideToolTip);
    }
  }

  public void OnAreaEntered(Area2D area2D) {
    if (!Targets.Contains(area2D)) {
      Targets.Add(area2D);
    }
  }

  public void OnAreaExited(Area2D area2D) => Targets.Remove(area2D);
  #endregion

  private void AnimateToPosition(Vector2 position, float duration) {
    _tween = CreateTween().SetTrans(Tween.TransitionType.Circ).SetEase(Tween.EaseType.Out);
    _tween.TweenProperty(this, "global_position", position, duration);
  }

  private async void SetCard(Card card) {
    if (!IsNodeReady()) {
      await ToSignal(this, SignalName.Ready);
    }
    _card = card;
    Cost.Text = Card.Cost.ToString();
    Icon.Texture = Card.Icon;
  }

  private async void SetCharacterStats(CharacterStats value) {
    if (_characterStats != null) {
      _characterStats.Changed -= OnCharacterStatsChanged;
    }
    if (!IsNodeReady()) {
      await ToSignal(this, SignalName.Ready);
    }
    _characterStats = value;
    _characterStats.Changed += OnCharacterStatsChanged;
  }

  public void SetIsPlayable(bool isPlayable) {
    _isPlayable = isPlayable;
    if (_isPlayable) {
      Cost.RemoveThemeColorOverride("font_color");
      Icon.Modulate = new Color(1, 1, 1, 1);
    }
    else {
      Cost.AddThemeColorOverride("font_color", Colors.Red);
      Icon.Modulate = new Color(1, 1, 1, 0.5f);
    }
  }
}
