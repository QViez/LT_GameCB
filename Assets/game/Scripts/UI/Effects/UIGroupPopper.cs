using UnityEngine;
using DG.Tweening;
using System.Collections.Generic; 

namespace LabDiner.Shared.UI
{
    public class UIGroupPopper : MonoBehaviour
    {
        [SerializeField] private List<Transform> _elementsToPop = new List<Transform>();

        [Header("Animation Settings")]
        [SerializeField] private float _startDelay = 0f; 
        [SerializeField] private float _delayBetween = 0.1f; 
        [SerializeField] private float _duration = 0.6f;
        [SerializeField] private float _overshoot = 1.7f;

        private void OnEnable()
        {
            if (_elementsToPop.Count == 0) return;
            for (int i = 0; i < _elementsToPop.Count; i++)
            {
                Transform t = _elementsToPop[i];
                if (t == null) continue;
                t.localScale = Vector3.zero;
               
                float itemDelay = _startDelay + (i * _delayBetween);

                t.DOScale(Vector3.one, _duration)
                    .SetDelay(itemDelay)
                    .SetEase(Ease.OutBack, _overshoot)
                    .SetUpdate(true);
            }
        }

        private void OnDisable()
        {
            foreach (Transform t in _elementsToPop)
            {
                if (t != null) t.DOKill();
            }
        }
    }
}