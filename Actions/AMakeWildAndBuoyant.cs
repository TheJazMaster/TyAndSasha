using System.Collections.Generic;
using System.Linq;
using Nickel;
using TheJazMaster.TyAndSasha.Features;

#nullable enable
namespace TheJazMaster.TyAndSasha.Actions;

public class AMakeWildAndBuoyant : CardAction
{
    public bool permanent;
    public bool showCard;

	public override Route? BeginWithRoute(G g, State s, Combat c)
	{
        if (selectedCard != null) {
            ModEntry.Instance.Helper.Content.Cards.SetCardTraitOverride(s, selectedCard, WildManager.WildTrait, true, permanent);
            selectedCard.buoyantOverride = true;
            if (permanent)
                selectedCard.buoyantOverrideIsPermanent = permanent;

            return showCard ? new CustomShowCards
			{
				message = ModEntry.Instance.Localizations.Localize(["action", "makeWildAndBuoyant", "showCardText"]),
				cardIds = [selectedCard.uuid]
			} : null;
        }
        return null;
	}
	

    public override Icon? GetIcon(State s) {
        return new Icon(ModEntry.Instance.WildIcon.Sprite, null, Colors.textMain);
    }

    public string GetDuration()
    {
        return permanent ? Loc.T("actionShared.durationForever") : Loc.T("actionShared.durationCombat");
    }

	public override List<Tooltip> GetTooltips(State s)
	{
        return [
            new GlossaryTooltip($"action.{GetType().Namespace!}::MakeWildAndBuoyant") {
                Icon = ModEntry.Instance.WildIcon.Sprite,
                TitleColor = Colors.action,
                Title = ModEntry.Instance.Localizations.Localize(["action", "makeWildAndBuoyant", "name"]),
                Description = ModEntry.Instance.Localizations.Localize(["action", "makeWildAndBuoyant", "description"], new { Duration = GetDuration() })
			},
            .. WildManager.WildTrait.Configuration.Tooltips!(s, selectedCard),
        ];
	}

	public override string? GetCardSelectText(State s)
	{
		return ModEntry.Instance.Localizations.Localize(["action", "makeWildAndBuoyant", "cardSelectText", permanent ? "forever" : "temp"]);
	}
}