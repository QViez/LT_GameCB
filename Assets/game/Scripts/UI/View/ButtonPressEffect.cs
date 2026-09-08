using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonPressEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("Cài đặt hiệu ứng")]
    public float pressScale = 0.9f;
    public float animationDuration = 0.1f;

    [Header("Kích thước chuẩn của nút")]
    [Tooltip("Ghim cứng kích thước ở đây để không bị lỗi với UIGroupPopper")]
    public Vector3 defaultScale = Vector3.one; 

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(defaultScale * pressScale, animationDuration).SetEase(Ease.OutQuad).SetUpdate(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(defaultScale, animationDuration).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(defaultScale, animationDuration).SetEase(Ease.OutQuad).SetUpdate(true);
    }

    private void OnDisable()
    {
        transform.DOKill();
    }
}