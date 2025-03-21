using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Nickel;
using Nickel.Models.Content;
using TheJazMaster.TyAndSasha.Actions;
using TheJazMaster.TyAndSasha.Cards;
using TheJazMaster.TyAndSasha.Features;

#nullable enable
namespace TheJazMaster.TyAndSasha.Artifacts;

internal sealed class LycanthropyArtifact : Artifact, ITyArtifact
{
	public static void Register(IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("Lycanthropy", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.TyDeck.Deck,
				pools = [ArtifactPool.Common]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/Lycanthropy.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Lycanthropy", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Lycanthropy", "description"]).Localize
		});
	}

	public override void OnReceiveArtifact(State state)
	{
		state.GetCurrentQueue().QueueImmediate(
			new ACardSelect
			{
				browseAction = new AMakeWild {
					permanent = true,
					showCard = true
				},
				browseSource = CardBrowse.Source.Deck
			}.ApplyModData(CardBrowseFilterManager.FilterWildKey, false)
		);
		state.GetCurrentQueue().QueueImmediate(
			new ACardSelect
			{
				browseAction = new AMakeWild {
					permanent = true,
					showCard = true
				},
				browseSource = CardBrowse.Source.Deck
			}.ApplyModData(CardBrowseFilterManager.FilterWildKey, false)
		);
	}

	public override List<Tooltip>? GetExtraTooltips()
	{
		return WildManager.WildTrait.Configuration.Tooltips!(DB.fakeState, null).ToList();
	}
}


internal sealed class NaturalTalentArtifact : Artifact, ITyArtifact
{
	public static void Register(IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("NaturalTalent", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.TyDeck.Deck,
				pools = [ArtifactPool.Common]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/NaturalTalent.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "NaturalTalent", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "NaturalTalent", "description"]).Localize
		});
	}

	public override void OnReceiveArtifact(State state)
	{
		state.GetCurrentQueue().QueueImmediate(
			new ACardSelect {
				browseAction = new CardSelectAddBuoyantForever(),
				browseSource = CardBrowse.Source.Deck,
				filterBuoyant = false
			}
		);
	}

	public override List<Tooltip>? GetExtraTooltips()
	{
		return WildManager.WildTrait.Configuration.Tooltips!(DB.fakeState, null).Append(new TTGlossary("cardtrait.buoyant")).ToList();
	}
}


internal sealed class ChewToyArtifact : Artifact, ITyArtifact
{
	public static void Register(IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("ChewToy", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.TyDeck.Deck,
				pools = [ArtifactPool.Common]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/ChewToy.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "ChewToy", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "ChewToy", "description"]).Localize
		});
	}

	public override void OnTurnStart(State state, Combat combat)
	{
		if (combat.turn == 1)
		{
			combat.QueueImmediate(new AStatus
			{
				status = ModEntry.Instance.XFactorStatus.Status,
				statusAmount = 2,
				targetPlayer = true,
				artifactPulse = Key()
			});
		}
	}

	public override List<Tooltip>? GetExtraTooltips()
	{
		return StatusMeta.GetTooltips(ModEntry.Instance.XFactorStatus.Status, 2);
	}
}


internal sealed class BabyStarnacleArtifact : Artifact, ITyArtifact
{
	public bool active = false;

	public static void Register(IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("BabyStarnacle", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.TyDeck.Deck,
				pools = [ArtifactPool.Common]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/BabyStarnacle.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "BabyStarnacle", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "BabyStarnacle", "description"]).Localize
		});
	}

	public override void OnCombatStart(State state, Combat c)
	{
		if (!active && state.map.markers[state.map.currentLocation].contents is MapBattle mapBattle)
		{
			if (mapBattle.battleType == BattleType.Boss || mapBattle.battleType == BattleType.Elite)
			{
				active = true;
				state.ship.baseDraw += 1;
			} else {
				active = false;
			}
		}
	}

	public override void OnTurnStart(State state, Combat combat)
	{
		if (active && combat.turn == 1)
			Pulse();
	}

	public override void OnCombatEnd(State state)
	{
		if (active) {
			active = false;
			state.ship.baseDraw -= 1;
		}
	}

	public override void OnRemoveArtifact(State state)
	{
		if (active) {
			state.ship.baseDraw -= 1;
			active = false;
		}
	}
}


internal sealed class ScratchingPostArtifact : Artifact, ITyArtifact
{
	public bool turnedOn = true;

	static Spr ActiveSprite;
	static Spr InactiveSprite;

	public static void Register(IModHelper helper)
	{
		ActiveSprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/ScratchingPost.png")).Sprite;
		InactiveSprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/ScratchingPostInactive.png")).Sprite;
		helper.Content.Artifacts.RegisterArtifact("ScratchingPost", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.TyDeck.Deck,
				pools = [ArtifactPool.Common]
			},
			Sprite = ActiveSprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "ScratchingPost", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "ScratchingPost", "description"]).Localize
		});
	}

	public override Spr GetSprite()
	{
		return turnedOn ? ActiveSprite : InactiveSprite;
	}

	public override void OnTurnStart(State state, Combat combat)
	{
		turnedOn = true;
	}

	public override void OnCombatStart(State state, Combat combat)
	{
		turnedOn = true;
	}

	public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
	{
		if (WildManager.IsWild(card, state)) {
			turnedOn = false;
			Pulse();
		}
	}

	public override int ModifyBaseDamage(int baseDamage, Card? card, State state, Combat? combat, bool fromPlayer)
	{
		if (!fromPlayer || card == null || !turnedOn)
		{
			return 0;
		}
		if (WildManager.IsWild(card, state))
		{
			return 1;
		}
		return 0;
	}

	public override List<Tooltip>? GetExtraTooltips()
	{
		return WildManager.WildTrait.Configuration.Tooltips!(DB.fakeState, null).ToList();
	}
}


internal sealed class GenomeSplicingArtifact : Artifact, ITyArtifact, ITyAndSashaApi.IHook
{
	public static void Register(IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("GenomeSplicing", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.TyDeck.Deck,
				pools = [ArtifactPool.Boss]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Artifacts/GenomeSplicing.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "GenomeSplicing", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "GenomeSplicing", "description"]).Localize
		});
	}

	public int AffectX(ITyAndSashaApi.IHook.IAffectXArgs args)
	{
		foreach(CardAction action in args.Actions) {
			if (action is AVariableHint && action is not AVariableHintWild) {
				return WildManager.CountWildsInHand(args.State, args.Combat);
			}
		}
		return 0;
	}

	public override List<Tooltip>? GetExtraTooltips()
	{
		return WildManager.WildTrait.Configuration.Tooltips!(DB.fakeState, null).ToList();
	}
}