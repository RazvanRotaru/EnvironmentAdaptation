using GeneticAlgorithmForSpecies.UDCT.Utilities.DevelopersConsole.Commands;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GeneticAlgorithmForSpecies.UDCT.Utilities.DevelopersConsole {
    public class DeveloperConsoleBehaviour : MonoBehaviour {

        [SerializeField] private string prefix = string.Empty;
        [SerializeField] private ConsoleCommand[] commands = new ConsoleCommand[0];

        [Header("UI")]
        [SerializeField] private GameObject UICanvas = null;
        [SerializeField] private TMP_InputField inputField = null;

        private float pausedTimeScale;

        private static DeveloperConsoleBehaviour instance;

        private DevelopersConsole developersConsole;

        private DevelopersConsole DevelopersConsole {
            get {
                if (developersConsole != null) {
                    return developersConsole;
                }

                return developersConsole = new DevelopersConsole(prefix, commands);
            }
        }

        private void Awake() {
            if (instance != null && instance != this) {
                Destroy(gameObject);
                return;
            }

            instance = this;

            DontDestroyOnLoad(gameObject);
        }

        public void Toggle(InputAction.CallbackContext context) {
            if (!context.action.triggered) {
                return;
            }

            if (UICanvas.activeSelf) {
                Time.timeScale = pausedTimeScale;
                UICanvas.SetActive(false);
            }
            else {
                pausedTimeScale = Time.timeScale;
                Time.timeScale = 0;
                UICanvas.SetActive(true);
                inputField.ActivateInputField();
            }
        }

        public void ProcessCommand(string input) {
            DevelopersConsole.ProcessCommand(input);
            inputField.text = string.Empty;
        }
    }
}