using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public Camera playerCam;
    public GameObject Pistol;
    public GameObject impactFlash;
    public Shake cameraShake;
    public StartGunAnimation pistolAnim;
    public StartGunAnimation rifleAnim;
    public StartGunAnimation shotgunAnim;
    private MovementInput playerMovement;

    public Text crosshairUI;
    public Text shotgunCrosshairUI;

    private bool canShoot;
    

    private RaycastHit hitObject;

    private AudioSource gunSource;

    public Scrollbar ammoUI;
    private AmmoType currentGun;
    AmmoType pistol;
    AmmoType rifle;
    AmmoType shotgun;

    private float pistolDamage;
    public Light pistolFlash;
    public GameObject pistolMesh;
    private float pistolMaxAmmo;
    private float pistolCost;
    private float pistolRegenRate;
    public float pistolDelay;
    public AudioClip pistolSound;
    private float currentPistolAmmo;

    private float rifleDamage;
    public Light rifleFlash;
    public GameObject rifleMesh;
    private float rifleMaxAmmo;
    private float rifleCost;
    private float rifleRegenRate;
    public float rifleDelay;
    public AudioClip rifleSound;
    private float currentRifleAmmo;

    public Light shotgunFlash;
    public GameObject shotgunMesh;
    private float shotgunDamage;
    private float shotgunMaxAmmo;
    private float shotgunCost;
    private float shotgunRegenRate;
    public float shotgunDelay;
    public float shotgunRange;
    public AudioClip shotgunSound;
    private float currentShotgunAmmo;

    public int gunType;

    int playerMask;
    int weaponMask;
    int layerMask;

    struct AmmoType //Gun type struct
    {
        public int id;
        public float damage;
        public float currentAmmo;
        public float maxAmmo;
        public float ammoCost;
        public float regenRate;
        public float delay;
        public Light flash;
        public AudioClip gunSound;
        public StartGunAnimation animation;
    }

    void Start()
    {
        int difficulty = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().GetDifficulty();
        if (difficulty == 0) //Changes weapon stats based on diffuclty
        {
            pistolDamage = 10; pistolMaxAmmo = 120; pistolCost = 15; pistolRegenRate = 60;
            rifleDamage = 15; rifleMaxAmmo = 200; rifleCost = 10; rifleRegenRate = 20;
            shotgunDamage = 80; shotgunMaxAmmo = 225; shotgunCost = 75; shotgunRegenRate = 50;
        }
        else if (difficulty == 1)
        {
            pistolDamage = 50f; pistolMaxAmmo = 200; pistolCost = 5; pistolRegenRate = 200;
            rifleDamage = 50; rifleMaxAmmo = 200; rifleCost = 5; rifleRegenRate = 200;
            shotgunDamage = 150; shotgunMaxAmmo = 225; shotgunCost = 20; shotgunRegenRate = 200;
        }
        else if (difficulty == 3)
        {
            pistolDamage = 10f; pistolMaxAmmo = 120; pistolCost = 15; pistolRegenRate = 60;
            rifleDamage = 15; rifleMaxAmmo = 200; rifleCost = 10; rifleRegenRate = 20;
            shotgunDamage = 80; shotgunMaxAmmo = 225; shotgunCost = 75; shotgunRegenRate = 50;
        }
        else // Normal difficulty
        {
            pistolDamage = 10; pistolMaxAmmo = 120; pistolCost = 15; pistolRegenRate = 60; 
            rifleDamage = 15; rifleMaxAmmo = 300; rifleCost = 10; rifleRegenRate = 20;
            shotgunDamage = 80; shotgunMaxAmmo = 350; shotgunCost = 75; shotgunRegenRate = 50;
        }

        playerMask = 1 << 7;
        weaponMask = 1 << 6;
        layerMask = playerMask | weaponMask;
        layerMask = ~layerMask; //Not including layer 6 (Weapons)

        canShoot = true;
        playerMovement = GetComponent<MovementInput>();
        gunSource = GetComponent<AudioSource>();
        gunType = 1;

        //Assigns different values for each weapon type
        pistol.id = 1; pistol.damage = pistolDamage;  pistol.maxAmmo = pistolMaxAmmo; pistol.ammoCost = pistolCost; 
        pistol.currentAmmo = pistol.maxAmmo; pistol.regenRate = pistolRegenRate; pistol.delay = pistolDelay; 
        pistol.flash = pistolFlash; pistol.gunSound = pistolSound; pistol.animation = pistolAnim;

        rifle.id = 2; rifle.damage = rifleDamage; rifle.maxAmmo = rifleMaxAmmo; rifle.ammoCost = rifleCost;
        rifle.currentAmmo = rifle.maxAmmo; rifle.regenRate = rifleRegenRate; rifle.delay = rifleDelay; 
        rifle.flash = rifleFlash; rifle.gunSound = rifleSound; rifle.animation = rifleAnim;

        shotgun.id = 3; shotgun.damage = shotgunDamage; shotgun.maxAmmo = shotgunMaxAmmo; shotgun.ammoCost = shotgunCost; 
        shotgun.currentAmmo = shotgun.maxAmmo; shotgun.regenRate = shotgunRegenRate; shotgun.delay = shotgunDelay; 
        shotgun.flash = shotgunFlash; shotgun.gunSound = shotgunSound; shotgun.animation = shotgunAnim;

        currentPistolAmmo = pistolMaxAmmo;
        currentRifleAmmo = rifleMaxAmmo;
        currentShotgunAmmo = shotgunMaxAmmo;

        pistol.flash.enabled = false;
        rifle.flash.enabled = false;
        shotgun.flash.enabled = false;
        currentGun = pistol;
        rifleMesh.GetComponentInChildren<Renderer>().enabled = false;
        shotgunMesh.GetComponentInChildren<Renderer>().enabled = false;
    }

    private void ChangeGun(int type) //Changes to the new gun and disables the old one
    {
        if (type == 1) 
        { 
            currentGun = pistol; pistolMesh.GetComponentInChildren<Renderer>().enabled = true; 
            rifleMesh.GetComponentInChildren<Renderer>().enabled = false;  rifleFlash.range = 0;
            shotgunMesh.GetComponentInChildren<Renderer>().enabled = false; shotgunFlash.range = 0;
        }

        if (type == 2) 
        { 
            currentGun = rifle; rifleMesh.GetComponentInChildren<Renderer>().enabled = true; 
            pistolMesh.GetComponentInChildren<Renderer>().enabled = false; pistolFlash.range = 0;
            shotgunMesh.GetComponentInChildren<Renderer>().enabled = false; shotgunFlash.range = 0;
        }

        if (type == 3) 
        { 
            currentGun = shotgun; shotgunMesh.GetComponentInChildren<Renderer>().enabled = true; 
            pistolMesh.GetComponentInChildren<Renderer>().enabled = false; pistolFlash.range = 0;
            rifleMesh.GetComponentInChildren<Renderer>().enabled = false; rifleFlash.range = 0;
            crosshairUI.enabled = false; shotgunCrosshairUI.enabled = true; 
        }
        else
        {
            crosshairUI.enabled = true; shotgunCrosshairUI.enabled = false;
        }
    
    }

    private void UpdateAmmo() //Keeps track of all weapon's ammo regeneration
    {
        if (gunType == 1)
        {
            currentRifleAmmo += rifle.regenRate * Time.deltaTime; //Automatically regenerates stamina over time
            if (currentRifleAmmo > rifle.maxAmmo) { currentRifleAmmo = rifle.maxAmmo; }
            rifle.currentAmmo = currentRifleAmmo;

            currentShotgunAmmo += shotgun.regenRate * Time.deltaTime; 
            if (currentShotgunAmmo > shotgun.maxAmmo) { currentShotgunAmmo = shotgun.maxAmmo; }
            shotgun.currentAmmo = currentShotgunAmmo;
        }
        else if (gunType == 2)
        {
            currentPistolAmmo += pistol.regenRate * Time.deltaTime; 
            if (currentPistolAmmo > pistol.maxAmmo) { currentPistolAmmo = pistol.maxAmmo; }
            pistol.currentAmmo = currentPistolAmmo;

            currentShotgunAmmo += shotgun.regenRate * Time.deltaTime; 
            if (currentShotgunAmmo > shotgun.maxAmmo) { currentShotgunAmmo = shotgun.maxAmmo; }
            shotgun.currentAmmo = currentShotgunAmmo;
        }
        else if (gunType == 3)
        {
            currentPistolAmmo += pistol.regenRate * Time.deltaTime; 
            if (currentPistolAmmo > pistol.maxAmmo) { currentPistolAmmo = pistol.maxAmmo; }
            pistol.currentAmmo = currentPistolAmmo;

            currentRifleAmmo += rifle.regenRate * Time.deltaTime;
            if (currentRifleAmmo > rifle.maxAmmo) { currentRifleAmmo = rifle.maxAmmo; }
            rifle.currentAmmo = currentRifleAmmo;
        }



        currentGun.currentAmmo += currentGun.regenRate * Time.deltaTime; //Automatically regenerates stamina over time
        if (currentGun.currentAmmo > currentGun.maxAmmo) { currentGun.currentAmmo = currentGun.maxAmmo; }
        ammoUI.size = currentGun.currentAmmo / currentGun.maxAmmo; //Links ammo to ammo UI

    }

    public void BoostAmmo(float amount) //When the player collects an ammo pickup
    {
        if (currentGun.id == 1)
        {
            currentRifleAmmo += amount;
            currentShotgunAmmo += amount;
        }
        else if (currentGun.id == 2)
        {
            currentPistolAmmo += amount;
            currentShotgunAmmo += amount;
        }
        else if (currentGun.id == 3)
        {
            currentPistolAmmo += amount;
            currentRifleAmmo += amount;
        }
        currentGun.currentAmmo += amount;
    }

    private void ChangeAmmo(int oldType, int newType) //Swaps what ammo is currently held by the player
    {
        if (oldType == 1)
        {
            currentPistolAmmo = currentGun.currentAmmo;

            if (newType == 2)
            {
                rifle.currentAmmo = currentRifleAmmo;
            }
            else if (newType == 3)
            {
                shotgun.currentAmmo = currentShotgunAmmo;
            }
        }
        else if (oldType == 2)
        {
            currentRifleAmmo = currentGun.currentAmmo;

            if (newType == 1)
            {
                pistol.currentAmmo = currentPistolAmmo;
            }
            else if (newType == 3)
            {
                shotgun.currentAmmo = currentShotgunAmmo;
            }
        }
        else if (oldType == 3)
        {
            currentShotgunAmmo = currentGun.currentAmmo;

            if (newType == 1)
            {
                pistol.currentAmmo = currentPistolAmmo;
            }
            else if (newType == 2)
            {
                rifle.currentAmmo = currentRifleAmmo;
            }
        }

    }

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0) //Changes gun with scrollwheel input
        {
            int oldType = gunType;
            gunType++;
            if (gunType > 3) { gunType = 1; }

            ChangeAmmo(oldType, gunType);
            ChangeGun(gunType);
            
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            int oldType = gunType;
            gunType--;
            if (gunType < 1) { gunType = 3; }


            ChangeAmmo(oldType, gunType);
            ChangeGun(gunType);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) && gunType != 1) //Changes gun with number input
        {
            int oldType = gunType;
            gunType = 1;

            ChangeAmmo(oldType, gunType);
            ChangeGun(gunType);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && gunType != 2)
        {
            int oldType = gunType;
            gunType = 2;

            ChangeAmmo(oldType, gunType);
            ChangeGun(gunType);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && gunType != 3)
        {
            int oldType = gunType;
            gunType = 3;

            ChangeAmmo(oldType, gunType);
            ChangeGun(gunType);
        }

        if (Input.GetMouseButtonDown(0) && gunType != 2 && canShoot && currentGun.currentAmmo - currentGun.ammoCost >= 0 && !playerMovement.isPaused) //Activates shot
        {
            canShoot = false;
            currentGun.flash.enabled = true;
            /*flashMesh.GetComponent<MeshRenderer>().enabled = false;*/
            StartCoroutine(ChangeSize());
            Fire();
            Invoke(nameof(ShootDelegate), currentGun.delay);
        }

        else if (Input.GetMouseButton(0) && gunType == 2 && canShoot && currentGun.currentAmmo - currentGun.ammoCost >= 0 && !playerMovement.isPaused) //Rifle automatic shooting
        {
            canShoot = false;
            currentGun.flash.enabled = true;
            /*flashMesh.GetComponent<MeshRenderer>().enabled = false;*/
            StartCoroutine(ChangeSize());
            Fire();
            Invoke(nameof(ShootDelegate), currentGun.delay);
        }

        
        bool objectHit;

        if (gunType == 3) //Checks if the player if aiming at an enemy
        {
            Vector3 shotgunCast = playerCam.transform.position;
            Vector3 shotgunExtent = new Vector3(0.35f, 0.35f, 0.35f);
            objectHit = Physics.BoxCast(shotgunCast, shotgunExtent, playerCam.transform.forward, out hitObject, new Quaternion(), shotgunRange, layerMask);
        }
        else
        {
            objectHit = Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hitObject, 500f, layerMask);
        }
        
        if (objectHit && hitObject.collider.tag == "AI")
        {
            if (gunType == 3)
            {
                shotgunCrosshairUI.color = new Color(162, 0, 0, 255);
            }
            else
            {
                crosshairUI.color = new Color(162, 0, 0, 255); //Changes crosshairs to red if true
            }
            
        }
        else
        {
            if (gunType == 3)
            {
                shotgunCrosshairUI.color = new Color(255, 255, 255, 255);
            }
            else
            {
                crosshairUI.color = new Color(255, 255, 255, 255);
            }
            
        }

        UpdateAmmo();
    }

    private void Fire() //Fires a shot
    {
        currentGun.currentAmmo -= currentGun.ammoCost;
        gunSource.PlayOneShot(currentGun.gunSound); //Plays sound
        
        playerMovement.isShooting = true;
        currentGun.animation.ShootAnimation(); //Plays animation

        StartCoroutine(cameraShake.ShakeCamera(0.025f, 0.01f)); //Initiates camera shake

        bool objectHit;
        if (gunType == 3) //Box cast for shotgun & raycast for pistol/rifle
        {
            Vector3 shotgunCast = playerCam.transform.position;
            Vector3 shotgunExtent = new Vector3(0.35f, 0.35f, 0.35f);
            objectHit = Physics.BoxCast(shotgunCast, shotgunExtent, playerCam.transform.forward, out hitObject, new Quaternion(), shotgunRange, layerMask);
        }
        else
        {
            objectHit = Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hitObject, 500f);
        }
        
        if (objectHit && hitObject.collider.tag == "AI")
        {
            hitObject.collider.GetComponent<Health>().TakeDamage(currentGun.damage); //Damages enemy if hit
        }
        if (objectHit && hitObject.collider.tag == "SpawnEnemy")
        {
            hitObject.collider.GetComponent<SpawnEnemy>().CreateEnemy(); //Spawns object if hit spawner object
        }

        if (objectHit)
        {
            Instantiate(impactFlash, hitObject.point, new Quaternion()); //Creates impact light
        }
    }

    private void ShootDelegate() //Allows player to shoot again
    {
        canShoot = true;
        playerMovement.playingWalkAnim = true;
        playerMovement.isShooting = false;
    }

    IEnumerator ChangeSize() //Smoothly adjusts gun flash when shooting
    {
        double timer = 0;
        while (true)
        {
            timer += 1 * Time.deltaTime;

            if (timer > 0.01)
            {
                currentGun.flash.range -= 0.1f;
                timer = 0;
            }

            if (currentGun.flash.range < 0.5f)
            {
                currentGun.flash.range = 0.75f;
                currentGun.flash.enabled = false;
                break;
            }

            yield return null;
        }
    }

}
