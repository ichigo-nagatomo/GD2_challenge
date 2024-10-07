using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float explosionTime;
    public float targetTime;
    public Vector2 move;
    // Start is called before the first frame update
    void Start()
    {
        explosionTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        SetExpand();

        explosionTime += Time.deltaTime;

        transform.localScale += new Vector3(move.x * Time.deltaTime, move.y * Time.deltaTime, 0);

        if (explosionTime >= targetTime) {
            Destroy(gameObject);
        }
    }

    protected virtual void SetExpand() {
        move = new Vector2(1f, 1f);
        targetTime = 2f;
    }
}
