#if !SWITCH
using System.Collections;
using System.Collections.Generic;
using Dpr.Battle.Logic;
using Pml.PokePara;

namespace Udpr.Cheats
{
    // Battle-scoped snapshots: opponent HP/reflection and toggling cannot alter them.
    public static class TrainerCaptureCheat
    {
        static readonly Dictionary<BATTLE_SETUP_PARAM, Queue<PokemonParam>> pending =
            new Dictionary<BATTLE_SETUP_PARAM, Queue<PokemonParam>>();

        public static void Reset() { pending.Clear(); }

        public static bool CanCapture(MainModule main, BTL_POKEPARAM user)
        {
            return CheatService.TrainerCaptureEnabled
                && main.GetCompetitor() == BtlCompetitor.BTL_COMPETITOR_TRAINER
                && !main.IsRecordPlayMode()
                && MainModule.PokeIDtoClientID(user.GetID()) == main.GetPlayerClientID();
        }

        public static bool HasCapacity(BATTLE_SETUP_PARAM setup)
        {
            Queue<PokemonParam> queue;
            int reserved = pending.TryGetValue(setup, out queue) ? queue.Count : 0;
            int free = 6 - (int)PlayerWork.playerParty.GetMemberCount()
                + Dpr.Box.BoxPokemonWork.TRAY_MAX * Dpr.Box.BoxPokemonWork.TRAY_POKE_MAX
                - Dpr.Box.BoxPokemonWork.GetPokemonCountAll();
            return free > reserved;
        }

        public static void Store(BATTLE_SETUP_PARAM setup, BTL_POKEPARAM target)
        {
            target.ReflectToPP(true);
            var copy = new PokemonParam(target.GetSrcData());
            Queue<PokemonParam> queue;
            if (!pending.TryGetValue(setup, out queue))
                pending.Add(setup, queue = new Queue<PokemonParam>());
            queue.Enqueue(copy);
        }

        public static IEnumerator Deliver(BATTLE_SETUP_PARAM setup)
        {
            Queue<PokemonParam> queue;
            if (!pending.TryGetValue(setup, out queue)) yield break;
            while (queue.Count > 0)
            {
                var pokemon = queue.Peek();
                pokemon.SetMemories(Pml.PokePara.Memories.FIRST_CONTACT_PLACE, (uint)PlayerWork.zoneID);
                if (pokemon.GetGetBall() == (uint)Pml.BallId.HIIRUBOORU) pokemon.RecoverAll();
                poketool.poke_memo.poketool_poke_memo.SetFromCapture(pokemon, PlayerWork.playerStatus, PlayerWork.zoneID);
                yield return EvolveDemoTools.RegisterZukanCoroutine(pokemon);
                queue.Dequeue();
                Dpr.EncountTools.OnPostRegisterZukan(pokemon, 0);
                PlayerWork.capturedMonsNo = pokemon.GetMonsNo();
                PlayReportManager.AddCapture();
                PlayReportManager.SaveReportLog_PokemonId((uint)pokemon.GetMonsNo(), pokemon.GetFormNo(), 0, pokemon.IsRare() ? 1u : 0u);
                FieldManager.Instance.SetAutoSaveState(FieldManager.AutoSaveState.BattleResult);
            }
            pending.Remove(setup);
        }
    }
}
#endif
