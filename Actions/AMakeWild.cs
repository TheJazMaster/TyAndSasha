using System.Collections.Generic;
using Nickel;
using TheJazMaster.TyAndSasha.Features;

#nullable enable
namespace TheJazMaster.TyAndSasha.Actions;

public class AMakeWild : CardAction
{
    public bool permanent;
    public bool showCard;


	public override Route? BeginWithRoute(G g, State s, Combat c)
	{
        if (selectedCard != null) {
            ModEntry.Instance.Helper.Content.Cards.SetCardTraitOverride(s, selectedCard, WildManager.WildTrait, true, permanent);
            // ModEntry.Instance.WildManager.SetWild(selectedCard, true, permanent);
            
            return showCard ? new CustomShowCards
			{
				message = ModEntry.Instance.Localizations.Localize(["action", "makeWild", "showCardText"]),
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
            new GlossaryTooltip($"action.{GetType().Namespace!}::MakeWild") {
                Icon = ModEntry.Instance.WildIcon.Sprite,
                TitleColor = Colors.action,
                Title = ModEntry.Instance.Localizations.Localize(["action", "makeWild", "name"]),
                Description = ModEntry.Instance.Localizations.Localize(["action", "makeWild", "description"], new { Duration = GetDuration() })
			},
            .. WildManager.WildTrait.Configuration.Tooltips!(s, selectedCard),
            // new CustomTTGlossary(
            //     CustomTTGlossary.GlossaryType.cardtrait,
            //     () => ModEntry.Instance.WildIcon.Sprite,
            //     () => ModEntry.Instance.Localizations.Localize(["trait", "wild", "name"]),
            //     () => ModEntry.Instance.Localizations.Localize(["trait", "wild", "description"]),
			// 	key: "trait.wild"
            // )
        ];
	}

	public override string? GetCardSelectText(State s)
	{
		return ModEntry.Instance.Localizations.Localize(["action", "makeWild", "cardSelectText", permanent ? "forever" : "temp"]);
	}
}