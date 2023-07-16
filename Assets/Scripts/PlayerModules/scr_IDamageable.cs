using System.Collections;

public interface scr_IDamageable
{

    public void ApplyDamage(float damage = 1f, string tag = "Untagged", bool instantKill = false);

    IEnumerator Die();

}
