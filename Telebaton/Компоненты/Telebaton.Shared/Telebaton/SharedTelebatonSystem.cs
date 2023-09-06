using Content.Server.Telebaton.Components;
using Content.Shared.Damage;
using Content.Shared.Weapons.Melee.Events;

namespace Content.Shared.Telebaton;

public abstract class SharedTelebatonSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TelebatonComponent, GetMeleeDamageEvent>(OnGetMeleeDamage);
    }

    private void OnGetMeleeDamage(EntityUid uid, TelebatonComponent component, ref GetMeleeDamageEvent args)
    {
        if (!component.Activated)
            return;

        args.Damage = new DamageSpecifier();
    }
}

