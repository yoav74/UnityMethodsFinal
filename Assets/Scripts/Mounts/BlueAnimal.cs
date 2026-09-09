/// <summary>
/// Blue animal mount: its attack is a melee <b>tail swipe</b>, which is exactly the base
/// <see cref="Animal"/> reach-in-front (damages enemies, shatters rocks, spares bonfires). It is
/// its own type so the mount system and the heart pickup (ME-49) can target it specifically; no
/// attack override is needed because the shared melee is the tail.
/// </summary>
public class BlueAnimal : Animal
{
}
