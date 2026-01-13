using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class NumberClear : MonoBehaviour
{
	private bool isClearing;

	protected NumberObj numberObj;

	public bool IsClearing
	{
		get
		{
			return this.isClearing;
		}
	}

	public virtual void Clear()
	{
		this.isClearing = true;
		base.StartCoroutine(this.ClearCoroutine());
	}

	private IEnumerator ClearCoroutine()
	{
		int num = 0;
		while (num == 0)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (num != 1)
		{
			yield break;
		}
		UnityEngine.Object.Destroy(base.gameObject);
		yield break;
	}
}
