using Nickel;
using TheJazMaster.TyAndSasha.Actions;
using System.Collections.Generic;
using System.Reflection;
using TheJazMaster.TyAndSasha.Features;

namespace TheJazMaster.TyAndSasha.Cards;


internal sealed class TreatCard : Card, ITyCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Treat", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.TyDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Treat.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Treat", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 2 : 1,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch
	{
		Upgrade.A => [
			new AStatus
			{
				status = Status.shield,
				statusAmount = 2,
				targetPlayer = true
			},
			new AStatus
			{
				status = ModEntry.Instance.XFactorStatus.Status,
				statusAmount = 1,
				targetPlayer = true
			}
		],
		Upgrade.B => [
			new AStatus
			{
				status = Status.shield,
				statusAmount = 1,
				targetPlayer = true
			},
			new AStatus
			{
				status = Status.tempShield,
				statusAmount = 1,
				targetPlayer = true
			},
			new AStatus
			{
				status = ModEntry.Instance.XFactorStatus.Status,
				statusAmount = 2,
				targetPlayer = true
			}
		],
		_ => [
			new AStatus
			{
				status = Status.shield,
				statusAmount = 1,
				targetPlayer = true
			},
			new AStatus
			{
				status = ModEntry.Instance.XFactorStatus.Status,
				statusAmount = 1,
				targetPlayer = true
			}
		],
	};
}


internal sealed class BiteCard : Card, IHasCustomCardTraits, ITyCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Bite", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.TyDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Bite.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Bite", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		artTint = "ffffff"
	};
	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => new HashSet<ICardTraitEntry>() { WildManager.WildTrait };

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch
	{
		Upgrade.A => [
			new AVariableHintWild(),
			new AAttack {
				damage = GetDmg(s, WildManager.CountWildsInHand(s, c)),
				xHint = 1
			},
			new AStatus
			{
				status = ModEntry.Instance.XFactorStatus.Status,
				statusAmount = 1,
				targetPlayer = true
			}
		],
		Upgrade.B => [
			new AVariableHintWild(),
			new AAttack {
				damage = GetDmg(s, WildManager.CountWildsInHand(s, c)),
				piercing = true,
				xHint = 1
			}
		],
		_ => [
			new AVariableHintWild(),
			new AAttack {
				damage = GetDmg(s, WildManager.CountWildsInHand(s, c)),
				xHint = 1
			}
		]
	};
}

internal sealed class ScratchCard : Card, IHasCustomCardTraits, ITyCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Scratch", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.TyDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Scratch.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Scratch", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 3 : 2,
		artTint = "ffffff"
	};

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => new HashSet<ICardTraitEntry>() { WildManager.WildTrait };

	public override List<CardAction> GetActions(State s, Combat c)
	{
		var dmg = WildManager.CountWildsInHand(s, c);
		return upgrade switch
		{
			Upgrade.A => [
				new AVariableHintWild(),
				new AAttack {
					damage = GetDmg(s, dmg),
					xHint = 1
				},
				new AAttack {
					damage = GetDmg(s, dmg),
					xHint = 1
				},
				new AAttack {
					damage = GetDmg(s, dmg),
					xHint = 1
				},
			],
			Upgrade.B => [
				new AVariableHintWild(),
				new AAttack {
					damage = GetDmg(s, dmg),
					xHint = 1
				},
				new AAttack {
					damage = GetDmg(s, dmg),
					xHint = 1
				},
				new AAttack {
					damage = GetDmg(s, dmg),
					xHint = 1
				},
				new AAttack {
					damage = GetDmg(s, dmg),
					xHint = 1
				},
			],
			_ => [
				new AVariableHintWild(),
				new AAttack {
					damage = GetDmg(s, dmg),
					xHint = 1
				},
				new AAttack {
					damage = GetDmg(s, dmg),
					xHint = 1
				},
			]
		};
	}
}

