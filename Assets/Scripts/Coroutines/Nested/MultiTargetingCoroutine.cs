
using System.Collections;

namespace PimPamPum
{
    public class MultiTargetingCoroutine<T> : Enumerator where T : ResponseCoroutine, new()
    {
        private int player, next;
        private Card card;
        private bool[] hitPlayers;
        private PlayerController[] playerControllers;

        public override bool MoveNext()
        {
            if (finished) return false;
            T responseCoroutine = Current as T;
            if (responseCoroutine != null)
            {
                hitPlayers[next] = responseCoroutine.TakeHit;
            }
            PlayerController pc;
            do
            {
                next = GameController.Instance.NextPlayerAlive(next);
                pc = playerControllers[next];
            } while (pc.IsDead || pc.Immune(card));
            if (next != player)
            {
                Current = new T();
                ((ResponseCoroutine)Current).SetPlayerController(pc);
                return true;
            }
            Current = FinishMultiTargeting();
            finished = true;
            return true;
        }

        private IEnumerator FinishMultiTargeting()
        {
            int MaxPlayers = playerControllers.Length;
            for (int i = player == MaxPlayers - 1 ? 0 : player + 1; i != player; i = i == MaxPlayers - 1 ? 0 : i + 1)
            {
                if (hitPlayers[i]) yield return playerControllers[i].Hit(player);
            }
            for (int i = player == MaxPlayers - 1 ? 0 : player + 1; i != player; i = i == MaxPlayers - 1 ? 0 : i + 1)
            {
                if (hitPlayers[i]) yield return playerControllers[i].Dying(player);
            }
            for (int i = player == MaxPlayers - 1 ? 0 : player + 1; i != player; i = i == MaxPlayers - 1 ? 0 : i + 1)
            {
                if (hitPlayers[i]) yield return playerControllers[i].Die(player);
            }
        }

        public MultiTargetingCoroutine(PlayerController[] playerControllers, int player, Card card)
        {
            this.playerControllers = playerControllers;
            this.player = player;
            this.card = card;
            next = player;
            hitPlayers = new bool[playerControllers.Length];
        }

    }
}
