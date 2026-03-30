# Roll King

**Roll King** est un jeu d'adresse 2D procédural pour mobile développé sous Unity. Le joueur doit naviguer d'une roue tournante à l'autre en jaugeant précisément la force et l'angle de ses sauts. 

![rollking-clip1](https://github.com/user-attachments/assets/879850cd-bb80-481a-ae95-db2aaf8af4c6)


## Technologies & Outils
* **Moteur :** Unity 2D
* **Langage :** C#
* **Librairies tierces :** LeanTween pour les animations UI fluides

## Fonctionnalités Principales
* **Génération procédurale infinie :** La partie est générée dynamiquement à l'infini avec un ajustement de la difficulté en temps réel (vitesse de rotation, taille des roues, distance entre les roues, roues mobiles).
* **Système de physique custom :** Charge de saut variable avec prédiction et affichage de la trajectoire parabolique en temps réel.
* **Progression du joueur :** Système de High-Score sauvegardé (`PlayerPrefs`) permettant de débloquer différents skins animés.

## Points techniques

Si vous souhaitez explorer le code source, voici les scripts les plus représentatifs de l'architecture du jeu :

1. **Physique et mouvements du personnage (`herosMouvements.cs`) :**
   Gestion du déplacement du joueur et des animations en fonction des input (Tap & Hold pour charger, Swipe pour se déplacer autour de l'axe de la roue).

2. **Génération des roues et Difficulté (`GameManager.cs`) :**
   Le script orchestre la génération des roues (`CreateNewWheel()`). La difficulté évolue via la fonction `HandleDifficulty()` qui modifie la probabilité d'apparition d'obstacles complexes en fonction du score :
    - Roues mobiles sur les axes X/Y
    - Réduction des rayons
    - Augmentation de la vitesse
    - Augmentation la distance entre les roues
    - Disparition progressive de la prédiction de trajectoire


3. **Prédiction de trajectoire (`TrajectoryPreview.cs`) :**
   Implémentation d'un algorithme simulant la physique du moteur (prend compte dfe la gravité et de la force initiale) via une série de `Physics2D.Raycast` pour calculer et afficher l'arc de saut exact du joueur avant l'action, tout en détectant les collisions avec le décor.

4. **Système de skins (`SkinSelection.cs`) :**
   Dans le menu, le joueur peut choisir le skin à appliquer au personnage. Les skins se débloquent à différents paliers de score. Ce fichier gère le débloquage, la sélection et l'affichage visuel de la sélection des skins dans le menu.

![rollking-clip3bis](https://github.com/user-attachments/assets/c7fb497f-df91-47b8-8ea1-3a38dd080512)
   
---
Note : Ce dépôt contient uniquement le code source (C#) du projet pour des raisons de droits d'auteur sur les assets visuels et sonores utilisés dans le prototype
*Projet personnel développé par Vassili Nakov.*
