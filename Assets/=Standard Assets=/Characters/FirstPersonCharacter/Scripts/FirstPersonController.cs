using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private bool m_IsWalking;
        public float m_WalkSpeed;
        public float m_RunSpeed;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private bool VR;
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private bool m_UseFovKick;
        [SerializeField] private FOVKick m_FovKick = new FOVKick();
        [SerializeField] private bool m_UseHeadBob;
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] private float m_StepInterval;
        [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.
        [SerializeField] private bl_Joystick joystick;

        private Camera m_Camera;
        private Transform VRCamera;
        private bool m_Jump;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;
        private AudioSource m_AudioSource;
        private bool isAndroid;
        private int VRControlMethod = 1; //1: Touch (Like Cardboard) 2: Joystick 3: Automatic
        private Vector3 desiredMove;
        private bool move;

        // Use this for initialization
        private void Start()
        {
            LoadVRMethod(0);
            isAndroid = Application.platform == RuntimePlatform.Android;
            m_CharacterController = GetComponent<CharacterController>();
            if (!VR)
            {
                m_Camera = Camera.main;
                m_OriginalCameraPosition = m_Camera.transform.localPosition;
                m_FovKick.Setup(m_Camera);
                m_HeadBob.Setup(m_Camera, m_StepInterval);
            }
            else
            {
                UnityEngine.XR.XRSettings.enabled = true;
                VRCamera = transform.Find("CameraRig/MainCamera").transform;
            }
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle/2f;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
			if(!VR) m_MouseLook.Init(transform , m_Camera.transform);
        }       

        // Update is called once per frame
        private void Update()
        {
            if (!VR) RotateView();
            // the jump state needs to read here to make sure it is not missed            

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                StartCoroutine(m_JumpBob.DoBobCycle());
                PlayLandingSound();
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;
        }


        private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }


        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
                        
            desiredMove = VR ? VRCamera.gameObject.transform.forward * m_Input.y +
                VRCamera.gameObject.transform.right * m_Input.x : 
                transform.forward * m_Input.y + transform.right * m_Input.x;            

            // get a normal for the surface that is being touched to move along it
                RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height/2f);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x*speed;
            m_MoveDir.z = desiredMove.z*speed;


            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;
            }
            else
            {
                m_MoveDir += Physics.gravity*m_GravityMultiplier*Time.fixedDeltaTime;
            }
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);
        }
       


        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed*(m_IsWalking ? 1f : m_RunstepLenghten)))*
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }


        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed*(m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }


        private void GetInput(out float speed)
        {
            // Read input
           
            float horizontal = (isAndroid && !VR) ? joystick.Horizontal : CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = (isAndroid && !VR) ? joystick.Vertical : CrossPlatformInputManager.GetAxis("Vertical");                        

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            if (!VR) m_Input = new Vector2(horizontal, vertical);
            else
            {
                switch (VRControlMethod)
                {
                    case 1:                        
                        m_Input = new Vector2(0, Input.GetButton("VR") ? 1 : 0);
                        break;
                    case 2:
                        m_Input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                        break;
                    case 3:
                        if (VRCamera.eulerAngles.z > 40 && VRCamera.eulerAngles.z < 90) horizontal = 1;
                        m_Input = new Vector2((VRCamera.eulerAngles.z > 40 && VRCamera.eulerAngles.z < 90) ? -1 : 
                            (VRCamera.eulerAngles.z < 320 && VRCamera.eulerAngles.z > 90) ? 1 : 0,
                            (VRCamera.eulerAngles.x < 20 || VRCamera.eulerAngles.x > 340) ? 1 : 0);
                        break;
                    default:
                        break;
                }
            }

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }

        public void MoveIt(int moveItOrNot)
        {
            if (moveItOrNot == 1) move = true;
            else move = false;
        }


        private void RotateView()
        {
            m_MouseLook.LookRotation (transform, m_Camera.transform);
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }

        public IEnumerator RUN()
        {
            m_WalkSpeed = 7;
            m_RunSpeed = 7;
            yield return new WaitForSeconds(2);
            m_WalkSpeed = 4;
            m_RunSpeed = 4;
            yield return new WaitForSeconds(3);
            m_WalkSpeed = 2;
            m_RunSpeed = 2;
        }

        public void LoadVRMethod(int method)
        {
            if (method == 0)
                if (PlayerPrefs.HasKey("VRControlMethod"))
                    VRControlMethod = PlayerPrefs.GetInt("VRControlMethod");
                else VRControlMethod = 1;
            else
            {
                VRControlMethod = method;
                PlayerPrefs.SetInt("VRControlMethod", method);
                if (method == 2)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }
    }

}
