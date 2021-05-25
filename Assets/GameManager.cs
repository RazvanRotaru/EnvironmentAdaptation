using UnityEngine;

public class GameManager : MonoBehaviour {
    // Start is called before the first frame update
    public static GameManager instance;

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
