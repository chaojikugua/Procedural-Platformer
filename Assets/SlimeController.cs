﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : MonoBehaviour {

	// Use this for initialization
	Animator animator;
	public float moveSpeed = 1f;
	public float moveTime = 1f;
	public float idleTime = 1f;
	public int hp = 1;
	public int strength = 1;
	public LayerMask walls;

	public AnimationCurve xSpeed;

	bool dead = false;

	Direction currentDirection;
	void Start () {
		animator = gameObject.GetComponent<Animator> ();
		currentDirection = Random.value < 0.5f ? Direction.Left : Direction.Right;
		currentDirection = Direction.Right;

		StartCoroutine (MoveSelector());

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	enum Direction {
		Left,
		Right
	}


	void SwitchDirection() {
		currentDirection = currentDirection == Direction.Left ? Direction.Right : Direction.Left;	
	}

	IEnumerator MoveSelector() {
		while (!dead) {
			StartCoroutine(Move(currentDirection));
			yield return new WaitForSeconds (idleTime + moveTime + Random.value);
		}
	}
		
	IEnumerator Move(Direction d) {
		float startTime = Time.time;
		Vector3 startPos = gameObject.transform.position;
		Vector3 rayDirection = d == Direction.Left ? -Vector3.right : Vector3.right;
		RaycastHit2D hit = Physics2D.Raycast (transform.position, rayDirection, moveSpeed, walls);
		float xChange = d == Direction.Left ? -moveSpeed : moveSpeed;
		if (hit.collider != null) {
			// A wall is in the way
//			Debug.DrawLine(transform.position, hit.point, Color.red);
//			Debug.Break ();
			xChange = (hit.point.x - transform.position.x);
			float halfWidth = gameObject.GetComponent<BoxCollider2D> ().size.x / 2;
			xChange += d == Direction.Right ? -halfWidth : halfWidth;
		}

		Vector2 newScale = gameObject.transform.localScale;
		newScale.x = d == Direction.Left ? -1 : 1;
		gameObject.transform.localScale = newScale;

		animator.SetBool ("Idle", false);

		Vector3 finalPos = gameObject.transform.position;
		finalPos.x += xChange;
		bool onEdge = false;
		while (Time.time < startTime + moveTime) {
			if (OnEdge()) {
				onEdge = true;
				break;
			}
			if (dead) {
				break;
			}
			gameObject.transform.position = Vector3.Lerp (
				startPos,
				finalPos,
				xSpeed.Evaluate( GetBetweenValue(startTime, startTime + moveTime, Time.time))
			);
			yield return null;
		}
		if (hit.collider != null || onEdge) {
			SwitchDirection ();
		}
		animator.SetBool ("Idle", true);
	}

	bool OnEdge() {
		RaycastHit2D hit = Physics2D.Linecast (transform.position, transform.Find ("GroundCheck").position, walls);

		if (hit.collider == null) {
			return true;
		}
		return false;
	}
	public void GetHurt(int damage = 1) {
		hp -= damage;
		if (hp <= 0) {
			Die ();
		}
	}

	void Die() {
		animator.SetBool ("Dead", true);
		Destroy(gameObject.GetComponent<BoxCollider2D>());
		dead = true;
	}

	void OnCollisionEnter2D(Collision2D col) {
		if (col.gameObject.tag == "Player") {
			bool fromTop = false;
			foreach (ContactPoint2D c in col.contacts) {
				if (c.normal.y < 0) {
					fromTop = true;

				}
				if (fromTop) {
					GetHurt ();
					col.gameObject.GetComponent<PlayerController> ().BounceOffEnemy (1f);
				} else {
					col.gameObject.GetComponent<PlayerController> ().TakeDamage (strength);
				}

			}
		} else {
			print (col.gameObject);
		}
	}

	float GetBetweenValue(float min, float max, float inputValue) {
		return(inputValue - min) / (max - min);
	}
}