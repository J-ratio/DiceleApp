using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

public class CoinsManager : MonoBehaviour
{
	//References
	[Header ("UI references")]
	[SerializeField] GameObject animatedCoinPrefab;
	[SerializeField] Transform[] target;
	[SerializeField] Vector3 collectedCoinPosition;

	[Space]
	[Header ("Available coins : (coins to pool)")]
	[SerializeField] int maxCoins;
	Queue<GameObject> coinsQueue = new Queue<GameObject> ();


	[Space]
	[Header ("Animation settings")]
	[SerializeField] [Range (0.5f, 0.9f)] float minAnimDuration;
	[SerializeField] [Range (0.9f, 2f)] float maxAnimDuration;

	[SerializeField] Ease easeType;
	[SerializeField] float spread;



	private int _c = 0;

	void Awake ()
	{
		//prepare pool
		PrepareCoins ();
	}

	void PrepareCoins ()
	{
		GameObject coin;
		for (int i = 0; i < maxCoins; i++) {
			coin = Instantiate (animatedCoinPrefab);
			coin.transform.parent = transform;
			coin.SetActive (false);
			coinsQueue.Enqueue (coin);
		}
	}

	void Animate (int temp)
	{
		for (int i = 0; i < maxCoins; i++) {
			//check if there's coins in the pool
			if (coinsQueue.Count > 0) {
				//extract a coin from the pool
				GameObject coin = coinsQueue.Dequeue ();
				coin.SetActive (true);

				//move coin to the collected coin pos
				coin.transform.position = collectedCoinPosition + new Vector3 (Random.Range (-spread, spread), 0f, 0f);

				//animate coin to target position
				float duration = Random.Range (minAnimDuration, maxAnimDuration);
				coin.transform.DOMove (target[temp].position, duration)
				.SetEase (easeType)
				.OnComplete (() => {
					//executes whenever coin reach target position
					coin.SetActive (false);
					coinsQueue.Enqueue (coin);

				});
			}
		}
	}

	public void AddCoins ( int temp)
	{
		Animate (temp);
	}
}
