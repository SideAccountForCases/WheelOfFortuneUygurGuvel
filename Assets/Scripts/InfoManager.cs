using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;

public class InfoManager : MonoBehaviour
{
	[SerializeField] private ZoneInfoScriptable zoneInfoScriptable;
	[SerializeField] private PropertieHolder propertieHolder;
	[SerializeField] private GameObject rewardSlotPrefab;
	[SerializeField] private GameObject collectEffectPrefab;

	private void Awake()
	{
		ActionManager.GetRewardSlot = GetRewardSlot;
		ActionManager.GetSlotSprite = GetSlotSprite;
		ActionManager.GetWheelProperties = GetWheelProperties;
		ActionManager.GetRewardSprite = GetRewardSprite;
	}

	public Sprite GetSlotSprite(ItemType type)
	{
		SlotProperties slotPropertie = Array.Find(propertieHolder.slotProperties, sl => sl.slotType == type);
		return slotPropertie.sprite;
	}

	public RewardSlot GetRewardSlot(Transform parent)
	{
		return (Instantiate(rewardSlotPrefab, parent)).GetComponent<RewardSlot>();
	}

	public Transform GetRewardSprite(Transform parent, Sprite sprite)
	{
		GameObject collectSprite = Instantiate(collectEffectPrefab, parent);
		collectSprite.GetComponent<Image>().sprite = sprite;
		return collectSprite.transform;
	}

	public WheelProperties GetWheelProperties(ZoneFactor zoneFactor)
	{
		return Array.Find(propertieHolder.wheelProperties, wp => wp.zoneFactor == zoneFactor);
	}

}
