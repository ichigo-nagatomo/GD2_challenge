using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    private float destroyTime;
    // Start is called before the first frame update
    void Start()
    {
        destroyTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        destroyTime += Time.deltaTime;
        if( destroyTime >= 1f ) {
            Destroy(gameObject);
        }
    }
}
