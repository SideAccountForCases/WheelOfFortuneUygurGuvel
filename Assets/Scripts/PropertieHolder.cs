using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SlotProperties", menuName = "WheelOfFortune/SlotProperties")]
public class PropertieHolder : ScriptableObject
{
	public SlotProperties[] slotProperties;
	public WheelProperties[] wheelProperties;
}
