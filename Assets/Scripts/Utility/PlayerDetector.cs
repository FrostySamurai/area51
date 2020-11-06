using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] private bool _requireVision = false;
    [SerializeField] private float _raycastAngleStep = 5f;

    private List<Vector2> _raycastDirections = new List<Vector2>();
    private float _raycastLength = 0f;

    private void Awake()
    {
        Light2D light = GetComponent<Light2D>();
        if (light == null)
        {
            Debug.LogError($"{name} is missing point light.");
            return;
        }

        CalculateRaycastDirections(light);
        _raycastLength = light.pointLightOuterRadius;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag != "Player")
            return;

        DetectPlayer(other);
    }

    private void DetectPlayer(Collider2D player)
    {
        if (_requireVision && !IsPlayerInVision(player))
            return;

        AppData.GameManager.PlayerDetected();
    }

    private bool IsPlayerInVision(Collider2D player)
    {
        float direction = player.transform.position.x < transform.position.x ? -1 : 1;
        foreach (Vector2 raycastDirection in _raycastDirections)
        {
            if (RaycastForPlayer(transform.position, new Vector2(raycastDirection.x * direction, raycastDirection.y),player))
                return true;
        }

        return false;
    }

    private bool RaycastForPlayer(Vector2 position, Vector2 direction, Collider2D player)
    {
        Debug.DrawRay(position, direction * _raycastLength, Color.cyan);
        RaycastHit2D hit = Physics2D.Raycast(position, direction, _raycastLength);

        return hit.collider == player;
    }

    private void CalculateRaycastDirections(Light2D light)
    {
        print(light.pointLightOuterAngle);
        float currentAngle = (light.pointLightOuterAngle / 2) * Mathf.Deg2Rad + 1.5f;
        int raycastCount = Mathf.FloorToInt(light.pointLightOuterAngle / _raycastAngleStep);

        for (int i = 0; i < raycastCount; i++)
        {
            Vector2 raycastDirection = Vector2.zero;
            raycastDirection.x = Mathf.Sin(currentAngle);
            raycastDirection.y = Mathf.Cos(currentAngle);
            _raycastDirections.Add(raycastDirection);
            currentAngle -= _raycastAngleStep * Mathf.Deg2Rad;
        }
    }
}

