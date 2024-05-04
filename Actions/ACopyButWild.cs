using System.Collections.Generic;
using Nickel;
using TheJazMaster.TyAndSasha.Features;

#nullable enable
namespace TheJazMaster.TyAndSasha.Actions;

public class ACopyButWild : CardAction
{
	public override Route? BeginWithRoute(G g, State s, Combat c)
	{
		if (selectedCard != null)
		{
			Card card = selectedCard.CopyWithNewId();

			ModEntry.Instance.Helper.Content.Cards.SetCardTraitOverride(s, card, WildManager.WildTrait, true, true);

			s.deck.Add(card);
            return new CustomShowCards
			{
				message = ModEntry.Instance.Localizations.Localize(["action", "copyButWild", "showCardText"]),
				cardIds = [card.uuid]
			};
		}
        return null;
	}	

	public override string? GetCardSelectText(State s)
	{
		return ModEntry.Instance.Localizations.Localize(["action", "copyButWild", "cardSelectText"]);
	}
}