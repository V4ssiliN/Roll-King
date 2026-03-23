using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomUtils
{
    private static System.Random rng = new System.Random();

    // Génère une valeur autour de mean avec étendue et concentration
    public static float SampleClamped(float min, float max, float mean, float spread, float curve = 1f)
    {
        float value = mean;
        int attempts = 0;

        // On fait plusieurs essais pour rester dans les bornes
        do
        {
            // Génère une valeur normale centrée sur 0
            float gauss = Gaussian() * spread;

            // Applique la courbure : >1 = plus concentré, <1 = plus étalé
            gauss = Mathf.Sign(gauss) * Mathf.Pow(Mathf.Abs(gauss), curve);

            value = mean + gauss;
            attempts++;
        } while ((value < min || value > max) && attempts < 10);
        // Clamp final au cas où
        return Mathf.Clamp(value, min, max);
    }

    // Retourne un échantillon normal standard (moyenne=0, sigma=1)
    private static float Gaussian()
    {
        double u1 = 1.0 - rng.NextDouble(); // uniform(0,1] random doubles
        double u2 = 1.0f - rng.NextDouble();
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log((float)u1)) *
                              Mathf.Sin(2.0f * Mathf.PI * (float)u2); // random normal(0,1)
        return randStdNormal;
    }
}
