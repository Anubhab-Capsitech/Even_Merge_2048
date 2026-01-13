using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ADSimulator : MonoBehaviour
{
	public Text m_timer;

	public int m_count = 5;

	private void Start()
	{
		Sequence _sequence = DOTween.Sequence();
        _sequence.AppendInterval(1f);
        _sequence.AppendCallback(delegate
		{
			this.m_count--;
			this.m_timer.text = this.m_count.ToString();
			if (this.m_count == 0)
			{
				DialogManager.GetInstance().Close(null);
				JavaInvokeCShape.GetInstance().OnAdsComplateCallback("win");
			}
		});
        _sequence.SetLoops(5);
        _sequence.SetTarget(this.m_timer);
	}

	private void Update()
	{
	}

	private void OnDestroy()
	{
		DOTween.Kill(this.m_timer, false);
	}

	public void OnClickClose()
	{
		DialogManager.GetInstance().Close(null);
		JavaInvokeCShape.GetInstance().OnAdsCancelCallback("win");
	}
}
