using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using GeneticAlgorithmForSpecies.Genes;
using GeneticAlgorithmForSpecies.Equipment;
using GeneticAlgorithmForSpecies.Environment;
using GeneticAlgorithmForSpecies.Mutation;

public class PlayerController : MonoBehaviour {
    [SerializeField] private new Camera camera;
    [SerializeField] private InputActionMap _controls;
    private new Rigidbody rigidbody;
    [SerializeField] private float playerSpeed = 1E-02F;
    [SerializeField] private float jumpHeight = 2.0f;
    [SerializeField] private float groundedThreshold = 1E-02F;
    [SerializeField] private bool _isGrounded = true;
    [SerializeField] private Vector2 _direction;

    [Header("Health")]
    [SerializeField] private float health = 100;

    public float Health { get => health; set => health = value; }

    void Start() {
        // load data from PlayerPrefs

        //camera = Camera.main;
        rigidbody = GetComponent<Rigidbody>();

        //_controls = ScriptableObject.CreateInstance<InputActionAsset>();
        //_controls.LoadFromJson("ActionInput.inputactions");

        //_controls["Jump"].performed += ctx => OnJump(ctx);
        //_controls["Movement"].performed += ctx => OnJump(ctx);

        _controls = GameManager.Instance.GetComponent<PlayerInput>().actions.FindActionMap("Gameplay");

        _controls.FindAction("Movement").performed += ctx => OnMove(ctx);
        _controls.FindAction("Jump").performed += ctx => OnJump(ctx);
    }

    private void OnEnable() {
        //_controls.Enable();
    }

    private void OnDisable() {
        //_controls.Disable();
    }

    private void Update() {
    }

    void LateUpdate() {
        //Debug.Log("Timescale is: " + Time.timeScale);
        Move();
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

    private void OnDeath()
    {
        Debug.Log("<color=red> PLAYER DIED </color>");
    }
}
