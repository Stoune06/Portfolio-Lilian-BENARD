// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Other
{
    
    public struct Utils
    {
        //SO - File Asset Name
        public const string WAVE_FILE_NAME = "CustomWave";
        public const string CARD_FILE_NAME = "Character";
        public const string ENEMY_FILE_NAME = "EnemyData";
        public const string QUEST_FILE_NAME = "Quests";

        //SO - Menu Asset Name
        public const string ENEMY_WAVE_MENU_NAME = "SO Enemy And Wave/EnemyWave";
        public const string WAVE_BUILDER_MENU_NAME = "SO Enemy And Wave/WaveBuilder";
        public const string CARD_CHARACTER_MENU_NAME = "SO Card/Card";
        public const string ENEMY_MENU_NAME = "SO Enemy And Wave/EnemyData";
        public const string QUEST_MENU_NAME = "SO New Quest/WeeklyQuest";
        
        //TAGS
        public const string TAG_COIN = "COIN";

        //TOOLS - Menu Path
        public const string TOOL_UPDATE_ILLUSTRATIONS = "Tools/Healer Survivor/Update All Wave Illustrations";

        //GENERAL PATHS
        public const string CIRCLE_PATH = "Assets/Textures/Trigonometric_Circle.png";

        //GUID - REFERENCES
        public const string GUIDS_ENEMY_WAVE_SO = "t:EnemyWaveSO";

        //RESOURCES - REFERENCES
        public const string CHARACTER_CARD_SO_FILE_REF = "SO - Cards/";
        public const string ENEMY_DATA_CARD_SO_FILE_REF = "SO - Data/";
        public const string QUEST_DATA_SO_FILE_REF = "SO - Quests/";

        //HEADER
        public const string HEADER_PARAMETERS_MENU = "Parameters - Menu";
        public const string HEADER_PARAMETERS_SOUNDS = "Parameters - Sound";
        public const string HEADER_WAVE_BUILDER = "Liste des vagues à spawn";

        //GENERAL TEXT
        public const string DAILY_REWARD_TIME_TXT = "Time before new reward : ";
        public const string ILLUSTRATION_SO_CIRCLE = "Aperçu de la formation en cercle :";

        //FMOD - PATH
        public const string BUS_MASTER_PATH = "bus:/";
        public const string GLOBAL_PARAM_MUSIC_VOLUME = "MUSIC Volume";
        public const string GLOBAL_PARAM_SFX_VOLUME = "SFX Volume";
        
        //SHOP - Save
        public const string SHOP_JSON_FILE_NAME = "CurrentShop.json";
        public const string QUEST_JSON_FILE_NAME = "WeeklyQuest.json";
        
        //SOUND
        public const string MASTER_PARAM = "MasterVolume";
        public const string MUSIC_PARAM = "MusicVolume";
        public const string SOUND_PARAM = "SFXVolume";
        
        //SCENES
        public const string GAME_SCENE = "Game";

        //ERRORS
        public const string ERR_SPRITE_CIRCLE = "Sprite non trouvé ! Vérifie le chemin.";

        //DESCRIPTION - LONG
        public const string SO_DESCRIPTION_ENEMY = @"				//--- ENEMY SCRIPTABLE OBJECT ---\\\
Description : Permet de créer une vague d'un type d'ennemis lié directement avec le code.

Fonctionnement :
- Sélectionner le nombre d'ennemis à faire apparaître.
- Sélectionner le type d'ennemi.
- Choisir un angle de départ et un angle d'arrivé pour l'apparition.

NOTE : L'apparition se fait en format cercle. Donc chaque angle représente une partie d'un cercle.

ATTENTION : Ce système fonctionne dans le sens inverse des aiguilles d'une montre !!";

        public const string SO_DESCRIPTION_WAVE_BUILDER = @"			//--- WAVE BUILDER SCRIPTABLE OBJECT ---\\\
Description : Permet de construire une vague entière de jeu, selon les différents type d'ennemis que l'on veut créer.

Fonctionnement :
- Créer un Scriptable Object de type EnemyWaveSO.
- Appliquer tous les paramètres nécessaires pour la vague.
- Ajouter l'objet dans la liste ci-dessous pour faire en sorte que la vague génère les ennemis appropriés.";

        public const string SO_DESCRIPTION_ENEMY_DATA = @"			//--- ENEMY DATA SCRIPTABLE OBJECT ---\\\
Description : Permet de stocker les informations nécessaires pour lier les types d'ennemis et leur prefab.

Fonctionnement :
- Assigner le type d'ennemis voulu.
- Ajouter son préfab voulu pour l'instanciation.";

        public const string SO_DESCRIPTION_CARD = @"			    //--- CARD SCRIPTABLE OBJECT ---\\\
Description : Permet de créer un héros en jeu parmis les héros disponible dans le deck builder.

Fonctionnement :
- Donner un nom au héros.
- Ajouter son préfab voulu pour l'instanciation.
- Lui donner une icône pour les menus.
- Lui définir un nombre de HP.
- Lui définir un nombre de dégâts.";
    }
}