using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class WebRequestManager : MonoBehaviour {

	string baseUrl = "https://battlebuddy.herokuapp.com";
	User player1;

	IEnumerator getUser (string code) {
		UnityWebRequest www = UnityWebRequest.Get (baseUrl + "/users/" + code);
		yield return www.Send ();

		if (www.isError) {
			Debug.LogWarning (www.error);
		} else {
			player1 = JsonUtility.FromJson<User> (www.downloadHandler.text);
		}
	}

	IEnumerator createUser (User user) {
		WWWForm data = new WWWForm ();
		data.AddField ("name", user.name);
		data.AddField ("gender", user.gender);
		data.AddField ("type", user.type);
		UnityWebRequest www = UnityWebRequest.Post(baseUrl + "/users", data);
		yield return www.Send ();

		if (www.isError) {
			Debug.LogWarning (www.error);
		} else {
			Debug.Log(www.downloadHandler.text);
			player1 = JsonUtility.FromJson<User> (www.downloadHandler.text);
		}
	}

	void Start () {
		User newUser = new User ();
		newUser.name = "Bob Lob Law";
		newUser.gender = "male";
		newUser.type = "attack";
		StartCoroutine(createUser(newUser));
	}
}
