using UnityEngine;

public interface IDamage 
{
    void takeDamage(int amount);
    bool heal(int amount);
}
