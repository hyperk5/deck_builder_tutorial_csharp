namespace DeckBuilder;

using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

[SuperNode(typeof(AutoNode))]
public partial class IntentUI : HBoxContainer {
  public override partial void _Notification(int what);

  [Node] public TextureRect Icon { get; set; } = default!;
  [Node] public Label Number { get; set; } = default!;

  public void UpdateIntent(Intent? intent) {
    if (intent == null) {
      Hide();
      return;
    }

    Icon.Texture = intent.Icon;
    Icon.Visible = intent.Icon != null;
    Number.Text = intent.Number;
    Number.Visible = intent.Number.Length > 0;
    Show();
  }
}
