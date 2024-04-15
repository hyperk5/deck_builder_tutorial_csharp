namespace DeckBuilder;

using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

[SuperNode(typeof(AutoNode))]
public partial class ManaUI : Panel {
  public override partial void _Notification(int what);

  private CharacterStats _characterStats = default!;
  public CharacterStats CharacterStats {
    get => _characterStats;
    set => UpdateCharacterStats(value);
  }

  [Node] public Label ManaLabel { get; set; } = default!;

  private async void UpdateCharacterStats(CharacterStats characterStats) {
    if (characterStats == null) {
      return;
    }
    if (_characterStats != null) {
      _characterStats.Changed -= OnCharacterStatsChanged;
    }
    if (!IsNodeReady()) {
      await ToSignal(this, SignalName.Ready);
    }
    _characterStats = characterStats;
    _characterStats.Changed += OnCharacterStatsChanged;
    OnCharacterStatsChanged();
  }

  private void OnCharacterStatsChanged() {
    if (CharacterStats == null) {
      return;
    }
    ManaLabel.Text = $"{CharacterStats.Mana}/{CharacterStats.MaxMana}";
  }
}