internal sealed class ScurryCard : Card, IHasCustomCardTraits, ITyCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Scurry", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.TyDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Scurry.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Scurry", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 0 : 2,
		exhaust = upgrade == Upgrade.B,
		artTint = "ffffff"
	};

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => new HashSet<ICardTraitEntry>() { WildManager.WildTrait };

	public override List<CardAction> GetActions(State s, Combat c)
	{
		var amt = WildManager.CountWildsInHand(s, c);
		return upgrade switch
		{
			Upgrade.A => [
				new AVariableHintWild(),
				new AStatus {
					status = Status.evade,
					statusAmount = amt,
					targetPlayer = true,
					xHint = 1
				},
				new AStatus {
					status = Status.evade,
					statusAmount = 1,
					targetPlayer = true
				},
			],
			_ => [
				new AVariableHintWild(),
				new AStatus {
					status = Status.evade,
					statusAmount = amt,
					targetPlayer = true,
					xHint = 1
				}
			]
		};
	}
}


internal sealed class HugsCard : Card, ITyCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Hugs", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.TyDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Hugs.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Hugs", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		retain = upgrade == Upgrade.A,
		exhaust = upgrade != Upgrade.B,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus
		{
			status = ModEntry.Instance.XFactorStatus.Status,
			statusAmount = 2,
			targetPlayer = true
		}
	];
}

internal sealed class ZoomiesCard : Card, IHasCustomCardTraits, ITyCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Zoomies", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.TyDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Zoomies.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Zoomies", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 0 : 1,
		flippable = true,
		artTint = "ffffff"
	};

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => new HashSet<ICardTraitEntry>() { WildManager.WildTrait };

	public override List<CardAction> GetActions(State s, Combat c)
	{
		var amt = WildManager.CountWildsInHand(s, c);
		return upgrade switch
		{
			Upgrade.B => [
				new AVariableHintWild(),
				new AMove {
					dir = amt,
					targetPlayer = true,
					xHint = 1
				},
				new AStatus {
					status = Status.evade,
					statusAmount = 1,
					targetPlayer = true
				},
			],
			_ => [
				new AVariableHintWild(),
				new AMove {
					dir = amt,
					targetPlayer = true,
					xHint = 1
				},
			]
		};
	}
}

internal sealed class PotShotCard : Card, IHasCustomCardTraits, ITyCard
{
	private static Spr CardArt;
	private static Spr CardArtA;
	public static void Register(IModHelper helper) {
		CardArt = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/PotShot.png")).Sprite;
		CardArtA = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/PotShotA.png")).Sprite;

		helper.Content.Cards.RegisterCard("PotShot", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.TyDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "PotShot", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 0,
		exhaust = true,
		art = upgrade == Upgrade.A ? CardArtA : CardArt,
		buoyant = true,
		artTint = "ffffff"
	};

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => upgrade switch {
		Upgrade.A => [WildManager.WildTrait],
		_ => new HashSet<ICardTraitEntry>()
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch
	{
		Upgrade.B => [
			new AAttack {
				damage = GetDmg(s, 4)
			},
			new ADrawCard {
				count = 1
			}
		],
		_ => [
			new AAttack {
				damage = GetDmg(s, upgrade == Upgrade.A ? 5 : 4)
			},
		]
	};
}

internal sealed class StackCard : Card, ITyCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Stack", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.TyDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Stack.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Stack", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 0 : 1,
		artTint = "ffffff"
	};

	public static int GetX(State s)
	{
		return s.ship.Get(Status.shield);
	}

	public override List<CardAction> GetActions(State s, Combat c)
	{
		return upgrade switch
		{
			Upgrade.B => [
				new AStatus {
					status = Status.shield,
					statusAmount = 1,
					targetPlayer = true
				},
				new AVariableHint {
					status = Status.shield
				},
				new AStatus {
					status = Status.tempShield,
					statusAmount = GetX(s) + 1,
					targetPlayer = true,
					xHint = 1
				},
			],
			_ => [
				new AVariableHint {
					status = Status.shield
				},
				new AStatus {
					status = Status.tempShield,
					statusAmount = GetX(s),
					targetPlayer = true,
					xHint = 1
				},
			]
		};
	}
}

