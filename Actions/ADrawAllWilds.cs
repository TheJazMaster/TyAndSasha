using System;
using System.Collections.Generic;
using System.Linq;
using FSPRO;
using TheJazMaster.TyAndSasha.Features;

namespace TheJazMaster.TyAndSasha.Actions
{
    public class ADrawAllWilds : CardAction
    {

        public override void Begin(G g, State s, Combat c)
	    {
            int num = 0;
            int i = s.deck.Count - 1;
            while (i >= 0)
            {
                Card card = s.deck[i];
                if (WildManager.IsWild(card, s)) {
                    if (c.hand.Count >= 10)
                    {
                        c.PulseFullHandWarning();
                        return;
                    }
                    num++;
                    c.DrawCardIdx(s, i, CardDestination.Deck);
                }
                i--;
            }
            if (num == 0)
            {
                timer = 0.0;
                return;
            }
            foreach (Artifact item in s.EnumerateAllArtifacts())
            {
                item.OnDrawCard(s, c, num);
            }
            Audio.Play(Event.CardHandling);
        }
        public override Icon? GetIcon(State s) => new Icon(StableSpr.icons_drawCard, null, Colors.textMain);

        public override List<Tooltip> GetTooltips(State s) => WildManager.WildTrait.Configuration.Tooltips(s, null).ToList();
    }
}