using System.Collections.Generic;

#nullable enable
namespace TheJazMaster.TyAndSasha;

public interface ITyAndSashaApi
{
	int GetXBonus(Card card, List<CardAction> actions, State s, Combat c);

	void SetWild(Card card, bool? @override, bool? permanent);
	bool IsWild(Card card, State s, Combat c);
	int CountWildsInHand(State s, Combat c);

	Deck TyDeck { get; }
	Status PredationStatus { get; }
	Status XFactorStatus { get; }
	Status ExtremeMeasuresStatus { get; }
}