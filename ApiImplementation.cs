using System;
using System.Collections.Generic;
using Nickel;
using TheJazMaster.TyAndSasha.Features;

#nullable enable
namespace TheJazMaster.TyAndSasha;

public sealed class ApiImplementation : ITyAndSashaApi
{
	ModEntry Instance = ModEntry.Instance;

	public int GetXBonus(Card card, List<CardAction> actions, State s, Combat c) => XAffectorManager.GetXBonus(card, actions, s, c);

	public int CountWildsInHand(State s, Combat c) => WildManager.CountWildsInHand(s, c);

	public void RegisterHook(ITyAndSashaApi.IHook hook, double priority = 0) {
		ModEntry.Instance.HookManager.Register(hook, priority);
	}

	public void UnregisterHook(ITyAndSashaApi.IHook hook) {
		ModEntry.Instance.HookManager.Unregister(hook);
	}

	public Deck TyDeck => Instance.TyDeck.Deck;
	public Status PredationStatus => Instance.PredationStatus.Status;
	public Status XFactorStatus => Instance.XFactorStatus.Status;
	public Status ExtremeMeasuresStatus => Instance.ExtremeMeasuresStatus.Status;
	
	public ICardTraitEntry WildTrait => WildManager.WildTrait;
}
