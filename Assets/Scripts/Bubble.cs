using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Bubble : PausableObject
{
    public float density = 0.2f; //1 = Same as air
    private float tempY;

    public AnimationCurve speedCurve;
    public GameObject popEffect;
    const float gravity = -2.4f;
    const float fixed_height = 7.15f; // max speed is height in 1 second
    const float max_speed = fixed_height * 0.5f;

    private Vector3 cached_velocity;
    public float pos = 0.0f;
    private Rigidbody _rigid = null;
    private Rigidbody rigid { get {
            if (!_rigid)
                _rigid = GetComponent<Rigidbody>();
            return _rigid; 
        } 
    }

    // Start is called before the first frame update
    void Start()
    {
        if (density >= 1.5f)
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

        if(transform.position.y >= fixed_height || transform.localPosition.y < 0 )
            Pop( false );

        if (density >= 1.5f)
            Pop(false);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!collider.enabled)
            return;

        var speed = rigid.velocity;
        if (collider.gameObject.tag == "Wall")
            speed.x = -speed.x;

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
        speed.y = speedCurve.Evaluate(pos) * (1.0f - density) * max_speed;

        rigid.velocity = speed;
        //var upwards_force = (density_ratio) * gravity;
        //var combined_force = new Vector2(0, upwards_force);
        //var temp = transform.position;
        //speed.x = Mathf.Min(speed.x + combined_force.x * Time.deltaTime, max_speed);
        //speed.y = Mathf.Min(speed.y + combined_force.y * Time.deltaTime, max_speed);
    }

    public void SetVelocity(Vector3 velocity)
    {
        var rigid = GetComponent<Rigidbody>();
        rigid.velocity = velocity;
        
        pos = speedCurve.EvaluateInverse(rigid.velocity.y / max_speed);
    }

    void Pop( bool allow_splitting )
    {
        var rigid = GetComponent<Rigidbody>();
        rigid.velocity = Vector3.zero;
        if (allow_splitting)
        {

        }

        popEffect.SetActive(true);
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
