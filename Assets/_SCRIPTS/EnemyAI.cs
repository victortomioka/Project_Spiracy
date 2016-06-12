using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{

    public float targetDistance = 40f;
    public float linearSpeed = 1f;
    public enum State { Idle, Closing, Shooting, Melee, Stopped };
    State currentState = State.Closing;

	public Attack[] attacks;
	public float coolDown = 1f;
	private Attack currAtk;

    //variables for movement
    public Vector2 Frequency = new Vector2(20f, 20f);
    public Vector2 Magnitude = new Vector2(0.5f, 0.5f);
    private Vector3 pos;
    private Vector3 axis;

	public Transform[] muzzles;

    private Transform target;
    private HealthManager enHealth;
    private bool Roaming;

	private int staggerCount;
	private float spreadFactor = 0;
	private float currentBurstCoolDown;
	private float currentCoolDown;
	private int burstLeft;

	public GameObject[] Pickups;

    void Awake()
    {
        axis = new Vector3(transform.position.x, transform.position.y, targetDistance);
    }

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        enHealth = gameObject.GetComponent<HealthManager>();
		enHealth.OnDeath += spawnPickup;
        currentState = State.Closing;
        pos = transform.position;
        currentCoolDown = coolDown;

    }

    void FixedUpdate()
    {
		switch(currentState){
			case State.Closing: StateClosing(); break;
			case State.Idle: StateIdle(); break;
			case State.Shooting: StateShooting(); break;
			case State.Stopped: StateStopped(); break;
		} 
    }

	void StateClosing(){
		if ((target.transform.position - transform.position).magnitude <= targetDistance && currentState == State.Closing)
		{
			Vector3 newPosition = transform.position;
			
			newPosition.z = target.transform.position.z + targetDistance;
			transform.position = newPosition;
			currentState = State.Idle;
			
			pos = axis;
			currentCoolDown = 0;
			currentBurstCoolDown = 0;
		}
		
		
		else
		{
			float dist = Vector3.Distance(transform.position, target.position);
			pos += transform.forward * linearSpeed * (dist / targetDistance) * Time.deltaTime * Globals.speedScale;
			Move();
		}

	}

	void StateIdle(){
		Move();
		if (currentState != State.Shooting)
		{
			currentCoolDown -= Time.deltaTime;
			if (currentCoolDown <= 0)
			{
				currentCoolDown = 0;
				currentBurstCoolDown = 0;
				currAtk = attacks[Random.Range(0,attacks.Length)];
				burstLeft = currAtk.burstLength;
				currentState = State.Shooting;
		

			}
		}
	}

	void StateShooting(){
		Burst(currAtk);
		Move();
	}

	void StateStopped(){
		this.gameObject.GetComponent<EnemyAI>().enabled=false;
	}

    void Move()
    {
        if (Time.timeScale>0 && this.enabled==true)
        {
			transform.position = pos + new Vector3(Mathf.Sin(Time.time * Frequency.x) * Magnitude.x, Mathf.Sin(Time.time * Frequency.y) * Magnitude.y, 0.1f);
        }
    }
	
    void Burst(Attack atk)
    {
        currentBurstCoolDown -= Time.deltaTime;
        if (currentBurstCoolDown <= 0 && burstLeft > 0)
        {
            currentBurstCoolDown = 0;
            shoot(atk.spreadBase,atk.projectiles,muzzles,atk.shotCount,atk.isStaggered,atk.isScattered);
            currentBurstCoolDown = atk.burstCoolDown;
            burstLeft--;
        }

        if (burstLeft <= 0)
        {
            burstLeft = 0;
            currentCoolDown = coolDown;
            currentState = State.Idle;
        
        }
    }

    void aim(Transform tgt)
    {
        foreach (Transform muz in muzzles)
        {
            muz.LookAt(tgt);
        }
    }

    void disableFSM()
    {
        currentState = State.Stopped;
    }

	void spawnPickup()
	{
		float pickupChance = Random.Range(0,100);
		if(pickupChance<50){
			int pickupItem = Random.Range(0,Pickups.Length);
			Instantiate (Pickups[pickupItem],transform.position,transform.rotation);
			}
		disableFSM();
	}

	void shoot(float spr, Rigidbody[] bul, Transform[] muz, int cnt, bool stag, bool scat)
    {

        float spreadIncrease = spr / 10;
        float spreadMax = spr * 3;
        spreadFactor += spreadIncrease;
        
        aim(target);

        if (spreadFactor >= spreadMax)
        {
            spreadFactor = spreadMax;
        }

        currentBurstCoolDown -= Time.deltaTime;

        if (currentBurstCoolDown <= 0)
        {
            if (!stag)
            {
                for (int j = 0; j < muz.Length; j++)
                {
                    if (scat)
                    {
                        for (int i = 0; i < cnt; i++)
                        {
                            Rigidbody newBullet = Instantiate(bul[Random.Range(0,bul.Length)], muz[j].position, muz[j].rotation) as Rigidbody;
                            newBullet.transform.Rotate(Utility.calcSpread(spreadFactor));
                        }
                    }

                    if (!scat)
                    {
						Rigidbody newBullet = Instantiate(bul[Random.Range(0,bul.Length)], muz[j].position, muz[j].rotation) as Rigidbody;
                        newBullet.transform.Rotate(Utility.calcSpread(spreadFactor));
                    }
                }
            }
            if (stag)
            {
                staggerCount++;
                if (staggerCount >= muz.Length)
                {
                    staggerCount = 0;
                }
                if (scat)
                {
                    for (int i = 0; i < cnt; i++)
                    {
						Rigidbody newBullet = Instantiate(bul[Random.Range(0,bul.Length)], muz[staggerCount].position, muz[staggerCount].rotation) as Rigidbody;
                        newBullet.transform.Rotate(Utility.calcSpread(spreadFactor));
                    }
                }


                if (!scat)
                {
					Rigidbody newBullet = Instantiate(bul[Random.Range(0,bul.Length)], muz[staggerCount].position, muz[staggerCount].rotation) as Rigidbody;
                    newBullet.transform.Rotate(Utility.calcSpread(spreadFactor));
                }
            }
        }
    }

	[System.Serializable]
	public class Attack{
		public bool isStaggered;
		public bool isScattered;
		public Rigidbody[] projectiles;
		public int burstLength = 3;
		public float spreadBase;
		public int shotCount = 1;
		public float burstCoolDown = 1f;

	}

}
