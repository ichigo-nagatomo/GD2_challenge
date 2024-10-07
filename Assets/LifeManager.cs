using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeManager : MonoBehaviour
{
    [SerializeField] private Image lifeBar;
    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        lifeBar.rectTransform.sizeDelta = new Vector2(gameManager.GetLife() * 5, 70);
    }
}
