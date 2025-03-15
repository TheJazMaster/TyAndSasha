using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TheJazMaster.TyAndSasha.Features;

namespace TheJazMaster.TyAndSasha.Actions;

public class AVariableHintWild : AVariableHint
{   
    public int displayAdjustment = 0;

    public bool useRegularHandSprite = false;
    public bool alsoHand;
    public int handCount;

    public AVariableHintWild() : base() {
        hand = true;
    }

    public override Icon? GetIcon(State s) {
        return new Icon(useRegularHandSprite ? StableSpr.icons_dest_hand : ModEntry.Instance.WildHandIcon.Sprite, null, Colors.textMain);
    }

	public override List<Tooltip> GetTooltips(State s)
	{
		List<Tooltip> list = [];
        string parentheses = "";
        if (s.route is Combat c)
        {
            var amt = WildManager.CountWildsInHand(s, c) + (alsoHand ? handCount : 0);
            DefaultInterpolatedStringHandler stringHandler = new(22, 1);
            stringHandler.AppendLiteral(" </c>(<c=keyword>");
            stringHandler.AppendFormatted(amt + displayAdjustment);
            stringHandler.AppendLiteral("</c>)");
            
            parentheses = stringHandler.ToStringAndClear();
        }
        if (alsoHand)
            list.Add(new TTText(ModEntry.Instance.Localizations.Localize(["action", "variableHintWildAndHand", "description"], new { Amount = parentheses })));
        else 
            list.Add(new TTText(ModEntry.Instance.Localizations.Localize(["action", "variableHintWild", "description"], new { Amount = parentheses })));            
        return list;
	}
}