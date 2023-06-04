using System.Collections;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 2.0f;
    [SerializeField] private float rotationAngle = 90.0f;
    private bool rotating = false;
    private Quaternion initialRotation;
    private Quaternion targetRotation;

    private void Start()
    {
        initialRotation = transform.rotation;
        targetRotation = Quaternion.Euler(initialRotation.eulerAngles + new Vector3(-rotationAngle, 0, 0));
    }

    private void Update()
    {
        //si existe el Manager_Controller, el juego acabó y aún no rotó la cámara
        if (Manager_Controller.instance != null && Manager_Controller.instance.EndGame && !rotating)
        {   
            //corrutina para que rote la camara
            StartCoroutine(RotateCamera());
        }
    }

    //se rota la cámara, tras moverse entonces se activa el menú de pausa
    private IEnumerator RotateCamera()
    {
        rotating = true;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * rotationSpeed;
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t);
            yield return null;
        }

        
        Manager_Controller.instance.TogglePause();
    }
}
