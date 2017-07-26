using PN.Events;
using System;

namespace PN
{
    public class EnemyBot
    {
        private double _energy;

        public event EventHandler<BulletFiredEvent> BulletFired;

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

        private void OnBulletFired(BulletFiredEvent e)
        {
            BulletFired?.Invoke(this, e);
        }
    }
}