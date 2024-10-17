using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Inscribed")]
    public float speed = 10f;       //Movement speed is 10m/s
    public float fireRate = 0.3f;   //Seconds/shot
    public float health = 10;       //Damage needed to destroy this enemy
    public int score = 100;       //Points earned for destroying this
    public float powerUpDropChance = 1f;    //Chance to drop a PowerUp

    protected bool calledShipDestroyed = false;
    protected BoundsCheck bndCheck;

    void Awake() {
        bndCheck = GetComponent<BoundsCheck>();
    }

    //This is a Property: A method that acts like a field
    public Vector3 pos {
        get {
            return this.transform.position;
        }
        set {
            this.transform.position = value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        //Check whether this enemy has gone off the bottom of the screen
        if( bndCheck.LocIs( BoundsCheck.eScreenLocs.offDown ) ) {
            Destroy( gameObject );
        }


        //if( !bndCheck.isOnScreen ) {
        //    if( pos.y < bndCheck.camHeight - bndCheck.radius ) {
        //        //We're off the bottom, so destroy this game object
        //        Destroy( gameObject );
        //        }
        //}
    }

    public virtual void Move() {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    void OnCollisionEnter( Collision coll ) {
        GameObject otherGO = coll.gameObject;

        //Check for collisions with ProjectileHero
        ProjectileHero p = otherGO.GetComponent<ProjectileHero>();
        if( p != null ) {
            //Only damage this Enemy if it's on screen
            if( bndCheck.isOnScreen ) {
                //Get teh damage amount from the Main WEAP_DICT
                health -= Main.GET_WEAPON_DEFINITION( p.type ).damageOnHit;
                if( health <= 0 ) {
                    //Tell Main that this ship was destroyed
                    if( !calledShipDestroyed ) {
                        calledShipDestroyed = true;
                        Main.SHIP_DESTROYED( this );
                    }
                    //Destroy this Enemy
                    Destroy( this.gameObject );
                }
            }
            //Destroy the ProjectileHero regardless
            Destroy( otherGO );
        }
        else {
            print( "Enemy hit by non-ProjectileHero: " + otherGO.name );
        }
    }
}
