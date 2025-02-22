using UnityEngine;

public class TimePercentMono_PointsPath : MonoBehaviour
{
    [Range(0f, 1f)] public float m_percentPath;
    public Transform m_whatToMove;
    public Transform[] m_pathPoints;

    public bool m_useValidate = true;

    public void OnValidate()
    {
        if (!m_useValidate) return;
        SetPathPercent(m_percentPath);
    }

    public void SetPathPercent(float percent)
    {
        // Ensure the path points exist
        if (m_pathPoints == null || m_pathPoints.Length < 2)
        {
            Debug.LogWarning("Not enough path points defined.");
            return;
        }

        // Ensure the percent is within the valid range
        percent = Mathf.Clamp(percent, 0f, 1f);

        // Get the interpolated position on the path using a smooth curve
        Vector3 position = GetPointOnCatmullRom(m_pathPoints, percent);

        // Calculate the target rotation based on the path direction
        float nextPercent = Mathf.Clamp(percent + 0.01f, 0f, 1f);
        Vector3 nextPosition = GetPointOnCatmullRom(m_pathPoints, nextPercent);
        Vector3 direction = (nextPosition - position).normalized;

        // Ensure we have a valid direction before computing rotation
        Quaternion rotation = direction.sqrMagnitude > 0 ? Quaternion.LookRotation(direction) : m_whatToMove.rotation;

        // Apply the position and rotation using iTween
        iTween.MoveTo(m_whatToMove.gameObject, iTween.Hash(
            "position", position,
            "time", 1.0f,
            "easetype", iTween.EaseType.linear
        ));

        iTween.RotateTo(m_whatToMove.gameObject, iTween.Hash(
            "rotation", rotation.eulerAngles,  // Use Euler angles for iTween
            "time", 1.0f,
            "easetype", iTween.EaseType.linear
        ));
    }

    private Vector3 GetPointOnCatmullRom(Transform[] points, float t)
    {
        int numPoints = points.Length;
        int p0, p1, p2, p3;

        // Ensure the index values loop back to the start and end correctly
        if (numPoints < 4)
        {
            Debug.LogWarning("Need at least 4 points for Catmull-Rom spline.");
            return points[0].position;  // Return the first point if not enough points
        }

        t = Mathf.Clamp01(t);
        float adjustedT = t * (numPoints - 3);  // Scale t to fit the segment range

        p1 = Mathf.FloorToInt(adjustedT);
        p0 = Mathf.Clamp(p1 - 1, 0, numPoints - 1);
        p2 = Mathf.Clamp(p1 + 1, 0, numPoints - 1);
        p3 = Mathf.Clamp(p2 + 1, 0, numPoints - 1);

        Vector3 point = CatmullRom(
            points[p0].position,
            points[p1].position,
            points[p2].position,
            points[p3].position,
            adjustedT - p1
        );

        return point;
    }

    private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        // Catmull-Rom spline formula
        Vector3 result = 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );

        return result;
    }
}
