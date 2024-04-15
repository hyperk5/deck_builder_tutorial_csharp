namespace DeckBuilder;

using System;
using System.Collections.Generic;
using Chickensoft.AutoInject;
using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

[SuperNode(typeof(AutoNode), typeof(Dependent))]
public partial class CardTargetSelector : Node2D {

  #region Signal
  [Signal]
  public delegate void TargetEnterEventHandler(Area2D area2D);
  [Signal]
  public delegate void TargetExitEventHandler(Area2D area2D);
  #endregion

  public override partial void _Notification(int what);

  #region Node
  [Node] public Area2D Area2D = default!;
  [Node("CanvasLayer/CardArc")] public Line2D CardArc = default!;
  #endregion

  #region State
  private bool _isTargeting;
  #endregion

  private const int POINT_NUM = 8;

  public void OnResolved() {
    Area2D.AreaEntered += OnAreaEntered;
    Area2D.AreaExited += OnAreaExited;
  }

  public void OnPredelete() {
    Area2D.AreaEntered -= OnAreaEntered;
    Area2D.AreaExited -= OnAreaExited;
  }

  public void StartAiming() {
    _isTargeting = true;
    Area2D.Monitorable = true;
    Area2D.Monitoring = true;
  }

  public void EndAiming() {
    _isTargeting = false;
    Area2D.Monitorable = false;
    Area2D.Monitoring = false;
    Area2D.Position = Vector2.Zero;
    CardArc.ClearPoints();
  }

  private void OnAreaEntered(Area2D area) => EmitSignal(SignalName.TargetEnter, area);
  private void OnAreaExited(Area2D area) => EmitSignal(SignalName.TargetExit, area);

  public override void _Process(double delta) {
    if (!_isTargeting) {
      Area2D.Monitorable = false;
      Area2D.Monitoring = false;
      return;
    }
    Area2D.Position = GetLocalMousePosition();
    CardArc.Points = GetPoints().ToArray();
  }

  private List<Vector2> GetPoints() {
    var points = new List<Vector2>();
    var start = GlobalPosition;
    var target = GetGlobalMousePosition();
    var distance = target - start;

    for (var i = 0; i < POINT_NUM; ++i) {
      var t = 1.0 / POINT_NUM * i;
      var x = start.X + (distance.X / POINT_NUM * i);
      var y = start.Y + (EaseOutCubic((float)t) * distance.Y);

      points.Add(new Vector2(x, y));
    }
    return points;
  }

  private static float EaseOutCubic(float number) => 1.0f - (float)Math.Pow(1.0 - (double)number, 3.0);
}
