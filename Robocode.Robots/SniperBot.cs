using Robocode;

namespace PN
{
    /// <summary>
    /// A bit more advanced robot, who takes into account the velocity, angle and distance of
    /// any scanned robot when shooting.
    /// </summary>
    public class SniperBot : Robot
    {
        public override void Run()
        {
            IsAdjustRadarForGunTurn = true;

            while (true)
            {
                TurnRadarRight(45);
            }
        }
    
        public override void OnScannedRobot(ScannedRobotEvent e)
        {
            // Turn the tank in order to shoot.
            TurnRight(e.Bearing);

            // We fire the gun with bullet power = 1
            Fire(1);

            // Normalize values if necessary.
            var radarHeading = RadarHeading;
            var heading = Heading;
            if (heading < 45 && radarHeading > 315)
            {
                radarHeading -= 360;
            } else if(radarHeading < 45 && heading > 315)
            {
                heading -= 360;
            }
            
            // Turn radar left when we need to.
            if (radarHeading > heading)
            {
                var delta = radarHeading - heading;
                TurnRadarLeft(delta + 22.5);
            }
        }
    }
}