using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public enum EnemyState
{
    Idle,//待机
    Patrol,//巡逻
    Chase,//追逐
    Attack,//攻击
    GetHit,//受击
    Death,//死亡
}

public class EnemyBase : MonoBehaviour
{
    [Header("Enemy 基础属性")]
    public float HPMax = 100f;
    public float HPNow = 100f;
    public float AttackDamage = 10f;

    [Header("Enemy 待机")]
    public float idleTime = 2f;
    public SpriteRenderer sr;
    public Animator enemyAnimator;

    [Header("Enemy 攻击")]
    public bool isAttacking = false;
    public bool canAttack = true;
    public float AttackRange = 1.5f;
    [HideInInspector]public float AttackTime = 1f;//攻击时间
    public float AttackWindup = 1f;//攻击前摇时间
    public GameObject AttackBox;
    public Transform AttackPoint1;
    public Transform AttackPoint2;


    [Header("Enemy 移动")]
    public EnemyState currentState = EnemyState.Patrol;
    public Transform left;
    public Transform right;
    public Rigidbody2D rb;
    public bool isRight = true;
    public float speed = 1f;
    public bool canMove = true;
    public float chaseSpeed = 1.75f;
    public float patrolSpeed = 1f;

    [Header("Enemy 追击")]
    public GameObject player;
    //public virtual void Start()
    //{
    //    ChangeState(EnemyState.Patrol);
    //}
    private IEnumerator Start()
    {
        if (worldStateManager != null)
        {
            //等待服务器存档加载完成
            yield return new WaitUntil(() => worldStateManager.IsInitialized);
            //检查这个敌人是否已经被击败
            if (worldStateManager.IsEnemyDefeated(enemyId))
            {
                Debug.Log("敌人已经被击败，不再生成：" + enemyId);
                Destroy(EnemyAndPosition);
                yield break;
            }
        }
        ChangeState(EnemyState.Patrol);
    }

    [Header("Enemy 受击")]
   
    public bool isGetHit = false;
    public float GetHitForce = 5f;

    //[Header("血条")]
    //public Slider hpSlider;

    [Header("Enemy 死亡")]
    public GameObject EnemyAndPosition;



    //存档
    [Header("Enemy 存档")]
    public string enemyId;
    public WorldStateManager worldStateManager;



