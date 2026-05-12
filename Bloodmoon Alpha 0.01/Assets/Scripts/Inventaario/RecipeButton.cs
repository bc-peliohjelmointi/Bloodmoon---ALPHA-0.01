using UnityEngine;

public class RecipeButton : MonoBehaviour
{
    [SerializeField] private Recipe recipe;

    [Header("Sound")]
    [SerializeField] private AudioClip craftSound;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.ignoreListenerPause = true; // works even when timeScale = 0
    }

    public void OnClickCraft()
    {
        if (recipe == null) return;

        Inventory.Singleton.Craft(recipe);

        if (craftSound != null && audioSource != null)
            audioSource.PlayOneShot(craftSound);

        CraftingUI.Instance?.RefreshAll();
    }
}