internal sealed class PatOnTheBackCard : Card, ITyCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("PatOnTheBack", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.TyDeck.Deck,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/PatOnTheBack.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "PatOnTheBack", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 0 : 1,
		exhaust = upgrade == Upgrade.B,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => [
		new AStatus {
			status = ModEntry.Instance.XFactorStatus.Status,
			statusAmount = upgrade == Upgrade.B ? 2 : 1,
			targetPlayer = true
		},
		new ADrawCard {
			count = upgrade == Upgrade.B ? 1 : upgrade == Upgrade.A ? 3 : 2
		}
	];
}


internal sealed class CrossAttackCard : Card, IHasCustomCardTraits, ITyCard
{
	private static Spr CardArt;
	private static Spr CardArtB;
	public static void Register(IModHelper helper) {
		CardArt = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/CrossAttack.png")).Sprite;
		CardArtB = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/CrossAttackB.png")).Sprite;

		helper.Content.Cards.RegisterCard("CrossAttack", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.TyDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "CrossAttack", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		art = upgrade == Upgrade.B ? CardArtB : CardArt,
		artTint = "ffffff"
	};

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => upgrade switch {
		Upgrade.B => [WildManager.WildTrait],
		_ => new HashSet<ICardTraitEntry>()
	};

	public override List<CardAction> GetActions(State s, Combat c)
	{
		return upgrade switch
		{
			Upgrade.A => [
				new AVariableHintNumber {
					number = 1
				},
				new AAttack {
					damage = GetDmg(s, 1),
					xHint = 1
				},
				new AAttack {
					damage = GetDmg(s, 1),
					xHint = 1
				},
				new AAttack {
					damage = GetDmg(s, 1),
					xHint = 1
				},
			],
			Upgrade.B => [
				new AVariableHintWild(),
				new AAttack {
					damage = GetDmg(s, WildManager.CountWildsInHand(s, c)),
					xHint = 1
				},
				new AAttack {
					damage = GetDmg(s, WildManager.CountWildsInHand(s, c)),
					xHint = 1
				},
			],
			_ => [
				new AVariableHintNumber {
					number = 1
				},
				new AAttack {
					damage = GetDmg(s, 1),
					xHint = 1
				},
				new AAttack {
					damage = GetDmg(s, 1),
					xHint = 1
				},
			]
		};
	}
}


internal sealed class InstinctsCard : Card, IHasCustomCardTraits, ITyCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Instincts", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.TyDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Instincts.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Instincts", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 1 : 2,
		retain = true,
		buoyant = upgrade == Upgrade.B,
		artTint = "ffffff"
	};

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => new HashSet<ICardTraitEntry>() { WildManager.WildTrait };

	public override List<CardAction> GetActions(State s, Combat c) => 
	[
		new AVariableHintWild(),
		new AStatus {
			status = Status.tempShield,
			statusAmount = WildManager.CountWildsInHand(s, c),
			targetPlayer = true,
			xHint = 1
		},
	];
}


internal sealed class AdrenalineCard : Card, IHasCustomCardTraits, ITyCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Adrenaline", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.TyDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Adrenaline.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Adrenaline", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 2 : upgrade == Upgrade.A ? 0 : 1,
		exhaust = upgrade != Upgrade.B,
		artTint = "ffffff"
	};

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => new HashSet<ICardTraitEntry>() { WildManager.WildTrait };

	public override List<CardAction> GetActions(State s, Combat c) => 
	[
		new AVariableHintWild(),
		new AEnergy {
			changeAmount = WildManager.CountWildsInHand(s, c),
			xHint = 1
		},
	];
}


internal sealed class PounceCard : Card, IHasCustomCardTraits, ITyCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Pounce", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.TyDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Pounce.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Pounce", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		exhaust = true,
		buoyant = true,
		artTint = "ffffff"
	};

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => new HashSet<ICardTraitEntry>() { WildManager.WildTrait };

	public override List<CardAction> GetActions(State s, Combat c) {
		var amt = WildManager.CountWildsInHand(s, c);
		return upgrade switch {
			Upgrade.A => [
				new AVariableHintWild(),
				new AAttack {
					damage = GetDmg(s, 2*amt),
					xHint = 2
				},
				new AStatus {
					status = Status.tempShield,
					statusAmount = amt,
					targetPlayer = true,
					xHint = 1
				}
			],
			Upgrade.B => [
				new AVariableHintWild(),
				new AAttack {
					damage = GetDmg(s, amt),
					xHint = 1
				},
				new AStatus {
					status = Status.tempShield,
					statusAmount = amt,
					targetPlayer = true,
					xHint = 1
				},
				new AStatus {
					status = Status.tempShield,
					statusAmount = 4,
					targetPlayer = true
				}
			],
			_ => [
				new AVariableHintWild(),
				new AAttack {
					damage = GetDmg(s, amt),
					xHint = 1
				},
				new AStatus {
					status = Status.tempShield,
					statusAmount = amt,
					targetPlayer = true,
					xHint = 1
				}
			]
		};
	}
}


