using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class NumberObj : MonoBehaviour
{
  
	private int x;

	private int y;

	private Game.NumType type;

	private int tagNumber;

	[HideInInspector]
	public Game game;

	private NumberControl numberComponent;

	private NumberClear clearCompoent;

	private IEnumerator moveCoroutine;

	public int X
	{
		get
		{
			return this.x;
		}
		set
		{
			this.x = value;
		}
	}

	public int Y
	{
		get
		{
			return this.y;
		}
		set
		{
			this.y = value;
		}
	}

	public Game.NumType Type
	{
		get
		{
			return this.type;
		}
	}

	public NumberControl NumberComponent
	{
		get
		{
			return this.numberComponent;
		}
		set
		{
			this.numberComponent = value;
		}
	}

	public NumberClear ClearCompoent
	{
		get
		{
			return this.clearCompoent;
		}
	}

	public bool CanSetNumber()
	{
		return this.numberComponent != null;
	}

	public void Init(int _x, int _y, Game _game, Game.NumType _type)
	{
		this.x = _x;
		this.y = _y;
		this.game = _game;
		this.type = _type;
	}

	public void ChangeNewType(Game.NumType _type)
	{
		this.type = _type;
	}

	public void Awake()
	{
		this.numberComponent = base.GetComponent<NumberControl>();
		this.clearCompoent = base.GetComponent<NumberClear>();
	}

	public bool CanClear()
	{
		return this.clearCompoent != null;
	}

	public void Move(int newX, int newY, float time)
	{
		if (this.moveCoroutine != null)
		{
			base.StopCoroutine(this.moveCoroutine);
		}
		this.moveCoroutine = this.MoveCoroutinere(newX, newY, time);
		base.StartCoroutine(this.moveCoroutine);
		this.x = newX;
		this.y = newY;
	}

	private IEnumerator MoveCoroutinere(int newX, int newY, float time)
	{
		Vector2 numPos = Vector2.zero;
		while (true)
		{
			int num = 0;
			float num2 = 0.0f;
			Vector2 a = Vector2.zero;
			if (num != 0)
			{
				if (num != 1)
				{
					break;
				}
				num2 += Time.deltaTime;
			}
			else
			{
				a = base.transform.position;
				numPos = this.game.GetNumPos((float)newX, (float)newY);
				num2 = 0f;
			}
			if (num2 >= time)
			{
				goto Block_3;
			}
			base.transform.position = Vector2.Lerp(a, numPos, num2 / time);
			yield return 0;
		}
		yield break;
		Block_3:
		base.transform.position = numPos;
		yield break;
	}

	public bool CanMove()
	{
		return this.type != Game.NumType.EMPTY;
	}

	public void OnMouseDown()
	{
		base.transform.localScale = new Vector3(90f, 90f, 0f);
	}

	public void OnMouseUp()
	{
		base.transform.localScale = new Vector3(100f, 100f, 0f);
		this.game.AddNumber(base.transform.position.x, base.transform.position.y);
	}
}
