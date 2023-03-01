using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.R)) { SceneManager.LoadScene(0); }
    }
}
