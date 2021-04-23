using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] new Camera camera;
    new Rigidbody rigidbody;
    [SerializeField] float playerSpeed = 1E-02F;
    [SerializeField] float jumpHeight = 2.0f;
    [SerializeField] float groundedThreshold = 1E-02F;
    [SerializeField] bool _isGrounded = true;


    [SerializeField] GeneContainer genes;
    [SerializeField] EquipmentContainer equipment;

    [SerializeField] float health;

    [SerializeField] Transform playerBox;
    [SerializeField] Transform envBox;
    [SerializeField] Text prefabTextObj;

    void Start()
    {
        // load data from PlayerPrefs

        //camera = Camera.main;
        genes = new GeneContainer();
        rigidbody = GetComponent<Rigidbody>();
        playerBox = GameObject.Find("PlayerInfo").transform.Find("List");
        envBox = GameObject.Find("EnvironmentInfo").transform.Find("List");
    }

    void Update()
    {
        Move();
        HandleEnvironmnet();

        ShowInfo();
    }

    private void Move()
    {
        // Simple movement
        Vector3 moveDir = GetMoveDirection();
        ApplyVelocity(moveDir);
        CheckIfGrounded();
    }

    private Vector3 GetMoveDirection()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");


        Vector3 moveDir = v * camera.transform.forward
                                + h * camera.transform.right;
        moveDir.y = 0.0f;

        return moveDir.normalized;
    }

    private void ApplyVelocity(Vector3 moveDirection)
    {
        Vector3 deltaVelocity = moveDirection / Time.deltaTime;
        deltaVelocity *= playerSpeed/* * speedModifier * animator.deltaPosition.magnitude*/;
        float rbVelY = rigidbody.velocity.y;


        rigidbody.velocity = new Vector3(deltaVelocity.x, rbVelY, deltaVelocity.z);

        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            rigidbody.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
            _isGrounded = false;
        }
    }

    public void CheckIfGrounded()
    {
        Ray ray = new Ray(transform.position + Vector3.up * groundedThreshold, Vector3.down);
        Debug.DrawRay(transform.position + Vector3.up * groundedThreshold, Vector3.down, Color.red, 1);
        if (Physics.Raycast(ray, 2 * groundedThreshold))
        {
            _isGrounded = true;
        }
    }

    private void ShowInfo()
    {
        foreach (Transform child in playerBox)
            Destroy(child.gameObject);
        foreach (Transform child in envBox)
            Destroy(child.gameObject);

        foreach (KeyValuePair<Gene.Type, Gene.Interval> entry in genes.map)
        {
            string text = entry.Key.ToString() + ": [" + entry.Value.min + ", " + entry.Value.max + "]";

            prefabTextObj.text = text;
            Instantiate(prefabTextObj, playerBox);
        }

        Dictionary<EnvironmentController.Aspect, float> currEnvAspects;
        currEnvAspects = EnvironmentManager.instance.GetAspects(transform.position);

        foreach (KeyValuePair<EnvironmentController.Aspect, float> entry in currEnvAspects)
        {
            string text = entry.Key.ToString() + ": " + entry.Value;

            prefabTextObj.text = text;
            Instantiate(prefabTextObj, envBox);
        }
    }

    private void HandleEnvironmnet()
    {
        Dictionary<EnvironmentController.Aspect, float> currEnvAspects;
        currEnvAspects = EnvironmentManager.instance.GetAspects(transform.position);

        foreach (KeyValuePair<EnvironmentController.Aspect, float> entry in currEnvAspects)
        {
            foreach (KeyValuePair<Gene.Type, Gene.Interval> gene in genes.map)
            {
                if (entry.Key.ToString() == gene.Key.ToString())
                {
                    if (!gene.Value.Contains(entry.Value))
                    {
                        Debug.Log(gene.Key.ToString() + " is affected by " 
                                    + EnvironmentManager.instance.GetEnvironmentType(transform.position));
                        Mutate();
                    }
                }
            }
        }
    }


    void Mutate()
    {
        // mutate to be adapt to current region
    }
}
