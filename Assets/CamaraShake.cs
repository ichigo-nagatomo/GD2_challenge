using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraShake : MonoBehaviour
{
    private struct ShakeInfo {
        public ShakeInfo(float duration, float strength, float vibrato) {
            Duration = duration;
            Strength = strength;
            Vibrato = vibrato;
        }
        public float Duration {
            get;
        } // ����
        public float Strength {
            get;
        } // �h��̋���
        public float Vibrato {
            get;
        }  // �ǂ̂��炢�U�����邩
    }
    private ShakeInfo _shakeInfo;

    private Vector3 _initPosition; // �����ʒu
    private bool _isDoShake;       // �h����s�����H
    private float _totalShakeTime; // �h��o�ߎ���

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isDoShake) {
            return;
        }

        // �h��ʒu���X�V
        transform.position = UpdateShakePosition(
            transform.position,
            _shakeInfo,
            _totalShakeTime,
            _initPosition);

        // duration���̎��Ԃ��o�߂�����h�炷�̂��~�߂�
        _totalShakeTime += Time.deltaTime;
        if (_totalShakeTime >= _shakeInfo.Duration) {
            _isDoShake = false;
            _totalShakeTime = 0.0f;
            // �����ʒu�ɖ߂�
            transform.position = new Vector3(0, 0, -10);
        }
    }

    private Vector3 UpdateShakePosition(Vector3 currentPosition, ShakeInfo shakeInfo, float totalTime, Vector3 initPosition) {
        // -strength ~ strength �̒l�ŗh��̋������擾
        var strength = shakeInfo.Strength;
        var randomX = Random.Range(-1.0f * strength, strength);
        var randomY = Random.Range(-1.0f * strength, strength);

        // ���݂̈ʒu�ɉ�����
        var position = currentPosition;
        position.x += randomX;
        position.y += randomY;

        // �����ʒu-vibrato ~ �����ʒu+vibrato �̊ԂɎ��߂�
        var vibrato = shakeInfo.Vibrato;
        var ratio = 1.0f - totalTime / shakeInfo.Duration;
        vibrato *= ratio; // �t�F�[�h�A�E�g�����邽�߁A�o�ߎ��Ԃɂ��h��̗ʂ�����
        position.x = Mathf.Clamp(position.x, initPosition.x - vibrato, initPosition.x + vibrato);
        position.y = Mathf.Clamp(position.y, initPosition.y - vibrato, initPosition.y + vibrato);
        return position;
    }

    public void StartShake(float duration, float strength, float vibrato) {
        // �h�����ݒ肵�ĊJ�n
        _shakeInfo = new ShakeInfo(duration, strength, vibrato);
        _isDoShake = true;
        _totalShakeTime = 0.0f;
    }
}
