using FMOD;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TheJazMaster.TyAndSasha.Artifacts;
using TheJazMaster.TyAndSasha.Cards;
using TheJazMaster.TyAndSasha.Features;

namespace TheJazMaster.TyAndSasha;

public sealed class ModEntry : SimpleMod {
    internal static ModEntry Instance { get; private set; } = null;

    internal Harmony Harmony { get; }
	internal IKokoroApi KokoroApi { get; }
	internal IMoreDifficultiesApi MoreDifficultiesApi { get; }

	internal WildManager WildManager { get; }


	internal ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations { get; }
	internal ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations { get; }

    internal ICharacterEntry TyCharacter { get; }

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

    internal static IReadOnlyList<Type> StarterCardTypes { get; } = [
		typeof(TreatCard),
		typeof(BiteCard),
	];

	internal static IReadOnlyList<Type> CommonCardTypes { get; } = [
		typeof(ScratchCard),
		typeof(ScurryCard),
		typeof(HugsCard),
		typeof(ZoomiesCard),
		typeof(PotShotCard),
		typeof(StackCard),
        typeof(PatOnTheBackCard),
	];

	internal static IReadOnlyList<Type> UncommonCardTypes { get; } = [
		typeof(CrossAttackCard),
		typeof(InstinctsCard),
		typeof(AdrenalineCard),
		typeof(PounceCard),
		typeof(AugmentDNACard),
		typeof(CurlUpCard),
		typeof(CompoundCard),
	];

	internal static IReadOnlyList<Type> RareCardTypes { get; } = [
		typeof(PredationCard),
		typeof(ExtremeMeasuresCard),
		typeof(DiverterCard),
		typeof(HibernateCard),
		typeof(FetchCard),
	];

    internal static IEnumerable<Type> AllCardTypes
		=> StarterCardTypes
			.Concat(CommonCardTypes)
			.Concat(UncommonCardTypes)
			.Concat(RareCardTypes);

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

	internal static IEnumerable<Type> AllArtifactTypes
		=> CommonArtifacts.Concat(BossArtifacts);

    
    public ModEntry(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
	{
		Instance = this;
		Harmony = new(package.Manifest.UniqueName);
		MoreDifficultiesApi = helper.ModRegistry.GetApi<IMoreDifficultiesApi>("TheJazMaster.MoreDifficulties")!;
		KokoroApi = helper.ModRegistry.GetApi<IKokoroApi>("Shockah.Kokoro")!;
		KokoroApi.RegisterTypeForExtensionData(typeof(Part));

		WildManager = new WildManager();
		_ = new CardBrowseFilterManager();
		_ = new XAffectorManager();
		_ = new StatusManager();
		CustomTTGlossary.ApplyPatches(Harmony);

		AnyLocalizations = new JsonLocalizationProvider(
			tokenExtractor: new SimpleLocalizationTokenExtractor(),
			localeStreamFunction: locale => package.PackageRoot.GetRelativeFile($"I18n/{locale}.json").OpenRead()
		);
		Localizations = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
			new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(AnyLocalizations)
		);


        TyPortrait = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Character/Ty_neutral_0.png"));
        TyPortraitMini = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Character/Ty_mini.png"));
		TyFrame = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Character/Panel.png"));
        TyCardBorder = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Character/CardBorder.png"));

        XFactorStatus = helper.Content.Statuses.RegisterStatus("XFactor", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/XFactor.png")).Sprite,
				color = new("d83273")
			},
			Name = AnyLocalizations.Bind(["status", "XFactor", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "XFactor", "description"]).Localize
		});

        ExtremeMeasuresStatus = helper.Content.Statuses.RegisterStatus("ExtremeMeasures", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/ExtremeMeasures.png")).Sprite,
				color = new("d83273")
			},
			Name = AnyLocalizations.Bind(["status", "ExtremeMeasures", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "ExtremeMeasures", "description"]).Localize
		});

        PredationStatus = helper.Content.Statuses.RegisterStatus("Predation", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Predation.png")).Sprite,
				color = new("c82424")
			},
			Name = AnyLocalizations.Bind(["status", "Predation", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "Predation", "description"]).Localize
		});


		WildIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Wild.png"));
		WildHandIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/WildHand.png"));
		EnergyIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Icons/Energy.png"));


		TyDeck = helper.Content.Decks.RegisterDeck("TyAndSasha", new()
		{
			Definition = new() { color = new Color("CB2867"), titleColor = Colors.black },
			DefaultCardArt = StableSpr.cards_colorless,
			BorderSprite = TyCardBorder.Sprite,
			Name = AnyLocalizations.Bind(["character", "name"]).Localize
		});

        foreach (var cardType in AllCardTypes)
			AccessTools.DeclaredMethod(cardType, nameof(ITyCard.Register))?.Invoke(null, [helper]);
		foreach (var artifactType in AllArtifactTypes)
			AccessTools.DeclaredMethod(artifactType, nameof(ITyCard.Register))?.Invoke(null, [helper]);

		MoreDifficultiesApi?.RegisterAltStarters(TyDeck.Deck, new StarterDeck {
            cards = {
                new PatOnTheBackCard(),
                new ZoomiesCard()
            }
        });

        TyCharacter = helper.Content.Characters.RegisterCharacter("TyAndSasha", new()
		{
			Deck = TyDeck.Deck,
			Description = AnyLocalizations.Bind(["character", "description"]).Localize,
			BorderSprite = TyFrame.Sprite,
			StarterCardTypes = StarterCardTypes,
			NeutralAnimation = new()
			{
				Deck = TyDeck.Deck,
				LoopTag = "neutral",
				Frames = [
					TyPortrait.Sprite
				]
			},
			MiniAnimation = new()
			{
				Deck = TyDeck.Deck,
				LoopTag = "mini",
				Frames = [
					TyPortraitMini.Sprite
				]
			}
		});

		helper.Content.Characters.RegisterCharacterAnimation("GameOver", new()
		{
			Deck = TyDeck.Deck,
			LoopTag = "gameover",
			Frames = [TyPortrait.Sprite]
		});
		helper.Content.Characters.RegisterCharacterAnimation("Squint", new()
		{
			Deck = TyDeck.Deck,
			LoopTag = "squint",
			Frames = [TyPortrait.Sprite]
		});
    }
}