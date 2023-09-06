using Content.Server.Power.EntitySystems;
using Content.Server.Telebaton.Components;
using Content.Shared.Audio;
using Content.Shared.Damage.Events;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Telebaton;
using Content.Shared.Toggleable;
using Robust.Shared.Audio;
using Robust.Shared.Player;
using Content.Shared.Examine;


namespace Content.Server.Telebaton;
public sealed class TelebatonSystem : SharedTelebatonSystem
    {
        [Dependency] private readonly SharedItemSystem _item = default!;
        [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
        [Dependency] private readonly RiggableSystem _riggableSystem = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<TelebatonComponent, UseInHandEvent>(OnUseInHand);
            SubscribeLocalEvent<TelebatonComponent, ExaminedEvent>(OnExamined);
            SubscribeLocalEvent<TelebatonComponent, StaminaDamageOnHitAttemptEvent>(OnStaminaHitAttempt);

        }

        private void OnStaminaHitAttempt(EntityUid uid, TelebatonComponent component, ref StaminaDamageOnHitAttemptEvent args)
        {
            if (!component.Activated)
            {
                args.Cancelled = true;
                return;
            }
        }

        private void OnUseInHand(EntityUid uid, TelebatonComponent comp, UseInHandEvent args)
        {
            if (comp.Activated & TryComp(uid, out ItemComponent? item))
            {
                TurnOff(uid, comp);
                _item.SetSize(uid, 10, item);
            }
            else
            {
                TurnOn(uid, comp, args.User);
                _item.SetSize(uid, 50, item);
            }
        }

        private void OnExamined(EntityUid uid, TelebatonComponent comp, ExaminedEvent args)
        {
            var msg = comp.Activated
                ? Loc.GetString("comp-telebaton-examined-on")
                : Loc.GetString("comp-telebaton-examined-off");
            args.PushMarkup(msg);
        }

        private void TurnOff(EntityUid uid, TelebatonComponent comp)
        {
            if (!comp.Activated)
                return;

            if (TryComp<AppearanceComponent>(comp.Owner, out var appearance) &&
                TryComp<ItemComponent>(comp.Owner, out var item))
            {
                _item.SetHeldPrefix(comp.Owner, "off", item);
                _appearance.SetData(uid, ToggleVisuals.Toggled, false, appearance);
            }

            SoundSystem.Play(comp.OnSound.GetSound(), Filter.Pvs(comp.Owner), comp.Owner, AudioHelpers.WithVariation(0.25f));

            comp.Activated = false;
            Dirty(comp);
        }

        private void TurnOn(EntityUid uid, TelebatonComponent comp, EntityUid user)
        {

            if (comp.Activated)
                return;

            var playerFilter = Filter.Pvs(comp.Owner, entityManager: EntityManager);

            if (EntityManager.TryGetComponent<AppearanceComponent>(comp.Owner, out var appearance) &&
                EntityManager.TryGetComponent<ItemComponent>(comp.Owner, out var item))
            {
                _item.SetHeldPrefix(comp.Owner, "on", item);
                _appearance.SetData(uid, ToggleVisuals.Toggled, true, appearance);
            }

            SoundSystem.Play(comp.OnSound.GetSound(), playerFilter, comp.Owner, AudioHelpers.WithVariation(0.25f));
            comp.Activated = true;
            Dirty(comp);
        }

    }

