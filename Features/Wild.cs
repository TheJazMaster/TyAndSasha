using System;
using System.Collections.Generic;
using System.Linq;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using System.Threading.Tasks;
using Nickel;
using HarmonyLib;
using System.Reflection.Emit;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Input.Touch;
using System.Globalization;
using TheJazMaster.TyAndSasha.Cards;
using System.Net.Security;

namespace TheJazMaster.TyAndSasha.Features;
#nullable enable

public class WildManager
{
    private static ModEntry Instance => ModEntry.Instance;
    private static IModData ModData => ModEntry.Instance.Helper.ModData;

    internal static readonly string WildOverrideKey = "WildOverride";
    internal static readonly string WildOverridePermanentKey = "WildOverridePermanent";

    public WildManager()
    {
        ModEntry.Instance.Harmony.TryPatch(
		    logger: ModEntry.Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.ReturnCardsToDeck)),
			postfix: new HarmonyMethod(GetType(), nameof(Combat_ReturnCardsToDeck_Postfix))
		);
        ModEntry.Instance.Harmony.TryPatch(
		    logger: ModEntry.Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(Card), nameof(Card.Render)),
			transpiler: new HarmonyMethod(GetType(), nameof(Card_Render_Transpiler))
		);
        ModEntry.Instance.Harmony.TryPatch(
		    logger: ModEntry.Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(Card), nameof(Card.GetAllTooltips)),
			postfix: new HarmonyMethod(GetType(), nameof(Card_GetAllTooltips_Postfix))
		);
    }

    public void SetWild(Card card, bool? @override, bool? permanent)
    {
        if (@override != null)
            ModData.SetModData(card, WildOverrideKey, @override);

        if (permanent != null)
            ModData.SetModData(card, WildOverridePermanentKey, permanent);
    }

    public bool IsWild(Card card, State s, Combat c)
    {
        return (card is IWildCard wildCard && wildCard.IsWild(s, c)) || (ModData.TryGetModData<bool>(card, WildOverrideKey, out var data) && data);
    }

    public int CountWildsInHand(State s, Combat c) {
        return c.hand.Where(card => IsWild(card, s, c)).Count();
    }



	private static void Combat_ReturnCardsToDeck_Postfix(Combat __instance, State state)
	{
		foreach (Card card in state.deck) {
			if (!(ModData.TryGetModData<bool>(card, WildOverridePermanentKey, out var permanent) && permanent))
            {
                ModData.RemoveModData(card, WildOverrideKey);
            }
		}
	}

    
    private static IEnumerable<CodeInstruction> Card_Render_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase originalMethod)
    {
        int index = -1;
        foreach (LocalVariableInfo info in originalMethod.GetMethodBody()!.LocalVariables)
        {
            if (info.LocalType == typeof(State))
            {
                index = info.LocalIndex;
            }
        }
        if (index == -1) throw new Exception("Failed!!");

        return new SequenceBlockMatcher<CodeInstruction>(instructions)
            .Find(
                ILMatches.Ldloc<CardData>(originalMethod).ExtractLabels(out var labels).Anchor(out var findAnchor),
                ILMatches.Ldfld("buoyant"),
                ILMatches.Brfalse
            )
            .Find(
                ILMatches.Ldloc<Vec>(originalMethod).CreateLdlocInstruction(out var ldlocVec),
                ILMatches.Ldfld("y"),
                ILMatches.LdcI4(8),
                ILMatches.Ldloc<int>(originalMethod).CreateLdlocaInstruction(out var ldlocaCardTraitIndex),
                ILMatches.Instruction(OpCodes.Dup),
                ILMatches.LdcI4(1),
                ILMatches.Instruction(OpCodes.Add),
                ILMatches.Stloc<int>(originalMethod)
            )
            .Anchors().PointerMatcher(findAnchor)
            .Insert(
                SequenceMatcherPastBoundsDirection.Before, SequenceMatcherInsertionResultingBounds.IncludingInsertion,
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(labels),
                new(OpCodes.Ldloc, index),
                ldlocaCardTraitIndex,
                ldlocVec,
                new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(RenderWildIcon)))
            )
            .AllElements();
    }

    private static void RenderWildIcon(Card card, State s, ref int cardTraitIndex, Vec vec)
    {
        Combat c = (s.route as Combat) ?? DB.fakeCombat;
        if (Instance.WildManager.IsWild(card, s, c))
        {
            Draw.Sprite(Instance.WildIcon.Sprite, vec.x, vec.y - (8 * cardTraitIndex++));
        }
    }

    private static void Card_GetAllTooltips_Postfix(Card __instance, State s, bool showCardTraits, ref IEnumerable<Tooltip> __result)
	{
		if (!showCardTraits)
			return;

		if (!Instance.WildManager.IsWild(__instance, s, s.route as Combat ?? DB.fakeCombat))
			return;

		CustomTTGlossary MakeTooltip()
			=> new(
				CustomTTGlossary.GlossaryType.cardtrait,
				() => ModEntry.Instance.WildIcon.Sprite,
				() => ModEntry.Instance.Localizations.Localize(["trait", "wild", "name"]),
				() => ModEntry.Instance.Localizations.Localize(["trait", "wild", "description"]),
                key: "trait.wild"
			);

		IEnumerable<Tooltip> ModifyTooltips(IEnumerable<Tooltip> tooltips)
		{
			bool addTooltip = true;

			foreach (var tooltip in tooltips)
			{
				if (addTooltip && tooltip is CustomTTGlossary glossary && glossary.key == "trait.wild")
				{
					addTooltip = false;
				}
				yield return tooltip;
			}

			if (addTooltip)
				yield return MakeTooltip();
		}

		__result = ModifyTooltips(__result);
	}
}