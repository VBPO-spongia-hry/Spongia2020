public interface IInteractable
{
   void SetInteracting();
   void Interact();
}

public interface IDamageable
{
   void ApplyDamage(int damage);
   void Dead();
}