using System.Collections.Generic;

namespace TheJazMaster.TyAndSasha.Actions;

public class AVariableHintNumber : AVariableHint
{
    public int number = 1;

    public AVariableHintNumber() : base()
    {
        hand = true;
    }

    public override Icon? GetIcon(State s)
    {
        return new Icon(ModEntry.Instance.EnergyIcon.Sprite, number, Colors.textMain);
    }

    public override List<Tooltip> GetTooltips(State s) => 
        [new TTText(ModEntry.Instance.Localizations.Localize(["action", "variableHintNumber", "description"], new { Amount = number.ToString() }))];
}