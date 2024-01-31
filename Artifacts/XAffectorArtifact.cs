using System.Collections.Generic;

namespace TheJazMaster.TyAndSasha.Artifacts;

public interface XAffectorArtifact
{
	int AffectX(Card card, List<CardAction> actions, State s, Combat c, int xBonus);
}