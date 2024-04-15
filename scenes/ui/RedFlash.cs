namespace DeckBuilder;

using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

[SuperNode(typeof(AutoNode))]
public partial class RedFlash : CanvasLayer {

  public override partial void _Notification(int what);

  [Node] public ColorRect ColorRect = default!;
  [Node] public Timer Timer = default!;

  public void OnReady() => Timer.Timeout += OnTimeout;

  public void ShowRedFlash() {
    var color = ColorRect.Color;
    color.A = 0.2f;
    ColorRect.Color = color;
    Timer.Start();
  }

  private void OnTimeout() {
    var color = ColorRect.Color;
    color.A = 0f;
    ColorRect.Color = color;
  }
}
