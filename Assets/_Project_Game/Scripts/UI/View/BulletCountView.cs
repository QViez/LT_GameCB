using UnityEngine;
using TMPro; 

public class BulletCountView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtCount;

    public void UpdateAmmoText(int currentAmmo)
    {
        if (txtCount != null)
        {
            txtCount.text = currentAmmo.ToString();
        }
    }
}