internal sealed class AugmentDNACard : Card, ITyCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("AugmentDNA", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.TyDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/AugmentDNA.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "AugmentDNA", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		exhaust = upgrade == Upgrade.B,
		description = ModEntry.Instance.Localizations.Localize(["card", "AugmentDNA", "description", upgrade.ToString()]),
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) {
		var amt = WildManager.CountWildsInHand(s, c);
		return upgrade switch {
			Upgrade.A => [
				new ACardSelectImproved
				{
					browseAction = new AMakeWild(),
					browseSource = CardBrowse.Source.Hand,
					filterUUID = uuid
				}.ApplyModData(CardBrowseFilterManager.FilterWildKey, false),
				new ACardSelectImproved
				{
					browseAction = new AMakeWild(),
					browseSource = CardBrowse.Source.Hand,
					filterUUID = uuid
				}.ApplyModData(CardBrowseFilterManager.FilterWildKey, false)
			],
			Upgrade.B => [
				new ACardSelectImproved
				{
					browseAction = new AMakeWild(),
					browseSource = CardBrowse.Source.Hand,
					filterUUID = uuid
				}.ApplyModData(CardBrowseFilterManager.FilterWildKey, false),
				new ACardSelectImproved
				{
					browseAction = new AMakeWild(),
					browseSource = CardBrowse.Source.Hand,
					filterUUID = uuid
				}.ApplyModData(CardBrowseFilterManager.FilterWildKey, false),
				new ACardSelectImproved
				{
					browseAction = new AMakeWild(),
					browseSource = CardBrowse.Source.Hand,
					filterUUID = uuid
				}.ApplyModData(CardBrowseFilterManager.FilterWildKey, false)
			],
			_ => [
				new ACardSelectImproved
				{
					browseAction = new AMakeWild(),
					browseSource = CardBrowse.Source.Hand,
					filterUUID = uuid
				}.ApplyModData(CardBrowseFilterManager.FilterWildKey, false)
			]
		};
	}
}


internal sealed class CurlUpCard : Card, IHasCustomCardTraits, ITyCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("CurlUp", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.TyDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/CurlUp.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "CurlUp", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 2,
		artTint = "ffffff"
	};

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => new HashSet<ICardTraitEntry>() { WildManager.WildTrait };

	public override List<CardAction> GetActions(State s, Combat c) {
		var amt = WildManager.CountWildsInHand(s, c);
		return upgrade switch {
			Upgrade.A => [
				new AStatus {
					status = ModEntry.Instance.XFactorStatus.Status,
					statusAmount = 1,
					targetPlayer = true
				},
				new AVariableHintWild {
					displayAdjustment = 1
				}.ApplyModData(XAffectorManager.InnateIncreasedHintsKey, 1),
				new AStatus {
					status = Status.shield,
					statusAmount = amt + 1,
					targetPlayer = true,
					xHint = 1
				},
				new AStatus {
					status = Status.shield,
					statusAmount = 1,
					targetPlayer = true
				}
			],
			Upgrade.B => [
				new AVariableHintWild(),
				new AStatus {
					status = Status.shield,
					statusAmount = amt,
					targetPlayer = true,
					xHint = 1
				},
				new AStatus {
					status = ModEntry.Instance.XFactorStatus.Status,
					statusAmount = 2,
					targetPlayer = true
				}
			],
			_ => [
				new AStatus {
					status = ModEntry.Instance.XFactorStatus.Status,
					statusAmount = 1,
					targetPlayer = true
				},
				new AVariableHintWild {
					displayAdjustment = 1
				}.ApplyModData(XAffectorManager.InnateIncreasedHintsKey, 1),
				new AStatus {
					status = Status.shield,
					statusAmount = amt + 1,
					targetPlayer = true,
					xHint = 1
				}
			]
		};
	}
}


