using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class BattleUnit : MonoBehaviour
{
    public PokymonBase _base;
    public int _level;

    [SerializeField] private bool isPlayer;
    public bool IsPlayer => isPlayer;
    
    public Pokymon Pokymon { get; set; }
    private Image pokymonImage;
    private Vector3 initialPosition;
    private Color initialColor;
    [SerializeField] private BattleHUD hud;
    public BattleHUD Hud => hud;

    [SerializeField] private float startTimeAnim = 1.0f,
        attackTimeAnim = 0.3f, hitTimeAnimation = 0.25f, dieTimeAnim = 1.0f;
    
    private void Awake()
    {
        pokymonImage = GetComponent<Image>();
        initialPosition = pokymonImage.transform.localPosition;
        initialColor = pokymonImage.color;
    }

    public void SetupPokymon(Pokymon pokymon)
    {
        Pokymon = pokymon;

        pokymonImage.sprite = (isPlayer ? Pokymon.Base.BackSprite : Pokymon.Base.FrontSprite);
        pokymonImage.color = initialColor;
        
        hud.SetPokymonData(pokymon);
        PlayStartAnimation();
    }

    public void PlayStartAnimation()
    {
        pokymonImage.transform.localPosition = 
            new Vector3(initialPosition.x + (isPlayer ? -1 : 1) * 400, initialPosition.y);
        pokymonImage.transform.DOLocalMoveX(initialPosition.x, startTimeAnim);
    }

    public void PlayAttackAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(pokymonImage.transform.DOLocalMoveX(initialPosition.x + (isPlayer ? 1 : -1) * 60, attackTimeAnim));
        seq.Append(pokymonImage.transform.DOLocalMoveX(initialPosition.x, attackTimeAnim));
    }

    public void PlayReceiveAttackAnimation()
    {
        var seq = DOTween.Sequence();
        for (int i = 0; i < 2; i++)
        {
            seq.Append(pokymonImage.DOColor(Color.gray, hitTimeAnimation));
            seq.Append(pokymonImage.DOColor(initialColor, hitTimeAnimation));   
        }
    }

    public void PlayFaintAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(pokymonImage.transform.DOLocalMoveY(initialPosition.y - 200, dieTimeAnim));
        seq.Join(pokymonImage.DOFade(0, dieTimeAnim));
    }
}
