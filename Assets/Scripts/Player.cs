using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	// Spaceshipコンポーネント
	Spaceship spaceship;
	float swipeSpeed;
	//Touch touch;
	Vector2 prevMousePos;
	Vector2 currentMousePos;

	IEnumerator Start ()
	{
		// Spaceshipコンポーネントを取得
		spaceship = GetComponent<Spaceship> ();
		prevMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		while (true) {
			
			// 弾をプレイヤーと同じ位置/角度で作成
			if(Input.GetMouseButton(0)){
				spaceship.Shot (transform);
			}

			// ショット音を鳴らす
			GetComponent<AudioSource>().Play();
			
			// shotDelay秒待つ
			yield return new WaitForSeconds (spaceship.shotDelay);
		}
	}
		void Update ()
	{   
		prevMousePos = currentMousePos;
		currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		float swipeSpeed = (currentMousePos - prevMousePos).magnitude / Time.deltaTime;
	
		Vector2 direction = (currentMousePos - prevMousePos).normalized;

		// 移動の制限

		if (Input.GetMouseButton (0)){
			Move (direction, swipeSpeed);
		}
		
	}

	// 機体の移動
	void Move (Vector2 direction, float swipeSpeed)
	{
		// 画面左下のワールド座標をビューポートから取得
		Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
		
		// 画面右上のワールド座標をビューポートから取得
		Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
		
		// プレイヤーの座標を取得
		Vector2 pos = transform.position;
		//Debug.Log (pos);
		// 移動量を加える
		pos += direction  * spaceship.speed * swipeSpeed * (Time.deltaTime);
		//Debug.Log ("Time" + Time.deltaTime	);
		// プレイヤーの位置が画面内に収まるように制限をかける
		pos.x = Mathf.Clamp (pos.x, min.x, max.x);
		pos.y = Mathf.Clamp (pos.y, min.y, max.y);
		// 制限をかけた値をプレイヤーの位置とする
		transform.position = pos;
	}
	
	// ぶつかった瞬間に呼び出される
	void OnTriggerEnter2D (Collider2D c)
	{
		// レイヤー名を取得
		string layerName = LayerMask.LayerToName(c.gameObject.layer);

		// レイヤー名がBullet (Enemy)またはEnemyの場合は爆発
		if( layerName == "Bullet (Enemy)" || layerName == "Enemy")
		{
			// Managerコンポーネントをシーン内から探して取得し、GameOverメソッドを呼び出す
			FindObjectOfType<Manager>().GameOver();

			// 爆発する
			spaceship.Explosion();
		
			// プレイヤーを削除
			Destroy (gameObject);
		}
	}
}