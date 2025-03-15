using FMOD;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TheJazMaster.TyAndSasha.Actions;
using TheJazMaster.TyAndSasha.Artifacts;
using TheJazMaster.TyAndSasha.Cards;
using TheJazMaster.TyAndSasha.Features;

namespace TheJazMaster.TyAndSasha;

public sealed class ModEntry : SimpleMod, ITyAndSashaApi.IHook {
    internal static ModEntry Instance { get; private set; } = null!;
	internal readonly HookManager<ITyAndSashaApi.IHook> HookManager;
	internal readonly ITyAndSashaApi Api;

    internal Harmony Harmony { get; }
	internal IKokoroApi KokoroApi { get; }
	internal IMoreDifficultiesApi? MoreDifficultiesApi { get; }
	internal IDuoArtifactsApi? DuoArtifactsApi { get; }
	internal ILouisApi? LouisApi { get; }

	internal WildManager WildManager { get; }
	internal HeavyManager HeavyManager { get; }


	internal ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations { get; }
	internal ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations { get; }

    internal IPlayableCharacterEntryV2 TyCharacter { get; }

    internal IDeckEntry TyDeck { get; }

	internal IStatusEntry ExtremeMeasuresStatus { get; }
	internal IStatusEntry XFactorStatus { get; }
    internal IStatusEntry PredationStatus { get; }

    internal ISpriteEntry TyPortrait { get; }
    internal ISpriteEntry TyPortraitMini { get; }
    internal ISpriteEntry TyFrame { get; }
    internal ISpriteEntry TyCardBorder { get; }

    internal ISpriteEntry WildIcon { get; }
	internal ISpriteEntry WildHandIcon { get; }
	internal ISpriteEntry EnergyIcon { get; }
    internal ISpriteEntry HeavyIcon { get; }
    internal ISpriteEntry HeavyUsedIcon { get; }

    internal static IReadOnlyList<Type> StarterCardTypes { get; } = [
		typeof(TreatCard),
		typeof(BiteCard),

        typeof(PatOnTheBackCard),
		typeof(PackBondCard),
	];

	internal static IReadOnlyList<Type> CommonCardTypes { get; } = [
		typeof(HugsCard),
		typeof(ZoomiesCard),
		typeof(PotShotCard),
		typeof(ReplicateCard),
		typeof(CurlUpCard),
	];

	internal static IReadOnlyList<Type> UncommonCardTypes { get; } = [
		typeof(ScratchCard),
		typeof(ScurryCard),
		typeof(HibernateCard),
		typeof(CrossAttackCard),
		typeof(InstinctsCard),
		typeof(AdrenalineCard),
		typeof(PounceCard),
	];

	internal static IReadOnlyList<Type> RareCardTypes { get; } = [
		typeof(PredationCard),
		typeof(ExtremeMeasuresCard),
		typeof(DiverterCard),
		typeof(AugmentDNACard),
		typeof(FetchCard),
	];

    internal static IEnumerable<Type> AllCardTypes
		=> StarterCardTypes
			.Concat(CommonCardTypes)
			.Concat(UncommonCardTypes)
			.Concat(RareCardTypes).AddItem(typeof(TyExeCard));

    internal static IReadOnlyList<Type> CommonArtifacts { get; } = [
		typeof(LycanthropyArtifact),
		typeof(NaturalTalentArtifact),
		typeof(ChewToyArtifact),
		typeof(BabyStarnacleArtifact),
		typeof(ScratchingPostArtifact),
	];

	internal static IReadOnlyList<Type> BossArtifacts { get; } = [
		typeof(GenomeSplicingArtifact)
	];

	internal static IReadOnlyList<Type> DuoArtifacts { get; } = [
		typeof(AimingReticleArtifact),
		typeof(SashaSnacksArtifact),
		typeof(PartyBalloonArtifact),
		typeof(PetRockArtifact),
		typeof(PirateMapArtifact),
		typeof(FunhouseMirrorArtifact),
		typeof(DruidismArtifact),
		typeof(VirtualPetSimArtifact),
		typeof(TigersEyeArtifact)
	];

