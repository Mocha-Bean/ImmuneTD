using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Attacker
{
    [SerializeField]
    private Transform gunTransform;
    [SerializeField]
    private Animator gunAnimator;
    [SerializeField]
    private GameObject bulletPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 diff = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float diffAngle = Mathf.Atan2(diff.y, diff.x);
        gunTransform.rotation = Quaternion.Euler(0f, 0f, diffAngle * Mathf.Rad2Deg);

        if (Input.anyKeyDown)
        {
            InputKeyHandler();
        }
    }

    protected void InputKeyHandler()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    protected void Shoot()
    {
        gunAnimator.SetTrigger("Shoot");
        GameObject bulletObject = GameObject.Instantiate(bulletPrefab, gunTransform.position, Quaternion.identity);
        Bullet bullet = bulletObject.GetComponent<Bullet>();
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        bullet.ShootTowards(mousePos);
        bullet.shotBy = this;   // bullet shot by me
    }
}
