namespace DeckBuilder;

using Godot;

public partial class Shaker : Node {

  public void Shake(Node2D thing, float strength, float duration) {
    if (thing == null) {
      return;
    }
    var originalPosition = thing.Position;
    var shakeCount = 10;
    var tween = thing.CreateTween();

    for (var i = 0; i < shakeCount; ++i) {
      var shakeOffset = new Vector2(GD.RandRange(-1, 1), GD.RandRange(-1, 1));
      var target = originalPosition + (strength * shakeOffset);
      if (i % 2 == 0) {
        target = originalPosition;
      }
      tween.TweenProperty(thing, "position", target, duration / shakeCount);
      strength *= 0.75f;
    }
    tween.Finished += () => thing.Position = originalPosition;
  }
}
