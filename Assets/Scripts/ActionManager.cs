using System;
using UnityEngine;

public static class ActionManager
{
	public static Action OnSpinButtonPressed { get; set; }
	public static Action Fail { get; set; }
	public static Action ZoneComplete { get; set; }
	public static Action GiveUp { get; set; }
	public static Action Revive { get; set; }
	public static Action<int> StartNewZone { get; set; }
	public static Func<Transform, RewardSlot> GetRewardSlot { get; set; }
	public static Func<ItemType, Sprite> GetSlotSprite { get; set; }
	public static Func<Transform, Sprite, Transform> GetRewardSprite { get; set; }
	public static Func<ZoneFactor, WheelProperties> GetWheelProperties { get; set; }
	public static Func<WheelSlot[]> WheelSlots { get; set; } //This information exists solely to determine how many slots there are. This number may be necessary for us in the future to maintain a sustainable code structure.
}
