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


internal sealed class BiteCard : Card, IWildCard, ITyCard
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

	public bool IsWild(State s, Combat c) => true;

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch
	{
		Upgrade.A => [
			new AVariableHintWild(),
			new AAttack {
				damage = GetDmg(s, ModEntry.Instance.WildManager.CountWildsInHand(s, c)),
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
				damage = GetDmg(s, ModEntry.Instance.WildManager.CountWildsInHand(s, c)),
				piercing = true,
				xHint = 1
			}
		],
		_ => [
			new AVariableHintWild(),
			new AAttack {
				damage = GetDmg(s, ModEntry.Instance.WildManager.CountWildsInHand(s, c)),
				xHint = 1
			}
		]
	};
}

internal sealed class ScratchCard : Card, IWildCard, ITyCard
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

	public bool IsWild(State s, Combat c) => true;

	public override List<CardAction> GetActions(State s, Combat c)
	{
		var dmg = ModEntry.Instance.WildManager.CountWildsInHand(s, c);
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

internal sealed class ScurryCard : Card, IWildCard, ITyCard
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

	public bool IsWild(State s, Combat c) => true;

	public override List<CardAction> GetActions(State s, Combat c)
	{
		var amt = ModEntry.Instance.WildManager.CountWildsInHand(s, c);
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

internal sealed class ZoomiesCard : Card, IWildCard, ITyCard
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

	public bool IsWild(State s, Combat c) => true;

	public override List<CardAction> GetActions(State s, Combat c)
	{
		var amt = ModEntry.Instance.WildManager.CountWildsInHand(s, c);
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

internal sealed class PotShotCard : Card, IWildCard, ITyCard
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

	public bool IsWild(State s, Combat c) => upgrade == Upgrade.A;

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch
	{
		Upgrade.B => [
			new AAttack {
				damage = GetDmg(s, 3)
			},
			new ADrawCard {
				count = 1
			}
		],
		_ => [
			new AAttack {
				damage = GetDmg(s, upgrade == Upgrade.A ? 4 : 3)
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


internal sealed class CrossAttackCard : Card, ITyCard
{
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("CrossAttack", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.TyDeck.Deck,
				rarity = Rarity.uncommon,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/CrossAttack.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "CrossAttack", "name"]).Localize
		});
	}

	public override CardData GetData(State state) => new() {
		cost = 1,
		exhaust = upgrade == Upgrade.B,
		artTint = "ffffff"
	};

	public int GetX(State s)
	{
		return s.ship.Get(upgrade == Upgrade.None ? Status.shield : ModEntry.Instance.XFactorStatus.Status);
	}

	public override List<CardAction> GetActions(State s, Combat c)
	{
		return upgrade switch
		{
			Upgrade.A => [
				new AStatus {
					status = ModEntry.Instance.XFactorStatus.Status,
					statusAmount = 1,
					targetPlayer = true
				},
				new AVariableHintAdjusted {
					status = ModEntry.Instance.XFactorStatus.Status,
					displayAdjustment = 1
				}.ApplyModData(XAffectorManager.InnateIncreasedHintsKey, 1),
				new AAttack {
					damage = GetDmg(s, GetX(s) + 2),
					xHint = 1
				},
			],
			Upgrade.B => [
				new AStatus {
					status = ModEntry.Instance.XFactorStatus.Status,
					statusAmount = 2,
					targetPlayer = true
				},
				new AVariableHintAdjusted {
					status = ModEntry.Instance.XFactorStatus.Status,
					displayAdjustment = 2
				}.ApplyModData(XAffectorManager.InnateIncreasedHintsKey, 2),
				new AAttack {
					damage = GetDmg(s, GetX(s) + 4),
					xHint = 1
				},
			],
			_ => [
				new AVariableHint {
					status = Status.shield
				},
				new AAttack {
					damage = GetDmg(s, GetX(s)),
					xHint = 1
				},
				new AStatus {
					status = ModEntry.Instance.XFactorStatus.Status,
					statusAmount = 1,
					targetPlayer = true
				},
			]
		};
	}
}


internal sealed class InstinctsCard : Card, IWildCard, ITyCard
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

	public bool IsWild(State s, Combat c) => true;

	public override List<CardAction> GetActions(State s, Combat c) => 
	[
		new AVariableHintWild(),
		new AStatus {
			status = Status.tempShield,
			statusAmount = ModEntry.Instance.WildManager.CountWildsInHand(s, c),
			targetPlayer = true,
			xHint = 1
		},
	];
}


internal sealed class AdrenalineCard : Card, IWildCard, ITyCard
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
		cost = upgrade == Upgrade.B ? 0 : upgrade == Upgrade.A ? 1 : 2,
		exhaust = upgrade == Upgrade.B,
		artTint = "ffffff"
	};

	public bool IsWild(State s, Combat c) => true;

	public override List<CardAction> GetActions(State s, Combat c) => 
	[
		new AVariableHintWild(),
		new AEnergy {
			changeAmount = ModEntry.Instance.WildManager.CountWildsInHand(s, c),
			xHint = 1
		},
	];
}


internal sealed class PounceCard : Card, IWildCard, ITyCard
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

	public bool IsWild(State s, Combat c) => true;

	public override List<CardAction> GetActions(State s, Combat c) {
		var amt = ModEntry.Instance.WildManager.CountWildsInHand(s, c);
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
		var amt = ModEntry.Instance.WildManager.CountWildsInHand(s, c);
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


internal sealed class CurlUpCard : Card, IWildCard, ITyCard
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

	public bool IsWild(State s, Combat c) => true;

	public override List<CardAction> GetActions(State s, Combat c) {
		var amt = ModEntry.Instance.WildManager.CountWildsInHand(s, c);
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


internal sealed class PredationCard : Card, IWildCard, ITyCard
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

	public bool IsWild(State s, Combat c) => true;

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.B => [
			new AVariableHintWild(),
			new AStatus {
				status = ModEntry.Instance.PredationStatus.Status,
				statusAmount = ModEntry.Instance.WildManager.CountWildsInHand(s, c),
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
	public static void Register(IModHelper helper) {
		helper.Content.Cards.RegisterCard("ExtremeMeasures", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.TyDeck.Deck,
				rarity = Rarity.rare,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("Sprites/Cards/ExtremeMeasures.png")).Sprite,
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
		cost = 2,
		artTint = "ffffff"
	};

	public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
		Upgrade.A => [
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
		Upgrade.B => [
			new AVariableHintNumber {
				number = 1
			},
			new AStatus {
				status = ModEntry.Instance.XFactorStatus.Status,
				statusAmount = 2,
				targetPlayer = true,
				xHint = 2
			}
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


internal sealed class HibernateCard : Card, IWildCard, ITyCard
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

	public bool IsWild(State s, Combat c) => true;

	public override List<CardAction> GetActions(State s, Combat c) {
		var amt = ModEntry.Instance.WildManager.CountWildsInHand(s, c);
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


internal sealed class FetchCard : Card, IWildCard, ITyCard
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

	public bool IsWild(State s, Combat c) => true;

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