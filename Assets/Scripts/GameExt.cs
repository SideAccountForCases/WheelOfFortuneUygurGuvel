using System;
using UnityEngine;

public enum ItemType
{
	Money = 0,
	Gold = 1,
	GrenadeElectric = 2,
	GrenadeM67 = 3,
	GrenadeSnowBall = 4,
	HealthShot = 5,
	HealthShotAdrenaline = 6,
	MedkitEaster = 7,
	C4 = 8,
	GrenadeEmp = 9,
	Death = 10,
}

public enum ZoneFactor
{
	Bronze = 0,
	Silver = 1,
	Gold = 2,
}

[Serializable]
public struct SlotProperties
{
	public Sprite sprite;
	public ItemType slotType;
}

[Serializable]
public struct WheelProperties
{
	public ZoneFactor zoneFactor;
	public Sprite wheelMainSprite;
	public Sprite indicatorSprite;
	public Sprite spinButtonSprite;
}

[Serializable]
public struct Zone
{
	public ZoneFactor zoneFactor;
	public SlotInfo[] slots;
}

[Serializable]
public struct SlotInfo
{
	public ItemType type;
	public int value;
}

[Serializable]
public struct RewardInflation
{
	public ItemType type;
	public Vector2 minMaxValues;
}

public static class GameExt
{
	public static int Remap(this int val, int currentMin, int currentMax, int newMin, int newMax)
	{
		return newMin + (val - currentMin) * (newMax - newMin) / (currentMax - currentMin);
	}
}

