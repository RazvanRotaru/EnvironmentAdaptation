using UnityEngine;

public class GameManager : MonoBehaviour {
    // Start is called before the first frame update
    private static GameManager _instance;
    public static GameManager Instance { get => _instance; }


    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
