using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WheelController : MonoBehaviour
{
	[SerializeField] private Image wheelSprite;
	[SerializeField] private Image wheelIndicatorSprite;
	[SerializeField] private Transform contentHolder;
	[SerializeField] private WheelSlot[] wheelSlots;
	[SerializeField] private ZoneInfoScriptable zoneInfoScriptable;
	[SerializeField] private SpinButton spinButton;

	private int zoneIterator = 0;
	private List<RewardSlot> rewardSlots = new List<RewardSlot>();

	private Zone activeZone;
	private ZoneFactor currentZoneFactor = ZoneFactor.Bronze;

	private void OnValidate()
	{
		ActionManager.WheelSlots += () => wheelSlots;
	}

	private void Awake()
	{
		ActionManager.OnSpinButtonPressed += OnSpinButtonPressed;
		ActionManager.ZoneComplete += InitNewZone;
		ActionManager.Fail += OnFail;
		ActionManager.GiveUp += OnGiveUp;
		ActionManager.Revive += OnRevive;

	}

	private void Start()
	{
		OnZoneFactorChanged(currentZoneFactor);
		InitNewZone();
	}

	private void InitNewZone()
	{
		activeZone = zoneInfoScriptable.GetZone(zoneIterator);

		if (currentZoneFactor != activeZone.zoneFactor)
			OnZoneFactorChanged(activeZone.zoneFactor);

		currentZoneFactor = activeZone.zoneFactor;

		wheelSprite.transform.DOLocalRotate(Vector3.zero, 0.85f).SetOptions(false)
			.OnComplete(() =>
			{
				InitSlots();
				ActionManager.StartNewZone?.Invoke(zoneIterator++);
			});
	}

	private void InitSlots()
	{
		for (int i = 0; i < wheelSlots.Length; i++)
		{
			wheelSlots[i].Init(activeZone.slots[i]);
		}
	}

	private void ClearSlots()
	{
		for (int i = 0; i < wheelSlots.Length; i++)
		{
			wheelSlots[i].ClearSlot();
		}
	}

	private void OnZoneFactorChanged(ZoneFactor zoneFactor)
	{
		WheelProperties properties = ActionManager.GetWheelProperties(zoneFactor);
		spinButton.SetSprite(properties.spinButtonSprite);
		wheelIndicatorSprite.sprite = properties.indicatorSprite;
		wheelSprite.sprite = properties.wheelMainSprite;
	}

	private void OnSpinButtonPressed()
	{
		GetRandomReward();
	}

	private void GetRandomReward()
	{
		int randomIndex = Random.Range(0, wheelSlots.Length);

		float angleStep = 360f / wheelSlots.Length;
		float rotateAmount = angleStep * randomIndex;

		WheelSlot randomSlot = wheelSlots[randomIndex];
		Rotate(rotateAmount, randomSlot, angleStep);
	}

	private void Rotate(float rotateAmount, WheelSlot slot, float angleStep)
	{
		float doTime = 5;
		int extraRotateAmount = 4;
		float outBackEffectAmount = (360 / wheelSlots.Length) * 0.35f;  //this magic number is multiplier of the outbackEffect

		Ease ease = Ease.InOutQuad;

		wheelSprite.transform.DORotate(Vector3.forward * (rotateAmount + (360 * extraRotateAmount) + (outBackEffectAmount)), doTime).SetRelative()
			.OnStart(() =>
			{
				StartCoroutine(IEIndicatorShake(doTime, ease, angleStep, (rotateAmount + outBackEffectAmount) + (360 * extraRotateAmount)));
			})
			.SetEase(ease)
			.OnComplete(() => // return of the outBack;
			{
				float effectDoTime = doTime / extraRotateAmount * 0.5f;

				wheelSprite.transform.DORotate(Vector3.forward * -outBackEffectAmount, effectDoTime).SetRelative()
				.SetEase(Ease.InSine)
				.OnComplete(() =>
				{
					SpinCompleted(slot);
				});

			});

	}

	private IEnumerator IEIndicatorShake(float time, Ease ease, float angleStep, float totalRotateAmount)
	{
		WaitForFixedUpdate wait = new WaitForFixedUpdate();

		float rotateAmount = 0;
		float temp = 0;
		bool isMoving = true;

		DOTween.To(() => rotateAmount, x => rotateAmount = x, totalRotateAmount, time).SetEase(ease)
			.OnComplete(() => isMoving = false);

		while (isMoving)
		{
			yield return wait;

			if (rotateAmount - temp >= angleStep)
				temp = rotateAmount;

			wheelIndicatorSprite.transform.localEulerAngles = -Vector3.forward * (rotateAmount - temp);
		}


		wheelIndicatorSprite.transform.DOLocalRotate(Vector3.zero, 1).SetEase(Ease.OutElastic).From(Vector3.forward * -angleStep * 0.5f);

	}

	private void SpinCompleted(WheelSlot slot)
	{
		SlotInfo info = slot.GetInfo();

		if (info.type == ItemType.Death)
			ActionManager.Fail?.Invoke();
		else
			StartCoroutine(GiveReward(slot, info));
	}

	private IEnumerator GiveReward(WheelSlot slot, SlotInfo info)
	{
		Sprite sprite = ActionManager.GetSlotSprite(info.type);
		int spriteAmount = Random.Range(4, 6);

		List<Transform> sprites = new List<Transform>();
		float doTime = 0.7f;

		for (int i = 0; i < spriteAmount; i++)
		{
			Transform rewardSprite = ActionManager.GetRewardSprite(transform, sprite);
			sprites.Add(rewardSprite);
			rewardSprite.transform.position = slot.transform.position;

			Vector3 rndPos = Random.insideUnitCircle * 60;
			rewardSprite.DOLocalMove(rndPos, doTime).SetRelative().SetEase(Ease.OutBack);
		}

		RewardSlot rewardSlot = rewardSlots.Find(rS => rS.GetInfo().type == info.type);

		if (rewardSlot == null)
			rewardSlot = CreateNewRewardSlot(info);
		else
			rewardSlot.AddValue(info.value);

		yield return new WaitForSeconds(doTime);

		foreach (Transform item in sprites)
		{
			yield return new WaitForSeconds(0.05f);

			Transform targetPoint = rewardSlot.SpritePoint();
			item.SetParent(targetPoint);

			Vector3[] path = new Vector3[3];
			path[0] = item.localPosition;
			path[2] = Vector3.zero;
			path[1] = (path[0] + path[2]) * 0.5f + Vector3.up * Random.Range(85f, 120f);

			item.DOLocalPath(path, 0.7f, PathType.CatmullRom).SetEase(Ease.OutSine)
				.OnStart(() =>
				{
					item.DOScale(0.85f, 0.7f).SetEase(Ease.OutSine)
					.OnComplete(() => Destroy(item.gameObject)); //We can use objectPooling for those sprites. But i dont have time to implement this. (I know how to make!!1);
				});
		}

	}

	private RewardSlot CreateNewRewardSlot(SlotInfo info)
	{
		RewardSlot newRewardSlot = ActionManager.GetRewardSlot(contentHolder);
		newRewardSlot.Init(info);
		rewardSlots.Add(newRewardSlot);

		return newRewardSlot;
	}

	private void OnFail()
	{
		wheelSprite.transform.DOScale(0, 0.5f).SetEase(Ease.InSine);
		wheelIndicatorSprite.transform.DOScale(0, 0.5f).SetEase(Ease.InSine);
	}

	private void OnRevive()
	{
		ClearSlots();

		zoneIterator--;
		InitNewZone();

		wheelSprite.transform.DOScale(1, 0.5f).SetEase(Ease.OutSine);
		wheelIndicatorSprite.transform.DOScale(1, 0.5f).SetEase(Ease.OutSine);

	}

	private void OnGiveUp()
	{
		for (int i = 0; i < rewardSlots.Count; i++)
		{
			Destroy(rewardSlots[i].gameObject);
		}

		rewardSlots.Clear();
		ClearSlots();

		zoneIterator = 0;
		InitNewZone();

		wheelSprite.transform.DOScale(1, 0.5f).SetEase(Ease.OutSine);
		wheelIndicatorSprite.transform.DOScale(1, 0.5f).SetEase(Ease.OutSine);

	}


#if UNITY_EDITOR

	[Button]
	private void SetSlotPositions(float radius = 200f) //This function just for set the slot postions and get the WheelSlots on editor.
	{
		wheelSlots = GetComponentsInChildren<WheelSlot>();

		int slotAmount = wheelSlots.Length;
		float angleStep = 360f / slotAmount;

		for (int i = 0; i < wheelSlots.Length; i++)
		{
			// Calculate the angleStep for this object
			float angle = i * angleStep;

			// Calculate the x and y coordinates of this object based on the angleStep and radius
			float x = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
			float y = radius * Mathf.Cos(angle * Mathf.Deg2Rad);

			Vector3 position = new Vector3(x, y, 0);

			// Set the position and rotation of the object
			wheelSlots[i].transform.localPosition = position;
			wheelSlots[i].transform.localRotation = Quaternion.Euler(Vector3.forward * -angle);
		}

	}

#endif
}
