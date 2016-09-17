using System.Collections.Generic;

[System.Serializable]
public class User {
	public string name;
	public string gender;
	public string type;
	public int coins;
	public int wins;
	public int loses;
	public int code;
	public List<Loadout> loadouts;
}