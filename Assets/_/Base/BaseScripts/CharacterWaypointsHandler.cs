using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using V_AnimationSystem;

/*
 * Character moves between waypoints
 * */
public class CharacterWaypointsHandler : MonoBehaviour {
        
    private const float speed = 30f;

    [SerializeField] private List<Vector3> waypointList;
    [SerializeField] private List<float> waitTimeList;
    private int waypointIndex;

    [SerializeField] private string idleAnimation = "dZombie_Idle";
    [SerializeField] private string walkAnimation = "dZombie_Walk";

    [SerializeField] private float idleFrameRate = 1f;
    [SerializeField] private float walkFrameRate = 1f;

    private V_UnitSkeleton unitSkeleton;
    private V_UnitAnimation unitAnimation;
    private AnimatedWalker animatedWalker;

    private enum State {
        Waiting,
        Moving,
    }

    private State state;
    private float waitTimer;

    private void Start() {
        Transform bodyTransform = transform.Find("Body");
        unitSkeleton = new V_UnitSkeleton(1f, bodyTransform.TransformPoint, (Mesh mesh) => bodyTransform.GetComponent<MeshFilter>().mesh = mesh);
        unitAnimation = new V_UnitAnimation(unitSkeleton);
        animatedWalker = new AnimatedWalker(unitAnimation, UnitAnimType.GetUnitAnimType(idleAnimation), UnitAnimType.GetUnitAnimType(walkAnimation), idleFrameRate, walkFrameRate);
        state = State.Waiting;
    }

    private void Update() {
        HandleMovement();
        unitSkeleton.Update(Time.deltaTime);
    }

    private void HandleMovement() {
        switch (state) {
        case State.Waiting:
            waitTimer -= Time.deltaTime;
            animatedWalker.SetMoveVector(Vector3.zero);
            if (waitTimer <= 0f) {
                state = State.Moving;
            }
            break;
        case State.Moving:
            Vector3 waypoint = waypointList[waypointIndex];

            Vector3 waypointDir = (waypoint - transform.position).normalized;

            float distanceBefore = Vector3.Distance(transform.position, waypoint);
            animatedWalker.SetMoveVector(waypointDir);
            transform.position = transform.position + waypointDir * speed * Time.deltaTime;
            float distanceAfter = Vector3.Distance(transform.position, waypoint);

            if (distanceBefore <= distanceAfter) {
                // Go to next waypoint
                waitTimer = waitTimeList[waypointIndex];
                waypointIndex = (waypointIndex + 1) % waypointList.Count;
                state = State.Waiting;
            }
            break;
        }
    }

}
