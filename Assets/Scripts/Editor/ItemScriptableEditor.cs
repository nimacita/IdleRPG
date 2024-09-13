using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MyInventoryItem))]
public class ItemScriptableEditor : Editor
{
    SerializedProperty itemName;
    SerializedProperty itemIcon;
    SerializedProperty itemType;
    SerializedProperty itemPrice;
    SerializedProperty poitionHealthAmount;
    SerializedProperty armorArmor;
    SerializedProperty armorHealth;
    SerializedProperty armorLuck;
    SerializedProperty weaponDamage;
    SerializedProperty weaponLuck;
    SerializedProperty swordAttackDurability;
    SerializedProperty swordInHandSprite;
    SerializedProperty bowAttackDurability;
    SerializedProperty bowArrowInHandSprite;
    SerializedProperty bowLimbInHandSprite;
    SerializedProperty bowRiserInHandSprite;
    SerializedProperty helmetOnCharacterSprite;
    SerializedProperty glovesFinger;
    SerializedProperty glovesForearmR;
    SerializedProperty glovesForearmL;
    SerializedProperty glovesHandL;
    SerializedProperty glovesHandR;
    SerializedProperty glovesSleever;
    SerializedProperty torsoArmL;
    SerializedProperty torsoArmR;
    SerializedProperty torsoPelvis;
    SerializedProperty torsoTorso;
    SerializedProperty bootsLeg;
    SerializedProperty bootsShin;
    SerializedProperty armorAddReadyToAtck;
    SerializedProperty swordSwingSound;
    SerializedProperty swordHitSound;
    SerializedProperty bowShotSound;
    SerializedProperty bowDrawSound;
    SerializedProperty armorBlowSound;

    private void OnEnable()
    {
        // Инициализируем сериализованные свойства
        itemName = serializedObject.FindProperty("itemName");
        itemIcon = serializedObject.FindProperty("itemIcon");
        itemType = serializedObject.FindProperty("itemType");
        itemPrice = serializedObject.FindProperty("itemPrice");
        poitionHealthAmount = serializedObject.FindProperty("poitionHealthAmount");
        armorArmor = serializedObject.FindProperty("armorArmor");
        armorHealth = serializedObject.FindProperty("armorHealth");
        armorLuck = serializedObject.FindProperty("armorLuck");
        weaponDamage = serializedObject.FindProperty("weaponDamage");
        weaponLuck = serializedObject.FindProperty("weaponLuck");
        swordAttackDurability = serializedObject.FindProperty("swordAttackDurability");
        swordInHandSprite = serializedObject.FindProperty("swordInHandSprite");
        bowAttackDurability = serializedObject.FindProperty("bowAttackDurability");
        bowArrowInHandSprite = serializedObject.FindProperty("bowArrowInHandSprite");
        bowLimbInHandSprite = serializedObject.FindProperty("bowLimbInHandSprite");
        bowRiserInHandSprite = serializedObject.FindProperty("bowRiserInHandSprite");
        helmetOnCharacterSprite = serializedObject.FindProperty("helmetOnCharacterSprite");
        glovesFinger = serializedObject.FindProperty("glovesFinger");
        glovesForearmR = serializedObject.FindProperty("glovesForearmR");
        glovesForearmL = serializedObject.FindProperty("glovesForearmL");
        glovesHandL = serializedObject.FindProperty("glovesHandL");
        glovesHandR = serializedObject.FindProperty("glovesHandR");
        glovesSleever = serializedObject.FindProperty("glovesSleever");
        torsoArmL = serializedObject.FindProperty("torsoArmL");
        torsoArmR = serializedObject.FindProperty("torsoArmR");
        torsoPelvis = serializedObject.FindProperty("torsoPelvis");
        torsoTorso = serializedObject.FindProperty("torsoTorso");
        bootsLeg = serializedObject.FindProperty("bootsLeg");
        bootsShin = serializedObject.FindProperty("bootsShin");
        armorAddReadyToAtck = serializedObject.FindProperty("armorAddReadyToAtck");
        swordSwingSound = serializedObject.FindProperty("swordSwingSound");
        swordHitSound = serializedObject.FindProperty("swordHitSound");
        bowShotSound = serializedObject.FindProperty("bowShotSound");
        bowDrawSound = serializedObject.FindProperty("bowDrawSound");
        armorBlowSound = serializedObject.FindProperty("armorBlowSound");

    }

