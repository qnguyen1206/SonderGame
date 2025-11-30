using UnityEngine;
using UnityEngine.SceneManagement;

public class teleport : MonoBehaviour
{
    [SerializeField]
    private int scene;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Teleport created");
        string sceneName = SceneManager.GetSceneByBuildIndex(scene).name;
        Debug.Log("Scene: " + sceneName);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered");
            SceneManager.LoadScene(scene);
        }
    }
}
