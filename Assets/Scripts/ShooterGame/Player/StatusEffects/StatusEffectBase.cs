using ShooterGame.Player.Stats;

namespace ShooterGame.Player.StatusEffects
{
    public abstract class StatusEffectBase
    {
        private readonly int _maxDuration;
        private PlayerStats _player;

        private string Name { get; set; }  //# name for HUD
        public int Duration { get; private set; } //# of turns left
        public int MaxDuration { get { return this._maxDuration; } }

        protected StatusEffectBase(string name, int duration, PlayerStats affectedPlayer)
        {
            this.Name = name;
            this._maxDuration = this.Duration = duration;
            this._player = affectedPlayer;
        }

        public virtual void ApplyStatusEffect()
        {
            this.Duration -= 1;
        }
    }
}
