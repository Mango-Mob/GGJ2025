using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : PausableObject
{
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
            Pop(false);
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

        if(transform.position.y >= fixed_height || transform.position.y < 0 )
            Pop( false );

        if (density >= max_density)
            Pop(false);
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
            var bubble = collider.gameObject.GetComponent<Bubble>();
            var result_scale = Mathf.Max(bubble.transform.localScale.x, transform.localScale.x) + Mathf.Min(bubble.transform.localScale.x, transform.localScale.x) / 2;
            var result_density = GameManager.Instance.DensityScaleCurve.EvaluateInverse(result_scale);
            var velocity = rigid.velocity + bubble.GetComponent<Rigidbody>().velocity / 2.0f;

            GameManager.Instance.SpawnBubble(transform.parent, (collider.transform.position + transform.position) / 2.0f, velocity, result_density);
            collider.enabled = false;
            gameObject.GetComponent<Collider>().enabled = false;

            Destroy(gameObject);
            Destroy(collider.gameObject);
        }

        if (collider.gameObject.tag == "Enemy")
        {
            gameObject.GetComponent<Collider>().enabled = false;
            Pop(true);
            return;
        }

        if (collider.gameObject.tag == "Player")
        {
            is_colliding_with_player = true;
            var player = collider.GetComponent<PlayerController>();
            if (!player.isBoosting)
                player.HitPlayer(this);
            else
                Pop(false);

            return;
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

    public void Pop( bool allow_splitting )
    {
        gameObject.GetComponent<Collider>().enabled = false;
        var rigid = GetComponent<Rigidbody>();
        rigid.velocity = Vector3.zero;

        if (allow_splitting && density > min_density_for_split)
        {
            var bubble = gameObject.GetComponent<Bubble>();
            var result_density = density / 2.0f;
            var angle = Random.Range(45.0f, 70.0f);
            for (int i = 0; i < 2; i++)
            {
                pos = speedCurve.EvaluateInverse(rigid.velocity.y / max_speed.Evaluate(density));
                var speed = speedCurve.Evaluate(pos) * max_speed.Evaluate(result_density);
                var velocity = Quaternion.Euler(0, 0, i == 0 ? -angle : angle) * new Vector3(0, speed, 0);
                var obj = GameManager.Instance.SpawnBubble(transform.parent, transform.position, velocity, result_density);
                obj.GetComponent<Bubble>().immune = 0.25f;
            }
        }

        transform.parent = null;
        var list = GameManager.Instance.GetBubblesInRange(transform.position, transform.localScale.x * 5f);

        foreach (var bubble in list)
        {
            bubble.GetComponent<Rigidbody>().AddExplosionForce(50, transform.position, transform.localScale.x * 5f);
        }

        popEffect.SetActive(true);
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
