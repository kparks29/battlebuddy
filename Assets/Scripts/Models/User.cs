using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class User {
	public string name;
	public string gender;
	public string type;
	public int coins;
	public int wins;
	public int loses;
	public int code;
	public int equiped_loadout_index;
	public List<Loadout> loadouts;
}