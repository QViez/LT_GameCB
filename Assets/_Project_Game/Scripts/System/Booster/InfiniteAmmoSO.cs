using UnityEngine;

public class InfiniteAmmoSO : BoosterSO
{
    public float duration = 10f;

    public override bool ActivateBooster(SimpleCannon cannon)
    {
        return cannon != null && cannon.ActivateInfiniteAmmo(duration);
    }
}