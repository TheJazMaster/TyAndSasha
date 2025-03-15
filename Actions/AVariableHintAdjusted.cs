using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Nickel;
using TheJazMaster.TyAndSasha.Features;

#nullable disable
namespace TheJazMaster.TyAndSasha.Actions;

public class AVariableHintAdjusted : AVariableHint
{
	internal static IModData ModData => ModEntry.Instance.Helper.ModData;
	public int displayAdjustment = 0;

	public override List<Tooltip> GetTooltips(State s)
	{
		if (hand)
		{
			TTGlossary item = (s.route is Combat) ? new TTGlossary("action.xHintHand.desc", handAmount) : new TTGlossary("action.xHintHand.desc.alt");
			return new List<Tooltip> { item };
		}
		if (status.HasValue)
		{
			List<Tooltip> list = new List<Tooltip>();
			object[] obj = new object[4]
			{
				"<c=status>" + status.Value.GetLocName().ToUpperInvariant() + "</c>",
				null,
				null,
				null
			};
			object obj2;
			if (!(s.route is Combat))
			{
				obj2 = "";
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new	(22, 1);
				defaultInterpolatedStringHandler.AppendLiteral(" </c>(<c=keyword>");
				defaultInterpolatedStringHandler.AppendFormatted(s.ship.Get(status.Value) + displayAdjustment);
				defaultInterpolatedStringHandler.AppendLiteral("</c>)");
				obj2 = defaultInterpolatedStringHandler.ToStringAndClear();

				ModData.TryGetModData(this, XAffectorManager.IncreasedHintsKey, out int value);
				ModData.TryGetModData(this, XAffectorManager.InnateIncreasedHintsKey, out int innate);
				if (innate + value != 0)
					obj2 += "<c=keyword> + " + (value + innate) + "</c>";
			}
			obj[1] = obj2;
			obj[2] = secondStatus.HasValue ? (" </c>+ <c=status>" + secondStatus.Value.GetLocName().ToUpperInvariant() + "</c>") : "";
			object obj3;
			if (!secondStatus.HasValue || s.route is not Combat)
			{
				obj3 = "";
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new(22, 1);
				defaultInterpolatedStringHandler.AppendLiteral(" </c>(<c=keyword>");
				defaultInterpolatedStringHandler.AppendFormatted(s.ship.Get(secondStatus.Value));
				defaultInterpolatedStringHandler.AppendLiteral("</c>)");
				obj3 = defaultInterpolatedStringHandler.ToStringAndClear();
			}
			obj[3] = obj3;
			list.Add(new TTGlossary("action.xHint.desc", obj));

			return list;
		}

		return new List<Tooltip>();
	}
}