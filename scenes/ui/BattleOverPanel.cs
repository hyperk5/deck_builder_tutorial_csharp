namespace DeckBuilder;

using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

[SuperNode(typeof(AutoNode))]
public partial class BattleOverPanel : Panel {
  public override partial void _Notification(int what);

  public enum Type {
    WIN,
    LOSE
  }

  [Node("%Label")] public Label Label = default!;
  [Node("%ContinueButton")] public Button ContinueButton = default!;
  [Node("%RestartButton")] public Button RestartButton = default!;

  public void OnReady() {
    ContinueButton.Pressed += () => GetTree().Quit();
    RestartButton.Pressed += () => GetTree().ReloadCurrentScene();
  }

  public void ShowScreen(string message, Type type) {
    Label.Text = message;
    ContinueButton.Visible = type == Type.WIN;
    RestartButton.Visible = type == Type.LOSE;
    Show();
    GetTree().Paused = true;
  }
}
