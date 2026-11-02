# Cheat Menu

F1 opens/closes the persistent PC-only menu. Focus the Game view first.
CheatMenuController handles UI/cursor. CheatService handles game operations.

Money: displays PlayerWork.GetMoney and adds up to 1,000 with MoneyWork.Add,
capped by MoneyWork.MAX_MOENY (999,999). Revalidates on click. Requires an active
field player, enabled player input, no battle and SaveSystem.IsBusy == false.
No disk save is forced: normal save/autosave persists the change.
Items remain disabled. The diagnostic button never changes game data.

The game continues running; gameplay input is not suppressed by the overlay.
Use the mouse. Verify opening/closing, scene persistence, title-screen disabled
state, +1,000 balance delta and the cap. Back up saves before using mutations.


Random encounters toggle: session-only, off on a new Play session (including
disabled domain reload), retained across scenes and while the panel is closed.
The PC-only hook in FieldEncount.MapEncountCheck skips movement-based random
rolls. Turning it off restores the original code path; it does not reset walk
counters, consume Repel, write saves or end battles already started.
Trainers, fishing, scripted encounters, radar patches and underground visible
Pokemon are outside this toggle's scope.
Manual test: enable while exploring grass/caves/surf, close F1, walk; disable
and confirm random encounters can occur again. Check new Play starts disabled.


F2 toggles random encounters even with the panel closed; a 3-second message
confirms the new state. Focus Game view for keyboard shortcuts.
Guaranteed catch is a separate panel toggle, session-only and initially off.
It uses Capture.isMustCapureSuccess, sharing the normal guaranteed-result path
with Master Ball (3 shakes, non-critical). The chosen ball and its consumption
are unchanged. Existing restrictions on using balls are not bypassed.
Turning off restores normal odds on the next capture calculation; it cannot
undo an already calculated throw. Caught Pokemon persist with normal saves.
Test F2 twice with menu closed and verify panel state; test a basic Poke Ball
with guaranteed catch on, then off. Confirm both flags reset on new Play.


Guaranteed shiny: continuous session-only toggle, off on new Play. Applies
CoreParam.SetRare to the newly generated enemy party in the movement encounter
path before battle setup, including double, Safari and roaming encounters.
Radar encounters entering this same path are also affected. Fishing, scripted
encounters, trainers and visible underground Pokemon are not modified.
OFF leaves natural generation untouched (natural shinies remain possible).
Closing the menu or changing scenes does not reset the toggle. Existing battles
and owned Pokemon are not changed. Captured shinies persist through normal saves.
Manual check: turn encounter blocking OFF, shiny ON, enter a grass encounter and
check the sparkle/color; capture and inspect the Pokemon. Toggle OFF and confirm
normal generation; a shiny can still occur naturally. Restart Play to check reset.


## Captura de treinadores
Toggle no F1, desligado ao iniciar Play. Apenas batalhas locais contra treinadores
(sem link, facility ou replay). Usa a chance normal; o toggle de captura 100% continua
independente. Mantem limite de nivel e demais restricoes de uso de bolas.
Falha consome a bola e continua o turno. Sucesso guarda uma copia antes de zerar HP,
remove o alvo sem animacao de desmaio e usa o fluxo normal de EXP/substituicao/vitoria.
Nao altera o resultado para CAPTURE. Multiplas capturas sao registradas/entregues em
sequencia ao fim da batalha, mesmo se desligar o toggle depois. Espaco no time/PC
e reservado contando capturas pendentes. Nao encerre Play antes da entrega.
Validacao manual: chance normal com falha; 100% ligado; treinador com varios Pokemon;
ultimo Pokemon; varias capturas com time cheio; toggle desligado apos capturar.
