﻿using Robocode;
using System;

namespace PN
{
    /// <summary>
    /// A bit more advanced robot, who takes into account the velocity, angle and distance of
    /// any scanned robot when shooting.
    /// </summary>
    public class SniperBot : Robot
    {
        const double RADAR_TURN_MAX_DEGREES = 45.00;
        private double? _nextRadarTurnDegrees = null;

        public override void Run()
        {
            IsAdjustGunForRobotTurn = true;
            IsAdjustRadarForGunTurn = true;

            while (true)
            {
                if(_nextRadarTurnDegrees.HasValue)
                {
                    TurnRadarRight(_nextRadarTurnDegrees.Value);
                    _nextRadarTurnDegrees = null;
                } else
                {
                    TurnRadarRight(RADAR_TURN_MAX_DEGREES);
                }
            }
        }

        public override void OnScannedRobot(ScannedRobotEvent e)
        {
            // Normalize values if necessary.
            var radarHeading = RadarHeading;
            var headingTarget = Heading + e.Bearing;
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

                // Center the radar on the target.
                delta += RADAR_TURN_MAX_DEGREES / 2;

                // Maximize turning the radar for one tick. If we need to turn further left, we 
                // just turn right a bit less the next Run().
                if (delta > RADAR_TURN_MAX_DEGREES) {
                    _nextRadarTurnDegrees = RADAR_TURN_MAX_DEGREES - (delta - RADAR_TURN_MAX_DEGREES);
                    delta = RADAR_TURN_MAX_DEGREES;
                }

                TurnRadarLeft(delta);
            }
        }
    }
}