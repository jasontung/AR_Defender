using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CrystalController : MonoBehaviour {
    private Collider mCollider;
    public float crystalChargeTotalTime = 30f;
    private float curChargeTime;
    public Slider powerSlider;
    public Text progressLabel;
    public ParticleSystem powerEffect;
    public ParticleSystem chargeCompleteEffect;
    public bool isDead;
    private void Awake()
    {
        mCollider = GetComponent<Collider>();
        powerSlider.maxValue = crystalChargeTotalTime;
        StartCoroutine(Execute());
    }

    public IEnumerator Execute()
    {
        powerEffect.Play();
        while(enabled)
        {
            powerSlider.value += Time.deltaTime;
            progressLabel.text = Mathf.FloorToInt(powerSlider.normalizedValue * 100) + "%";
            if(powerSlider.value == powerSlider.maxValue)
            {
                enabled = false;
                powerEffect.Stop();
                chargeCompleteEffect.Play();
                //殺光所有活著的敵人
                foreach (var rhino in GameObject.FindObjectsOfType<RhinoController>())
                {
                    rhino.GetComponent<AttackableBehavior>().Hurt(int.MaxValue);
                }
            }
            yield return null;
        }
    }

    public void OnDead()
    {
        mCollider.enabled = false;
        powerEffect.Stop();
        enabled = false;
        isDead = true;
    }
}
