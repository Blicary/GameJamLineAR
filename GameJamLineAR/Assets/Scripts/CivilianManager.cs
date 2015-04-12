using UnityEngine;
using System.Collections;

public class CivilianManager : MonoBehaviour 
{
    public int num_civs = 100;
    public Civilian prefab;

    public void Start()
    {
        for (int i = 0; i < num_civs; ++i)
        {
            Civilian civ = (Civilian)Instantiate<Civilian>(prefab);
            civ.transform.position = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), 0);
            civ.transform.parent = transform;
        }
    }
	
}
