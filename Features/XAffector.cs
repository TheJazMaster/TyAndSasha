using System;
using System.Collections.Generic;
using System.Linq;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using System.Text.RegularExpressions;
using Nickel;
using HarmonyLib;
using System.Reflection.Emit;
using System.Reflection;
using Microsoft.Extensions.Logging;
using TheJazMaster.TyAndSasha.Artifacts;
using TheJazMaster.TyAndSasha.Actions;
using System.ComponentModel;

namespace TheJazMaster.TyAndSasha.Features;
#nullable enable

public class XAffectorManager
{
    private static IModData ModData => ModEntry.Instance.Helper.ModData;

    internal static readonly string IncreasedHintsKey = "IncreasedHints";
    internal static readonly string InnateIncreasedHintsKey = "InnateIncreasedHints";

    public XAffectorManager()
    {
        ModEntry.Instance.Harmony.TryPatch(
		    logger: ModEntry.Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(Card), nameof(Card.GetActionsOverridden)),
			postfix: new HarmonyMethod(GetType(), nameof(Card_GetActionsOverridden_Postfix))
		);
        ModEntry.Instance.Harmony.TryPatch(
		    logger: ModEntry.Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(Card), nameof(Card.RenderAction)),
			transpiler: new HarmonyMethod(GetType(), nameof(Card_RenderAction_Transpiler))
		);
        ModEntry.Instance.Harmony.TryPatch(
		    logger: ModEntry.Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(AVariableHint), nameof(AVariableHint.GetTooltips)),
			postfix: new HarmonyMethod(GetType(), nameof(AVariableHint_GetTooltips_Postfix))
		);
    }
    
    internal static int GetXBonus(Card card, List<CardAction> actions, State s, Combat c) {
        int baseXBonus = 0;
        foreach (Artifact item in s.EnumerateAllArtifacts()) {
            if (item is IXAffectorArtifact xAffector) {
                baseXBonus += xAffector.AffectX(card, actions, s, c, baseXBonus);
            }
        }
        baseXBonus += s.ship.Get(ModEntry.Instance.XFactorStatus.Status) + s.ship.Get(ModEntry.Instance.ExtremeMeasuresStatus.Status);
        return baseXBonus;
    }

    private static void ImproveActionX(CardAction action, int baseXBonus) {
        if (action is AVariableHint) {
            ModData.SetModData(action, IncreasedHintsKey, baseXBonus);
        }
        var xHint = action.xHint;
        if (xHint == null)
            return;

        int xBonus = (int)xHint * baseXBonus;

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
    }

    private static void Card_GetActionsOverridden_Postfix(Card __instance, ref List<CardAction> __result, State s, Combat c) {
        int baseXBonus = GetXBonus(__instance, __result, s, c);
        if (baseXBonus == 0) return;

        foreach (CardAction wrappedAction in __result) {
            foreach (CardAction action in ModEntry.Instance.KokoroApi.Actions.GetWrappedCardActionsRecursively(wrappedAction, false)) {
                ImproveActionX(action, baseXBonus);
            }
        }
    }

    public static ElementMatch<CodeInstruction> CallRegex(string methodName)
        => new($"{{(any) call to method named `{methodName}`}}", i => ILMatches.AnyCall.Matches(i) && Regex.Match((i.operand as MethodBase)?.Name ?? "", methodName).Success);

    static Type displayClassType = null!;
    private static IEnumerable<CodeInstruction> Card_RenderAction_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase originalMethod)
    {
        displayClassType = originalMethod.GetMethodBody()!.LocalVariables[0].LocalType;
        var change = new SequenceBlockMatcher<CodeInstruction>(instructions)
            .Find(
                ILMatches.Brfalse.GetBranchTarget(out var branchTarget),
                ILMatches.Ldloca(0),
                ILMatches.Ldloc(0),
                ILMatches.Ldfld("w"),
                ILMatches.LdcI4(4),
                ILMatches.Instruction(OpCodes.Sub),
                ILMatches.Stfld("w"),
                ILMatches.Ldloca(0),
                CallRegex(".*PlusIcon.*").Anchor(out var anchor)
            )
            .PointerMatcher(branchTarget)
            .ExtractLabels(out var extractedLabels)
			.Insert(SequenceMatcherPastBoundsDirection.Before, SequenceMatcherInsertionResultingBounds.IncludingInsertion, new List<CodeInstruction> {
                new CodeInstruction(OpCodes.Ldloca_S, 0).WithLabels(extractedLabels),
                new CodeInstruction(OpCodes.Ldloca_S, 0),
                new(OpCodes.Ldfld, displayClassType.GetField("w")),
                new CodeInstruction(OpCodes.Ldloca_S, 0),
                new(OpCodes.Ldfld, displayClassType.GetField("action")),
                new CodeInstruction(OpCodes.Ldloca_S, 0),
                new(OpCodes.Ldfld, displayClassType.GetField("spriteColor")),
                new CodeInstruction(OpCodes.Ldloca_S, 0),
                new(OpCodes.Ldfld, displayClassType.GetField("dontDraw")),
                new CodeInstruction(OpCodes.Ldloca_S, 0),
                new(OpCodes.Ldfld, displayClassType.GetField("g")),
                new CodeInstruction(OpCodes.Ldloca_S, 0),
                new(OpCodes.Ldfld, displayClassType.GetField("iconWidth")),
                new(OpCodes.Call, AccessTools.DeclaredMethod(typeof(XAffectorManager), nameof(RenderXIncrease))),
                new(OpCodes.Stfld, displayClassType.GetField("w")),
            })
            .AllElements();

        return new SequenceBlockMatcher<CodeInstruction>(change)
            .Find(
                ILMatches.Ldloca<Icon?>(originalMethod).ExtractLabels(out var labels),
                ILMatches.Call("get_HasValue"),
                ILMatches.Brfalse.GetBranchTarget(out branchTarget)
            )
			.Insert(SequenceMatcherPastBoundsDirection.Before, SequenceMatcherInsertionResultingBounds.IncludingInsertion, new List<CodeInstruction> {
                new CodeInstruction(OpCodes.Ldloca_S, 0).WithLabels(labels),
                new(OpCodes.Ldflda, displayClassType.GetField("w")),
                new CodeInstruction(OpCodes.Ldloca_S, 0),
                new(OpCodes.Ldfld, displayClassType.GetField("action")),
                new CodeInstruction(OpCodes.Ldloca_S, 0),
                new(OpCodes.Ldfld, displayClassType.GetField("spriteColor")),
                new CodeInstruction(OpCodes.Ldloca_S, 0),
                new(OpCodes.Ldfld, displayClassType.GetField("dontDraw")),
                new CodeInstruction(OpCodes.Ldloca_S, 0),
                new(OpCodes.Ldfld, displayClassType.GetField("g")),
                new CodeInstruction(OpCodes.Ldloca_S, 0),
                new(OpCodes.Ldfld, displayClassType.GetField("iconWidth")),
                new CodeInstruction(OpCodes.Ldloca_S, 0),
                new(OpCodes.Ldfld, displayClassType.GetField("iconNumberPadding")),
                new CodeInstruction(OpCodes.Ldloca_S, 0),
                new(OpCodes.Ldfld, displayClassType.GetField("numberWidth")),
                new(OpCodes.Call, AccessTools.DeclaredMethod(typeof(XAffectorManager), nameof(Number))),
                new(OpCodes.Brtrue, branchTarget.Value)
            })
            .AllElements();
    }

    private static int RenderXIncrease(int w, CardAction action, Color spriteColor, bool dontDraw, G g, int iconWidth) {
        ModData.TryGetModData(action, IncreasedHintsKey, out int value);
        ModData.TryGetModData(action, InnateIncreasedHintsKey, out int innate);

        if (value + innate != 0) {
            // Plus
            // w += 3;
            w--;
            if (!dontDraw)
            {
                Rect? rect = new Rect(w + 1);
                Vec xy = g.Push(null, rect).rect.xy;
                Spr? plus = StableSpr.icons_plus;
                Color? trueColor = action.disabled ? spriteColor : Colors.textMain;
                Draw.Sprite(plus, xy.x, xy.y, flipX: false, flipY: false, 0.0, null, null, null, null, trueColor);
                g.Pop();
            }
            w += iconWidth - 1;

            // Number
            w += 4;
            if (!dontDraw)
            {
                Rect? rect = new Rect(w - 1);
                Vec xy = g.Push(null, rect).rect.xy;
                Color textColor = action.disabled ? Colors.disabledText : Colors.textMain;;
                Draw.Text(value + innate + "", xy.x, xy.y + 2, null, textColor, null, null, null, null, dontDraw: false, null, spriteColor, null, null, null, dontSubstituteLocFont: true);
                g.Pop();
            }
            w += (iconWidth - 5) * (int)Math.Max(1, Math.Ceiling(Math.Log10(value+1)));
        }
        return w;
    }

    private static bool Number(ref int w, CardAction action, Color spriteColor, bool dontDraw, G g, int iconWidth, int iconNumberPadding, int numberWidth)
    {
        if (action is AVariableHintNumber aVariableHintNumber)
        {
            var icon = action.GetIcon(g.state);
            Color textColor = action.disabled ? Colors.disabledText : (icon?.color ?? Colors.textMain);
            int number = aVariableHintNumber.number;
            w += iconNumberPadding;
            string text = DB.IntStringCache(number);
            if (!dontDraw)
            {
                Vec xy = g.Push(null, new Rect(w)).rect.xy;
                BigNumbers.Render(number, xy.x, xy.y, textColor);
                g.Pop();
            }
            w += text.Length * numberWidth;
            return true;
        }
        return false;
    }

    private static void AVariableHint_GetTooltips_Postfix(CardAction __instance, List<Tooltip> __result, State s) {
        ModData.TryGetModData(__instance, IncreasedHintsKey, out int value);
        ModData.TryGetModData(__instance, InnateIncreasedHintsKey, out int innate);
        if (innate + value == 0)
            return;


        foreach (Tooltip t in __result) {
            if (t is TTGlossary glossary && glossary.vals != null && glossary.vals.Length > 0) {
                string last = glossary.vals[^1]?.ToString() ?? "";
                glossary.vals[^1] = last + " + " + (value + innate);
            }
        }
    }
}