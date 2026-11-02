#if !SWITCH
namespace Udpr.Cheats
{
    public sealed class CheatService
    {
        // Session-only flag: persists across scenes, never enters save data.
        public static bool RandomEncountersDisabled { get; private set; }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetSession() { RandomEncountersDisabled = false; GuaranteedCatch = false; GuaranteedShiny = false; TrainerCaptureEnabled = false; TrainerCaptureCheat.Reset(); }

        public string SetRandomEncountersDisabled(bool disabled)
        {
            RandomEncountersDisabled = disabled;
            return disabled ? "Encontros aleatorios desativados."
                : "Encontros aleatorios restaurados.";
        }

        public static bool GuaranteedCatch { get; private set; }

        public string SetGuaranteedCatch(bool enabled)
        {
            GuaranteedCatch = enabled;
            return enabled ? "Captura 100% ativada." : "Chance de captura normal restaurada.";
        }

        public static bool GuaranteedShiny { get; private set; }

        public string SetGuaranteedShiny(bool enabled)
        {
            GuaranteedShiny = enabled;
            return enabled ? "Shiny garantido nos novos encontros aleatorios."
                : "Chances normais de shiny restauradas.";
        }

        // Called only for newly generated movement encounters, before battle setup.
        public static void ApplyShinyToRandomEncounter(Pml.PokeParty party)
        {
            if (!GuaranteedShiny || party == null) return;
            for (uint i = 0; i < party.GetMemberCount(); i++)
            {
                var pokemon = party.GetMemberPointer(i);
                if (pokemon != null) pokemon.SetRare();
            }
        }

        public static bool TrainerCaptureEnabled { get; private set; }

        public string SetTrainerCapture(bool enabled)
        {
            TrainerCaptureEnabled = enabled;
            return enabled ? "Captura de Pokemon de treinadores ativada."
                : "Captura de treinadores desativada; capturas feitas serao mantidas.";
        }

        public int TestCount { get; private set; }
        public bool CanUseMoney
        {
            get
            {
                var player = EntityManager.activeFieldPlayer;
                return player != null && player.gameObject.activeInHierarchy
                    && PlayerWork.isPlayerInputActive && !PlayerWork.isBattling
                    && !Dpr.NX.SaveSystem.IsBusy();
            }
        }

        public bool TryGetMoney(out int money)
        {
            money = 0;
            if (!CanUseMoney) return false;
            // Read without MoneyWork.Get's implicit normalization writes.
            money = PlayerWork.GetMoney();
            return money >= 0 && money <= MoneyWork.MAX_MOENY;
        }

        public string Add10k()
        {
            int before;
            if (!TryGetMoney(out before))
                return "Dinheiro indisponivel. Volte a exploracao e aguarde o salvamento.";
            int amount = System.Math.Min(10000, MoneyWork.MAX_MOENY - before);
            if (amount == 0) return "Limite de dinheiro atingido.";
            MoneyWork.Add(amount);
            int after = PlayerWork.GetMoney();
            return "Adicionado: " + (after - before).ToString("N0")
                + " | Total: " + after.ToString("N0");
        }

        public string TestConnection()
        {
            TestCount++;
            return "Teste " + TestCount + " concluido (sem alterar dados).";
        }
    }
}
#endif
