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

"Neurorun" to projekt gry w Unity, w którym gracz może sterować postacią za pomocą pada lub klawiatury. Gra pozwala graczowi na interakcję z otoczeniem, za pomocą urządzenia EEG (NeuroSky Mobile 2 lub czepka EEG obsługiwanego przez Neurostate), za pomocą parametru skupienia.

## Wymagania

- Unity (wersja 2022.3.7f1)
- Pad lub klawiatura
- Urządzenie EEG, jedno z:
  - NeuroSky Mobile 2 z Neurofiszka Bridge i Thinkgear Connector
  - dowolny czepek EEG nadający sygnał przez LSL, z uruchomionym Neurostate

## Uruchamianie Projektu

1. Sklonuj repozytorium na swoje lokalne urządzenie.
2. Otwórz projekt w Unity (wersja 2022.3.7f1).
3. Przejdź do folderu "Scenes" i otwórz scenę "Menu" lub wybierz opcję "Build and Run" w Unity.
4. Aby połączyć się z urządzeniem EEG, uruchom Neurostate albo, dla NeuroSky Mobile 2, Neurofiszka Bridge i Thinkgear Connector (szczegóły poniżej).

### Połączenie z EEG

Gra odczytuje poziom skupienia (attention) z jednego z dwóch źródeł:

- **Neurostate** – dla dowolnego czepka EEG nadającego sygnał przez LSL (Lab Streaming Layer), np. BrainAccess. Gra łączy się z jego wyjściem JSON pod adresem `127.0.0.1:13855`.
- **ThinkGear Connector** – dla NeuroSky Mobile 2, pod adresem `127.0.0.1:13854`. Działa też każdy inny program obsługujący protokół ThinkGear na tym porcie.

Domyślnie gra używa Neurostate, jeśli jest uruchomiony, a w przeciwnym razie ThinkGear Connectora. Aby zawsze używać jednego z nich, ustaw pole **Source** komponentu `TGCConnectionController` w prefabie `Player` na `NeuroState` lub `ThinkGear`. Tam też można zmienić adres i porty.

Źródło EEG można uruchomić przed grą lub po niej, a także zrestartować w trakcie gry: gra sama się połączy i połączy ponownie.

Przez pierwszą minutę dobrego sygnału Neurostate mierzy stan spoczynku gracza, a ikona połączenia jest żółta. Usiądź wtedy spokojnie z otwartymi oczami, aż ikona zmieni kolor na zielony; do tego czasu skupienie nie steruje grą. Neurostate robi to raz, po swoim uruchomieniu, więc restart gry tego nie powtarza.

## Sterowanie

Gracz może sterować postacią za pomocą pada lub klawiatury. Urządzenie EEG jest wykorzystywane do sterowania elementami otoczenia.

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