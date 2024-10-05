using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletParticle : MonoBehaviour
{
    private float destroyTime;
    private float move;
    // Start is called before the first frame update
    void Start() {
        destroyTime = 0f;
        move = 0.2f;
    }

    // Update is called once per frame
    void Update() {
        destroyTime += Time.deltaTime;
        transform.localScale -= new Vector3(move * Time.deltaTime, move * Time.deltaTime, 0);
        if (destroyTime >= 1f) {
            Destroy(gameObject);
        }
    }
}