	internal static IEnumerable<Type> AllArtifactTypes
		=> CommonArtifacts.Concat(BossArtifacts).Concat(DuoArtifacts);

    
    public ModEntry(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
	{
		Instance = this;
		Harmony = new(package.Manifest.UniqueName);
		HookManager = new(package.Manifest.UniqueName);
		Api = new ApiImplementation();
		
		MoreDifficultiesApi = helper.ModRegistry.GetApi<IMoreDifficultiesApi>("TheJazMaster.MoreDifficulties");
		KokoroApi = helper.ModRegistry.GetApi<IKokoroApi>("Shockah.Kokoro")!;
		DuoArtifactsApi = helper.ModRegistry.GetApi<IDuoArtifactsApi>("Shockah.DuoArtifacts");
		LouisApi = helper.ModRegistry.GetApi<ILouisApi>("TheJazMaster.Louis");

		AnyLocalizations = new JsonLocalizationProvider(
			tokenExtractor: new SimpleLocalizationTokenExtractor(),
			localeStreamFunction: locale => package.PackageRoot.GetRelativeFile($"I18n/{locale}.json").OpenRead()
		);
		Localizations = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
			new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(AnyLocalizations)
		);

		WildManager = new WildManager();
		HeavyManager = new HeavyManager();
		_ = new CardBrowseFilterManager();
		_ = new XAffectorManager();
		_ = new StatusManager();

        TyPortrait = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Character/Ty_neutral_0.png"));
        TyPortraitMini = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Character/Ty_mini.png"));
		TyFrame = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Character/Panel.png"));
        TyCardBorder = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Character/CardBorder.png"));

        XFactorStatus = helper.Content.Statuses.RegisterStatus("XFactor", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/XFactor.png")).Sprite,
				color = new("d83273"),
				isGood = true
			},
			Name = AnyLocalizations.Bind(["status", "XFactor", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "XFactor", "description"]).Localize
		});

        ExtremeMeasuresStatus = helper.Content.Statuses.RegisterStatus("ExtremeMeasures", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/ExtremeMeasures.png")).Sprite,
				color = new("d83273"),
				isGood = true
			},
			Name = AnyLocalizations.Bind(["status", "ExtremeMeasures", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "ExtremeMeasures", "description"]).Localize
		});

        PredationStatus = helper.Content.Statuses.RegisterStatus("Predation", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Predation.png")).Sprite,
				color = new("c82424"),
				isGood = true
			},
			Name = AnyLocalizations.Bind(["status", "Predation", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "Predation", "description"]).Localize
		});


		Api.RegisterHook(this);

		WildIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Wild.png"));
		WildHandIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/WildHand.png"));
		EnergyIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Energy.png"));
    	HeavyIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Heavy.png"));
		HeavyUsedIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/HeavyUsed.png"));


		TyDeck = helper.Content.Decks.RegisterDeck("TyAndSasha", new()
		{
			Definition = new() { color = new Color("FF6895"), titleColor = Colors.black },
			DefaultCardArt = StableSpr.cards_colorless,
			BorderSprite = TyCardBorder.Sprite,
			Name = AnyLocalizations.Bind(["character", "name"]).Localize
		});

        foreach (var cardType in AllCardTypes)
			AccessTools.DeclaredMethod(cardType, nameof(ITyCard.Register))?.Invoke(null, [helper]);
		foreach (var artifactType in AllArtifactTypes)
			AccessTools.DeclaredMethod(artifactType, nameof(ITyArtifact.Register))?.Invoke(null, [helper]);

		MoreDifficultiesApi?.RegisterAltStarters(TyDeck.Deck, new StarterDeck {
            cards = {
                new PatOnTheBackCard(),
                new PackBondCard()
            }
        });

        TyCharacter = helper.Content.Characters.V2.RegisterPlayableCharacter("TyAndSasha", new()
		{
			Deck = TyDeck.Deck,
			Description = AnyLocalizations.Bind(["character", "description"]).Localize,
			BorderSprite = TyFrame.Sprite,
			Starters = new StarterDeck {
				cards = [ new TreatCard(), new BiteCard() ],
			},
			ExeCardType = typeof(TyExeCard),
			NeutralAnimation = new()
			{
				CharacterType = TyDeck.Deck.Key(),
				LoopTag = "neutral",
				Frames = [
					TyPortrait.Sprite
				]
			},
			MiniAnimation = new()
			{
				CharacterType = TyDeck.Deck.Key(),
				LoopTag = "mini",
				Frames = [
					TyPortraitMini.Sprite
				]
			}
		});

		helper.Content.Characters.V2.RegisterCharacterAnimation("GameOver", new()
		{
			CharacterType = TyDeck.Deck.Key(),
			LoopTag = "gameover",
			Frames = [TyPortrait.Sprite]
		});
		helper.Content.Characters.V2.RegisterCharacterAnimation("Squint", new()
		{
			CharacterType = TyDeck.Deck.Key(),
			LoopTag = "squint",
			Frames = [TyPortrait.Sprite]
		});

		ICardTraitEntry TemporaryCardTrait = helper.Content.Cards.TemporaryCardTrait;
		ICardTraitEntry BuoyantCardTrait = helper.Content.Cards.BuoyantCardTrait;
		helper.Content.Cards.OnGetFinalDynamicCardTraitOverrides += (_, data) => {
			State state = data.State;
			if (state.route is Combat combat) {
				WildManager.ignoreCount = true;
				if (!data.TraitStates[WildManager.WildTrait].IsActive) {
					if (data.State.EnumerateAllArtifacts().Any(item => item is GenomeSplicingArtifact)) {
						foreach (CardAction action in data.Card.GetActions(state, combat)) {
							if (action is AVariableHint) {
								data.SetOverride(WildManager.WildTrait, true);
								break;
							}
						}
					}
				}
				if (data.TraitStates[TemporaryCardTrait].IsActive) {
					foreach (Artifact item in data.State.EnumerateAllArtifacts()) {
						if (item is VirtualPetSimArtifact) {
							data.SetOverride(WildManager.WildTrait, true);
						}
					}
				}
				if (data.TraitStates[BuoyantCardTrait].IsActive) {
					foreach (Artifact item in data.State.EnumerateAllArtifacts()) {
						if (item is NaturalTalentArtifact) {
							data.SetOverride(WildManager.WildTrait, true);
						}
					}
				}
				WildManager.ignoreCount = false;
			}
		};
		if (LouisApi != null) helper.Content.Cards.OnGetFinalDynamicCardTraitOverrides += (_, data) => {
			State state = data.State;
			if (state.route is Combat combat) {
				WildManager.ignoreCount = true;
				if (data.TraitStates[LouisApi.GemTrait].IsActive || data.TraitStates[LouisApi.PreciousGemTrait].IsActive) {
					if (data.State.EnumerateAllArtifacts().Any(item => item is TigersEyeArtifact)) {
						data.SetOverride(WildManager.WildTrait, true);
					}
				}
				WildManager.ignoreCount = false;
			}
		};
    }

	public override object? GetApi(IModManifest requestingMod)
		=> new ApiImplementation();
	
	
	public int AffectX(ITyAndSashaApi.IHook.IAffectXArgs args) {
		return args.State.ship.Get(Instance.XFactorStatus.Status) + args.State.ship.Get(Instance.ExtremeMeasuresStatus.Status);
	}

	public bool ApplyXBonus(ITyAndSashaApi.IHook.IApplyXBonusArgs args) {
		(CardAction action, int xBonus) = (args.Action, args.Bonus);
		if (action is AAttack attack) {
			attack.damage += xBonus;
		} else if (action is AStatus status) {
			status.statusAmount += xBonus;
		} else if (action is ADrawCard draw) {
			draw.count += xBonus;
		} else if (action is AEnergy energy) {
			energy.changeAmount += xBonus;
		} else if (action is AAddCard add) {
			add.amount += xBonus;
		} else if (action is ADiscard discard) {
			discard.count += xBonus;
		} else if (action is AHeal heal) {
			heal.healAmount += xBonus;
		} else if (action is AHurt hurt) {
			hurt.hurtAmount += xBonus;
		} else if (action is AMove move) {
			if (move.dir > 0) {
				move.dir += xBonus;
			} else {
				move.dir -= xBonus;
			}
		}
		return false;
	}
}