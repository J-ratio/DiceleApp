using UnityEngine;
using UnityEngine.UI;

public class ToggleInteract : MonoBehaviour
{
    private Toggle toggle;
    private Animator anime;
    [SerializeField] private Color dayThemeColor;
    [SerializeField] private Color nightThemeColor;
    [SerializeField] private ParticleSystem particleSystem;
    private void Start()
    {
        toggle = GetComponent<Toggle>();
        anime = GetComponent<Animator>();
        Toggle_Button(1);
        toggle.onValueChanged.AddListener(delegate
        {
            Toggle_Button(0);
        });

        if (particleSystem == null) return;

        particleSystem.Stop();
    }
    public void Toggle_Button(int normalTime)
    {
        if (toggle != null && anime != null)
        {
            if (toggle.isOn)
            {
                anime.Play("On", 0, normalTime);
            }
            else
            {
                anime.Play("Off", 0, normalTime);
            }
        }
    }

    private void DayModeParticle()
    {
        if (particleSystem == null) return;

        particleSystem.startColor = dayThemeColor;
        particleSystem.Play();
    }
    private void NightModeParticle()
    {
        if (particleSystem == null) return;

        particleSystem.startColor = nightThemeColor;
        particleSystem.Play();
    }
}