    public override void OnInspectorGUI()
    {
        // Обновляем сериализованный объект
        serializedObject.Update();

        //риусем картинку сверху
        if (itemIcon.objectReferenceValue != null)
        {
            Texture2D texture = AssetPreview.GetAssetPreview(itemIcon.objectReferenceValue);
            if (texture != null)
            {
                GUILayout.Label(texture, GUILayout.Width(128), GUILayout.Height(128)); // Измените размеры по необходимости
            }
        }

        // Отображаем основные поля
        EditorGUILayout.PropertyField(itemName);

        // Отображаем спрайт в большом размере
        EditorGUILayout.PropertyField(itemIcon);

        EditorGUILayout.PropertyField(itemType);
        EditorGUILayout.PropertyField(itemPrice);

        // Отображаем дополнительные поля в зависимости от типа предмета
        InventoryItemType currentType = (InventoryItemType)itemType.enumValueIndex;

        switch (currentType)
        {
            case InventoryItemType.HealthPotion:
                EditorGUILayout.PropertyField(poitionHealthAmount);
                break;
            case InventoryItemType.Helmet:
                EditorGUILayout.PropertyField(armorArmor);
                EditorGUILayout.PropertyField(armorHealth);
                EditorGUILayout.PropertyField(armorLuck);
                EditorGUILayout.PropertyField(armorAddReadyToAtck);
                EditorGUILayout.PropertyField(helmetOnCharacterSprite);
                break;
            case InventoryItemType.Torso:
                EditorGUILayout.PropertyField(armorArmor);
                EditorGUILayout.PropertyField(armorHealth);
                EditorGUILayout.PropertyField(armorLuck);
                EditorGUILayout.PropertyField(armorAddReadyToAtck);
                EditorGUILayout.PropertyField(torsoArmL);
                EditorGUILayout.PropertyField(torsoArmR);
                EditorGUILayout.PropertyField(torsoPelvis);
                EditorGUILayout.PropertyField(torsoTorso);
                EditorGUILayout.PropertyField(armorBlowSound);
                break;
            case InventoryItemType.Gloves:
                EditorGUILayout.PropertyField(armorArmor);
                EditorGUILayout.PropertyField(armorHealth);
                EditorGUILayout.PropertyField(armorLuck);
                EditorGUILayout.PropertyField(armorAddReadyToAtck);
                EditorGUILayout.PropertyField(glovesFinger);
                EditorGUILayout.PropertyField(glovesForearmR);
                EditorGUILayout.PropertyField(glovesForearmL);
                EditorGUILayout.PropertyField(glovesHandL);
                EditorGUILayout.PropertyField(glovesHandR);
                EditorGUILayout.PropertyField(glovesSleever);
                break;
            case InventoryItemType.Boots:
                EditorGUILayout.PropertyField(armorArmor);
                EditorGUILayout.PropertyField(armorHealth);
                EditorGUILayout.PropertyField(armorLuck);
                EditorGUILayout.PropertyField(armorAddReadyToAtck);
                EditorGUILayout.PropertyField(bootsLeg);
                EditorGUILayout.PropertyField(bootsShin);
                break;
            case InventoryItemType.SwordWeapon:
                EditorGUILayout.PropertyField(weaponDamage);
                EditorGUILayout.PropertyField(weaponLuck);
                EditorGUILayout.PropertyField(swordAttackDurability);
                EditorGUILayout.PropertyField(swordInHandSprite);
                EditorGUILayout.PropertyField(swordSwingSound);
                EditorGUILayout.PropertyField(swordHitSound);
                break;
            case InventoryItemType.BowWeapon:
                EditorGUILayout.PropertyField(weaponDamage);
                EditorGUILayout.PropertyField(weaponLuck);
                EditorGUILayout.PropertyField(bowAttackDurability);
                EditorGUILayout.PropertyField(bowArrowInHandSprite);
                EditorGUILayout.PropertyField(bowLimbInHandSprite);
                EditorGUILayout.PropertyField(bowRiserInHandSprite);
                EditorGUILayout.PropertyField(bowShotSound);
                EditorGUILayout.PropertyField(bowDrawSound);
                break;
        }

        // Применяем изменения к сериализованному объекту
        serializedObject.ApplyModifiedProperties();
    }
}
