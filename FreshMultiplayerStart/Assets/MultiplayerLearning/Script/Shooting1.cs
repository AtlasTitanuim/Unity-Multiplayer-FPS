﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Shooting1 : NetworkBehaviour
{
    public Transform cirvle;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public Camera myCamera;
    public int Bulletspeed;
    public float BulletLife;
    public bool ableToShoot = true;

    public int WichHero = 0;
    private Quaternion rotation;
    private float time;

    void Update()
    {  
        if (!isLocalPlayer)
        {
            return;
        }

        Aim();

        switch (WichHero)
        {
        case 2:
            print ("Hero 2.");
            if(ableToShoot){
                if (Input.GetMouseButton(0)){
                    CmdFireHero2();
                }
                if (Input.GetKeyDown("e")){
                    CmdHero2Action();
                }
            }
            break;
        case 1:
            print ("Hero 1.");
            if(ableToShoot){
                if (Input.GetMouseButton(0)){
                    CmdFireHero1();
                }
                if (Input.GetKeyDown("e")){
                    CmdHero1Action();
                }
            }
            break;
        default:
            print ("Incorrect hero number.");
            break;
        }
    }

    [Command]
    public void CmdFireHero1(){   
        Rpc_FireHero1();
    }

    [Command]
    public void CmdFireHero2(){   
        Rpc_FireHero2();
    }

    [Command]
    public void CmdHero1Action(){   
        Rpc_Hero1Action();
    }

    [Command]
    public void CmdHero2Action(){   
        Rpc_Hero2Action();
    }

    [ClientRpc]
	void Rpc_FireHero1(){
        BulletLife = 2.0f;
        Bulletspeed = 15;

        var bullet = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        StartCoroutine(Wait(0.1f));

        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * Bulletspeed;
        NetworkServer.Spawn(bullet);
        Destroy(bullet, BulletLife);
	}

    [ClientRpc]
	void Rpc_FireHero2(){
        BulletLife = 0.5f;
        Bulletspeed = 12;

        StartCoroutine(Wait(1));
        for(int i = 0; i < 20; i++){
            rotation = Quaternion.EulerRotation(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f),0);
            var bullet = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation*rotation);
            
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * Bulletspeed;
            NetworkServer.Spawn(bullet);
            Destroy(bullet, BulletLife);
        }
	}

    [ClientRpc]
	void Rpc_Hero1Action(){
        BulletLife = 0.2f;
        Bulletspeed = 12;

        StartCoroutine(Wait(1));
        //rotation = Quaternion.EulerRotation(bulletSpawn.rotation.x,-0.5f,bulletSpawn.rotation.z);
        time = -0.8f;
        for(int i = 0; i < 20; i++){
            rotation = Quaternion.EulerRotation(bulletSpawn.rotation.x,time,bulletSpawn.rotation.z);
            time += 0.1f;
            Debug.Log(rotation);
            var bullet = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation*rotation);
            
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * Bulletspeed;
            NetworkServer.Spawn(bullet);
            Destroy(bullet, BulletLife);
        }
	}

    [ClientRpc]
	void Rpc_Hero2Action(){
        BulletLife = 2f;
        Bulletspeed = 6;
        
        var bullet = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        bullet.GetComponent<Bullet>().strength = 40;
        StartCoroutine(Wait(2f));

        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * Bulletspeed;
        bullet.transform.localScale = new Vector3(bullet.transform.localScale.x*4,bullet.transform.localScale.y*4,bullet.transform.localScale.z*4);
        
        NetworkServer.Spawn(bullet);
        Destroy(bullet, BulletLife);
        bullet.GetComponent<Bullet>().strength = 10;
	}
    
    public void Aim(){
        float x = Screen.width / 2;
        float y = Screen.height / 2;
        
        bulletSpawn.transform.LookAt(cirvle);
    }

    IEnumerator Wait(float seconds)
    {
        ableToShoot = false;
        yield return new WaitForSeconds(seconds);
        ableToShoot = true;
    }
}
