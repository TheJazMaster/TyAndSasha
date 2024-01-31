using System;
using System.Collections.Generic;

namespace TheJazMaster.TyAndSasha.Actions
{
    public class AStatusAdjusted : AStatus
    {
        public int amountAdjustment = 0;
        public int amountDisplayAdjustment = 0;

        public override void Begin(G g, State s, Combat c)
	    {
            var oldStatusAmount = statusAmount;
            statusAmount = Math.Max(0, statusAmount + amountAdjustment);
            base.Begin(g, s, c);
            statusAmount = oldStatusAmount;
        }
        public override Icon? GetIcon(State s)
        {
            var oldStatusAmount = statusAmount;
            statusAmount = Math.Max(0, statusAmount + amountDisplayAdjustment);
            Icon? icon = base.GetIcon(s);
            statusAmount = oldStatusAmount;
            return icon;
        }

        public override List<Tooltip> GetTooltips(State s)
        {
            var oldStatusAmount = statusAmount;
            statusAmount = Math.Max(0, statusAmount + amountDisplayAdjustment);
            List<Tooltip> tooltips = base.GetTooltips(s);
            statusAmount = oldStatusAmount;
            return tooltips;
        }
    }
}