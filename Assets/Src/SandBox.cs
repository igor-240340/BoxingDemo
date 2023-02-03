using UnityEngine;

namespace Src
{
    public class SandBox
    {
        public SandBox()
        {
            int healthA = 100;
            int[] attackA = {1, 0, 1, 0, 0, 0, 0, 2};
            int[] defenceA = {0, 0, 1, 1, 0, 1, 0, 1}; // 0 - часть тела защищена, 1 - не защищена.

            int healthB = 100;
            int[] attackB = {0, 0, 0, 3, 0, 0, 1, 0};
            int[] defenceB = {1, 0, 1, 0, 1, 0, 1, 0};

            healthB -= AttackAndReturnDamage(attackA, defenceB);
            healthA -= AttackAndReturnDamage(attackB, defenceA);

            Debug.Log($"healthA: {healthA}");
            Debug.Log($"healthB: {healthB}");
        }

        private int AttackAndReturnDamage(int[] attackA, int[] defenceB)
        {
            int totalDamage = 0;

            // Последовательно "атакуем" каждую часть тела.
            // Если текущая часть тела - под защитой, то AttackPoints для неё множится на ноль.
            for (int i = 0; i < 8; i++)
                totalDamage += attackA[i] * defenceB[i];

            return totalDamage;
        }
    }
}