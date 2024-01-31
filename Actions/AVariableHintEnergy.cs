using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace TheJazMaster.TyAndSasha.Actions;

public class AVariableHintEnergy : AVariableHint
{
	public int setAmount = 0;

	public AVariableHintEnergy() : base()
	{
		hand = true;
	}

	public override Icon? GetIcon(State s)
	{
		return new Icon(ModEntry.Instance.EnergyIcon.Sprite, null, Colors.textMain);
	}

	public override List<Tooltip> GetTooltips(State s)
	{
		List<Tooltip> list = [];
		string parentheses = "";
		if (s.route is Combat && setAmount >= 0)
		{
			DefaultInterpolatedStringHandler stringHandler = new(22, 1);
			stringHandler.AppendLiteral(" </c>(<c=keyword>");
			stringHandler.AppendFormatted(setAmount);
			stringHandler.AppendLiteral("</c>)");
			
			parentheses = stringHandler.ToStringAndClear();
		}
		list.Add(new TTText(ModEntry.Instance.Localizations.Localize(["action", "variableHintEnergy", "description"], new { Amount = parentheses })));
		return list;
	}
}