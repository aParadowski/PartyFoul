using UnityEngine;
using System.Collections;

public class SpyMove : MonoBehaviour {

public AnimationClip idleAnimation;
    public AnimationClip walkAnimation;

    public float walkMaxAnimationSpeed = 0.75F;

	
    private Animation _animation;

    enum CharacterState {

        Idle = 0,
        Walking = 1,
    }

    private CharacterState _characterState;

    // The speed when walking
    public float walkSpeed = 2.0F;

    public float inAirControlAcceleration = 3.0F;

    // The gravity for the character
    public float gravity = 20.0F;

    // The gravity in controlled descent mode
    public float speedSmoothing = 10.0F;
    public float rotateSpeed = 500.0F;
    public float trotAfterSeconds = 3.0F;

// The camera doesnt start following the target immediately but waits for a split second to avoid too much waving around.
private float lockCameraTimer = 0.0F;

// The current move direction in x-z

private Vector3 moveDirection = Vector3.zero;

// The current vertical speed

private float verticalSpeed = 0.0F;

// The current x-z move speed

private float moveSpeed = 0.0F;

// The last collision flags returned from controller.Move

private CollisionFlags collisionFlags ;

// Are we moving backwards (This locks the camera to not do a 180 degree spin)

private bool movingBack = false;

// Is the user pressing any keys?

private bool isMoving = false;

// When did the user start walking (Used for going into trot after a while)

private float walkTimeStart = 0.0F;

private bool isControllable = true;

 

    // Use this for initialization

  void  Awake ()
	{
		Debug.Log("Using the spymove script");
	    moveDirection = transform.TransformDirection(Vector3.forward);
	
	    _animation = GetComponent<Animation>();
	
	    if(!_animation)
	
	        Debug.Log("The character you would like to control doesn't have animations. Moving her might look weird.");
	
	   
	
	    /*
	
	public AnimationClip idleAnimation;
	
	public AnimationClip walkAnimation;
	
	public AnimationClip runAnimation;
	
	public AnimationClip jumpPoseAnimation; 
	
	    */
	
	    if(!idleAnimation) {
	        _animation = null;
	        Debug.Log("No idle animation found. Turning off animations.");
	    }
	
	    if(!walkAnimation) {
	
	        _animation = null;
	
	        Debug.Log("No walk animation found. Turning off animations.");
	
	    }

	}

    void  UpdateSmoothedMovementDirection (){

    Transform cameraTransform = GameObject.Find("SpyCamera").transform;

    bool grounded = IsGrounded();

    // Forward vector relative to the camera along the x-z plane   

    Vector3 forward= cameraTransform.TransformDirection(Vector3.forward);

    forward.y = 0;

    forward = forward.normalized;

 

    // Right vector relative to the camera

    // Always orthogonal to the forward vector

     Vector3 right= new Vector3(forward.z, 0, -forward.x);

 

    float v= Input.GetAxisRaw("Vertical");

    float h= Input.GetAxisRaw("Horizontal");

 

    // Are we moving backwards or looking backwards

    if (v < -0.2f)

        movingBack = true;

    else

        movingBack = false;

   

    bool wasMoving= isMoving;

    isMoving = Mathf.Abs (h) > 0.1f || Mathf.Abs (v) > 0.1f;

       

    // Target direction relative to the camera

    Vector3 targetDirection= h * right + v * forward;

   

    // Grounded controls

    if (grounded)

    {

        // Lock camera for short period when transitioning moving & standing still

        lockCameraTimer += Time.deltaTime;

        if (isMoving != wasMoving)

            lockCameraTimer = 0.0f;

 

        // We store speed and direction seperately,

        // so that when the character stands still we still have a valid forward direction

        // moveDirection is always normalized, and we only update it if there is user input.

        if (targetDirection != Vector3.zero)

        {

            // If we are really slow, just snap to the target direction

            if (moveSpeed < walkSpeed * 0.9f && grounded)

            {

                moveDirection = targetDirection.normalized;

            }

            // Otherwise smoothly turn towards it

            else

            {

                moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);

               

                moveDirection = moveDirection.normalized;

            }

        }

       

        // Smooth the speed based on the current target direction

        float curSmooth= speedSmoothing * Time.deltaTime;

       

        // Choose target speed

        //* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways

        float targetSpeed= Mathf.Min(targetDirection.magnitude, 1.0f);

   

        _characterState = CharacterState.Idle;

       

        // Pick speed modifier

        if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift))

        {

//            targetSpeed *= runSpeed;

//            _characterState = CharacterState.Running;

        }
        else
        {
            targetSpeed *= walkSpeed;
            _characterState = CharacterState.Walking;
        }

       

        moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, curSmooth);

       

        // Reset walk time start when we slow down

        if (moveSpeed < walkSpeed * 0.3f)

            walkTimeStart = Time.time;

    }

 

       

}


void  ApplyGravity (){

    if (isControllable) // don't move player at all if not controllable.
    {
        // Apply gravity
        if (IsGrounded ())

            verticalSpeed = 0.0f;

        else
            verticalSpeed -= gravity * Time.deltaTime;

    }

}

void  Update (){

		Debug.Log ("Update Triggered");


    if (!isControllable)

    {

        // kill all inputs if not controllable.

        Input.ResetInputAxes();

    }
    UpdateSmoothedMovementDirection();

    // Apply gravity

    // - extra power jump modifies gravity

    // - controlledDescent mode modifies gravity

    ApplyGravity ();

    // Calculate actual motion

    Vector3 movement= moveDirection * moveSpeed;

    movement *= Time.deltaTime;

   

    // Move the controller

    CharacterController controller = GetComponent<CharacterController>();

    collisionFlags = controller.Move(movement);

   

    // ANIMATION sector

    if(_animation) {

			Debug.Log ("Animation Triggered");
            if(controller.velocity.sqrMagnitude < 0.1f) {

                _animation.CrossFade(idleAnimation.name);

            }

            else

            {
                if(_characterState == CharacterState.Walking) {

                    _animation[walkAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0f, walkMaxAnimationSpeed);

                    _animation.CrossFade(walkAnimation.name);   

                }

        }

    }

    // ANIMATION sector

   

    // Set rotation to the move direction

    if (IsGrounded())

    {

        transform.rotation = Quaternion.LookRotation(moveDirection);

    }   

    else

    {

        Vector3 xzMove= movement;

        xzMove.y = 0;

        if (xzMove.sqrMagnitude > 0.001f)

        {

            transform.rotation = Quaternion.LookRotation(xzMove);

        }

    }   

}

void  OnControllerColliderHit ( ControllerColliderHit hit   ){

//  Debug.DrawRay(hit.point, hit.normal);

    if (hit.moveDirection.y > 0.01f)

        return;

}   

    float  GetSpeed (){

    return moveSpeed;

}

bool  IsGrounded (){

    return (collisionFlags & CollisionFlags.CollidedBelow) != 0;

}

 

Vector3  GetDirection (){

    return moveDirection;

}

 

bool  IsMovingBackwards (){

    return movingBack;

}

 

float  GetLockCameraTimer (){

    return lockCameraTimer;

}

 

bool IsMoving (){

     return Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5f;

}

void  Reset (){

    gameObject.tag = "Player";

}

}