using System;
using System.Collections.Generic;
using System.Linq;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using Nickel;
using HarmonyLib;
using System.Reflection.Emit;
using System.Reflection;

namespace TheJazMaster.TyAndSasha.Features;
#nullable enable

public class CardBrowseFilterManager
{
	static ModEntry Instance => ModEntry.Instance;
    static Harmony Harmony => Instance.Harmony;
    static IModData ModData => Instance.Helper.ModData;

    internal const string FilterWildKey = "FilterWild";
    internal const string FilterWildAndBuoyantKey = "FilterWildAndBuoyant";

    public CardBrowseFilterManager()
    {
        Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(ACardSelect), nameof(ACardSelect.BeginWithRoute)),
			transpiler: new HarmonyMethod(GetType(), nameof(ACardSelect_BeginWithRoute_Transpiler))
		);
        Harmony.TryPatch(
		    logger: Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(CardBrowse), nameof(CardBrowse.GetCardList)),
			postfix: new HarmonyMethod(GetType(), nameof(CardBrowse_GetCardList_Postfix))
		);
    }

    private static IEnumerable<CodeInstruction> ACardSelect_BeginWithRoute_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase originalMethod)
    {
        return new SequenceBlockMatcher<CodeInstruction>(instructions)
            .Find(
                ILMatches.Newobj(typeof(CardBrowse).GetConstructor([])!),
                ILMatches.Instruction(OpCodes.Dup)
            )
			.Insert(SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion, new List<CodeInstruction> {
                new(OpCodes.Dup),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, AccessTools.DeclaredMethod(typeof(CardBrowseFilterManager), nameof(CopyDataToCardBrowse))),
            })
            .AllElements();
    }

    private static void CopyDataToCardBrowse(CardBrowse cardBrowse, ACardSelect cardSelect)
    {
        if (ModData.TryGetModData<bool>(cardSelect, FilterWildKey, out var filterWild))
            ModData.SetModData(cardBrowse, FilterWildKey, filterWild);

        if (ModData.TryGetModData<bool>(cardSelect, FilterWildAndBuoyantKey, out var filterWildAndBuoyant))
            ModData.SetModData(cardBrowse, FilterWildAndBuoyantKey, filterWildAndBuoyant);
    }


    private static void CardBrowse_GetCardList_Postfix(CardBrowse __instance, ref List<Card> __result, G g)
    {
        bool doesFilterWild = ModData.TryGetModData<bool>(__instance, FilterWildKey, out var filterWild);
        bool doesFilterWildAndBuoyant = ModData.TryGetModData<bool>(__instance, FilterWildAndBuoyantKey, out var filterWildAndBuoyant);
        Combat combat = g.state.route as Combat ?? DB.fakeCombat;
        if ((doesFilterWild || doesFilterWildAndBuoyant) && __instance.browseSource != CardBrowse.Source.Codex) {
            __result.RemoveAll(delegate(Card c)
            {
                CardData data = c.GetDataWithOverrides(g.state);

                if (doesFilterWild) {
                    if (Instance.WildManager.IsWild(c, g.state, combat) != filterWild)
                        return true;
                }

                if (doesFilterWildAndBuoyant) {
                    if ((Instance.WildManager.IsWild(c, g.state, combat) && data.buoyant) != filterWildAndBuoyant)
                        return true;
                }

                return false;
            });
        }
    }
}