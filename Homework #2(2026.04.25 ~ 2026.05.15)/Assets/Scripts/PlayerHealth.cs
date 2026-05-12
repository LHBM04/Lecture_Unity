using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    /// <summary>
    /// 해당 플레이어의 체력.
    /// </summary>
    [Tooltip("해당 플레이어의 체력.")]
    [SerializeField]
    private int health;
 
    /// <summary>
    /// 해당 플레이어가 공격을 받았을 때 호출되는 메서드입니다.
    /// </summary>
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }
    /// <summary>
    /// 해당 플레이어가 사망했을 때 호출되는 메서드입니다.
    /// </summary>
    private void Die()
    {
        // 사망 처리 로직을 여기에 작성합니다.
        Debug.Log("Player has died.");
        // 예: 게임 오버 화면으로 전환, 리스폰 등
    }
}