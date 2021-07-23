using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Combat;

public class MovingAttacker : Attacker
{
    public float moveSpeed;
    public Vector3 effectiveVector;
    public Tilemap Floor;
    public GameManager gameManager;
    public Attacker blockedBy;

    public int energyReward;

    [SerializeField]
    private TileBase path;
    [SerializeField]
    private Animator animator;

    private enum MoveDir
    {
        right,
        down,
        left,
        up
    }
    private MoveDir moveDir;
    private Vector3Int moveVector;
    public Vector3Int currentTilePos;
    private Vector3Int checkTilePos;
    private Vector3 potentialNextPos;

    public void BlockBy(MeleeTower tower)
    {
        blockedBy = tower;
        Debug.Log("start");
        StartCoroutine(WakeUp());
    }

    protected override void TimedAttack()
    {
        if (activeEffects.Contains(StatusEffect.MoveBlock))
        {
            blockedBy.Attack(effectiveDamage, attackEffects, this);
            animator.SetTrigger("Attack");
        }
        else
        {
            awake = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.position = Floor.CellToWorld(Floor.WorldToCell(transform.position)) + new Vector3(0.5f, 0.5f, 0f);    // snap to grid
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);   // make sure we're at z = 0
        moveVector = Vector3Int.right;
        moveDir = MoveDir.right;
    }

    private void FixedUpdate()
    {
        currentTilePos = Floor.WorldToCell(transform.position);
        checkTilePos = currentTilePos + moveVector;
        effectiveVector = (Vector3)moveVector * moveSpeed * moveSpeedMult;
        potentialNextPos = transform.position + effectiveVector * Time.deltaTime;
        if (Floor.GetTile(checkTilePos) == path)
        {
            transform.position = potentialNextPos;
        }
        else if (moveSpeedMult > 0)
        {
            if (((Floor.CellToWorld(currentTilePos) + new Vector3(0.5f, 0.5f, 0f)) - potentialNextPos).normalized == -moveVector) // is our next intended move past the center of our current tile?
            {
                switch (moveDir)    // we need to turn; where do we turn based on our current direction?
                {
                    case MoveDir.left:
                    case MoveDir.right:
                        checkTilePos = currentTilePos + Vector3Int.down;
                        if(Floor.GetTile(checkTilePos) == path)
                        {
                            moveDir = MoveDir.down;
                            moveVector = Vector3Int.down;
                            transform.localScale = new Vector3(1, 1, 1); 
                            break;
                        }
                        else
                        {
                            checkTilePos = currentTilePos + Vector3Int.up;
                            if(Floor.GetTile(checkTilePos) == path)
                            {
                                moveDir = MoveDir.up;
                                moveVector = Vector3Int.up;
                                transform.localScale = new Vector3(1, 1, 1);
                                break;
                            }
                            else
                            {
                                Kill(); // we give up lol
                                break;
                            }
                        }
                    case MoveDir.up:
                    case MoveDir.down:
                        checkTilePos = currentTilePos + Vector3Int.left;
                        if (Floor.GetTile(checkTilePos) == path)
                        {
                            moveDir = MoveDir.left;
                            moveVector = Vector3Int.left;
                            transform.localScale = new Vector3(-1, 1, 1);   // flip around, including scale of child objects like rangebox (which is directional)
                            break;
                        }
                        else
                        {
                            checkTilePos = currentTilePos + Vector3Int.right;
                            if (Floor.GetTile(checkTilePos) == path)
                            {
                                moveDir = MoveDir.right;
                                moveVector = Vector3Int.right;
                                transform.localScale = new Vector3(1, 1, 1);
                                break;
                            }
                            else
                            {
                                Kill();
                                break;
                            }
                        }
                }
                transform.position = Floor.CellToWorld(Floor.WorldToCell(transform.position)) + new Vector3(0.5f, 0.5f, 0f);    // snap to grid
            } else
            {
                transform.position = potentialNextPos;
            }
        }
    }

    protected override IEnumerator Kill()
    {
        gameManager.ReportEnemyDeath(this, energyReward);   // if we were killed by PlayerDamageTrigger, it will remotely set our energyReward to 0
        return base.Kill();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
