using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using Cinemachine;
using UnityEngine.Animations.Rigging;


#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

#if ENABLE_INPUT_SYSTEM
[RequireComponent(typeof(PlayerInput))]
#endif

public class JBR_StarterAssets_Aim : MonoBehaviour
{

    [Tooltip("Add Model from Third Person Controller Character, Bow attached to Left Hand")]
    public GameObject bowModelInUse;
    [Tooltip("Add Model from Third Person Controller Character, Arrow Model")]
    public GameObject arrowModel;
    [Tooltip("Add Model from Third Person Controller Character, Non Equiped Bow Model")]
    public GameObject bowModelMounted;
    [Space(5)]
    [Tooltip("Add Rig Here > Character > RigObjects")]
    public Rig leftHandRigAim;
    [Tooltip("Add Rig Here > Character > RigObjects ")]
    public Rig leftHandRigIK;  
    [Tooltip("IK Weight Multiplier, changes how fast the ik weight will change")]
    public float ik_WeightMulti = 2.0f;
    [Space(5)]
    [SerializeField]
    [Tooltip("Add the aiming Virtual Camera Here")]
    private CinemachineVirtualCamera _camera_Aim;
    [Space(5)]
    [Header("Dynamic Settings")]
    [Tooltip("Dynamically Set, true if currently aiming")]
    [SerializeField]
    private bool isAiming;
    [Tooltip("Dynamically Set, true if bow is holding ready to Fire")]
    [SerializeField]
    private bool fireHold = false;
    [Tooltip("Dynamically Set, true if bow is equiped")]
    [SerializeField]
    private bool bowEquiped = false;
    [Tooltip("Dynamically Set, the Current IK Weight")]
    [SerializeField]
    private float ik_Weight = 0;
 
    private SkinnedMeshRenderer bowRenderer;

#if ENABLE_INPUT_SYSTEM
  //  [SerializeField]
    private PlayerInput _playerInput;
#endif
  //  [SerializeField]
    private StarterAssetsInputs _input;
    private Animator _animator;
    private ThirdPersonController _thirdPersonController;
    private JBR_Trajectory _Trajectory;


    // Start is called before the first frame update
    void Start()
    {
        _thirdPersonController = GetComponent<ThirdPersonController>();
        _thirdPersonController.CinemachineCameraTargetCurrent = _thirdPersonController.CinemachineCameraTarget3rd;
      //  _camera_Aim = _thirdPersonController.
        _input = GetComponent<StarterAssetsInputs>();

#if ENABLE_INPUT_SYSTEM
        _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif 

        _animator = GetComponent<Animator>();
        _Trajectory = this.gameObject.GetComponent<JBR_Trajectory>();
        bowRenderer = bowModelInUse.GetComponent<SkinnedMeshRenderer>();
        SetModelActive(false, bowModelInUse);
        SetModelActive(true, bowModelMounted);
        //set Ik to zero on start
        leftHandRigAim.weight = 0;
        leftHandRigIK.weight = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //weapon select
        if(Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            if (bowEquiped == false)
            {
                bowEquiped = true;
                _animator.SetBool("EquipBow", true);

                SetModelActive(true, bowModelInUse);
                SetModelActive(false, bowModelMounted);
            }
            else
            {
                bowEquiped = false;
                _animator.SetBool("AimingBow", false);
                _animator.SetBool("EquipBow", false);

                SetModelActive(false, bowModelInUse);
                SetModelActive(true, bowModelMounted);
            }
        }

        Aim();

        if (fireHold)
        {
            if(ik_Weight < 1)
            {
                ik_Weight = Mathf.Lerp(ik_Weight, 1, Time.deltaTime * ik_WeightMulti);
                leftHandRigAim.weight = ik_Weight;
                leftHandRigIK.weight = ik_Weight;

                if (ik_Weight >= .80f)
                {
                    arrowModel.SetActive(true);
                    _Trajectory.showProjectilePath = true;
                    bowRenderer.SetBlendShapeWeight(0, ik_Weight * 100);
                }
                else
                {
                    _Trajectory.showProjectilePath = false;
                }
            }        
        }
        else
        {
            if(ik_Weight > .05f)
            {
                ik_Weight = Mathf.Lerp(ik_Weight, 0, Time.deltaTime * ik_WeightMulti);
                leftHandRigAim.weight = ik_Weight;
                leftHandRigIK.weight = ik_Weight;
            }
            else
            {
                ik_Weight = 0;
            }
            _Trajectory.showProjectilePath = false;
        }
    }


    private void Aim()
    {
        if (_input.aim)
        {
            //aim already called 
            if(isAiming)
            {
                Debug.Log("Aiming ******");
               // _animator.SetBool("AimingBow", true);
            }
            // start aiming
            else
            {
                isAiming = true;
              //  Debug.Log(" Start Aiming $$$$$$ ");
                _camera_Aim.Priority = 50;
                _thirdPersonController.CinemachineCameraTargetCurrent = _thirdPersonController.CinemachineCameraTargetAim;
            }
        }
        else
        {
            isAiming = false;
           // _animator.SetBool("AimingBow", false);
            _camera_Aim.Priority = 2;
            _thirdPersonController.CinemachineCameraTargetCurrent = _thirdPersonController.CinemachineCameraTarget3rd;
        }

        if (bowEquiped == true)
        {
            if (_input.fire)
            {
                if (fireHold == false)
                {
                    fireHold = true;
                    _animator.SetBool("AimingBow", true);

                }
            }
            else
            {
                if (fireHold)
                {
                    fireHold = false;
                    // canFire now
                    _animator.SetBool("AimingBow", false);
                    _animator.SetTrigger("FireArrow");
                    // //needs to be set over time
                    bowRenderer.SetBlendShapeWeight(0, 0);
                    arrowModel.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Sets model active or Inactive
    /// </summary>
    /// <param name="active"></param>
    /// <param name="model"></param>
    public void SetModelActive(bool active, GameObject model)
    {
        model.SetActive(active);
    }
}
