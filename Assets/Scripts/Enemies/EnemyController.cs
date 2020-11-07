using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 1f;
    [SerializeField] private float _patrolPointStayTime = 1f;
    [SerializeField] private float _patrolPointProximity = 0.3f;

    [SerializeField] private Transform _patrolPointsParent = null;
    
    private float _timeToMove = 0f;

    private Queue<Transform> _patrolPoints = new Queue<Transform>();
    private Transform _targetPatrolPoint = null;

    private Rigidbody2D _rigidbody = null;
    private Animator _animator = null;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        PopulatePatrolPoints();
        GoToNextPatrolPoint();
    }

    private void OnDisable()
    {
        _animator.SetFloat("velocityX", 0f);
    }

    private void Update()
    {
        WaitAtPatrolPoint();
    }

    private void FixedUpdate()
    {
        Move();
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        _animator.SetFloat("velocityX", Mathf.Abs(_rigidbody.velocity.x));
    }

    private void WaitAtPatrolPoint()
    {
        if (_targetPatrolPoint == null || _timeToMove <= 0f)
            return;

        _timeToMove -= Time.deltaTime;

        if (_timeToMove > 0f)
            return;

        GoToNextPatrolPoint();
    }

    private void Move()
    {
        if (_targetPatrolPoint == null || _timeToMove > 0f)
            return;

        float targetX = _targetPatrolPoint.position.x;
        float enemyX = transform.position.x;

        if (Mathf.Abs(targetX - enemyX) <= _patrolPointProximity)
        {
            ReachedPatrolPoint();
            return;
        }

        float direction = targetX < enemyX ? -1f : 1f;

        transform.localScale = new Vector2(direction * Mathf.Abs(transform.localScale.x), 1f * Mathf.Abs(transform.localScale.y));
        SetVelocity(_movementSpeed * direction, _rigidbody.velocity.y);
    }

    private void ReachedPatrolPoint()
    {
        if (_targetPatrolPoint == null)
            return;

        SetVelocity(0f, 0f);

        if (_patrolPointStayTime > 0f)
        {
            _timeToMove = _patrolPointStayTime;
            return;
        }

        GoToNextPatrolPoint();
    }

    private void PopulatePatrolPoints()
    {
        if (_patrolPoints == null)
            return;

        for (int i = 0; i < _patrolPointsParent.childCount; i++)
            _patrolPoints.Enqueue(_patrolPointsParent.GetChild(i));
    }

    private void GoToNextPatrolPoint()
    {
        if (_targetPatrolPoint != null)
            _patrolPoints.Enqueue(_targetPatrolPoint);

        if (_patrolPoints.Count == 0)
            return;

        _targetPatrolPoint = _patrolPoints.Dequeue();
    }

    private void SetVelocity(float x, float y)
    {
        _rigidbody.velocity = new Vector2(x, y);
    }
}

