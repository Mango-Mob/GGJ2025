using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBubble : PausableObject
{
    public static List<MenuBubble> global_list = new List<MenuBubble>();

    public float density = 0.2f; //1 = Same as air
    public float max_density = 2.0f;
    public float min_density_for_split = 0.8f;
    private float tempY;

    public AnimationCurve speedCurve;
    public GameObject popEffect;
    public float fixed_height = 7.15f; // max speed is height in 1 second
    public AnimationCurve max_speed;

    private Vector3 cached_velocity;
    public float pos = 0.0f;
    private Rigidbody _rigid = null;
    private Rigidbody rigid { get {
            if (!_rigid)
                _rigid = GetComponent<Rigidbody>();
            return _rigid; 
        } 
    }

    private bool is_colliding_with_player = false;
    private float immune = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        if (density >= max_density)
            Pop(true);
        else
            global_list.Add(this);
    }
    public void PlaySpawnAnimation()
    {
        GetComponent<Animator>().Play("Spawn");
    }

    private void Update()
    {
        if (is_paused)
            return;

        if(immune >= 0.0f)
            immune -= Time.deltaTime;

        if(transform.position.y >= fixed_height || transform.localPosition.y < 0 )
            Pop(false);

        if (density >= max_density)
            Pop(true);
    }

    private void OnDestroy()
    {
        global_list.Remove(this);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!collider.enabled || immune > 0.0f)
            return;

        var speed = rigid.velocity;
        if (collider.gameObject.tag == "Wall")
            speed.x = -speed.x;

        if (is_colliding_with_player)
            return;

        if(collider.gameObject.tag == "Bubble")
        {
            var bubble = collider.gameObject.GetComponent<MenuBubble>();
            var result_density = Mathf.Max(bubble.density, density) + Mathf.Min(bubble.density, density) / 2;
            var velocity = rigid.velocity + bubble.GetComponent<Rigidbody>().velocity / 2.0f;

            GetComponentInParent<MenuSpawner>().SpawnBubble(transform.parent, (collider.transform.position + transform.position) / 2.0f, velocity, result_density);
            collider.enabled = false;
            gameObject.GetComponent<Collider>().enabled = false;

            Destroy(gameObject);
            Destroy(collider.gameObject);
        }

        rigid.velocity = speed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (is_paused)
            return;

        var rigid = GetComponent<Rigidbody>();
        var speed = rigid.velocity;
        pos += Time.fixedDeltaTime;
        speed.y = speedCurve.Evaluate(pos) * max_speed.Evaluate(density);

        rigid.velocity = speed;
    }

    public void SetVelocity(Vector3 velocity)
    {
        var rigid = GetComponent<Rigidbody>();
        rigid.velocity = velocity;
        
        pos = speedCurve.EvaluateInverse(rigid.velocity.y / max_speed.Evaluate(density));
    }

    public void Pop( bool play_audio )
    {
        gameObject.GetComponent<Collider>().enabled = false;
        var rigid = GetComponent<Rigidbody>();
        rigid.velocity = Vector3.zero;

        transform.parent = null;

        if (!is_paused)
        {
            foreach (var bubble in global_list)
            {
                if (bubble == this)
                    continue;

                bubble.GetComponent<Rigidbody>().AddExplosionForce(150, transform.position, transform.localScale.x * 7f);
            }
        }

        popEffect.SetActive(true);
        if(play_audio)
            popEffect.GetComponent<MultiAudioAgent>().PlayRandom(false, Random.Range(0.75f, 1.25f));
        popEffect.transform.parent = null;
        popEffect.GetComponent<VFXTimerScript>().m_startedTimer = true;
        Destroy(gameObject);
    }

    protected override void OnPause()
    {
        cached_velocity = rigid.velocity;
        rigid.velocity = Vector3.zero;
    }

    protected override void OnUnPause()
    {
        rigid.velocity = cached_velocity;
    }
}
