using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SimulationController : MonoBehaviour
{
    public void RestartSimulation(InputAction.CallbackContext onRestart)
    {
        if (onRestart.started)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
