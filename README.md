# Neurorun

## Spis treści

1. [Opis]
2. [Wymagania]
3. [Uruchamianie Projektu]
4. [Sterowanie]
5. [Tworzenie Poziomów]
6. [Struktura Folderu Assets]
7. [Skrypty]
  

## Opis

"Neurorun" to projekt gry w Unity, w którym gracz może sterować postacią za pomocą pada lub klawiatury. Gra pozwala graczowi na interakcję z otoczeniem, za pomocą urządzenia NeuroSky Mobile 2, za pomocą parametru skupienia.

## Wymagania

- Unity (wersja 2022.3.7f1)
- Pad lub klawiatura
- Neurofiszka Bridge i Thinkgear Connector 
- Urządzenie NeuroSky Mobile 2 

## Uruchamianie Projektu

1. Sklonuj repozytorium na swoje lokalne urządzenie.
2. Otwórz projekt w Unity (wersja 2022.3.7f1).
3. Przejdź do folderu "Scenes" i otwórz scenę "Menu" lub wybierz opcję "Build and Run" w Unity.
4. Aby połączyć się z urządzeniem EEG NeuroSky Mobile 2, użyj Neurofiszka Bridge i Thinkgear Connector.

## Sterowanie

Gracz może sterować postacią za pomocą pada lub klawiatury. Urządzenie EEG NeuroSky Mobile 2 jest wykorzystywane do sterowania elementami otoczenia.

## Tworzenie Poziomów

Projekt gry "Neurorun" umożliwia tworzenie nowych poziomów. Aby stworzyć nowy poziom, wykonaj następujące kroki:

1. Przejdź do folderu "Scenes" i utwórz nową scenę, która będzie reprezentować Twój nowy poziom.
2. Dodaj obiekty, teren i inne elementy do nowej sceny, które chciałbyś umieścić na poziomie, gotowe elementy znajdują się w folderze Prefabs.
3. Zapisz scenę.

## Struktura Folderu Assets

Projekt "Neurorun" zawiera następującą strukturę folderu "assets":

- ### Animation

- ### AssetsExt 

	Folder to store assets from external sources

- ### Controls

- ### Materials

- ### Misc

- ### Music

- ### Plugins

- ### Prefabs

- ### Scenes

- ### Sprites

- ### Scripts

- ### Tiles

## Skrypty 

- ### Character
	Is a script that focuses on character actions like interactions with environment

- ### Controller
	Is a script that focuses on player movement and gathering the data about controls
- ### Interactable
	Is a script that focuses on making game object available for interaction with player
- ### Interactor
	Is a script that focuses on controlls interaction, it should be attached to the Player
- ### Connection manager
	Is a script that focuses on connecion with Neurosky Device
- ### Killer Entity
	Is a script that focuses on making object kill Player on touch like e.g. spikes
- ### Level Manager
	Is a script that focuses on managing levels, changing scenes
- ### Portal
	Is a script that focuses on creating logic for exit using portal
- ### Slider Manager
	Is a script that focuses on managing slider, it allows for player to save attention treshold but also to load it from PlayerPrefs
- ### Timer
	Is a script that focuses on counting time that has passed as the player progresses through the level and getting the final result