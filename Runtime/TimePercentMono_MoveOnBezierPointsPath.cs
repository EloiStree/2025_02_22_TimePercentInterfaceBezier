using UnityEngine;

public class TimePercentMono_MoveOnBezierPointsPath : MonoBehaviour
{

    [Range(0, 1)]
    public double m_currentPercentState = 0.0;
    public Transform m_whatToMove;
    public PathCreation.PathCreator m_pathCreator;

    public bool m_useValidate = true;

    public void OnValidate()
    {
        SetToPercent(m_currentPercentState);
    }
    public void SetToPercent(double percent)
    {
        m_currentPercentState = percent;
        m_whatToMove.position = m_pathCreator.path.GetPointAtTime((float)m_currentPercentState);
        m_whatToMove.rotation = m_pathCreator.path.GetRotation((float)m_currentPercentState);
    }
}
