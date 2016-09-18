using UnityEngine;
using System.Collections;

public class ItemSet : MonoBehaviour {

    public GameObject[] weapons;
    public GameObject[] projectiles;

    public GameObject[] armor;

    public GameObject[] speed;

    public Transform activeWeapon;
    public Transform activeArmor;
    public Transform activeSpeed;

    Buddy myBuddy;

    void Start()
    {
        myBuddy = GetComponent<Buddy>();
    }

    public void setupWeapon(string itemName)
    {
      int i = 0;
      foreach(GameObject g in weapons)
      {
            if(g.name == itemName)
            {
                print(GetComponent<Buddy>().name);
                print(projectiles[i].name);
                GetComponent<Buddy>().myBullet = projectiles[i];
            }
            else
            {
                g.SetActive(false);
            }
            i++;
        }
    }

    public void setupArmor(string itemName)
    {
        foreach (GameObject g in armor)
        {
            if (g.name == itemName)
            {
                g.SetActive(true);
            }
            else
            {
                g.SetActive(false);
            }
        }
    }

    public void setupSpeed(string itemName)
    {
        foreach (GameObject g in speed)
        {
            if (g.name == itemName)
            {
                g.SetActive(true);
            }
            else
            {
                g.SetActive(false);
            }
        }
    }
}
