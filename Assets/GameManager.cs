using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
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
    [SerializeField] private List<LaunchPad> launchPads = new List<LaunchPad>();
    [SerializeField] private Meteor meteorPrefab;
    [SerializeField] private Explosion explosionPrefab;
    [SerializeField] private FadeManager fadeManager;

    [SerializeField] private Text result;
    [SerializeField] private Text resultScore;

    [SerializeField] private CamaraShake camaraShake;
    [SerializeField] private List<GameObject> dropPoints = new List<GameObject>();
    [SerializeField] private GameObject ground;

    // Start is called before the first frame update
    void Start() {
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
    }

    // Update is called once per frame
    void Update() {
        mousePos = Input.mousePosition;
        reticlePos = Camera.main.ScreenToWorldPoint(mousePos);

        foreach (var launchPad in launchPads) {
            launchPad.SetGameManager(this);
        }
        if (Input.GetMouseButtonDown(0)) {
            Shoot();
        }

        dropTime += Time.deltaTime;

        if (dropTime >= dropTimeInterval) {
            if (displayTime <= 5f) {
                DropMeteor();
            }
            dropTime = 0f;
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(0);
        }

        if (life <= 0) {
            fadeManager.Out = true;
            time += Time.deltaTime;
            displayTime += Time.deltaTime;

            if (displayTime >= 5f) {
                result.enabled = true;
                resultScore.text = "" + score.ToString("D6");
                resultScore.enabled = true;

                if (Input.GetMouseButtonDown(0)) {
                    SceneManager.LoadScene(0);
                }
            } else {
                camaraShake.StartShake(5f, 0.1f, 0.5f);

                if (time >= 0.2f) {
                    float x = Random.Range(-8, 8);
                    float y = Random.Range(-5, 5);
                    Explosion explosion = Instantiate(explosionPrefab, new Vector3(x, y, -1), Quaternion.identity);
                    time = 0f;
                }
            }
        }
    }

    void Shoot() {
        foreach (var launchPad in launchPads) {
            if (launchPad != null && launchPad.IsReadyToShoot()) {
                Reticle reticle = Instantiate(reticlePrefab, new Vector3(reticlePos.x, reticlePos.y, -0.1f), Quaternion.identity);
                launchPad.SetShoot(true);
                launchPad.SetPoint(reticlePos);
                break;
            }
        }
    }

    void DropMeteor() {
        targetPoint.x = Random.Range(bounds.min.x, bounds.max.x);
        int randSpeed = Random.Range(2, 5);

        bool allLaunchPadsMissing = true;

        foreach (var launchPad in launchPads) {
            if (launchPad != null) {
                allLaunchPadsMissing = false;
                break;
            }
        }

        Meteor meteor = Instantiate(meteorPrefab, dropPoints[rand].transform.position, transform.rotation);
        if (allLaunchPadsMissing) {
            randSpeed = 6;
        }
        meteor.GetVector(targetPoint, dropPoints[rand].transform.position, randSpeed, this);

        if (allLaunchPadsMissing) {
            dropTimeInterval = 0.5f;
        }

        rand = Random.Range(0, dropPoints.Count);
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

    public void Shake(float duration, float strength, float vibrato) {
        camaraShake.StartShake(duration, strength, vibrato);
    }
}
