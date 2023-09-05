using Content.Shared.Telebaton;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Content.Shared.Damage;

namespace Content.Server.Telebaton.Components;

[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
[Access(typeof(SharedTelebatonSystem))]
public sealed partial class TelebatonComponent : Component
{
    [DataField("activated"), ViewVariables(VVAccess.ReadWrite)]
    [AutoNetworkedField]
    public bool Activated = false;

    [DataField("TurnOnSound")]
    public SoundSpecifier OnSound = new SoundPathSpecifier("/Audio/Machines/button.ogg");

}
