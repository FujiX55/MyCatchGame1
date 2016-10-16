﻿using UnityEngine;
using System.Collections;

public class GameMgr : MonoBehaviour 
{	
	// 状態
	enum eState
	{
		Main,		// メイン
		StageClear,	// ステージクリア
		GameOver	// ゲームオーバー
	}

	// 現在のステージ番号
	public int nStage = 1;

	// プレイヤへの参照
	Player player_ = null;

	// GamePad
	public Pad pad;

	GameObject cam;

	public FieldMgr field;

	// 開始
	void Start()
	{
		pad = new Pad();

		field = new FieldMgr ();

		// 壁オブジェクト管理生成
		Wall.parent = new TokenMgr<Wall> ("Wall", 128);

		// 移動床オブジェクト管理作成
		FloorMove.parent = new TokenMgr<FloorMove> ("FloorMove", 32);

		// パーティクル管理作成
		Particle.parent = new TokenMgr<Particle> ("Particle", 32);

		// トゲ監理生成
		Spike.parent = new TokenMgr<Spike> ("Spike", 32);

		// マップデータの読み込み
		Load (nStage);
	}

	// プレイヤのゲーム状態をチェックする
	void CheckPlayerGameState()
	{
		if (player_ == null) 
		{
			// プレイヤへの参照を取得する
			GameObject obj = GameObject.Find("Player") as GameObject;

			player_ = obj.GetComponent<Player>();

			cam = GameObject.Find("Main Camera") as GameObject;

			Vector3 pos = cam.transform.position;
			pos.x = player_.X;
			cam.transform.position = pos;
		}

		switch ( player_.GetGameState() ) 
		{
		case Player.eGameState.StageClear:
			// ステージクリア
			_state = eState.StageClear;
			break;

		case Player.eGameState.GameOver:
			// ゲームオーバー
			_state = eState.GameOver;
			break;
		}
	}

	// 更新
	void Update()
	{
		if ( Input.GetKeyDown( KeyCode.Escape ) )
		{
			Application.Quit();
			return;
		}

		if (player_ != null)
		{
			Vector3 pos = cam.transform.position;
			pos.x = player_.X;

			if (( pos.x + cam.transform.localScale.x < field.right )
			&&  ( pos.x - cam.transform.localScale.x > field.left ))
			{
				cam.transform.position = pos;
			}
		}

		pad.Update();

		switch (_state) 
		{
		case eState.Main:
			// プレイヤのゲーム状態をチェックする
			CheckPlayerGameState();
			break;

		case eState.StageClear:
			// ステージクリア
			if ( pad.IsPushed() )
			{
				// PUSH押下で次に進む
				Restore();

				// 次のステージに進む
				nStage++;

				if (nStage > 1)
				{
					// 全ステージクリア
					// ステージ1に戻る
					nStage = 1;
				}

				// マップデータ読み込み
				Load(nStage);

				_state = eState.Main;
			}
			break;

		case eState.GameOver:
			// ゲームオーバー
			if ( pad.IsPushed() )
			{
				// Spaceキーを押したらやり直し
				Restore();

				// マップデータ読み込み
				Load(nStage);

				_state = eState.Main;
			}
			break;
		}
	}

	// 状態
	eState _state = eState.Main;
	
	// マップをロードする
	void Load(int stage)
	{
		// マップデータの読み込み
		field.Load (stage);
	}

	// 各種オブジェクトを全部消す
	void Restore()
	{
		Wall.parent.Vanish ();
		FloorMove.parent.Vanish ();
		Particle.parent.Vanish ();
		Spike.parent.Vanish ();

		// プレイヤを復活させる
		player_.Revive ();

		// 初期状態に戻す
		player_.SetGameState (Player.eGameState.None);
	}

	// ラベルを画面中央に表示
	void DrawLabelCenter(string message)
	{
		// フォントサイズ設定
		Util.SetFontSize (32);

		// 中央揃え
		Util.SetFontAlignment (TextAnchor.MiddleCenter);

		// フォントの位置
		float w = 128;	// 幅
		float h = 32;	// 高さ
		float px = Screen.width / 2 - w / 2;
		float py = Screen.height / 2 - h / 2;

		// フォント描画
		Util.GUILabel (px, py, w, h, message);
	}

	void OnGUI()
	{
		switch (_state) 
		{
		case eState.StageClear:
			DrawLabelCenter("STAGE CLEAR!");
			break;

		case eState.GameOver:
			DrawLabelCenter("GAME OVER");
			break;
		}
	}
}
