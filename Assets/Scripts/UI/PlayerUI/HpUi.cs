using UnityEngine;

public class HpUi : MonoBehaviour
{
    [SerializeField]
    int Hp;

    void OnEnable()
    {
        EventBus.Subscribe<PlayerHealthChangedEvent>(OnHpChanged);
    }

    void OnHpChanged(PlayerHealthChangedEvent e)
    {
        Hp = e.NewHealth;
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayerHealthChangedEvent>(OnHpChanged);
    }
}
