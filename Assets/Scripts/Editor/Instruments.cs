using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TestInstruments))]
public class Instruments : Editor
{
    // Список опций
    private string[] EquipSets = new string[] { "Тяжелый Железный Сет", "Бандитский Сет", "Сет Темного Рыцаря", "Латный Сет" };
    // Индекс выбранной опции
    private int selectedEquipIndex = 0; 

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Рисует стандартные поля инспектора

        TestInstruments script = (TestInstruments)target;

        // Выпадающий список
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Выдать выбранный сет экипировки");
        selectedEquipIndex = EditorGUILayout.Popup("Select Equip", selectedEquipIndex, EquipSets);

        // Кнопки управления анимацией
        if (GUILayout.Button("Give Selected Equip"))
        {
            script.GiveFullEquipSet(EquipSets[selectedEquipIndex]);
        }

        //выдаем новый уровень
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Выдать следующий уровень");

        //кнопка выдачида 
        if (GUILayout.Button("Give Next Level"))
        {
            script.GiveNewLevel();
        }

        //кнопка сброса настроек
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Сбросить все сохранения");

        //кнопка выдачида 
        if (GUILayout.Button("DeleteAll"))
        {
            script.ResetAllStats();
            //перезапускаем play mode
            RestartPlayMode();
        }


    }

    private static bool shouldRestartPlayMode = false;
    private void RestartPlayMode()
    {
        // Проверяем, находится ли редактор в Play Mode
        if (EditorApplication.isPlaying)
        {
            shouldRestartPlayMode = true;
            EditorApplication.isPlaying = false; // Останавливаем Play Mode
        }
        else
        {
            Debug.LogWarning("Play Mode is not currently active.");
        }
    }

    // Метод вызывается при изменении состояния Play Mode
    [InitializeOnLoadMethod]
    private static void Init()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode && shouldRestartPlayMode)
        {
            shouldRestartPlayMode = false;
            EditorApplication.isPlaying = true; // Перезапускаем Play Mode после входа в режим редактирования
        }
    }
}
