#if !SWITCH
using UnityEngine;

namespace Udpr.Cheats
{
    public sealed class CheatMenuController : MonoBehaviour
    {
        static CheatMenuController instance;
        readonly CheatService service = new CheatService();
        bool isOpen;
        bool previousCursorVisible;
        CursorLockMode previousCursorLock;
        string status = "Dinheiro pronto. Adicione apenas quando desejar.";
        Rect window = new Rect(24, 24, 420, 300);
        Vector2 scroll;
        float notificationUntil;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Install()
        {
            if (instance != null) return;
            var existing = Object.FindObjectOfType<CheatMenuController>();
            if (existing != null) { instance = existing; return; }
            new GameObject("CheatMenu").AddComponent<CheatMenuController>();
        }

        void Awake()
        {
            if (instance != null && instance != this) { Destroy(gameObject); return; }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1)) SetOpen(!isOpen);
            if (Input.GetKeyDown(KeyCode.F2))
            {
                status = service.SetRandomEncountersDisabled(!CheatService.RandomEncountersDisabled);
                notificationUntil = Time.unscaledTime + 3f;
            }
        }

        void LateUpdate()
        {
            if (!isOpen) return;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        void SetOpen(bool value)
        {
            if (isOpen == value) return;
            if (value)
            {
                previousCursorVisible = Cursor.visible;
                previousCursorLock = Cursor.lockState;
            }
            isOpen = value;
            if (!value)
            {
                Cursor.lockState = previousCursorLock;
                Cursor.visible = previousCursorVisible;
            }
        }

        void OnDisable() { SetOpen(false); }
        void OnDestroy()
        {
            if (instance != this) return;
            SetOpen(false);
            instance = null;
        }

        void OnGUI()
        {
            if (!isOpen)
            {
                if (Time.unscaledTime < notificationUntil)
                {
                    int depth = GUI.depth;
                    GUI.depth = -10000;
                    GUI.Box(new Rect(12, 12, Mathf.Min(420, Screen.width - 24), 36), status);
                    GUI.depth = depth;
                }
                return;
            }
            int oldDepth = GUI.depth;
            GUI.depth = -10000;
            window.width = Mathf.Min(420, Screen.width);
            window.height = Mathf.Min(300, Screen.height);
            window.x = Mathf.Clamp(window.x, 0, Mathf.Max(0, Screen.width - window.width));
            window.y = Mathf.Clamp(window.y, 0, Mathf.Max(0, Screen.height - window.height));
            window = GUI.Window(GetInstanceID(), window, DrawWindow, "uDPR - Cheat Menu [F1]");
            GUI.depth = oldDepth;
        }

        void DrawWindow(int id)
        {
            scroll = GUILayout.BeginScrollView(scroll);
            GUILayout.Label("O jogo continua rodando. Use o mouse neste painel.");
            GUILayout.Space(10);
            GUILayout.Label("Diagnostico");
            if (GUILayout.Button("Testar conexao do menu")) status = service.TestConnection();
            GUILayout.Space(10);
            GUILayout.Label("Exploracao");
            bool noEncounters = GUILayout.Toggle(CheatService.RandomEncountersDisabled,
                "Desativar encontros aleatorios [F2]");
            if (noEncounters != CheatService.RandomEncountersDisabled)
                status = service.SetRandomEncountersDisabled(noEncounters);
            GUILayout.Label(CheatService.RandomEncountersDisabled
                ? "Ligado: sem encontros aleatorios por movimento."
                : "Desligado: encontros normais.");
            GUILayout.Space(10);
            GUILayout.Label("Shiny");
            bool shiny = GUILayout.Toggle(CheatService.GuaranteedShiny,
                "Shiny 100% nos novos encontros aleatorios");
            if (shiny != CheatService.GuaranteedShiny)
                status = service.SetGuaranteedShiny(shiny);
            if (CheatService.GuaranteedShiny && CheatService.RandomEncountersDisabled)
                GUILayout.Label("Desligue o bloqueio de encontros [F2] para encontrar Pokemon.");
            GUILayout.Space(10);
            GUILayout.Label("Captura");
            bool guaranteedCatch = GUILayout.Toggle(CheatService.GuaranteedCatch,
                "Captura 100% com qualquer Poke Bola");
            if (guaranteedCatch != CheatService.GuaranteedCatch)
                status = service.SetGuaranteedCatch(guaranteedCatch);
            GUILayout.Label("Consome a bola normalmente.");
            bool trainerCapture = GUILayout.Toggle(CheatService.TrainerCaptureEnabled,
                "Permitir capturar Pokemon de treinadores");
            if (trainerCapture != CheatService.TrainerCaptureEnabled)
                status = service.SetTrainerCapture(trainerCapture);
            GUILayout.Label("Capturados saem da luta e sao recebidos ao final da batalha.");
            GUILayout.Space(10);
            GUILayout.Label("Dinheiro");
            int money;
            bool available = service.TryGetMoney(out money);
            GUILayout.Label(available ? "Saldo: " + money.ToString("N0") + " / " + MoneyWork.MAX_MOENY.ToString("N0")
                : "Disponivel durante a exploracao, fora de salvamentos.");
            bool enabled = GUI.enabled;
            GUI.enabled = enabled && available && money < MoneyWork.MAX_MOENY;
            if (GUILayout.Button("Adicionar 10.000")) status = service.Add10k();
            GUI.enabled = enabled;
            GUILayout.Label("A alteracao sera gravada no proximo save/autosave.");
            GUI.enabled = false;
            GUILayout.Button("Itens - ainda nao implementado");
            GUI.enabled = enabled;
            GUILayout.Label(status);
            GUILayout.EndScrollView();
            if (GUILayout.Button("Fechar [F1]")) SetOpen(false);
            GUI.DragWindow(new Rect(0, 0, 10000, 22));
        }
    }
}
#endif