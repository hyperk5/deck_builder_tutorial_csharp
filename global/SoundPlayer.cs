namespace DeckBuilder;

using Godot;

public partial class SoundPlayer : Node {
  public void Play(AudioStream? stream, bool isSingle = false) {
    if (stream == null) {
      return;
    }
    if (isSingle) {
      Stop();
    }
    foreach (var child in GetChildren()) {
      if (child is AudioStreamPlayer player) {
        if (!player.Playing) {
          player.Stream = stream;
          player.Play();
          break;
        }
      }
    }
  }

  public void Stop() {
    foreach (var child in GetChildren()) {
      if (child is AudioStreamPlayer player) {
        player.Stop();
        player.Stream = null;
      }
    }
  }

  public override void _ExitTree() => Stop();
}
