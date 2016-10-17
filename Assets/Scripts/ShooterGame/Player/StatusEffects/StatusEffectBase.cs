namespace ShooterGame.Player.StatusEffects
{
    public abstract class StatusEffectBase
    {
        private readonly int _maxDuration;
        private PlayerStats _player;

        public string Name { get; protected set; }  //# name for HUD
        public int Duration { get; protected set; } //# of turns left
        public int MaxDuration { get { return _maxDuration; } }

        protected StatusEffectBase(string name, int duration, PlayerStats affectedPlayer)
        {
            Name = name;
            _maxDuration = Duration = duration;
            _player = affectedPlayer;
        }

        public virtual void ApplyStatusEffect()
        {
            Duration -= 1;
        }
    }
}
