using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movimientoDelJugador : MonoBehaviour
{
    private Rigidbody2D rb2D;

    [Header("Movimento")]

    private float movimientoHorizontal = 0f;


    [SerializeField] private float velocidadDeMovimiento;

    [Range(0,0.3f)][SerializeField] private float suavizadoDeMovimiento;

    private Vector3 velocidad = Vector3.zero;

    private bool mirandoDerecha = true;

    [Header("Salto")]

    [SerializeField] private float fuerzaDeSalto;

    [SerializeField] private LayerMask queEsSuelo;

    [SerializeField] private Transform controladorSuelo;

    [SerializeField] private Vector3 dimensionesCaja;

    [SerializeField] private bool enSuelo;

    private bool salto = false;

    [Header("Teleport")]

    private bool puedeTeletransportarse = true;

    [SerializeField] private float distanciaTeletransporte = 0.01f;

    [SerializeField] private float tiempoDeEnfriamiento = 2f;


    private void Start () {
        rb2D = GetComponent<Rigidbody2D>();
   }

   private void Update() 
   {
    movimientoHorizontal = Input.GetAxisRaw("Horizontal") * velocidadDeMovimiento;

    if(Input.GetButtonDown("Jump")) {
        if(enSuelo){
            salto = true;
        }
    }

    // Caida Rapida
    if(!enSuelo && Input.GetAxisRaw("Vertical") < 0) {
        rb2D.gravityScale = 20;
    } else {
        rb2D.gravityScale = 1;
    }
    // Caida Rapida

    if (Input.GetKeyDown(KeyCode.F) && puedeTeletransportarse) {
        Teletransportarse();
        StartCoroutine(EnfriamientoTeletransporte());
    }


   }

    private void Teletransportarse() {
        Vector2 direccion = mirandoDerecha ? Vector2.right : Vector2.left;
        transform.Translate(direccion * distanciaTeletransporte);
    }

private IEnumerator EnfriamientoTeletransporte() {
    puedeTeletransportarse = false;
    yield return new WaitForSeconds(tiempoDeEnfriamiento);
    puedeTeletransportarse = true;
}

   private void FixedUpdate() 
   {
        enSuelo = Physics2D.OverlapBox (controladorSuelo.position, dimensionesCaja, 0f, queEsSuelo);
        //mover
        Mover(movimientoHorizontal * Time.fixedDeltaTime, salto);

        salto = false;
   }

    private void Mover(float mover, bool saltar) 
    {
        Vector3 velocidadObjetivo = new Vector2(mover, rb2D.velocity.y);
        rb2D.velocity = Vector3.SmoothDamp(rb2D.velocity, velocidadObjetivo, ref velocidad, suavizadoDeMovimiento);

        if (mover>0 && !mirandoDerecha)
        {
            Girar();
        }
        else if (mover<0 && mirandoDerecha)
        {
            Girar();
        }

        if(enSuelo && saltar) {
            enSuelo = false;
            rb2D.AddForce(new Vector2 (0f, fuerzaDeSalto));
        }
    }

    private void Girar () {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;

    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(controladorSuelo.position, dimensionesCaja);
    }
}
