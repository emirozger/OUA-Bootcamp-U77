using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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
    public float sprayAmount = 0.05f;
    public AudioSource audioSource;
    public AudioClip[] clips;

    public TMP_Text ammoText;
    public PlayerMovementAdvanced playerMovementAdvanced;
    Vector3 bulletDirection;
    public bool isFiring;
    private bool isMouseDown = false;
    private float holdTime = 0f;
    private float requiredHoldTime = 0.2f;


    private void Start()
    {
        playerMovementAdvanced = GameObject.Find("Player").GetComponent<PlayerMovementAdvanced>();
        gunRb = GetComponent<Rigidbody>();
        currentAmmo = magazineCapacity;
        audioSource = GetComponent<AudioSource>();
        UpdateAmmoText();
    }

    private void Update()
    {
        Debug.Log(playerMovementAdvanced.horizontalInput != 0f || playerMovementAdvanced.verticalInput != 0f);
        if (isReloading)
            return;

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < magazineCapacity)
        {
            StartCoroutine(Reload());
            return;
        }

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            isMouseDown = true;
            holdTime = 0f;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isMouseDown = false;
            if (holdTime >= requiredHoldTime)
            {
                isFiring = true;
            }
        }

        if (Input.GetMouseButton(0) && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
            ApplyRecoil();
        }
        if (isMouseDown)
        {
            holdTime += Time.deltaTime;
            if (holdTime >= requiredHoldTime)
            {
                isFiring = true;
            }
            else
            {
                isFiring = false;
            }
        }
        else
        {
            isFiring = false;
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        audioSource.PlayOneShot(clips[2]);
        //clip 2.79 saniye. başka bulursam değiştir.
        yield return new WaitForSeconds(2.79f);
        currentAmmo = magazineCapacity;
        isReloading = false;
        UpdateAmmoText();
        Debug.Log("Reloaded!");
    }

    void Shoot()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            ApplyRecoil();
            muzzleFlash.Play();
            audioSource.PlayOneShot(clips[0]);
            if (IsMoving() || isFiring || IsMoving() && isFiring)
            {
                // Geri tepme (spray) mekaniği
                Vector2 recoil = Random.insideUnitCircle * sprayAmount;
                Vector3 spray = new Vector3(recoil.x, 0f, recoil.y);
                bulletDirection = (fpsCam.transform.forward + spray).normalized;
            }
            else if (!IsMoving())
            {
                bulletDirection = fpsCam.transform.forward;
            }

            Ray bulletRay = new Ray(fpsCam.transform.position, bulletDirection);
            RaycastHit hit;
            if (Physics.Raycast(bulletRay, out hit, range))
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

            UpdateAmmoText();
        }
        else
        {
            Debug.Log("Out of ammo!");
        }
    }
    bool IsMoving()
    {
        return playerMovementAdvanced.horizontalInput != 0f || playerMovementAdvanced.verticalInput != 0f;
    }
    void UpdateAmmoText()
    {
        ammoText.text = currentAmmo.ToString() + " / " + magazineCapacity.ToString();
    }
    void ApplyRecoil()
    {
        transform.DOShakeRotation(recoilDuration, recoilForce, 10, 90f, false);
    }
}
