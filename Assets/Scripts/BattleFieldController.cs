using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.Events;
public class BattleFieldController : MonoBehaviour, ITrackableEventHandler {
    private float lostTime;
    public float maxLostTime = 1f;
    public UnityEvent onBattleFieldReady;
    public UnityEvent onBattleFieldLost;

    private void Start()
    {
        TrackableBehaviour trackableBehavior = GetComponent<TrackableBehaviour>();
        if(trackableBehavior)
        {
            trackableBehavior.RegisterTrackableEventHandler(this);
        }
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
                   newStatus == TrackableBehaviour.Status.TRACKED ||
                   newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            onBattleFieldReady.Invoke();
            enabled = false;
        }
        else
        {
            lostTime = 0;
            enabled = true;
        }
    }

    private void Update()
    {
        lostTime += Time.unscaledDeltaTime;
        if (lostTime >= maxLostTime)
        {
            onBattleFieldLost.Invoke();
            enabled = false;
        }
    }
}
