using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SpinButton : MonoBehaviour
{
	[SerializeField] private Image m_Image;

	private Button _button;
	private Vector3 initialScale;

	private void Awake()
	{
		_button = GetComponent<Button>();
		_button.onClick.AddListener(OnClick);

		initialScale = transform.localScale;
		transform.localScale = Vector3.zero;
		_button.interactable = false;

		ActionManager.ZoneComplete += OnZoneComplete;
		ActionManager.StartNewZone += OnNewZone;
	}

	private void OnNewZone(int zoneIndex)
	{
		transform.DOScale(initialScale, 0.4f).SetEase(Ease.OutBack)
			.OnComplete(() =>
			{
				_button.interactable = true;
			});
	}

	private void OnZoneComplete()
	{

	}
	private void OnClick()
	{
		_button.interactable = false;

		ActionManager.OnSpinButtonPressed?.Invoke();
		transform.DOScale(Vector3.zero, 0.35f).SetEase(Ease.InBack);

	}

	public void SetSprite(Sprite sprite)
	{
		m_Image.sprite = sprite;
	}

}