internal sealed class CompoundCard : Card, ITyCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Compound", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.TyDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Compound.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Compound", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 1 : 0,
		artTint = "ffffff"
	};

	public static int GetX(State s, Combat c)
	{
		return (c == DB.fakeCombat) ? 0 : c.energy;
	}

	public override List<CardAction> GetActions(State s, Combat c)
	{
		var currentCost = this.GetCurrentCostNoRecursion(s);
		var amt = GetX(s, c);
		var multiplier = upgrade == Upgrade.B ? 3 : 1;
		return [
			new AVariableHintEnergy {
				setAmount = amt - currentCost
			},
			new AStatusAdjusted {
				status = Status.shield,
				statusAmount = multiplier * amt,
				amountDisplayAdjustment = -currentCost * multiplier,
				targetPlayer = true,
				xHint = multiplier
			},
			new AEnergySet {
				setTo = upgrade == Upgrade.A ? 1 : 0
			}
		];
	}
}


internal sealed class PredationCard : Card, IHasCustomCardTraits, ITyCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Predation", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.TyDeck.Deck,
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Predation.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Predation", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 3 : upgrade == Upgrade.A ? 1 : 2,
		exhaust = true,
		artTint = "ffffff"
	};

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => new HashSet<ICardTraitEntry>() { WildManager.WildTrait };

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AVariableHintWild(),
			new AStatus {
				status = ModEntry.Instance.PredationStatus.Status,
				statusAmount = WildManager.CountWildsInHand(s, c),
				targetPlayer = true,
				xHint = 1
			},
		],
		_ => [
			new AStatus {
				status = ModEntry.Instance.PredationStatus.Status,
				statusAmount = 1,
				targetPlayer = true
			},
		]
	};
}


internal sealed class ExtremeMeasuresCard : Card, ITyCard
{
	internal static Spr Sprite;
	public static void Register(IModHelper helper) {
		Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/ExtremeMeasures.png")).Sprite;
		helper.Content.Cards.RegisterCard("ExtremeMeasures", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.TyDeck.Deck,
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "ExtremeMeasures", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 1 : 2,
		exhaust = true,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AStatus {
				status = ModEntry.Instance.ExtremeMeasuresStatus.Status,
				statusAmount = 1,
				targetPlayer = true
			},
			new AStatus {
				status = ModEntry.Instance.XFactorStatus.Status,
				statusAmount = 2,
				targetPlayer = true
			}
		],
		_ => [
			new AStatus {
				status = ModEntry.Instance.ExtremeMeasuresStatus.Status,
				statusAmount = 1,
				targetPlayer = true
			},
		]
	};
}


internal sealed class DiverterCard : Card, ITyCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Diverter", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.TyDeck.Deck,
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Diverter.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Diverter", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 1 : 2,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AVariableHint {
				status = Status.shield
			},
			new AStatus {
				status = ModEntry.Instance.XFactorStatus.Status,
				statusAmount = s.ship.Get(Status.shield),
				targetPlayer = true,
				xHint = 1
			},
			new AStatus {
				status = Status.shield,
				statusAmount = 2,
				mode = AStatusMode.Set,
				targetPlayer = true
			},
		],
		_ => [
			new AVariableHint {
				status = Status.shield
			},
			new AStatus {
				status = ModEntry.Instance.XFactorStatus.Status,
				statusAmount = s.ship.Get(Status.shield),
				targetPlayer = true,
				xHint = 1
			},
			new AStatus {
				status = Status.shield,
				statusAmount = 0,
				mode = AStatusMode.Set,
				targetPlayer = true
			},
		]
	};
}


