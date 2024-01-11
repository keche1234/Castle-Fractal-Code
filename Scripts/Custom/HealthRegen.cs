using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthRegen : Buff
{
    [SerializeField] protected float interval; //time in between ticks
    protected float intervalTimer = 0;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        while (intervalTimer >= interval)
        {
            Heal();
            intervalTimer -= interval;
        }
        intervalTimer += Time.deltaTime;

        base.Update();
    }

    public void OnDestroy()
    {
        //Heal();
    }

    public void Heal()
    {
        float heal = Mathf.Max((mod * owner.GetMaxHealth()) - Random.Range(0.001f, 1f) + 1, 1);
        owner.TakeDamage(-(int)heal, Vector3.zero);
    }

    public override void SetBuff(float val, float time)
    {
        SetBuff(val, time, 1);
    }

    public void SetBuff(float val, float time, float tick)
    {
        val = Mathf.Clamp01(val);
        base.SetBuff(val, time);
        interval = (tick < 1 / 60f) ? 1 : tick;
    }
}
