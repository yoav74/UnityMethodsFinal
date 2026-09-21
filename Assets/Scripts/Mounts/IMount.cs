/// <summary>
/// A rideable animal, seen as something the player attacks with. The mount slot
/// (<see cref="PlayerMount"/>) and the player's attack need nothing beyond <see cref="IAttacker"/>'s
/// <c>Attack</c>, so this adds no members — it is a thin marker that keeps "a mount" a distinct type
/// from "a weapon" (used where an animal is collected/mounted). The three animals (blue/red/green)
/// plug in without either knowing the concrete type (Open/Closed, DIP).
/// </summary>
public interface IMount : IAttacker
{
}
