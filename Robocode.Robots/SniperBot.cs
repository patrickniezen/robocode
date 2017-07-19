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
            while (true)
            {
                TurnRadarLeft(-45);
            }
        }

        public override void OnScannedRobot(ScannedRobotEvent e)
        {
            TurnRight(e.Bearing);
            // We fire the gun with bullet power = 1
            Fire(1);

            // By default, the onScannedRobot() method has the lowest event priority of all the event handlers in Robocode, so it is the last one to be triggered each tick.
        }
    }
}
