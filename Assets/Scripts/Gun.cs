using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Gun : MonoBehaviour
{
    public int damage = 10;
    public float range = 100f;
    public float impactForce = 30f;
    public float fireRate = 15f;
    public int magazineCapacity = 30;
    private int currentAmmo;
    private bool isReloading = false;
    private float nextTimeToFire = 0f;
    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public GameObject dieEffect;
    public float recoilDuration = 0.1f;
    public Vector3 recoilForce;
    private Rigidbody gunRb;
    public float swayAmount = 0.05f;
    public float swaySpeed = 2f;
    public AudioSource audioSource;
    public AudioClip[] clips;

    private void Start()
    {
        gunRb = GetComponent<Rigidbody>();
        currentAmmo = magazineCapacity;
        audioSource=GetComponent<AudioSource>();

    }

    private void Update()
    {
        if (isReloading)
            return;

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetMouseButton(0) && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
            ApplyRecoil();
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(1.5f);

        currentAmmo = magazineCapacity;
        isReloading = false;
        Debug.Log("Reloaded!");
    }

    void Shoot()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;

            RaycastHit hit;
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
            {
                muzzleFlash.Play();
                audioSource.PlayOneShot(clips[0]);
                Debug.Log(hit.transform.name);
                Enemy enemy = hit.transform.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                    if (enemy.health <= 0)
                    {
                        GameObject dieEffectGO = Instantiate(dieEffect, hit.point, Quaternion.LookRotation(hit.normal));
                        audioSource.PlayOneShot(clips[1]);
                    }
                }
                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(-hit.normal * impactForce);
                }
                GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 2f);
            }
        }
        else
        {
            Debug.Log("Out of ammo!");
        }
    }

    void ApplyRecoil()
    {
        transform.DOShakeRotation(recoilDuration, recoilForce, 10, 90f, false);
    }

/*
    void FixedUpdate()
    {
        //bunu koyarsam sway çalışmıyor.
        if (gunRb != null && gunRb.isKinematic == false)
        {
            Debug.Log("sways");
            float swayX = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
            float swayY = Mathf.Cos(Time.time * swaySpeed * 2f) * swayAmount;

            Vector3 targetRotation = new Vector3(swayX, swayY, 0f);
            transform.DOLocalRotate(targetRotation, 0.1f);
        }
    }
*/
}
