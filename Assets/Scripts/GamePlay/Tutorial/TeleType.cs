using System.Collections;
using UnityEngine;


namespace TMPro.Examples
{

    public class TeleType : MonoBehaviour
    {


        private TMP_Text m_textMeshPro;


        void Awake()
        {
            // Get Reference to TextMeshPro Component
            m_textMeshPro = GetComponent<TMP_Text>();
            m_textMeshPro.enableWordWrapping = true;
            m_textMeshPro.alignment = TextAlignmentOptions.Top;
        }
        private void OnEnable()
        {
            StartCoroutine(StartAnim());
        }

        IEnumerator StartAnim()
        {

            // Force and update of the mesh to get valid information.
            m_textMeshPro.ForceMeshUpdate();


            int totalVisibleCharacters = m_textMeshPro.textInfo.characterCount; // Get # of Visible Character in text object
            int counter = 0;
            int visibleCount = 0;

            while (visibleCount < totalVisibleCharacters)
            {
                visibleCount = counter % (totalVisibleCharacters + 1);

                m_textMeshPro.maxVisibleCharacters = visibleCount; // How many characters should TextMeshPro display?

                counter += 1;

                yield return new WaitForSeconds(0.04f);
            }

            //Debug.Log("Done revealing the text.");
        }

    }
}