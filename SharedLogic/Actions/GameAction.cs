
namespace SharedLogic.Actions
{
    public abstract class GameAction
    {
        public string TypeId { get { return this.GetType().FullName; } }
        public long Time { get; set; }
        public long ID { get; set; }
        public int Hash { get; set; }

        public GameAction() { }

        internal abstract void Execute(Context context);

    }
}