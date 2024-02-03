using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using Cinemachine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

#if ENABLE_INPUT_SYSTEM
[RequireComponent(typeof(PlayerInput))]
#endif

public class JBR_StarterAssets_Aim : MonoBehaviour
{

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


    }

    // Update is called once per frame
    void Update()
    {
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
                _animator.SetBool("AimingBow", true);
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
            _animator.SetBool("AimingBow", false);
            _camera_Aim.Priority = 2;
            _thirdPersonController.CinemachineCameraTargetCurrent = _thirdPersonController.CinemachineCameraTarget3rd;
        }
    }
}
