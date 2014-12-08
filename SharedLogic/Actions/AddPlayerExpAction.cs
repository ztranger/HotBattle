
using SharedLogic;
using SharedLogic.Actions;

namespace Tanat.SharedLogic.Actions
{
    public class AddPlayerExpAction : GameAction
    {
        private readonly int _exp;

        public AddPlayerExpAction(int exp)
        {
            _exp = exp;
        }

        internal override void Execute(Context context)
        {
            context.State.Player.Expirience.Value += _exp;
        }
    }
}
