using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour {
    [SerializeField] new Camera camera;
    [SerializeField] InputActionMap _controls;
    new Rigidbody rigidbody;
    [SerializeField] float playerSpeed = 1E-02F;
    [SerializeField] float jumpHeight = 2.0f;
    [SerializeField] float groundedThreshold = 1E-02F;
    [SerializeField] bool _isGrounded = true;

    [Header("Health")]
    [SerializeField] private float health = 100;

    [Header("Mutation")]
    [SerializeField] Mutator mutator;
    [SerializeField] int mutationRate = 10;
    [SerializeField] float damageRate = 0.5f;
    [SerializeField] float envDamage = 1E-01F;
    [SerializeField] bool mutationLock = false;
    [SerializeField] bool damageLock = false;

    [SerializeField] GeneContainer genes;
    [SerializeField] EquipmentContainer equipment;

    [Header("DisplayInfo")]
    [SerializeField] Transform playerBox;
    [SerializeField] Transform envBox;
    [SerializeField] Text prefabTextObj;

    [SerializeField] private Vector2 _direction;

    void Start() {
        // load data from PlayerPrefs

        //camera = Camera.main;
        genes = new GeneContainer();
        rigidbody = GetComponent<Rigidbody>();
        playerBox = GameObject.Find("PlayerInfo").transform.Find("List");
        envBox = GameObject.Find("EnvironmentInfo").transform.Find("List");

        //_controls = ScriptableObject.CreateInstance<InputActionAsset>();
        //_controls.LoadFromJson("ActionInput.inputactions");

        //_controls["Jump"].performed += ctx => OnJump(ctx);
        //_controls["Movement"].performed += ctx => OnJump(ctx);

        _controls = GameManager.instance.GetComponent<PlayerInput>().actions.FindActionMap("Gameplay");

        _controls.FindAction("Movement").performed += ctx => OnMove(ctx);
        _controls.FindAction("Jump").performed += ctx => OnJump(ctx);

        mutator = new Mutator();
    }

    private void OnEnable() {
        //_controls.Enable();
    }

    private void OnDisable() {
        //_controls.Disable();
    }

    private void Update() {
        HandleEnvironmnet();
    }

    void LateUpdate() {
        //Debug.Log("Timescale is: " + Time.timeScale);
        Move();
        ShowInfo();
    }

    private void Move() {
        // Simple movement
        Vector3 moveDir = _direction.y * camera.transform.forward
                                + _direction.x * camera.transform.right;
        moveDir.y = 0.0f;
        moveDir = moveDir.normalized;

        float factor = Time.deltaTime == 0 ? 0 : 1.0f / Time.deltaTime;
        Vector3 deltaVelocity = moveDir * factor;
        deltaVelocity *= playerSpeed/* * speedModifier * animator.deltaPosition.magnitude*/;
        float rbVelY = rigidbody.velocity.y;


        rigidbody.velocity = new Vector3(deltaVelocity.x, rbVelY, deltaVelocity.z);
    }

    public void OnJump(InputAction.CallbackContext context) {
        if (!IsGrounded()) {
            return;
        }

        rigidbody.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.Impulse);
        _isGrounded = false;
    }

    public void OnMove(InputAction.CallbackContext context) {
        //Debug.Log("OnMove is called");

        _direction = context.ReadValue<Vector2>();
    }

    public bool IsGrounded() {
        float jumpThresh = groundedThreshold + transform.localScale.y;
        Ray ray = new Ray(transform.position + Vector3.up * jumpThresh, Vector3.down);
        _isGrounded = false;
        if (Physics.Raycast(ray, 2 * jumpThresh, ~LayerMask.NameToLayer("Player"))) {
            _isGrounded = true;
        }

        return _isGrounded;
    }

    private void ShowInfo() {
        foreach (Transform child in playerBox)
            Destroy(child.gameObject);
        foreach (Transform child in envBox)
            Destroy(child.gameObject);
        
        var buffedGenes = equipment.ApplyBuffs(genes);
        foreach (KeyValuePair<string, Gene.Interval> entry in buffedGenes.map) {
            string text = entry.Key.ToString() + ": [" + entry.Value.min + ", " + entry.Value.max + "]";

            prefabTextObj.text = text;
            Instantiate(prefabTextObj, playerBox);
        }

        Dictionary<string, float> currEnvAspects;
        currEnvAspects = EnvironmentManager.instance.GetAspects(transform.position);

        if (currEnvAspects == null)
            return;

        foreach (KeyValuePair<string, float> entry in currEnvAspects) {
            string text = entry.Key.ToString() + ": " + entry.Value;

            prefabTextObj.text = text;
            Instantiate(prefabTextObj, envBox);
        }
    }

    private void HandleEnvironmnet() {
        Dictionary<string, float> currEnvAspects;
        currEnvAspects = EnvironmentManager.instance.GetAspects(transform.position);
        if (currEnvAspects == null)
            return;

        List<string> affectedGenes = new List<string>();
        string debugText = "<b>Genes affected by <color=cyan>"
                            + EnvironmentManager.instance.GetEnvironmentType(transform.position) + "</color></b>: ";
        foreach (KeyValuePair<string, Gene.Interval> gene in genes.map) {
            if (currEnvAspects.ContainsKey(gene.Key)) {
                var entry = currEnvAspects[gene.Key];
                if (!gene.Value.Compare(entry)) {
                    //Debug.Log(gene.Key.ToString() + " is affected by ");
                    debugText += "<i><color=orange>" + gene.Key + "</color></i> ";

                    // Take damage
                    affectedGenes.Add(gene.Key);
                }
            }
        }

        if (affectedGenes.Count != 0) {
            Debug.Log(debugText);

            if (!mutationLock)
                StartCoroutine(Mutate(EnvironmentManager.instance.GetController(transform.position), affectedGenes));

            if (!damageLock)
                StartCoroutine(TakeDamage(affectedGenes));
        }
    }

    private void OnDeath() {
        Debug.Log("<color=red> PLAYER DIED </color>");
    }

    IEnumerator TakeDamage(List<string> affectedGenes) {
        damageLock = true;


       /* foreach (var gene in affectedGenes)
        {

        }*/

        health -= affectedGenes.Count * envDamage;
        if (health <= 0) {
            health = 0;
            OnDeath();
            yield break;
        }

        yield return new WaitForSeconds(damageRate);

        damageLock = false;
        yield break;
    }

    IEnumerator Mutate(EnvironmentController envController, List<string> affectedGenes) {
        mutationLock = true;
        yield return new WaitForSeconds(mutationRate);

        string debugText = "<b>Mutation in progress...</b>\n";
        //Debug.Log("Mutation in progress... ");
        // mutate to be adapt to current region
        var buffedGenes = equipment.ApplyBuffs(genes);
        genes = mutator.Adapt(buffedGenes, envController, affectedGenes);
        // TODO remove buffs
        //genes = equipment.RemoveBuffs(genes);

        foreach (KeyValuePair<string, Gene.Interval> entry in buffedGenes.map) {
            debugText += "<color=orange>" + entry.Key.ToString() + "</color>\t -> mutated from <color=red>[" 
                            + entry.Value.min + ", " + entry.Value.max + "]</color> "
                            + "to <color=green>[" + genes.map[entry.Key].min +  ", " + genes.map[entry.Key].max + "]</color>\n";
            //Debug.Log(text);
        }
        Debug.Log(debugText);

        mutationLock = false;
        yield break;
    }
}
