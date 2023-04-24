using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;


[CreateAssetMenu(fileName = "ZoneInfos", menuName = "WheelOfFortune/ZoneInfos")]
public class ZoneInfoScriptable : ScriptableObject
{
	[Header("ZoneFactors")]
	[SerializeField] private int safeZoneFactor = 5;
	[SerializeField] private int superZoneFactor = 30;
	[SerializeField] private int superZoneRewardMultiplier = 3;

	[Header("InflationSettings")]
	[SerializeField] private RewardInflation[] rewardInflations;

	[SerializeField] Zone[] zoneInfos;

	public Zone GetZone(int zoneIndex)
	{
		return zoneInfos[zoneIndex];
	}

	public int CalculateSlotValue(int zoneIndex, ZoneFactor zoneFactor, ItemType itemType)
	{
		RewardInflation inflation = Array.Find(rewardInflations, ri => ri.type == itemType);
		Vector2 minMaxValue = inflation.minMaxValues;
		int reward = zoneIndex.Remap(0, superZoneFactor - 1, (int)minMaxValue.x, (int)minMaxValue.y);

		if (zoneFactor == ZoneFactor.Gold)
			reward *= superZoneRewardMultiplier;

		return reward;
	}

#if UNITY_EDITOR

	[Button]
	private void CreateZones(int zoneAmount = 30)
	{
		int slotAmount = ActionManager.WheelSlots().Length;
		zoneInfos = new Zone[zoneAmount];

		ItemType[] itemTypes = (ItemType[])Enum.GetValues(typeof(ItemType));

		for (int i = 0; i < zoneAmount; i++)
		{
			ZoneFactor zoneFactor = CalculateZoneFactor(i);
			bool isSafeZone = zoneFactor == ZoneFactor.Bronze ? false : true;

			SlotInfo[] zoneSlots = new SlotInfo[slotAmount];
			ItemType[] rndItems = GetRandomItems(itemTypes, slotAmount, isSafeZone);

			for (int j = 0; j < slotAmount; j++)
			{
				SlotInfo slotInfo = new SlotInfo();

				slotInfo.type = rndItems[j];
				slotInfo.value = CalculateSlotValue(i, zoneFactor, slotInfo.type);
				zoneSlots[j] = slotInfo;
			}

			zoneInfos[i].slots = zoneSlots;
			zoneInfos[i].zoneFactor = zoneFactor;
		}

	}

	private ZoneFactor CalculateZoneFactor(int zoneIndex)
	{
		ZoneFactor zoneFactor = ZoneFactor.Bronze;

		if ((zoneIndex + 1) % safeZoneFactor == 0)
			zoneFactor = ZoneFactor.Silver;

		if ((zoneIndex + 1) % superZoneFactor == 0)
			zoneFactor = ZoneFactor.Gold;

		return zoneFactor;
	}

	public ItemType[] GetRandomItems(ItemType[] itemTypes, int slotAmount, bool isSafeZone = false)
	{
		List<ItemType> items = new List<ItemType>();
		List<ItemType> itemStack = new List<ItemType>();

		itemStack.AddRange(itemTypes);
		itemStack.Remove(ItemType.Death);

		while (items.Count < slotAmount)
		{
			ItemType item = itemStack[Random.Range(0, itemStack.Count)];

			items.Add(item);
			itemStack.Remove(item);
		}

		if (!isSafeZone && !items.Contains(ItemType.Death))
			items[Random.Range(0, items.Count)] = ItemType.Death;

		return items.ToArray();
	}


#endif

}
