using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nickel;
using TheJazMaster.TyAndSasha.Actions;
using TheJazMaster.TyAndSasha.Features;

#nullable enable
namespace TheJazMaster.TyAndSasha.Artifacts;

internal sealed class AimingReticleArtifact : Artifact, ITyArtifact
{
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null) return;
		
		helper.Content.Artifacts.RegisterArtifact("AimingReticle", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/DuoArtifacts/AimingReticle.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "AimingReticle", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "AimingReticle", "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<AimingReticleArtifact>([Deck.peri, ModEntry.Instance.TyDeck.Deck]);
	}

	public override int ModifyBaseDamage(int baseDamage, Card? card, State state, Combat? combat, bool fromPlayer)
	{
		if (card != null && card.GetMeta().deck == Deck.peri) {
			return state.ship.Get(ModEntry.Instance.XFactorStatus.Status) + state.ship.Get(ModEntry.Instance.ExtremeMeasuresStatus.Status) - 1;
		}
		return 0;
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		.. StatusMeta.GetTooltips(ModEntry.Instance.XFactorStatus.Status, 1),
		.. StatusMeta.GetTooltips(ModEntry.Instance.ExtremeMeasuresStatus.Status, 1)
	];
}


internal sealed class SashaSnacksArtifact : Artifact, ITyArtifact
{
	internal static IModCards? Cards;
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null) return;
		Cards = ModEntry.Instance.Helper.Content.Cards;
		
		helper.Content.Artifacts.RegisterArtifact("SashaSnacks", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/DuoArtifacts/SashaSnacks.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "SashaSnacks", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "SashaSnacks", "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<SashaSnacksArtifact>([Deck.dizzy, ModEntry.Instance.TyDeck.Deck]);
	}

	public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
	{
		if (combat.turn == 1 && Cards!.IsCardTraitActive(state, card, WildManager.WildTrait)) {
			combat.Queue(new AStatus {
				status = Status.maxShield,
				statusAmount = 1,
				targetPlayer = true
			});
			combat.Queue(new AStatus {
				status = Status.shield,
				statusAmount = 1,
				targetPlayer = true
			});
		}
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		.. StatusMeta.GetTooltips(Status.maxShield, 1),
		.. StatusMeta.GetTooltips(Status.shield, 1),
		.. WildManager.WildTrait.Configuration.Tooltips!(DB.fakeState, null)
	];
}


internal sealed class PartyBalloonArtifact : Artifact, ITyArtifact
{
	internal static IModCards? Cards;
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null) return;
		Cards = ModEntry.Instance.Helper.Content.Cards;
		
		helper.Content.Artifacts.RegisterArtifact("PartyBalloon", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/DuoArtifacts/PartyBalloon.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "PartyBalloon", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "PartyBalloon", "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<PartyBalloonArtifact>([Deck.riggs, ModEntry.Instance.TyDeck.Deck]);
	}

	public static int GetNumber()
	{
		State s = MG.inst.g.state;
		Combat? c = (s.route is Combat combat) ? combat : null;
		if (c == null) return s.deck.Count(card => Cards!.IsCardTraitActive(s, card, Cards.BuoyantCardTrait));
		return s.deck.Concat(c.hand.Concat(c.exhausted.Concat(c.discard))).Count(card => Cards!.IsCardTraitActive(s, card, Cards.BuoyantCardTrait));
	}

	public override int? GetDisplayNumber(State s)
	{
		if (s.route is Combat) return null;
		return GetNumber();
	}

	public override void OnTurnStart(State state, Combat combat)
	{
		if (combat.turn == 1)
		{
			combat.QueueImmediate(new ADrawCard
			{
				count = GetNumber(),
				artifactPulse = Key()
			});
		}
	}


	public override List<Tooltip>? GetExtraTooltips() => [
		.. Cards?.BuoyantCardTrait.Configuration.Tooltips!(MG.inst.g.state, null)
	];
}


internal sealed class PetRockArtifact : Artifact, ITyArtifact, IXAffectorArtifact
{
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null) return;
		
		helper.Content.Artifacts.RegisterArtifact("PetRock", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/DuoArtifacts/PetRock.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "PetRock", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "PetRock", "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<PetRockArtifact>([Deck.goat, ModEntry.Instance.TyDeck.Deck]);
	}

	public override void OnCombatStart(State state, Combat combat)
	{
		List<int> list = new List<int>();
		for (int i = state.ship.x - 1; i < state.ship.x + state.ship.parts.Count() + 1; i++)
		{
			if (!combat.stuff.ContainsKey(i))
			{
				list.Add(i);
			}
		}
		List<int> list2 = list.Shuffle(state.rngActions).Take(1).ToList();
		foreach (int item in list2)
		{
			combat.stuff.Add(item, new Asteroid {
				targetPlayer = false,
				x = item,
				xLerped = item
			});
		}
		if (list2.Count > 0)
		{
			Pulse();
		}
	}

	public int AffectX(Card card, List<CardAction> actions, State s, Combat c, int xBonus)
	{
		int count = 0;
		for (int i = 0; i < s.ship.parts.Count; i++) {
			if (c.stuff.GetValueOrDefault(s.ship.x + i) is Asteroid) count++;
		}
		return count;
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		new TTGlossary("midrow.asteroid")
	];
}


