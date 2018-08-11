using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour {

    public GameObject bulletPrefab;
    List<GameObject> pool = new List<GameObject>();
    int pooledAmount = 15;

	// Use this for initialization
	void Start () {
		for (int i=0;i<pooledAmount;i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform);
            bullet.SetActive(false);
            pool.Add(bullet);
        }
	}

    public Bullet GetBullet()
    {
        foreach(GameObject bullet in pool)
        {
            if (!bullet.activeInHierarchy)
            {
                bullet.SetActive(true);
                return bullet.GetComponent<Bullet>();
            }
        }
        print("No more bullets available!");
        return null;
    }
}
