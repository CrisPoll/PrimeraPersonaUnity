using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Movemeent : MonoBehaviour
{
    // Variables serializadas para que aparezcan en el Inspector de Unity y se puedan modificar desde all�.
    [SerializeField] Transform playerCamera; // Referencia a la c�mara del jugador.
    [SerializeField][Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f; // Tiempo de suavizado del movimiento del rat�n.
    [SerializeField] bool cursorLock = true; // Bloquear el cursor en el centro de la pantalla.
    [SerializeField] float mouseSensitivity = 3.5f; // Sensibilidad del rat�n.
    [SerializeField] float Speed = 6.0f; // Velocidad de movimiento del jugador.
    [SerializeField][Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f; // Tiempo de suavizado del movimiento.
    [SerializeField] float gravity = -30f; // Valor de la gravedad.
    [SerializeField] Transform groundCheck; // Transform para verificar si el jugador est� en el suelo.
    [SerializeField] LayerMask ground; // Capa que representa el suelo.

    public float jumpHeight = 6f; // Altura del salto.
    float velocityY; // Velocidad vertical del jugador.
    bool isGrounded; // Indica si el jugador est� en el suelo.

    float cameraCap; // L�mite de rotaci�n de la c�mara.
    Vector2 currentMouseDelta; // Diferencia actual del movimiento del rat�n.
    Vector2 currentMouseDeltaVelocity; // Velocidad de suavizado del movimiento del rat�n.

    CharacterController controller; // Referencia al componente CharacterController.
    Vector2 currentDir; // Direcci�n actual del movimiento.
    Vector2 currentDirVelocity; // Velocidad de suavizado del movimiento.
    Vector3 velocity; // Vector de velocidad total.

    void Start()
    {
        // Obtener el componente CharacterController.
        controller = GetComponent<CharacterController>();

        // Bloquear el cursor en el centro de la pantalla y hacerlo invisible si cursorLock es verdadero.
        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
        }
    }

    void Update()
    {
        // Llamar a las funciones de actualizaci�n del rat�n y el movimiento en cada frame.
        UpdateMouse();
        UpdateMove();
    }

    void UpdateMouse()
    {
        // Obtener la diferencia de movimiento del rat�n.
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        // Suavizar el movimiento del rat�n.
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        // Actualizar el l�mite de rotaci�n de la c�mara.
        cameraCap -= currentMouseDelta.y * mouseSensitivity;
        cameraCap = Mathf.Clamp(cameraCap, -90.0f, 90.0f);

        // Rotar la c�mara en el eje X.
        playerCamera.localEulerAngles = Vector3.right * cameraCap;

        // Rotar el jugador en el eje Y.
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    void UpdateMove()
    {
        // Verificar si el jugador est� en el suelo.
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, ground);

        // Obtener la direcci�n de entrada del jugador.
        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize(); // Normalizar la direcci�n.

        // Suavizar la direcci�n del movimiento.
        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        // Aplicar la gravedad.
        velocityY += gravity * 2f * Time.deltaTime;

        // Calcular la velocidad total del jugador.
        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * Speed + Vector3.up * velocityY;

        // Mover al jugador utilizando el CharacterController.
        controller.Move(velocity * Time.deltaTime);

        // Hacer que el jugador salte si est� en el suelo y se presiona el bot�n de salto.
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Restablecer la velocidad vertical si el jugador no est� en el suelo y est� cayendo.
        if (!isGrounded && controller.velocity.y < -1f)
        {
            velocityY = -8f;
        }
    }
}
