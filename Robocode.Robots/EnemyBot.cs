using PN.Events;
using System;

namespace PN
{
    public class EnemyBot
    {
        private double _energy;

        public event EventHandler<BulletFiredEvent> BulletFired;
        public event EventHandler<LocationChangedEvent> LocationChanged;

        public EnemyBot()
        {
            _energy = 100.0;
        }

        public double Energy
        {
            get { return _energy; }
            set
            {
                var oldValue = _energy;
                _energy = value;

                if(oldValue != _energy)
                {
                    OnBulletFired(new BulletFiredEvent());
                }
            }
        }
        public double X { get; private set; }
        public double Y { get; private set; }
        public double BearingToTarget { get; set; }

        public void SetLocation(double newX, double newY)
        {
            var oldX = X;
            var oldY = Y;
            
            X = newX;
            Y = newY;

            if(newX != oldX || newY != oldY)
            {
                OnLocationChanged(new LocationChangedEvent());
            }
        }

        private void OnLocationChanged(LocationChangedEvent e)
        {
            LocationChanged?.Invoke(this, e);
        }

        private void OnBulletFired(BulletFiredEvent e)
        {
            BulletFired?.Invoke(this, e);
        }
    }
}