using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    public static float Map(float value, float originalMin, float originalMax, float newMin, float newMax, bool clamp)
    {
        float newValue = (value - originalMin) / (originalMax - originalMin) * (newMax - newMin) + newMin;
        if (clamp)
        {
            newValue = Mathf.Clamp(newValue, newMin, newMax);
        }
        return newValue;
    }

    public static bool IsVisibleFromVirtualCam(Vector3 worldPoint, Vector3 camPos, float sizeCam, float aspectCam, float margin = 0f)
    {
        // Calcul des demi-dimensions visibles
        float halfHeight = sizeCam;
        float halfWidth = sizeCam * aspectCam;

        // Conversion en coordonnées relatives à la caméra fictive
        Vector3 relative = worldPoint - camPos;

        // On est en 2D -> On ignore l'axe Z
        float minX = -halfWidth + margin;
        float maxX = halfWidth - margin;
        float minY = -halfHeight + margin;
        float maxY = halfHeight - margin;

        return (relative.x >= minX && relative.x <= maxX &&
                relative.y >= minY && relative.y <= maxY);
    }
}
