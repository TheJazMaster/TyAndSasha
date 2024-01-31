namespace TheJazMaster.TyAndSasha.Cards;
#nullable enable
public interface IWildCard
{
    bool IsWild(State s, Combat? c);
}