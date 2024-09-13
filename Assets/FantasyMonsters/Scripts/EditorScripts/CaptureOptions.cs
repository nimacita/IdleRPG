using UnityEngine;
using UnityEngine.UI;

namespace Assets.FantasyMonsters.Scripts.EditorScripts
{
    public class CaptureOptions : MonoBehaviour
    {
        public InputField FrameSize;
        public InputField FrameCount;
        public Toggle Shadow;

        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void Capture()
        {
            Close();
        }

        public void OnFrameSizeChanged(string value)
        {
            if (FrameSize.text == "") return;

            var valueInt = int.Parse(value);

            if (valueInt < 128) valueInt = 128;
            if (valueInt > 1024) valueInt = 1024;

            FrameSize.SetTextWithoutNotify(valueInt.ToString());
        }

        public void OnFrameCountChanged(string value)
        {
            if (FrameCount.text == "") return;

            var valueInt = int.Parse(value);

            if (valueInt < 4) valueInt = 4;
            if (valueInt > 16) valueInt = 16;

            FrameCount.SetTextWithoutNotify(valueInt.ToString());
        }
    }
}