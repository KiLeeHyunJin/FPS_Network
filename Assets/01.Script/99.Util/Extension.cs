using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class Extension
{
    public static void ReStartCoroutine<T>(this T gameObject, IEnumerator routine, ref Coroutine co) where T : MonoBehaviour
    {
		if (co != null)
			gameObject.StopCoroutine(co);
		co = gameObject.StartCoroutine(routine);
    }
    /// <summary>
    /// 해당 레이어의 비트가 올라와있는지 확인해서 반환합니다.
    /// </summary>
    /// <param name="layerMask"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static bool ContainCheck(this LayerMask layerMask, int layer)
    {
        return ((1 << layer) & layerMask) != 0;
    }
	/// <summary>
	/// 해당 레이어 플래그를 올려줍니다.
	/// </summary>
	/// <param name="layerMask"></param>
	/// <param name="layer"></param>
	public static void Contain(this ref LayerMask layerMask, int layer)
	{
		layerMask |= 1 << layer;
	}
	/// <summary>
	/// 해당 컴포넌트가 존재한다면 찾아서 반환하고 없을경우 추가합니다.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="go"></param>
	/// <returns></returns>
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
	{
		return Utils.GetOrAddComponent<T>(go);
	}

	public static void BindEvent(this GameObject go, Action action, Define.UIEvent type = Define.UIEvent.Click)
	{
		UI_Base.BindEvent(go, action, type);
	}

	static System.Random _rand = new System.Random();

	/// <summary>
	/// 버블소트를 이용해 모든 요소를 섞습니다.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	public static void Shuffle<T>(this IList<T> list)
	{
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = _rand.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}
	/// <summary>
	/// 리스트의 크기 범위 내 랜덤한 인덱스를 반환합니다.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	/// <returns></returns>
	public static T GetRandom<T>(this IList<T> list)
	{
		int index = _rand.Next(list.Count);
		return list[index];
	}

	public static void ResetVertical(this ScrollRect scrollRect)
	{
		scrollRect.verticalNormalizedPosition = 1;
	}

	public static void ResetHorizontal(this ScrollRect scrollRect)
	{
		scrollRect.horizontalNormalizedPosition = 1;
	}
}