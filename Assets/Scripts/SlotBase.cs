using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotBase : MonoBehaviour
{
	[SerializeField] protected Image m_Image;
	[SerializeField] protected TextMeshProUGUI valueText;

	protected Vector3 initialScale;

	protected SlotInfo myInfo;
	public SlotInfo GetInfo() => myInfo;

	protected virtual void Awake()
	{
		ActionManager.ZoneComplete += OnZoneComplete;
	}

	protected virtual void OnZoneComplete()
	{
		transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.OutSine);
	}

	public virtual void Init(SlotInfo slotInfo)
	{
		myInfo = slotInfo;
		m_Image.sprite = ActionManager.GetSlotSprite(myInfo.type);

		SetText();
	}

	protected virtual void SetText()
	{
		valueText.text = $"{myInfo.value}";
	}
}
