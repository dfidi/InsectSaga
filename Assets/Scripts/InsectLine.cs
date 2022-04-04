using UnityEngine;

public class InsectLine : PathLine
{
    static PlayerController player;
    static int layerMask = -100;

    public int currentPoint;

    void Start()
    {
        if (player is null)
        {
            player = FindObjectOfType<PlayerController>();
        }
        if (layerMask != -100) layerMask = 1 << LayerMask.NameToLayer("Player");
    }

    void FixedUpdate()
    {
        CheckPlayerIntersect();
    }

    void CheckPlayerIntersect()
    {
        if (points is null) return;
        for (int i = 1; i < points.Length; i++)
        {
            var p1 = points[i-1].position;
            var p2 = points[i].position;
            //RaycastHit2D hit = Physics2D.Raycast(p1, (p2-p1), Vector2.Distance(p1, p2), layerMask);

            if (Physics2D.Raycast(p1, (p2-p1), Vector2.Distance(p1, p2), layerMask).collider != null)
            {
                player.SetPathAndEnableRunMode(this, i-1);
            }
        }
    }

    public void GetNextPoint()
    {
        
    }
}
