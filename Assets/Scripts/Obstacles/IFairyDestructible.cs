/// <summary>
/// Something only the <b>fairy</b> can destroy (a bonfire). Distinct from
/// <see cref="IDestructible"/>, which weapons and animal attacks can also break (rocks): keeping
/// them separate is what lets a boomerang or an animal shatter a rock but never a bonfire, while
/// the fairy clears both.
/// </summary>
public interface IFairyDestructible
{
    void DestroyByFairy();
}
