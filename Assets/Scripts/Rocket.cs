using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;

    float levelLoadDelay = 1f;
    
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;
    

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State {Alive, Dying, Transcending }
    State state = State.Alive;

    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        rigidBody.constraints = RigidbodyConstraints.FreezeRotationY;
        rigidBody.constraints = RigidbodyConstraints.FreezeRotationX;
    }

    // Update is called once per frame
    void Update()
    {
        if(state== State.Alive)
        {
            rigidBody.position.Set(-11, rigidBody.position.y, rigidBody.position.z);
            RespondToThrustInput();
            RespondToRotateInput();

        }
    }

    private void StartSuccessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        Invoke("LoadNextLevel", levelLoadDelay); // parameterise time
    }
    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        Invoke("LoadFirstLevel", levelLoadDelay); // parameterise time
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) { return; } // ignore collisions when dead

        switch (collision.gameObject.tag)
        {
            case "friendly":
                break;
            case "Finish":
                successParticles.Play();
                StartSuccessSequence();
                break;
            default:
                deathParticles.Play();
                StartDeathSequence();
                break;
        }
    }
    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space)) 
        {
            ApplyThrust();
            mainEngineParticles.Play();
        }
        else
        {
            mainEngineParticles.Stop();
            audioSource.Stop();
        }
    }
    private void ApplyThrust()
    {
        //rigidBody.AddRelativeForce(new Vector3(0,0,-1) * mainThrust); // Z ось
        rigidBody.AddRelativeForce(Vector3.up* mainThrust); // Z ось
        if (!audioSource.isPlaying) // so it doesn't layer
        {
            audioSource.PlayOneShot(mainEngine);
        }
    }
    private void RespondToRotateInput()
    {
       rigidBody.freezeRotation = true; // take manual control of rotation
        rigidBody.constraints = RigidbodyConstraints.FreezeRotationY;
        rigidBody.constraints = RigidbodyConstraints.FreezeRotationX;
        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {

           // rigidBody.AddRelativeForce(new Vector3(0, 1, 0) * mainThrust); // Y ось
            transform.Rotate(new Vector3(0, 0, 1) * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            transform.Rotate(new Vector3(1, 0, 0) * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.Rotate(new Vector3(-1, 0, 0) * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(new Vector3(0, 0,-1) * rotationThisFrame);
        }

        rigidBody.freezeRotation = false; // resume physics control of rotation
        rigidBody.constraints = RigidbodyConstraints.FreezeRotationY;
        rigidBody.constraints = RigidbodyConstraints.FreezeRotationX;
    }


    private void LoadNextLevel()
    {
        SceneManager.LoadScene(1); // todo allow for more than 2 levels
    }
    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }


}