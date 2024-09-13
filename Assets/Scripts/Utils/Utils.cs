using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace IdleRPG.Utils
{
    public static class Utils
    {

        [Header("Damage Color")]
        public static Color damageColor = new Color(0.7924528f, 0.3252047f, 0.3252047f);
        //public static Color damageColor = Color.red;
        private static float duration = 0.2f;

        public static IEnumerator ChangeColorCoroutine(List<SpriteRenderer> allSprites, List<Color> allColors)
        {
            // ������ �������� ���� �� targetColor
            float elapsedTime = 0.0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;

                for (int i = 0; i < allSprites.Count; i++)
                {
                    allSprites[i].color = Color.Lerp(allColors[i], damageColor, t);
                }

                yield return null;
            }
            // ���������, ��� ���� ������������ ����������
            foreach (SpriteRenderer sr in allSprites)
            {
                sr.color = damageColor;
            }


            // ������ ���������� �������� ����
            elapsedTime = 0.0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;

                for (int i = 0; i < allSprites.Count; i++)
                {
                    allSprites[i].color = Color.Lerp(allSprites[i].color, allColors[i], t);
                }

                yield return null;
            }
            for (int i = 0; i < allSprites.Count; i++)
            {
                allSprites[i].color = allColors[i];
            }
        }

        //���������� ����������� ����
        public static void BackToStartColor(List<SpriteRenderer> allSprites, List<Color> allColors)
        {
            for (int i = 0; i < allSprites.Count; i++)
            {
                allSprites[i].color = allColors[i];
            }
        }
    }
}
