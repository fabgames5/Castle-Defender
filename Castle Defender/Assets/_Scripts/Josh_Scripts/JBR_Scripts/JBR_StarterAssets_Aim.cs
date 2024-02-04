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
    public bool fireHold = false;
    public bool bowEquiped = false;
    public GameObject bowModelInUse;
    public GameObject arrowModel;
    public GameObject bowModelMounted;
    public SkinnedMeshRenderer bowRenderer;
    public Rig leftHandRigAim;
    public Rig leftHandRigIK;


#if ENABLE_INPUT_SYSTEM
    [SerializeField]
    private PlayerInput _playerInput;
#endif
    [SerializeField]
    private StarterAssetsInputs _input;

    private Animator _animator;

    [SerializeField]
    private bool isAiming;
    [SerializeField]
    private CinemachineVirtualCamera _camera_Aim;
    private ThirdPersonController _thirdPersonController;

    // Start is called before the first frame update
    void Start()
    {
        _thirdPersonController = GetComponent<ThirdPersonController>();
        _thirdPersonController.CinemachineCameraTargetCurrent = _thirdPersonController.CinemachineCameraTarget3rd;
        _input = GetComponent<StarterAssetsInputs>();

#if ENABLE_INPUT_SYSTEM
        _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif 

        _animator = GetComponent<Animator>();

        bowRenderer = bowModelInUse.GetComponent<SkinnedMeshRenderer>();
        bowModelInUse.SetActive(false);
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
            }
            else
            {
                bowEquiped = false;
                _animator.SetBool("AimingBow", false);
                _animator.SetBool("EquipBow", false);
                bowModelInUse.SetActive(false);
            }
        }

        Aim();
        
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
                    leftHandRigAim.weight = 1;
                    leftHandRigIK.weight = 1;
                    _animator.SetBool("AimingBow", true);

                    // //needs to be set over time
                    bowRenderer.SetBlendShapeWeight(0, 100);

                    arrowModel.SetActive(true);
                }
            }
            else
            {
                if (fireHold)
                {
                    fireHold = false;
                    leftHandRigIK.weight = 0;
                    leftHandRigAim.weight = 0;
                    // fire now
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