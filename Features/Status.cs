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
using System.Diagnostics.Metrics;

namespace TheJazMaster.TyAndSasha.Features;
#nullable enable

public class StatusManager : IStatusLogicHook
{
    private static ModEntry Instance => ModEntry.Instance;

    public StatusManager()
    {
        ModEntry.Instance.KokoroApi.RegisterStatusLogicHook(this, 0);

        ModEntry.Instance.Harmony.TryPatch(
		    logger: ModEntry.Instance.Logger,
		    original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.TryPlayCard)),
			transpiler: new HarmonyMethod(GetType(), nameof(Combat_TryPlayCard_Transpiler))
		);
    }
    
    private static IEnumerable<CodeInstruction> Combat_TryPlayCard_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase originalMethod)
    {
        return new SequenceBlockMatcher<CodeInstruction>(instructions)
            .Find(
                ILMatches.Ldloc<int>(originalMethod),
                ILMatches.Call("OnPlayerPlayCard")
            )
            .EncompassUntil(SequenceMatcherPastBoundsDirection.After, ILMatches.Instruction(OpCodes.Leave_S).GetLeaveBranchTarget(out var branchTarget))
            .PointerMatcher(branchTarget).ExtractLabels(out var labels)
            .Insert(SequenceMatcherPastBoundsDirection.Before, SequenceMatcherInsertionResultingBounds.IncludingInsertion, new List<CodeInstruction> {
                new CodeInstruction(OpCodes.Ldarg_1).WithLabels(labels),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldarg_2),
                new(OpCodes.Call, AccessTools.DeclaredMethod(typeof(StatusManager), nameof(PredationTrigger))),
            })
            .AllElements();
    }

    private static void PredationTrigger(State s, Combat c, Card card)
    {
        var predation = Instance.PredationStatus.Status;
        if (WildManager.IsWild(card, s) && s.ship.Get(predation) > 0) {
            c.Queue(new AAttack {
                damage = card.GetDmg(s, s.ship.Get(predation)),
                statusPulse = predation
            });
        }
    }

    public bool HandleStatusTurnAutoStep(State state, Combat combat, StatusTurnTriggerTiming timing, Ship ship, Status status, ref int amount, ref StatusTurnAutoStepSetStrategy setStrategy)
	{
		if (status != Instance.XFactorStatus.Status)
			return false;
		if (timing != StatusTurnTriggerTiming.TurnEnd)
			return false;
        if (ship.Get(Status.timeStop) > 0)
            return false;

		if (amount > 0)
			amount = Math.Max(amount - 1, 0);
		return false;
	}
}