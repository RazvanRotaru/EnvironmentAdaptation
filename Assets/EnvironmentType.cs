using UnityEngine;

public class EnvironmentType : MonoBehaviour {
    public EnvironmentAspects aspects;

    private void Awake() {
        foreach (Transform child in transform) {
            child.gameObject.AddComponent<EnvironmentController>().SetAsepcts(aspects.list);
        }
    }
}
