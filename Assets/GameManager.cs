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
    private Vector2[] dropPoint = new Vector2[3];
    private int rand;
    private Vector2 targetPoint;
    private Bounds bounds;

    private int life;
    private int score;

    private float time;
    private float displayTime;

    [SerializeField] private Reticle reticlePrefab;
    [SerializeField] private LaunchPad[] launchPad = new LaunchPad[3];
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Meteor meteorPrefab;
    [SerializeField] private Explosion explosionPrefab;
    [SerializeField] private FadeManager fadeManager;
    [SerializeField] private Image lifeBar;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text result;
    [SerializeField] private Text resultScore;

    [SerializeField] public GameObject camaraPos;
    private struct ShakeInfo {
        public ShakeInfo(float duration, float strength, float vibrato) {
            Duration = duration;
            Strength = strength;
            Vibrato = vibrato;
        }
        public float Duration {
            get;
        } // 時間
        public float Strength {
            get;
        } // 揺れの強さ
        public float Vibrato {
            get;
        }  // どのくらい振動するか
    }
    private ShakeInfo _shakeInfo;

    private Vector3 _initPosition; // 初期位置
    private bool _isDoShake;       // 揺れ実行中か？
    private float _totalShakeTime; // 揺れ経過時間

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1280, 720, false);

        dropTime = 0f;
        dropTimeInterval = 1f;
        dropPoint[0] = new Vector2(-9, 5);
        dropPoint[1] = new Vector2(0, 5);
        dropPoint[2] = new Vector2(9, 5);
        rand = 2;
        targetPoint = new Vector2(0, -5);
        bounds.center = transform.position;
        bounds.extents = transform.localScale / 2;
        life = 100;
        score = 0;
        time = 0f;
        displayTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        lifeBar.rectTransform.sizeDelta = new Vector2(life * 5, 70);
        scoreText.text = "score : " + score;

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
                resultScore.text = "" + score;
                resultScore.enabled = true;

                if (Input.GetMouseButtonDown(0)) {
                    SceneManager.LoadScene(0);
                }
            } else {
                StartShake(5f, 0.1f, 0.5f);
            }
        }

        if (!_isDoShake) {
            return;
        }

        // 揺れ位置情報更新
        camaraPos.transform.position = UpdateShakePosition(
            camaraPos.transform.position,
            _shakeInfo,
            _totalShakeTime,
            _initPosition);

        // duration分の時間が経過したら揺らすのを止める
        _totalShakeTime += Time.deltaTime;
        if (_totalShakeTime >= _shakeInfo.Duration) {
            _isDoShake = false;
            _totalShakeTime = 0.0f;
            // 初期位置に戻す
            camaraPos.transform.position = new Vector3(0, 0, -10);
        }
    }

    void Shoot() {
        for (int i = 0; i < launchPad.Length; i++) {
            if (launchPad[i]) {
                if (launchPad[i].IsReadyToShoot()) {
                    Reticle reticle = Instantiate(reticlePrefab, new Vector3(reticlePos.x, reticlePos.y, -0.1f), Quaternion.identity);
                    Bullet bullet = Instantiate(bulletPrefab, launchPad[i].transform.position, Quaternion.identity);
                    bullet.GetVector(reticlePos, launchPad[i].transform.position);

                    launchPad[i].ResetLaunchPad();
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

            if (rand == i) {
                meteor = Instantiate(meteorPrefab, dropPoint[i], transform.rotation);
                if (allLaunchPadsMissing) {
                    randSpeed = 6;
                }
                meteor.GetVector(targetPoint, dropPoint[i], randSpeed);
                break;
            }
        }

        // launchPadがすべて存在しない場合、次の弾を早く発射する
        if (allLaunchPadsMissing) {
            dropTimeInterval = 0.5f;
        }

        rand = Random.Range(0, 3);
    }


    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "meteor") {
            StartShake(0.5f, 0.1f, 0.5f);
            life -= 10;
            Destroy(collision.gameObject);
        }
    }

    private Vector3 UpdateShakePosition(Vector3 currentPosition, ShakeInfo shakeInfo, float totalTime, Vector3 initPosition) {
        // -strength ~ strength の値で揺れの強さを取得
        var strength = shakeInfo.Strength;
        var randomX = Random.Range(-1.0f * strength, strength);
        var randomY = Random.Range(-1.0f * strength, strength);

        // 現在の位置に加える
        var position = currentPosition;
        position.x += randomX;
        position.y += randomY;

        // 初期位置-vibrato ~ 初期位置+vibrato の間に収める
        var vibrato = shakeInfo.Vibrato;
        var ratio = 1.0f - totalTime / shakeInfo.Duration;
        vibrato *= ratio; // フェードアウトさせるため、経過時間により揺れの量を減衰
        position.x = Mathf.Clamp(position.x, initPosition.x - vibrato, initPosition.x + vibrato);
        position.y = Mathf.Clamp(position.y, initPosition.y - vibrato, initPosition.y + vibrato);
        return position;
    }

    public void StartShake(float duration, float strength, float vibrato) {
        // 揺れ情報を設定して開始
        _shakeInfo = new ShakeInfo(duration, strength, vibrato);
        _isDoShake = true;
        _totalShakeTime = 0.0f;
    }

    public void AddScore() {
        if (life >= 0) {
            score += 100;
        }
    }
}
