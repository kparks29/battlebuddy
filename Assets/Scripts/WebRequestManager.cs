using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class WebRequestManager : MonoBehaviour {

	string baseUrl = "https://battlebuddy.herokuapp.com";

	public IEnumerator getUser (string code, System.Action<User> callback) {
		UnityWebRequest www = UnityWebRequest.Get (baseUrl + "/users/" + code);
		yield return www.Send ();

		if (www.isError) {
			Debug.LogWarning (www.error);
		} else {
			callback(JsonUtility.FromJson<User> (www.downloadHandler.text));
		}
	}

	public IEnumerator getItems (string category, System.Action<List<Item>> callback) {
		UnityWebRequest www = UnityWebRequest.Get (baseUrl + "/items?category=" + category);
		yield return www.Send ();

		if (www.isError) {
			Debug.LogWarning (www.error);
		} else {
            print("Collected Items");
			Items results = JsonUtility.FromJson<Items> (www.downloadHandler.text);
			callback (results.items);
		}
	}

	void Start () {
		User player;
		List<Item> weapons;
		StartCoroutine(getUser("123", (user) => {
			player = user;
		}));
		StartCoroutine(getItems("weapon", (items) => {
			weapons = items;
		}));
	}
}
