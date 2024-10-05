using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    private float move;
    private Vector3 dir;
    private Vector3 explosionPos;
    private float explosionThreshold = 0.1f; // ���e�͈͂̐ݒ�

    private float radius;
    private float particleTime;

    [SerializeField] public Explosion explosionPrefab;
    [SerializeField] public BulletParticle particlePrefab;

    // Start is called before the first frame update
    void Start() {
        move = 6f;

        radius = 0.1f;
        particleTime = 0f;
    }

    // Update is called once per frame
    void Update() {
        // �e�ۂ̈ړ�
        transform.position += new Vector3(dir.x * move * Time.deltaTime, dir.y * move * Time.deltaTime, 0);

        // �e�ۂ̌�������]������
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));

        // �w��̈ʒu�ɏ\���߂Â�����I�u�W�F�N�g��j�󂷂�
        if (Vector3.Distance(transform.position, explosionPos) <= explosionThreshold) {
            Explosion explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        particleTime += Time.deltaTime;
        if (particleTime >= 0.1f) {
            var circlePos = radius * Random.insideUnitCircle + new Vector2(transform.position.x, transform.position.y);
            BulletParticle particle = Instantiate(particlePrefab, new Vector3(circlePos.x, circlePos.y, 1), Quaternion.identity);
            particleTime = 0f;
        }
    }

    // �ړ������Ɣ����ʒu��ݒ肷�郁�\�b�h
    public void GetVector(Vector3 from, Vector3 to) {
        dir = new Vector3(from.x - to.x, from.y - to.y, 0).normalized;
        explosionPos = new Vector3(from.x, from.y, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "reticle") {
            Destroy(collision.gameObject);
        }
    }
}
