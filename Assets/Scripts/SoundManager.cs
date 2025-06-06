using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Weapon;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    public AudioSource ShootingChannel;

    public AudioClip P1911Shot;
    public AudioClip M4_8Shot;

    public AudioSource reloadSoundM4_8;
    public AudioSource reloadSound1911;

    public AudioSource emptyManagizeSound1911;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;
        }
    }

    public void PlayShootingSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.Pistol1911:
                ShootingChannel.PlayOneShot(P1911Shot);
                break;
            case WeaponModel.M4_8:
                ShootingChannel.PlayOneShot(M4_8Shot);
                break;
        }
    }

    public void PlayReloadSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.Pistol1911:
                reloadSound1911.Play();
                break;
            case WeaponModel.M4_8:
                reloadSoundM4_8.Play();
                break;
        }
    }
}
