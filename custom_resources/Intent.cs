namespace DeckBuilder;
using Godot;

[GlobalClass]
public partial class Intent : Resource {
  [Export] public string Number { get; set; } = default!;
  [Export] public Texture2D Icon { get; set; } = default!;
}
