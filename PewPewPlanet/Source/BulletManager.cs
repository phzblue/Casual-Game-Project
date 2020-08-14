using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletManager : MonoBehaviour
{
	[SerializeField] GameObject whichBullet = null;
	[SerializeField] GameObject laserBullet = null;
	[SerializeField] GameObject shootParticle = null;
	[SerializeField] Image bulletImage = null;
	[SerializeField] Image infiniteImage = null;
	[SerializeField] Text bulletCount = null;
	[SerializeField] List<Sprite> bulletImages = null;
	[SerializeField] BulletType type = BulletType.Default;

	[SerializeField] AudioClip shoot_laser = null;
	[SerializeField] AudioClip shoot_boomerang = null;
	[SerializeField] AudioClip shoot_ulti = null;
	[SerializeField] AudioClip shoot_default = null;
	[SerializeField] AudioClip changeBullet = null;

	public ObjectPooler bulletPool = null;
	public ObjectPooler feverBulletPool = null;
	public ObjectPooler boomerangBulletPool = null;
	public bool needSlowmo = false;

	int bulletAmount = -1;

	private void Start()
	{
		type = BulletType.Default;
	}

	public void ShootBullet()
	{
		if (GameSceneController.instance.isPlayable)
		{
			switch (type)
			{
				case BulletType.Boomerang:
					SoundManager.instance.PlaySFX(shoot_boomerang);
					bulletAmount--;
					whichBullet = boomerangBulletPool.GetPooledObject();
					whichBullet.transform.localPosition = new Vector2(0, 0);
					whichBullet.SetActive(true);
					break;
				case BulletType.Laser:
					SoundManager.instance.PlaySFX(shoot_laser);
					bulletAmount--;
					laserBullet.SetActive(true);
					break;
				case BulletType.Ulti:
					shootParticle.SetActive(true);
					SoundManager.instance.PlaySFX(shoot_ulti);
					whichBullet = feverBulletPool.GetPooledObject();
					whichBullet.transform.localPosition = new Vector2(0, 0);
					whichBullet.SetActive(true);
					break;
				case BulletType.Default:
					if (needSlowmo)
					{
						Time.timeScale = 0.1f;
						needSlowmo = false;
					}

					SoundManager.instance.PlaySFX(shoot_default);
					shootParticle.SetActive(true);

					whichBullet = bulletPool.GetPooledObject();
					whichBullet.transform.localPosition = new Vector2(0, 0);
					whichBullet.SetActive(true);
					break;
			}

			UpdateBulletCount();

			if (bulletAmount == 0)
			{
				ChangeBulletType(BulletType.Default);
			}
		}
	}

	public void ChangeBulletType(BulletType type)
	{
		SoundManager.instance.PlaySFX(changeBullet);

		this.type = type;
		bulletImage.transform.rotation = Quaternion.identity;

		switch (type)
		{
			case BulletType.Boomerang:
				bulletImage.sprite = bulletImages[3];
				bulletImage.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -45));
				bulletAmount = 1;
				break;

			case BulletType.Default:
				bulletImage.sprite = bulletImages[0];
				bulletAmount = -1;
				break;
			case BulletType.Laser:
				bulletImage.sprite = bulletImages[2];
				bulletAmount = 1;
				break;
			case BulletType.Ulti:
				bulletImage.sprite = bulletImages[1];
				bulletAmount = -1;
				break;
		}

		bulletImage.SetNativeSize();
		UpdateBulletCount();
	} 

	private void UpdateBulletCount()
	{
		if (bulletAmount != -1)
		{
			bulletCount.text = bulletAmount.ToString();
			bulletCount.gameObject.SetActive(true);
			infiniteImage.gameObject.SetActive(false);
		}
		else
		{
			bulletCount.gameObject.SetActive(false);
			infiniteImage.gameObject.SetActive(true);
		}
	}

	public enum BulletType
	{
		Laser,
		Boomerang,
		Ulti,
		Default
	}
}
