using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class UniversalStats : MonoBehaviour
{
    public event Action<int, int> updateHealthBarOnAttack;
    public soUniversalData universalData;
    public soUniversalData templateData;
    public soAttackData attackData;
    [HideInInspector]
    public bool isCrit;
    public bool isDead = false;
    private void Awake()
    {
        if (templateData != null)
            universalData = Instantiate(templateData);
    }
    #region 从playerData读取写入
    public int maxHP
    {
        get
        {
            if (universalData != null)
                return universalData.maxHP;
            else
                return 0;
        }
        set
        {
            universalData.maxHP = value;
        }
    }
    public int currentHP
    {
        get
        {
            if (universalData != null)
                return universalData.currentHP;
            else
                return 0;
        }
        set
        {
            universalData.currentHP = value;
        }
    }
    public int baseDEF
    {
        get
        {
            if (universalData != null)
                return universalData.baseDEF;
            else
                return 0;
        }
        set
        {
            universalData.baseDEF = value;
        }
    }
    public int currentDEF
    {
        get
        {
            if (universalData != null)
                return universalData.currentDEF;
            else
                return 0;
        }
        set
        {
            universalData.currentDEF = value;
        }
    }
    #endregion
    #region 通用战斗
    public void TakeDamage(UniversalStats attacker, UniversalStats defener)
    {
        int damage = Mathf.Max(attacker.RandomATK - defener.currentDEF, 0);
        defener.currentHP = Mathf.Max(defener.currentHP - damage, 0);
        if (attacker.isCrit)
        {
            defener.GetComponent<Animator>().SetTrigger("hit");
        }
        updateHealthBarOnAttack?.Invoke(currentHP, maxHP);
        if (defener.currentHP == 0)
            defener.isDead = true;
        if (defener.currentHP <= 0)
            attacker.universalData.UpdateExp(universalData.killPoint);
    }
    public void TakeDamage(int damage, UniversalStats defener)
    {
        int currentDamage = Mathf.Max(damage - defener.currentDEF, 0);
        defener.currentHP = Mathf.Max(defener.currentHP - damage, 0);
        updateHealthBarOnAttack?.Invoke(currentHP, maxHP);
        if (defener.currentHP == 0)
            defener.isDead = true;
        if (defener.currentHP <= 0)
            mGame.Inst.playerStats.universalData.UpdateExp(universalData.killPoint);
    }
    int RandomATK
    {
        get
        {
            float coreATK = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage + 1);
            if (isCrit)
            {
                coreATK *= attackData.critMultiplier;
            }
            return (int)coreATK;
        }
    }
    #endregion
}
