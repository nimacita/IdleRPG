using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TestInstruments))]
public class Instruments : Editor
{
    // ������ �����
    private string[] EquipSets = new string[] { "������� �������� ���", "���������� ���", "��� ������� ������", "������ ���" };
    // ������ ��������� �����
    private int selectedEquipIndex = 0; 

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // ������ ����������� ���� ����������

        TestInstruments script = (TestInstruments)target;

        // ���������� ������
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("������ ��������� ��� ����������");
        selectedEquipIndex = EditorGUILayout.Popup("Select Equip", selectedEquipIndex, EquipSets);

        // ������ ���������� ���������
        if (GUILayout.Button("Give Selected Equip"))
        {
            script.GiveFullEquipSet(EquipSets[selectedEquipIndex]);
        }

        //������ ����� �������
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("������ ��������� �������");

        //������ �������� 
        if (GUILayout.Button("Give Next Level"))
        {
            script.GiveNewLevel();
        }

        //������ ������ ��������
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("�������� ��� ����������");

        //������ �������� 
        if (GUILayout.Button("DeleteAll"))
        {
            script.ResetAllStats();
            //������������� play mode
            RestartPlayMode();
        }


    }

    private static bool shouldRestartPlayMode = false;
    private void RestartPlayMode()
    {
        // ���������, ��������� �� �������� � Play Mode
        if (EditorApplication.isPlaying)
        {
            shouldRestartPlayMode = true;
            EditorApplication.isPlaying = false; // ������������� Play Mode
        }
        else
        {
            Debug.LogWarning("Play Mode is not currently active.");
        }
    }

    // ����� ���������� ��� ��������� ��������� Play Mode
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
            EditorApplication.isPlaying = true; // ������������� Play Mode ����� ����� � ����� ��������������
        }
    }
}
