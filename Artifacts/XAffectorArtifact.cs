using System.Collections.Generic;

namespace TheJazMaster.TyAndSasha.Artifacts;

public interface IXAffectorArtifact
{
	int AffectX(Card card, List<CardAction> actions, State s, Combat c, int xBonus);
}