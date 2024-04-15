namespace DeckBuilder;

using System.Collections.Generic;
using Godot;

public abstract partial class Effect : RefCounted {
  public abstract void Execute(List<Node> targets);
}
