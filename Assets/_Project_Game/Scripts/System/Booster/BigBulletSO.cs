using UnityEngine;

public class BigBulletSO : BoosterSO
{
    public float scaleMultiplier = 2.5f;

    public override bool ActivateBooster(SimpleCannon cannon)
    {
        return cannon != null && cannon.ActivateBigBullet(scaleMultiplier);
    }
}