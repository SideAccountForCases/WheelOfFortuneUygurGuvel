using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RewardSlot : SlotBase
{
	private int value;
	private int tempValue = 0; //JUST FOR THE TEXTANIMATION

	public override void Init(SlotInfo slotInfo)
	{
		value = slotInfo.value;
		base.Init(slotInfo);
	}

	public void AddValue(int value)
	{
		m_Image.transform.DOScale(0.25f, 0.125f).SetEase(Ease.InOutQuad).SetLoops(2, LoopType.Yoyo).SetRelative();

		this.value += value;
		SetText();
	}

	protected override void SetText()
	{
		DOTween.To(() => tempValue, x => tempValue = x, value, 1).SetEase(Ease.OutSine)
			.SetDelay(0.7f)
			.OnStart(() =>
			{
				StartCoroutine(IEValueUpAnimation());
			});

	}

	private IEnumerator IEValueUpAnimation()
	{
		float animRate = Time.fixedDeltaTime * 2;

		while (tempValue < value)
		{
			yield return new WaitForSeconds(animRate);
			valueText.text = tempValue.ToString();
		}

		valueText.text = tempValue.ToString();
		ActionManager.ZoneComplete?.Invoke();
	}

	protected override void OnZoneComplete()
	{
	}

	public Transform SpritePoint()
	{
		return m_Image.transform;
	}
}
