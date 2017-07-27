using PN.Helpers;
using Robocode;
using Robocode.Util;
using System;

namespace PN
{
    /// <summary>
    /// A bit more advanced robot, who takes into account the velocity, angle and distance of
    /// any scanned robot when shooting.
    /// </summary>
    public class SniperBot : AdvancedRobot
    {
        const double RADAR_TURN_MAX_DEGREES = 45.00;
        const double MAX_DISTANCE_ENEMY_TOO_CLOSE = 300.00;
        private double? _nextRadarTurnDegrees = null;
        private bool _isMovingToCenter = true;

        private EnemyBot _enemy;

        public SniperBot()
        {
            _enemy = new EnemyBot();
            _enemy.BulletFired += OnEnemyBulletFired;
            _enemy.LocationChanged += OnEnemyLocationChanged;
        }
        
        public override void Run()
        {
            IsAdjustGunForRobotTurn = true;
            IsAdjustRadarForGunTurn = true;
            
            while (true)
            {
                Move();
                TurnRadar();
                Execute();
            }
        }

        private void TurnRadar()
        {
            if (_nextRadarTurnDegrees.HasValue)
            {
                SetTurnRadarRight(_nextRadarTurnDegrees.Value);
                _nextRadarTurnDegrees = null;
            }
            else
            {
                if(RadarTurnRemaining == 0.0)
                {
                    Console.WriteLine("Turning another " + RADAR_TURN_MAX_DEGREES);
                    SetTurnRadarRight(RADAR_TURN_MAX_DEGREES);
                }
            }
        }

        private void Move()
        {
            if(!_isMovingToCenter)
            {
                return;
            }

            var centerX = BattleFieldWidth / 2;
            var centerY = BattleFieldHeight / 2;

            // Calculate when to stop moving to the center.
            if(Math.Abs(centerX - X) < 50 && Math.Abs(centerY - Y) < 50)
            {
                _isMovingToCenter = false;
                return;
            }
            
            // Move a bit to the center.
            double centerAngle = Math.Atan2(centerX - X, centerY - Y);
            SetTurnRightRadians(Utils.NormalRelativeAngle(centerAngle - HeadingRadians));
            SetAhead(100);
        }

        public override void OnScannedRobot(ScannedRobotEvent e)
        {
            Console.WriteLine("Robot Scanned");
            
            // Calculate the heading of the scanned robot in degrees.
            var radarHeading = RadarHeading;
            var headingTarget = (Heading + e.Bearing) % 360;
            if (headingTarget < 0)
            {
                headingTarget = 360 + headingTarget;
            }
            Console.WriteLine("Heading: " + Heading + ". RadarHeading: " + RadarHeading + ". e.Bearing: " + e.Bearing + ". HeadingTarget: " + headingTarget);

            // Calculate the angle to the scanned robot.
            var angle = Utils.ToRadians(headingTarget);

            // Calculate the coordinates of the robot.
            var enemyX = X + Math.Sin(angle) * e.Distance;
            var enemyY = Y + Math.Cos(angle) * e.Distance;

            // Update enemy information
            _enemy.Energy = e.Energy;
            _enemy.SetLocation(enemyX, enemyY);
            _enemy.BearingToTarget = e.Bearing;

            // Normalize values if necessary.
            if (headingTarget < 45 && radarHeading > 315)
            {
                radarHeading -= 360;
            }
            else if (radarHeading < 45 && headingTarget > 315)
            {
                headingTarget -= 360;
            }

            // Turn radar left when we need to.
            if (radarHeading > headingTarget)
            {
                // Calculate the degrees between the radar and the target.
                var delta = radarHeading - headingTarget;
                Console.WriteLine("Delta: " + delta);

                // Center the radar on the target.
                delta += RADAR_TURN_MAX_DEGREES / 2;

                // Maximize turning the radar for one tick. If we need to turn further left, we 
                // just turn right a bit less the next Run().
                if (delta > RADAR_TURN_MAX_DEGREES) {
                    _nextRadarTurnDegrees = RADAR_TURN_MAX_DEGREES - (delta - RADAR_TURN_MAX_DEGREES);
                    Console.WriteLine("Next radar turns degrees: " + _nextRadarTurnDegrees);
                    delta = RADAR_TURN_MAX_DEGREES;
                }

                SetTurnRadarLeft(delta);
                Execute();
            }
        }

        public override void OnHitWall(HitWallEvent evnt)
        {
            _isMovingToCenter = true;
        }

        private void OnEnemyBulletFired(object sender, Events.BulletFiredEvent e)
        {
            Console.WriteLine("Enemy bullet fired!");

            var distance = 100;

            // If we're too close to the enemy, move away.
            var distanceToEnemy = MathHelper.CalculateDistanceBetweenCoordinates(X, Y, _enemy.X, _enemy.Y);
            if(distanceToEnemy < MAX_DISTANCE_ENEMY_TOO_CLOSE)
            {
                if(_enemy.BearingToTarget < 90 && _enemy.BearingToTarget > -90)
                {
                    distance = -distance;
                }
            }

            SetAhead(distance);
            Execute();
        }

        private void OnEnemyLocationChanged(object sender, Events.LocationChangedEvent e)
        {
            Console.WriteLine("Enemy location changed: X=" + _enemy.X + ";Y=" + _enemy.Y);

            if(_isMovingToCenter)
            {
                return;
            }

            // Always face sideways.
            var turnAngleInDegrees = Math.Abs(_enemy.BearingToTarget) - 90;
            SetTurnRight(turnAngleInDegrees);
        }
    }
}