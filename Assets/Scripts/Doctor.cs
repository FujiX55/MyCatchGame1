using UnityEngine;
using System.Collections;

public class Doctor : Token {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// プレイヤとの接触チェック
	public void OnTriggerEnter2D(Collider2D other)
	{
		// レイヤ名を取得
		string name = LayerMask.LayerToName (other.gameObject.layer);

		if (name == "Player")
		{
			this.AddVelocityXY(0.0f, 4.0f);
		}
	}


}
