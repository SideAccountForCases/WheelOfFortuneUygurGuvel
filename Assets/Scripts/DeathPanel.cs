using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DeathPanel : MonoBehaviour
{
	[SerializeField] private Transform deathCard;
	[SerializeField] private Image outerFrame;

	[SerializeField] private Button giveUpButton;
	[SerializeField] private Button reviveButton;
	[SerializeField] private Button rewardedReviveButton;

	private Tween outerFrameBreathAnim;
	private Tween deathCardAnim;

	private void Awake()
	{
		ActionManager.Fail += OnFail;
		gameObject.SetActive(false);

		giveUpButton.onClick.AddListener(OnGiveUp);
		reviveButton.onClick.AddListener(OnRevive);
		rewardedReviveButton.onClick.AddListener(OnRewardedRevive); // We can customize this button for rewarded ads. 
	}

	private void OnRevive()
	{
		ActionManager.Revive?.Invoke();
		PanelClose();
	}

	private void OnGiveUp()
	{
		ActionManager.GiveUp?.Invoke();
		PanelClose();
	}

	private void OnRewardedRevive()
	{
		ActionManager.Revive?.Invoke();
		PanelClose();
	}

	private void OnFail()
	{
		gameObject.SetActive(true);

		outerFrameBreathAnim = outerFrame.DOFade(0.05f, 0.85f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo).From(0.004f);

		deathCard.transform.DOScale(1, 1.7f).SetEase(Ease.OutBack, 2.5f)
			.From(Vector3.zero)
			.OnComplete(() =>
			{
				deathCardAnim = deathCard.transform.DOScale(1.125f, 0.85f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
			});
	}

	private void PanelClose()
	{
		outerFrameBreathAnim.Kill();
		deathCardAnim.Kill();

		gameObject.SetActive(false);
	}
}