    public virtual void Start()
    {
        ChangeState(EnemyState.Patrol);
        //hpSlider.value = HPNow / HPMax;
    }
    // Update is called once per frame
    public virtual void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                IdleUpdate();
                break;
            case EnemyState.Patrol:
                PatrolUpdate();
                break;
            case EnemyState.Chase:
                ChaseUpdate();
                break;
            case EnemyState.Attack:
                AttackUpdate();
                break;
            case EnemyState.GetHit:
                GetHitUpdate();
                break;
            case EnemyState.Death:
                DeathUpdate();
                break;
        }
    }

    public virtual void FixedUpdate()
    {
        
        if (canMove)
        {
            rb.velocity = new Vector2(isRight ? speed : -speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }


#region 状态机
    //待机
    public virtual void IdleEnter()
    {
        canMove = false;
        enemyAnimator.SetBool("IsRun",false);
        Invoke(nameof(Idle2Patrol), idleTime);
    }

    public virtual void IdleUpdate()
    {

    }

    public virtual void IdleExit()
    {
        CancelInvoke(nameof(Idle2Patrol));
    }    
    //巡逻
    public virtual void PatrolEnter()
    {
        canMove = true;
        speed = patrolSpeed;
        enemyAnimator.SetBool("IsRun",true);
    }

    public virtual void PatrolUpdate()
    {

        if (isRight && transform.position.x >= right.position.x)
        {
            ChangeState(EnemyState.Idle);
        }
        else if (!isRight && transform.position.x <= left.position.x)
        {
            ChangeState(EnemyState.Idle);
        }
    }
    public virtual void PatrolExit()
    {
        enemyAnimator.SetBool("IsRun",false);
    }    
    //追击
    public virtual void ChaseEnter()
    {
        canMove = true;
        speed = chaseSpeed;
        enemyAnimator.SetBool("IsChase",true);
        enemyAnimator.SetBool("IsRun",true);
    }

    public virtual void ChaseUpdate()
    {
        if (player != null)
        {
            if (transform.position.x < player.transform.position.x)
            {
                isRight = true;
                sr.flipX = false;
            }
            else
            {
                isRight = false;
                sr.flipX = true;
            }
            if (Mathf.Abs(transform.position.x - player.transform.position.x) <= AttackRange)
            {   
                if (player.transform.position.x <= transform.position.x && !isRight)
                {
                    ChangeState(EnemyState.Attack);
                }
                else if (player.transform.position.x >= transform.position.x && isRight)
                {
                    ChangeState(EnemyState.Attack);
                }
                
            }
        }
    }

    public virtual void ChaseExit()
    {
       enemyAnimator.SetBool("IsChase",false);
       enemyAnimator.SetBool("IsRun",false);

      
    }   
    //攻击
    public virtual void AttackEnter()
    {
        canMove = false;
        isAttacking = false;
        canAttack = true;
       
    }

    public virtual void AttackUpdate()
    {
        if (canAttack && !isAttacking)
        {
            isAttacking = true;
            canAttack = false;
            enemyAnimator.SetTrigger("Idle");
            Invoke(nameof(PerformAttack), AttackWindup);
        }
        if (Mathf.Abs(transform.position.x - player.transform.position.x) > AttackRange || (player.transform.position.x <= transform.position.x && isRight) || (player.transform.position.x >= transform.position.x && !isRight))
        {
            ChangeState(EnemyState.Chase);
        }
        
    }

    public virtual void AttackExit()
    {
        isAttacking = false;
        canAttack = true;
        CancelInvoke(nameof(PerformAttack));
        CancelInvoke(nameof(AttackCooldown));
    }   
    // 受击
    public virtual void GetHitEnter()
    {
        canMove = false;
        enemyAnimator.SetBool("GetHit", true);
        enemyAnimator.SetTrigger("GetHit_Trigger");
        // enemyAnimator.SetBool("isRun", false);

        if(player != null)
        {
            if (transform.position.x < player.transform.position.x)
            {
                rb.AddForce(new Vector2(-GetHitForce, 0));
            }
            else
            {
                rb.AddForce(new Vector2(GetHitForce, 0));
            }
        }
    }

    public virtual void GetHitUpdate()
    {
        if (!isGetHit)
        {
            isGetHit = true;

        }
        

    }

    public virtual void GetHitExit()
    {
        canMove = true;
        isGetHit = false;
        enemyAnimator.SetBool("GetHit", false);
    }   

    //死亡
    public virtual void DeathEnter()
    {
        canMove = false;
        //hpSlider.value = 0;
        enemyAnimator.SetBool("isRun", false);
        enemyAnimator.SetTrigger("IsDead");
        
    }
    public virtual void DeathUpdate()
    {

    }
    public virtual void DeathExit()
    {

    }
#endregion

    public virtual void ChangeState(EnemyState newState)
    {
        //退出当前状态
        switch (currentState)
        {
            case EnemyState.Idle:
                IdleExit();
                break;
            case EnemyState.Patrol:
                PatrolExit();
                break;
            case EnemyState.Chase:
                ChaseExit();
                break;
            case EnemyState.Attack:
                AttackExit();
                break;
            case EnemyState.GetHit:
                GetHitExit();
                break;
            case EnemyState.Death:
                DeathExit();
                break;
        }
        //进入新状态    
        currentState = newState;
        switch (currentState)
        {
            case EnemyState.Idle:
                IdleEnter();
                break;
            case EnemyState.Patrol:
                PatrolEnter();
                break;
            case EnemyState.Chase:
                ChaseEnter();
                break;
            case EnemyState.Attack:
                AttackEnter();
                break;
            case EnemyState.GetHit:
                GetHitEnter();
                break;
            case EnemyState.Death:
                DeathEnter();
                break;
        }
    }

    public virtual void Idle2Patrol()
    {
        isRight = !isRight;
        sr.flipX = !isRight;
        ChangeState(EnemyState.Patrol);       
    }

    public virtual void FindPlayer(GameObject mainPlayer)
    {
        if(currentState == EnemyState.Death)
        {
            return;
        }
        player = mainPlayer;
        ChangeState(EnemyState.Chase);
    }
    public virtual void OutPlayer()
    {
        if(currentState == EnemyState.Death)
        {
            return;
        }
        ChangeState(EnemyState.Patrol);
    }

    public virtual void PerformAttack()
    {
        enemyAnimator.SetTrigger("Attack1");
        Invoke(nameof(AttackCooldown), AttackTime);
    }

    public virtual void AttackCooldown()
    {
        isAttacking = false;
        canAttack = true;
    }

    public virtual void AddAttackBox()
    {
        GameObject go;
        if (isRight)
        {
            go = Instantiate(AttackBox,AttackPoint1.position,AttackPoint1.rotation,transform);
            go.transform.localScale = AttackPoint1.localScale;
        }
        else
        {
            go = Instantiate(AttackBox,AttackPoint2.position,AttackPoint2.rotation,transform);
            go.transform.localScale = AttackPoint2.localScale;
        }

        EnemyAttackBox attackBox = go.GetComponent<EnemyAttackBox>();

        if (attackBox != null)
        {
            attackBox.damage = AttackDamage;
        }
    }

    public virtual void GetHit(float damage)
    {
        if (currentState != EnemyState.Death && !isGetHit)
        {
            // 扣血逻辑
            HPNow -= damage;
            //hpSlider.value = HPNow / HPMax;
            if (HPNow <= 0)
            {
               ChangeState(EnemyState.Death);
            }
            else
            {
                ChangeState(EnemyState.GetHit);
            }
        }


    }

    public virtual void GetHitOut()
    {
        isGetHit = false;
        enemyAnimator.SetBool("GetHit", false);
        ChangeState(EnemyState.Chase);
    }

    /*public virtual void Delete()
    {
      Destroy(EnemyAndPosition,0.1f);
    }*/

    public virtual void Delete()
    {
        if (worldStateManager != null)
        {
            worldStateManager.RegisterDefeatedEnemy(enemyId);
        }
        Destroy(EnemyAndPosition, 0.1f);
    }
}
