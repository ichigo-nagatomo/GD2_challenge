using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour{
    private float move;
    private Vector3 dir;
    private Vector3 explosionPos;

    private float destroyTime;

    private float radius;
    private float particleTime;

    [SerializeField] public Explosion explosionPrefab;
    [SerializeField] public Particle particlePrefab;

    private GameManager gameManager;
    private CamaraShake camaraShake;


    // Start is called before the first frame update
    void Start() {
        destroyTime = 0f;
        radius = 0.7f;
        particleTime = 0f;
        gameManager = FindObjectOfType<GameManager>();
        camaraShake = FindObjectOfType<CamaraShake>();// é©ìÆìIÇ…GameManagerÇíTÇµÇƒéÊìæ
    }

    // Update is called once per frame
    void Update() {
        transform.position += new Vector3(dir.x * move * Time.deltaTime, dir.y * move * Time.deltaTime, 0);

        destroyTime += Time.deltaTime;
        if (destroyTime >= 8f) {
            Destroy(gameObject);
        }

        particleTime += Time.deltaTime;
        if (particleTime >= 0.1f) {
            var circlePos = radius * Random.insideUnitCircle + new Vector2(transform.position.x, transform.position.y);
            Particle particle = Instantiate(particlePrefab, new Vector3(circlePos.x, circlePos.y, 1), Quaternion.identity);
            particleTime = 0f;
        }
    }
    public void GetVector(Vector3 from, Vector3 to, float speed) {
        dir = new Vector3(from.x - to.x, from.y - to.y, 0).normalized;
        move = speed;
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "explosion") {
            if (gameManager != null){ // gameManagerÇ™ê≥ÇµÇ≠éÊìæÇ≈Ç´ÇƒÇ¢ÇÈÇ©ämîF
                camaraShake.StartShake(0.1f, 0.05f, 0.5f);
                gameManager.AddScore();
            }
            Destroy(gameObject);
            Explosion explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        if (collision.gameObject.tag == "ground") {
            camaraShake.StartShake(0.5f, 0.05f, 0.5f);
            gameManager.SubtractLife();
            Destroy(gameObject);
        }
    }
}
