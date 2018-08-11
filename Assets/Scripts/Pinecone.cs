using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinecone : MonoBehaviour {

    private PineconeManager pManager;
    private bool hasDropped = false;
    private Rigidbody mRigidbody;

    private void Start()
    {
        mRigidbody = GetComponent<Rigidbody>();
        pManager = transform.parent.GetComponent<PineconeManager>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet") && !hasDropped)
        {
            pManager.PineconeShot();
            hasDropped = true;
            mRigidbody.useGravity = true;
        }
    }
}
