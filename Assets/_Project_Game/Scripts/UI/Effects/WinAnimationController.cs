using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class WinAnimationController : MonoBehaviour
{
    [Header("Thành phần UI")]
    public Transform board;        
    public Transform[] smashLetters; 
    public Transform festWord;      

    [Header("Thời gian ")]
    public float boardAnimTime = 0.5f;
    public float letterAnimTime = 0.4f;
    public float letterDelay = 0.1f; 
    public float festAnimTime = 0.4f;

    [Header("Rung chấn ")]
    public float shakeDuration = 0.25f;
    public float shakeStrength = 20f; 

    [Header("Độ nảy lố ")]
    public float boardOvershoot = 1.2f;  
    public float letterOvershoot = 2.0f; 
    public float festOvershoot = 3.0f;   

    private void OnEnable()
    {
        PlayWinAnimation();
    }

    private void OnDisable()
    {

        transform.DOKill(true);
        if (board != null) board.DOKill(true);
        if (festWord != null) festWord.DOKill(true);
        foreach (var letter in smashLetters)
        {
            if (letter != null) letter.DOKill(true);
        }
    }

    public void PlayWinAnimation()
    {

        if (board != null) board.localScale = Vector3.zero;
        if (festWord != null) festWord.localScale = Vector3.zero;
        foreach (var letter in smashLetters)
        {
            if (letter != null) letter.localScale = Vector3.zero;
        }

        Sequence winSeq = DOTween.Sequence();

           if (board != null)
        {
            winSeq.Append(board.DOScale(Vector3.one, boardAnimTime).SetEase(Ease.OutBack, boardOvershoot));
        }

        float letterStartTime = boardAnimTime - 0.2f;
        for (int i = 0; i < smashLetters.Length; i++)
        {
            if (smashLetters[i] != null)
            {
                winSeq.Insert(letterStartTime + (i * letterDelay),
                    smashLetters[i].DOScale(Vector3.one, letterAnimTime).SetEase(Ease.OutBack, letterOvershoot));
            }
        }

        float festStartTime = letterStartTime + (smashLetters.Length * letterDelay) + 0.1f;
        if (festWord != null)
        {

            winSeq.Insert(festStartTime, festWord.DOScale(Vector3.one, festAnimTime).SetEase(Ease.OutBack, festOvershoot));
        }


        float shakeStartTime = festStartTime + festAnimTime;
        winSeq.InsertCallback(shakeStartTime, () =>
        {

            if (board != null)
                board.DOShakePosition(shakeDuration, new Vector3(0, shakeStrength, 0), 20, 90, false, true);


            foreach (var letter in smashLetters)
            {
                if (letter != null)
                    letter.DOShakePosition(shakeDuration, new Vector3(0, shakeStrength, 0), 20, 90, false, true);
            }
        });
    }
}