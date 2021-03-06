﻿using Robocode;
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
        private double? _nextRadarTurnDegrees = null;

        private EnemyBot _enemy;

        public SniperBot()
        {
            _enemy = new EnemyBot();
            _enemy.BulletFired += OnEnemyBulletFired;
        }

        public override void Run()
        {
            IsAdjustGunForRobotTurn = true;
            IsAdjustRadarForGunTurn = true;

            while (true)
            {
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
                if (RadarTurnRemaining == 0.0)
                {
                    Console.WriteLine("Turning another " + RADAR_TURN_MAX_DEGREES);
                    SetTurnRadarRight(RADAR_TURN_MAX_DEGREES);
                }
            }
        }

        public override void OnScannedRobot(ScannedRobotEvent e)
        {
            Console.WriteLine("Robot Scanned");

            // Update enemy information
            _enemy.Energy = e.Energy;

            var radarHeading = RadarHeading;
            var headingTarget = (Heading + e.Bearing) % 360;
            if (headingTarget < 0)
            {
                headingTarget = 360 + headingTarget;
            }
            Console.WriteLine("Heading: " + Heading + ". RadarHeading: " + RadarHeading + ". e.Bearing: " + e.Bearing + ". HeadingTarget: " + headingTarget);

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
                if (delta > RADAR_TURN_MAX_DEGREES)
                {
                    _nextRadarTurnDegrees = RADAR_TURN_MAX_DEGREES - (delta - RADAR_TURN_MAX_DEGREES);
                    Console.WriteLine("Next radar turns degrees: " + _nextRadarTurnDegrees);
                    delta = RADAR_TURN_MAX_DEGREES;
                }

                SetTurnRadarLeft(delta);
                Execute();
            }
        }

        private void OnEnemyBulletFired(object sender, Events.BulletFiredEvent e)
        {
            Console.WriteLine("Enemy bullet fired!");
            SetAhead(100);
            SetTurnRight(90);
            Execute();
        }
    }
}