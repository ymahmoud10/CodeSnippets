using System.Collections.Generic;
using ObscureGames;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    public List<OGUnit> zombies;
    public List<OGUnit> characters;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeLists();
    }

    private void InitializeLists()
    {
        foreach (OGUnit unit in FindObjectsOfType<OGUnit>())
        {
            if (unit.unitType == OGUnit.UnitType.Zombie && unit.GetComponent<OGKillable>().isAlive)
            {
                zombies.Add(unit);
            }

            if (unit.unitType == OGUnit.UnitType.Character)
            {
                characters.Add(unit);
            }
        }
    }
}
