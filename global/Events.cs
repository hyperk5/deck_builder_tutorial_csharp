namespace DeckBuilder;

using Godot;

public partial class Events : Node {
  [Signal]
  public delegate void ShowToolTipEventHandler(Card card);

  [Signal]
  public delegate void HideToolTipEventHandler();
}
