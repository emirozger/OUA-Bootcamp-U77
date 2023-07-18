using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public LineRenderer beam;
    public Transform muzzlePoint;
    public float maxLenght;
    public ParticleSystem muzzleParticles;
    public ParticleSystem hitParticles;
    public GameObject dieEffect;

    private void Awake()
    {
        beam.enabled = false;
    }
    private void Activate()
    {
        beam.enabled = true;
        muzzleParticles.Play();
        hitParticles.Play();
    }
    private void Deactivate()
    {
        beam.enabled = false;
        beam.SetPosition(0, muzzlePoint.position);
        beam.SetPosition(1, muzzlePoint.position);
        muzzleParticles.Stop();
        hitParticles.Stop();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Activate();
        }
        if (Input.GetMouseButtonUp(0))
        {
            Deactivate();
        }
    }
    private void FixedUpdate()
    {
        // if (beam.enabled)  return;

        Ray ray = new Ray(muzzlePoint.position, muzzlePoint.forward);
        bool cast = Physics.Raycast(ray, out RaycastHit hit, maxLenght);
        Vector3 hitPosition = cast ? hit.point : muzzlePoint.position + muzzlePoint.forward * maxLenght;
        beam.SetPosition(0, muzzlePoint.position);
        beam.SetPosition(1, hitPosition);
        hitParticles.transform.position = hitPosition;
        if (cast && Input.GetMouseButton(0))
        {
            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(100);
                //GameObject bloodEffect = Instantiate(bloodEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                if (enemy.health <= 0)
                {
                    GameObject dieEffectGO = Instantiate(dieEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    AudioManager.Instance.PlayOneShot("EnemyDead");
                }
            }
        }

    }

}
