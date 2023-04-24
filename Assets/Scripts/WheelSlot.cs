using DG.Tweening;
using System;
using UnityEngine;

public class WheelSlot : SlotBase
{
	protected override void Awake()
	{
		base.Awake();

		initialScale = transform.localScale;
		transform.localScale = Vector3.zero;
	}

	public override void Init(SlotInfo slotInfo)
	{
		base.Init(slotInfo);
		transform.DOScale(initialScale, 0.4f).SetEase(Ease.OutBack);

		if (slotInfo.type == ItemType.Death)
			valueText.text = string.Empty;
	}

	public void ClearSlot()
	{
		valueText.text = string.Empty;
		transform.localScale = Vector3.zero;
	}
}
