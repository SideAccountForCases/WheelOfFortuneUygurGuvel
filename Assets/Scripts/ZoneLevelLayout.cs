using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ZoneLevelLayout : MonoBehaviour
{
	[SerializeField] private int textOffset;

	[SerializeField] private HorizontalLayoutGroup contentHolder;
	[SerializeField] private TextMeshProUGUI[] texts;

	private RectTransform contentHolderRect;

	private void Awake()
	{
		ActionManager.StartNewZone += OnStartNewZone;
		contentHolderRect = contentHolder.GetComponent<RectTransform>();
	}

	private void OnStartNewZone(int zoneIndex)
	{
		DOTween.To(() => contentHolder.padding.left, x => contentHolder.padding.left = x, zoneIndex * -textOffset, 0.4f).SetEase(Ease.InSine)
			.OnUpdate(() =>
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(contentHolderRect);
			});
	}


#if UNITY_EDITOR

	[SerializeField] private GameObject textPrefab;

	[Button]
	public void CreateTextObjects(int amount)
	{
		for (int i = 0; i < texts.Length; i++)
		{
			DestroyImmediate(texts[i].gameObject);
		}

		texts = new TextMeshProUGUI[amount];

		for (int i = 0; i < amount; i++)
		{
			GameObject textObj = PrefabUtility.InstantiatePrefab(textPrefab, contentHolder.transform) as GameObject;
			texts[i] = textObj.GetComponent<TextMeshProUGUI>();
			texts[i].text = $"{i + 1}";
		}
	}

#endif
}