internal sealed class HibernateCard : Card, IHasCustomCardTraits, ITyCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Hibernate", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.TyDeck.Deck,
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Hibernate.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Hibernate", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 0 : 1,
		artTint = "ffffff"
	};

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => new HashSet<ICardTraitEntry>() { WildManager.WildTrait };

	public override List<CardAction> GetActions(State s, Combat c) {
		var amt = WildManager.CountWildsInHand(s, c);
		return upgrade switch {
			Upgrade.B => [
				new AVariableHintWild(),
				new AStatus {
					status = Status.maxShield,
					statusAmount = amt,
					targetPlayer = true,
					xHint = 1
				},
				new AStatus {
					status = Status.shield,
					statusAmount = 1,
					targetPlayer = true
				},
				new AEndTurn()
			],
			_ => [
				new AVariableHintWild(),
				new AStatus {
					status = Status.maxShield,
					statusAmount = amt,
					targetPlayer = true,
					xHint = 1
				},
				new AEndTurn()
			]
		};
	}
}


internal sealed class FetchCard : Card, IHasCustomCardTraits, ITyCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("Fetch", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.TyDeck.Deck,
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/Fetch.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Fetch", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.B ? 0 : 1,
		description = ModEntry.Instance.Localizations.Localize(["card", "Fetch", "description", upgrade.ToString()]),
		artTint = "ffffff"
	};

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state) => new HashSet<ICardTraitEntry>() { WildManager.WildTrait };

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.A => [
			new ACardSelectImproved
			{
				browseAction = new ChooseCardToPutInHand(),
				browseSource = CardBrowse.Source.DrawPile,
				filterUUID = uuid
			}.ApplyModData(CardBrowseFilterManager.FilterWildKey, true),
			new ACardSelectImproved
			{
				browseAction = new ChooseCardToPutInHand(),
				browseSource = CardBrowse.Source.DrawPile,
				filterUUID = uuid
			}.ApplyModData(CardBrowseFilterManager.FilterWildKey, true),
			new ACardSelectImproved
			{
				browseAction = new ChooseCardToPutInHand(),
				browseSource = CardBrowse.Source.DrawPile,
				filterUUID = uuid
			}.ApplyModData(CardBrowseFilterManager.FilterWildKey, true)
		],
		Upgrade.B => [
			new ACardSelectImproved
			{
				browseAction = new ChooseCardToPutInHand(),
				browseSource = CardBrowse.Source.DrawPile,
				filterUUID = uuid
			}.ApplyModData(CardBrowseFilterManager.FilterWildKey, true)
		],
		_ => [
			new ACardSelectImproved
			{
				browseAction = new ChooseCardToPutInHand(),
				browseSource = CardBrowse.Source.DrawPile,
				filterUUID = uuid
			}.ApplyModData(CardBrowseFilterManager.FilterWildKey, true),
			new ACardSelectImproved
			{
				browseAction = new ChooseCardToPutInHand(),
				browseSource = CardBrowse.Source.DrawPile,
				filterUUID = uuid
			}.ApplyModData(CardBrowseFilterManager.FilterWildKey, true)
		]
	};
}


internal sealed class TyExeCard : Card, ITyCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("TyExe", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = Deck.colorless,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = ExtremeMeasuresCard.Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "TyExe", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = upgrade == Upgrade.A ? 0 : 1,
		exhaust = true,
		description = ColorlessLoc.GetDesc(state, upgrade == Upgrade.B ? 3 : 2, ModEntry.Instance.TyDeck.Deck),
		artTint = "ffffff"
    };

	public override List<CardAction> GetActions(State s, Combat c)
    {
		Deck deck = ModEntry.Instance.TyDeck.Deck;
		return upgrade switch
		{
			Upgrade.B => [
				new ACardOffering
				{
					amount = 3,
					limitDeck = deck,
					makeAllCardsTemporary = true,
					overrideUpgradeChances = false,
					canSkip = false,
					inCombat = true,
					discount = -1,
					dialogueSelector = ".summonTy"
				}
			],
			_ => [
				new ACardOffering
				{
					amount = 2,
					limitDeck = deck,
					makeAllCardsTemporary = true,
					overrideUpgradeChances = false,
					canSkip = false,
					inCombat = true,
					discount = -1,
					dialogueSelector = ".summonTy"
				}
			],
		};
	}
}

