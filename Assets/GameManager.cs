using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private Vector2 mousePos;
    private Vector2 reticlePos;

    private float dropTime;
    private float dropTimeInterval;
    private int rand;
    private Vector2 targetPoint;
    private Bounds bounds;

    private int life;
    private int score;

    private float time;
    private float displayTime;

    [SerializeField] private Reticle reticlePrefab;
    [SerializeField] private LaunchPad[] launchPad = new LaunchPad[3];
    [SerializeField] private Meteor meteorPrefab;
    [SerializeField] private Explosion explosionPrefab;
    [SerializeField] private FadeManager fadeManager;

    [SerializeField] private Text result;
    [SerializeField] private Text resultScore;

    private CamaraShake camaraShake;
    [SerializeField] private GameObject[] dropPoint = new GameObject[3];
    [SerializeField] private GameObject ground;

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1280, 720, false);

        dropTime = 0f;
        dropTimeInterval = 1f;
        rand = 2;
        targetPoint = new Vector2(0, -5);
        bounds.center = ground.transform.position;
        bounds.extents = ground.transform.localScale / 2;
        life = 100;
        score = 0;
        time = 0f;
        displayTime = 0f;

        camaraShake = FindObjectOfType<CamaraShake>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Input.mousePosition;
        reticlePos = Camera.main.ScreenToWorldPoint(mousePos);
        
        if (Input.GetMouseButtonDown(0)) {
            Shoot();
        }

        dropTime += Time.deltaTime;

        if (dropTime >= dropTimeInterval) {
            DropMeteor();
            dropTime = 0f;
        }

        if(Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(0);
        }

        if (life <= 0) {
            fadeManager.Out = true;
            time += Time.deltaTime;
            displayTime += Time.deltaTime;
            if (time >= 0.2f) {
                float x = Random.Range(-8, 8);
                float y = Random.Range(-5, 5);
                Explosion explosion = Instantiate(explosionPrefab, new Vector3(x, y, -1), Quaternion.identity);
                time = 0f;
            }

            if (displayTime >= 5f) {
                result.enabled = true;
                resultScore.text = "" + score.ToString("D6");
                resultScore.enabled = true;

                if (Input.GetMouseButtonDown(0)) {
                    SceneManager.LoadScene(0);
                }
            } else {
                camaraShake.StartShake(5f, 0.1f, 0.5f);
            }
        }
    }

    void Shoot() {
        for (int i = 0; i < launchPad.Length; i++) {
            if (launchPad[i]) {
                if (launchPad[i].IsReadyToShoot()) {
                    Reticle reticle = Instantiate(reticlePrefab, new Vector3(reticlePos.x, reticlePos.y, -0.1f), Quaternion.identity);
                    launchPad[i].SetShoot(true);
                    launchPad[i].SetPoint(reticlePos);
                    break;
                }
            }
        }
    }
    void DropMeteor() {
        targetPoint.x = Random.Range(bounds.min.x, bounds.max.x);
        int randSpeed = Random.Range(2, 5);

        bool allLaunchPadsMissing = true;
        Meteor meteor;

        for (int i = 0; i < 3; i++) {
            if (launchPad[i]) {
                allLaunchPadsMissing = false;
            }
        }

        meteor = Instantiate(meteorPrefab, dropPoint[rand].transform.position, transform.rotation);
        if (allLaunchPadsMissing) {
            randSpeed = 6;
        }
        meteor.GetVector(targetPoint, dropPoint[rand].transform.position, randSpeed);

        // launchPad‚ª‚·‚×‚Ä‘¶Ý‚µ‚È‚¢ê‡AŽŸ‚Ì’e‚ð‘‚­”­ŽË‚·‚é
        if (allLaunchPadsMissing) {
            dropTimeInterval = 0.5f;
        }

        rand = Random.Range(0, 3);
    }

    public void SubtractLife() {
        life -= 10;
    }

    public void AddScore() {
        if (life >= 0) {
            score += 100;
        }
    }

    public int GetLife() {
        return life;
    }

    public int GetScore() {
        return score;
    }
}
