using System.Collections.Generic;
using TheJazMaster.TyAndSasha.Features;

#nullable enable
namespace TheJazMaster.TyAndSasha;

public sealed class ApiImplementation : ITyAndSashaApi
{
	ModEntry Instance = ModEntry.Instance;

	public int GetXBonus(Card card, List<CardAction> actions, State s, Combat c) => XAffectorManager.GetXBonus(card, actions, s, c);

	public void SetWild(Card card, bool? @override, bool? permanent) => Instance.WildManager.SetWild(card, @override, permanent);
	public bool IsWild(Card card, State s, Combat c) => Instance.WildManager.IsWild(card, s, c);
	public int CountWildsInHand(State s, Combat c) => Instance.WildManager.CountWildsInHand(s, c);

	public Deck TyDeck => Instance.TyDeck.Deck;
	public Status PredationStatus => Instance.PredationStatus.Status;
	public Status XFactorStatus => Instance.XFactorStatus.Status;
	public Status ExtremeMeasuresStatus => Instance.ExtremeMeasuresStatus.Status;
}
