using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinMagnet : MonoBehaviour
{
	[SerializeField]
	GameObject playerObject;
	[SerializeField]
	public bool magnetted = false;
	Vector2 playerDirection;
	float timeStamp;
	Rigidbody2D rb;

	void Start()
    {
		rb = GetComponent<Rigidbody2D>();
	}

	void Update()
	{
		if (magnetted)
		{
			playerDirection = -(transform.position - playerObject.transform.position).normalized;
			rb.velocity = new Vector2(playerDirection.x, playerDirection.y) * 30f * (Time.time / timeStamp);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.name.Equals("CoinMagnet") )
		{
			playerObject = collision.transform.parent.gameObject;
			timeStamp = Time.time;
			magnetted = playerObject.GetComponent<PlayerControl>().magnetOn;
		}
	}

	private void OnDisable()
	{
		magnetted = false;
		playerObject = null;
	}
}
