using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPad : MonoBehaviour {
    [SerializeField] public float coolTime;
    private float interval;
    [SerializeField] public bool canShoot;
    private int life;
    [SerializeField] public ExplosionLaunch explosionLaunchPrefab;
    private SpriteRenderer spriteRenderer;
    private Color startColor;
    private Color coolDownColor;

    // Start is called before the first frame update
    void Start() {
        coolTime = 0;
        interval = 3f;
        canShoot = true;
        life = 3;
        spriteRenderer = GetComponent<SpriteRenderer>();
        startColor = spriteRenderer.color; // 元の色を保存
        coolDownColor = Color.red; // クールタイム中の色
    }

    // Update is called once per frame
    void Update() {
        if (!canShoot) {
            if (coolTime <= interval) {
                coolTime += Time.deltaTime;
                // 色をクールタイムに応じて補間する
                float t = coolTime / interval;
                spriteRenderer.color = Color.Lerp(coolDownColor, startColor, t); // 色を戻す
            }

            if (coolTime >= interval) {
                canShoot = true;
                spriteRenderer.color = startColor; // クールタイムが終わったら色を元に戻す
            }
        }

        if (life <= 0) {
            ExplosionLaunch explosionLaunch = Instantiate(explosionLaunchPrefab, new Vector3(transform.position.x, 0.5f, 0.1f), Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public bool IsReadyToShoot() {
        return canShoot;
    }

    public void ResetLaunchPad() {
        canShoot = false;
        coolTime = 0f;
        spriteRenderer.color = coolDownColor; // クールタイムが開始したら色を変える
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "meteor") {
            life--;
            Destroy(collision.gameObject);
        }
    }
}
