using Unity.Profiling;
using UnityEngine;

public class Boids : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed;
    [SerializeField] float spinSpeed;
    [Header("Perception")]
    [SerializeField] float radius = 5;
    [SerializeField] float separationRadius = 4;
    public float weightCohesion = 1, weightAlineation = 1, weightSeparation = 1.5f;
    public float weightLimit = 5;
    float weightObstacle = 6;

    float weightFaction = 10;
    [SerializeField] public LayerMask factionMask;
    static readonly ProfilerMarker pNormasVecinidad = new ProfilerMarker("NormasVecinidad");


    float seeingAngle = 30;
    BoidManager boidManager;
    public LayerMask obstacleLayer;

    Vector3 direction;
    Vector3 cohesion;
    Vector3 alineation;
    Vector3 separation;

    Vector3 limit;
    Vector3 obstacle;
    Vector3 faction;
    public Vector3 objectiveDirection;

    const int MAX_NEIGHBOURS = 15;

    private void Start()
    {
        boidManager = BoidManager.Instance;
        direction = transform.forward;
    }
    private void Update()
    {
        using (pNormasVecinidad.Auto())
        {
            CalculateNeighbourhood();
        }
        limit = CalculateLimit();
        obstacle = CalculateObstacle();
        faction = CalculateFactionDistance();
        objectiveDirection =
            (cohesion * weightCohesion + alineation * weightAlineation
            + separation * weightSeparation + limit * weightLimit + obstacle * weightObstacle + faction * weightFaction).normalized;
        if (objectiveDirection.sqrMagnitude > 0.0001f)
        {
            direction = Vector3.RotateTowards(direction, objectiveDirection, spinSpeed * Time.deltaTime, 0.0f).normalized;
        }

        transform.forward = Vector3.Lerp(transform.forward, objectiveDirection, spinSpeed * Time.deltaTime);
        transform.position += transform.forward * speed * Time.deltaTime;

    }
    private void CalculateNeighbourhood()
    {
        Vector3 center = Vector3.zero;
        int numNeighbours = 0;
        Vector3 dir_Alineation = Vector3.zero;
        Vector3 dir_Separation = Vector3.zero;
        for (int i = 0; i < boidManager.boids.Count; i++)
        {
            if (numNeighbours > MAX_NEIGHBOURS)
            {
                break;
            }
            if (boidManager.boids[i] == this || (boidManager.boids[i].factionMask.value & this.factionMask.value) == 0)
            {
                continue;
            }
            Vector3 dif = (boidManager.boids[i].transform.position - this.transform.position);
            float d2 = dif.sqrMagnitude;
            if (d2 > 1e-6 && d2 < radius * radius && Seeing(boidManager.boids[i]))
            {
                dir_Separation += dif / d2;
                center += boidManager.boids[i].transform.position;
                dir_Alineation += boidManager.boids[i].transform.forward;
                numNeighbours++;
            }
        }
        separation = (dir_Separation).normalized;
        if (numNeighbours == 0)
        {
            return;
        }
        center /= numNeighbours;
        cohesion = (center - transform.position).normalized;
        alineation = (dir_Alineation / numNeighbours).normalized;


    }
    public Vector3 CalculateLimit()
    {
        if ((boidManager.transform.position - this.transform.position).sqrMagnitude < boidManager.radiusZone * boidManager.radiusZone)
        {
            return Vector3.zero;
        }
        return (boidManager.transform.position - transform.position).normalized;
    }
    public Vector3 CalculateObstacle()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, radius, obstacleLayer))
        {
            Vector3 dirEscape = Vector3.zero;

            dirEscape -= transform.forward;

            if (Physics.Raycast(transform.position, -transform.up, out hit, separationRadius, obstacleLayer))
            {
                dirEscape += transform.up;
            }
            else if (Physics.Raycast(transform.position, transform.up, out hit, separationRadius, obstacleLayer))
            {
                dirEscape -= transform.up;
            }
            else if (Physics.Raycast(transform.position, -transform.forward, out hit, separationRadius, obstacleLayer))
            {
                dirEscape += transform.forward;
            }
            else if (Physics.Raycast(transform.position, transform.forward, out hit, separationRadius, obstacleLayer))
            {
                dirEscape -= transform.forward;
            }
            return dirEscape.normalized;
        }

        return Vector3.zero;
    }
    public Vector3 CalculateFactionDistance()
    {
        Vector3 bestDir = Vector3.zero;
        float bestDist = float.MaxValue;
        for (int i = 0; i < boidManager.boids.Count; i++)
        {
            if (boidManager.boids[i] == this || (boidManager.boids[i].factionMask.value & this.factionMask.value) != 0)
            {
                continue;
            }
            if ((boidManager.boids[i].factionMask.value & this.factionMask.value) == 0 && Seeing(boidManager.boids[i]))
            {
                Vector3 toBoid = boidManager.boids[i].transform.position - transform.position;
                if (toBoid.sqrMagnitude < bestDist)
                {
                    bestDist = toBoid.sqrMagnitude;
                    bestDir = toBoid.normalized;
                }
            }

        }
        return bestDir;
    }

    bool Seeing(Boids other)
    {
        Vector3 dirOther = other.transform.position - this.transform.position;
        float angle = Vector3.Angle(this.transform.forward, dirOther);
        if (angle < seeingAngle)
        {
            return true;
        }
        return false;
    }
}
