using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float explosionTime;
    public Vector2 move;
    // Start is called before the first frame update
    void Start()
    {
        explosionTime = 0f;
        move = new Vector2(1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        explosionTime += Time.deltaTime;

        transform.localScale += new Vector3(move.x * Time.deltaTime, move.y * Time.deltaTime, 0);

        if (explosionTime >= 2f) {
            Destroy(gameObject);
        }
    }
}
