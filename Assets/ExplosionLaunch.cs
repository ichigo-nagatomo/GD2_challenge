using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ExplosionLaunch : Explosion {
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start() {
        gameManager = FindObjectOfType<GameManager>(); // 自動的にGameManagerを探して取得
        move = new Vector2(1f, 0f);
    }

    // Update is called once per frame
    void Update() {
        explosionTime += Time.deltaTime;

        transform.localScale += new Vector3(move.x * Time.deltaTime, move.y * Time.deltaTime, 0);

        if (explosionTime >= 3f) {
            Destroy(gameObject);
        }

        if (gameManager != null) // gameManagerが正しく取得できているか確認
        {
            gameManager.StartShake(0.5f, 0.1f, 0.2f);
        }
    }
}

