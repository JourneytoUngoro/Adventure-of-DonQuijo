using UnityEngine;

[System.Serializable]
public class PlayerAttributeData
{
    public Vector2 moveSpeed;
    public Vector2 dashSpeed;

    public float postureDamageAmount; // affects enemy's posture 

    public float maxHealthAmount; // player's max health amount
    public float currentHealthAmount; // player's current health amount
}
