using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectMove : MonoBehaviour {
    public float lifeTime = 2.0f;
    float speed = 0.2f;
    float elapsedTime = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        elapsedTime += speed * Time.deltaTime;

        if ( elapsedTime > lifeTime)
        {
            Destroy(gameObject);
        }
        //gameObject.transform.position = new Vector3(gameObject.transform.position.x,
        //    gameObject.transform.position.y + speed * Time.deltaTime,
        //    gameObject.transform.position.z);
	}
}
