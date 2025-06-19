using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class Weapon : MonoBehaviour
{
    public bool isActiveWeapon;

    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;
    public int bulletPerBurst = 3;
    public int burstBulletsLeft;
    public float spreadIntensity;

    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabLifeTime = 3f;
    public GameObject muzzleEffect;
    internal Animator animator;

    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;

    public Vector3 spawnPosition;
    public Vector3 spawnRotation;

    public bool drawSpreadPattern = true;
    public int spreadPatternSegments = 20;
    public float spreadPatternRadius = 0.5f;
    private Material debugMaterial;

    public enum WeaponModel { Pistol1911, M4_8 }
    public WeaponModel thisWeaponModel;

    public enum ShootingMode { Single, Burst, Auto }
    public ShootingMode currentShootingMode;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletPerBurst;
        animator = GetComponent<Animator>();
        bulletsLeft = magazineSize;
        CreateDebugMaterial();
    }

    void Update()
    {
        if (isActiveWeapon)
        {
            if (bulletsLeft == 0 && isShooting)
            {
                SoundManager.Instance.emptyManagizeSound1911.Play();
            }

            if (currentShootingMode == ShootingMode.Auto)
            {
                isShooting = Input.GetKey(KeyCode.Mouse0);
            }
            else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
            {
                isShooting = Input.GetKey(KeyCode.Mouse0);
            }

            if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !isReloading)
            {
                Reload();
            }

            if (readyToShoot && !isShooting && !isReloading && bulletsLeft <= 0)
            {
                // Reload();
            }

            if (readyToShoot && isShooting && bulletsLeft > 0)
            {
                burstBulletsLeft = bulletPerBurst;
                FireWeapon();
            }

            if (AmmoManager.Instance.ammoDisplay != null)
            {
                AmmoManager.Instance.ammoDisplay.text = $"{bulletsLeft / bulletPerBurst} / {magazineSize / bulletPerBurst}";
            } 
        }
    }

    void OnRenderObject()
    {
        if (!drawSpreadPattern || !readyToShoot) return;

        debugMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(Color.yellow);

        Vector3 origin = bulletSpawn.position;
        Vector3 direction = (CalculateDirectionAndSpread() - origin).normalized;

        for (int i = 0; i < spreadPatternSegments; i++)
        {
            float angle = 2 * Mathf.PI * i / spreadPatternSegments;
            Vector3 spreadOffset = new Vector3(
                Mathf.Cos(angle) * spreadPatternRadius,
                Mathf.Sin(angle) * spreadPatternRadius,
                0);

            Vector3 spreadDirection = (direction + spreadOffset).normalized;
            GL.Vertex(origin);
            GL.Vertex(origin + spreadDirection * 2f);
        }
        GL.End();
    }

    private void CreateDebugMaterial()
    {
        Shader shader = Shader.Find("Hidden/Internal-Colored");
        debugMaterial = new Material(shader);
        debugMaterial.hideFlags = HideFlags.HideAndDontSave;
        debugMaterial.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
        debugMaterial.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
        debugMaterial.SetInt("_Cull", (int)CullMode.Off);
        debugMaterial.SetInt("_ZWrite", 0);
    }

    private void FireWeapon()
    {
        bulletsLeft--;
        muzzleEffect.GetComponent<ParticleSystem>().Play();
        animator.SetTrigger("RECOIL");
        SoundManager.Instance.PlayShootingSound(thisWeaponModel);
        readyToShoot = false;

        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        bullet.transform.forward = shootingDirection;
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
    }

    private void Reload()
    {
        SoundManager.Instance.PlayReloadSound(thisWeaponModel);
        animator.SetTrigger("RELOAD");
        isReloading = true;
        Invoke("ReloadComplete", reloadTime);
    }

    private void ReloadComplete()
    {
        bulletsLeft = magazineSize;
        isReloading = false;
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint = Physics.Raycast(ray, out hit) ? hit.point : ray.GetPoint(100);

        Vector3 direction = targetPoint - bulletSpawn.position;
        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(x, y, 0);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }

    void OnDestroy()
    {
        if (debugMaterial != null)
        {
            Destroy(debugMaterial);
        }
    }
}