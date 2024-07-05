using Assets._Game.Utils.Bezier;
using UnityEngine;

namespace Assets._Game.Gameplay._Weapon.Scripts
{
    public class BezierBallisticMove : ProjectileMove
    {
        private Vector3 _direction;
        private Vector3 _perpendicular;

        private float _distance;
        private float _travelTime;

        //Warp points positions
        private const float P1_DELTA_X = 0.3f;
        private const float P2_DELTA_X = 0.7f;

        private const int T_MIN = 0;
        private const int T_MAX = 1;

        private readonly Vector3[] _points = new Vector3[4];

        private float _t;

        public override void Reset()
        {
            _t = 0;
        }

        public override void Move()
        {
            UpdateBezierCurve();

            _distance = CalculateCurveLength();
            _travelTime = _distance / _speed;

            IsMoving = _t < T_MAX;

            _t += Time.deltaTime / _travelTime;
            _t = Mathf.Clamp(_t, T_MIN, T_MAX);

            Vector3 newPosition = Bezier.GetPoint(_points, _t);

            Position = newPosition;

            if (_t < T_MAX)
            {
                Vector3 tangent = Bezier.GetFirstDerivative(_points, _t).normalized;

                if (_direction.x > 0)
                {
                    Rotation = Quaternion.LookRotation(Vector3.forward, -tangent) * Quaternion.Euler(180, 0, 0);
                }
                else
                {
                    Rotation = Quaternion.LookRotation(Vector3.forward, tangent);
                }
            }
        }

        private void UpdateBezierCurve()
        {
            float distance = Vector3.Distance(_startPosition, TargetPosition);
            _direction = (TargetPosition - _startPosition).normalized;

            if (_direction.x > 0)
            {
                _perpendicular = Quaternion.Euler(0, 0, 90) * _direction.normalized;
            }
            else
            {
                _perpendicular = Quaternion.Euler(0, 0, -90) * _direction.normalized;
            }

            _points[0] = _startPosition;
            _points[1] = _startPosition + P1_DELTA_X * distance * _direction + _perpendicular * (distance * _warp);
            _points[2] = _startPosition + P2_DELTA_X * distance * _direction + _perpendicular * (distance * _warp);
            _points[3] = TargetPosition;
        }

        private float CalculateCurveLength()
        {
            float length = 0.0f;
            Vector3 previousPoint = Bezier.GetPoint(_points, 0);
            int segments = 20;

            for (int i = 1; i <= segments; i++)
            {
                float t = i / (float) segments;
                Vector3 currentPoint = Bezier.GetPoint(_points, t);
                length += Vector3.Distance(previousPoint, currentPoint);
                previousPoint = currentPoint;
            }

            return length;
        }
    }
}