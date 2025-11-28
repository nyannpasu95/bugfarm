using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedSystem : MonoBehaviour
{
   //消息通知层
   //--------------------------------------

    public static SeedSystem Instance;
    public enum Phase{ phase1,  phase2, phase3,phase4 }
    private Phase currentPhase = Phase.phase1;
    public static event Action<Phase> OnPhaseChanged;

    private void Awake() => Instance = this;

    public void TryAdvancePhase()
    {
        if ((int)currentPhase < Enum.GetNames(typeof(Phase)).Length - 1)
        {
            currentPhase++;
            Debug.Log("Phase Changed to: " + currentPhase);
            OnPhaseChanged?.Invoke(currentPhase);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