internal sealed class PirateMapArtifact : Artifact, ITyArtifact
{
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null) return;
		
		helper.Content.Artifacts.RegisterArtifact("PirateMap", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/DuoArtifacts/PirateMap.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "PirateMap", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "PirateMap", "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<PirateMapArtifact>([Deck.eunice, ModEntry.Instance.TyDeck.Deck]);
	}

	public override void AfterPlayerOverheat(State state, Combat combat)
	{
		combat.Queue(new AStatus {
			status = ModEntry.Instance.ExtremeMeasuresStatus.Status,
			statusAmount = 1,
			targetPlayer = true,
			artifactPulse = Key()
		});
	}


	public override List<Tooltip>? GetExtraTooltips() => 
		StatusMeta.GetTooltips(ModEntry.Instance.ExtremeMeasuresStatus.Status, 1);
}


internal sealed class FunhouseMirrorArtifact : Artifact, ITyArtifact
{
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null) return;
		
		helper.Content.Artifacts.RegisterArtifact("FunhouseMirror", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/DuoArtifacts/FunhouseMirror.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "FunhouseMirror", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "FunhouseMirror", "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<FunhouseMirrorArtifact>([Deck.hacker, ModEntry.Instance.TyDeck.Deck]);
	}

	public override void OnReceiveArtifact(State state)
	{
		state.GetCurrentQueue().QueueImmediate(
			new ACardSelect
			{
				browseAction = new ACopyButWild(),
				browseSource = CardBrowse.Source.Deck
			}
		);
	}


	public override List<Tooltip>? GetExtraTooltips() => 
		WildManager.WildTrait.Configuration.Tooltips!(MG.inst.g.state, null).ToList();
}


internal sealed class DruidismArtifact : Artifact, ITyArtifact, IXAffectorArtifact
{
	internal static IModCards? Cards;
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null) return;
		Cards = ModEntry.Instance.Helper.Content.Cards;
		
		helper.Content.Artifacts.RegisterArtifact("Druidism", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/DuoArtifacts/Druidism.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "Druidism", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "Druidism", "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<DruidismArtifact>([Deck.shard, ModEntry.Instance.TyDeck.Deck]);
	}

	public int AffectX(Card card, List<CardAction> actions, State s, Combat c, int xBonus)
	{
		if (s.ship.Get(Status.shard) > 0 && Cards!.IsCardTraitActive(s, card, WildManager.WildTrait)) {
			return 2;
		}
		return 0;
	}

	public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
	{
		if (Cards!.IsCardTraitActive(state, card, WildManager.WildTrait)) {
			combat.Queue(new AStatus {
				status = Status.shard,
				statusAmount = -1,
				targetPlayer = true,
				artifactPulse = Key()
			});
		}
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		.. StatusMeta.GetTooltips(Status.shard, 1),
		.. WildManager.WildTrait.Configuration.Tooltips!(MG.inst.g.state, null)
	];
}


internal sealed class VirtualPetSimArtifact : Artifact, ITyArtifact
{
	internal static IModCards? Cards;
	public static void Register(IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi == null) return;
		Cards = ModEntry.Instance.Helper.Content.Cards;
		
		helper.Content.Artifacts.RegisterArtifact("VirtualPetSim", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/DuoArtifacts/VirtualPetSim.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "VirtualPetSim", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["duoArtifact", "VirtualPetSim", "description"]).Localize
		});
		ModEntry.Instance.DuoArtifactsApi.RegisterDuoArtifact<VirtualPetSimArtifact>([Deck.shard, ModEntry.Instance.TyDeck.Deck]);
	}

	public int AffectX(Card card, List<CardAction> actions, State s, Combat c, int xBonus)
	{
		if (Cards!.IsCardTraitActive(s, card, WildManager.WildTrait)) {
			return 2;
		}
		return 0;
	}

	public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
	{
		if (Cards!.IsCardTraitActive(state, card, WildManager.WildTrait)) {
			combat.Queue(new AStatus {
				status = Status.shard,
				statusAmount = -1,
				targetPlayer = true,
				artifactPulse = Key()
			});
		}
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		.. Cards!.TemporaryCardTrait.Configuration.Tooltips!(MG.inst.g.state, null),
		.. WildManager.WildTrait.Configuration.Tooltips!(MG.inst.g.state, null)
	];
}