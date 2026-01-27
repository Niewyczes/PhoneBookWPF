# Książka Telefoniczna

Kompletna aplikacja do zarządzania kontaktami stworzona w Windows Presentation Foundation (WPF) z obsługą trzech motywów graficznych i automatycznym zapisywaniem danych.

---

## Spis treści

- [Funkcjonalności](#funkcjonalności)
- [Szybki start](#szybki-start)
- [Instalacja](#instalacja)
- [Uruchamianie](#uruchamianie)
- [Architektura](#architektura)
- [Przechowywanie danych](#przechowywanie-danych)
- [Motywy graficzne](#motywy-graficzne)
- [Informacje dodatkowe](#informacje-dodatkowe)

---

## Funkcjonalności

### Zarządzanie kontaktami
- Dodawanie nowych kontaktów z walidacją danych  
- Edycja istniejących kontaktów  
- Usuwanie z potwierdzeniem  
- Wyszukiwanie w czasie rzeczywistym  
- Przeglądanie szczegółów kontaktów  

### Walidacja danych
- Sprawdzanie formatu numeru telefonu  
- Walidacja adresu email  
- Wymagane pola (imię, nazwisko, telefon)  
- Ograniczenia długości tekstu  

### Personalizacja
- 3 motywy graficzne: **Domyślny**, **Ciemny**, **Hakerski**  
- Automatyczne zapamiętywanie ustawień  
- Responsywny interfejs  

### Zarządzanie danymi
- Automatyczne zapisywanie do plików JSON  
- Ładowanie danych przy starcie aplikacji  
- Eksport i import danych  

---

## Szybki start

### Wymagania minimalne
- Windows 7 lub nowszy  
- .NET Framework 4.7.2 lub **.NET 6+**  
- Visual Studio 2022 (wersja **17.14.25+**)  

### Zalecane
- Windows 10 / 11  
- .NET 6.0+  
- 4 GB RAM  

---

## Instalacja

### Klonowanie repozytorium
```bash
git clone https://github.com/Niewyczes/PhoneBookWPF
cd phonebook-wpf
```
### Instalacja pakietów NuGet

#### Dla .NET Framework (4.7.2+):

Install-Package Newtonsoft.Json -Version 13.0.3
Install-Package System.ComponentModel.Annotations -Version 5.0.0
 
#### Konfiguracja w Visual Studio

Otwórz PhoneBookWPF.sln w Visual Studio 2022

Upewnij się, że wybrano odpowiedni .NET SDK

Kliknij Build → Build Solution (Ctrl+Shift+B)

---

## Uruchamianie
### Opcja 1: Z Visual Studio

Otwórz projekt w Visual Studio 2022

Naciśnij F5 lub kliknij Start Debugging

Aplikacja uruchomi się w trybie debugowania

### Opcja 2: Plik wykonywalny

Wybierz konfigurację Release

Build → Build Solution (Ctrl+Shift+B)

Przejdź do:

bin\Debug\net6.0-windows\


Uruchom PhoneBookWPF.exe

---

## Architektura

### Struktura projektu

PhoneBookWPF/
├── App.xaml                    # Zasoby i konwertery
├── App.xaml.cs                 # Punkt startowy
├── MainWindow.xaml             # Główny interfejs (XAML)
├── MainWindow.xaml.cs          # Code-behind głównego okna
├── MainViewModel.cs            # Logika biznesowa
├── Contact.cs                  # Model danych kontaktu
├── RelayCommand.cs             # Implementacja ICommand
├── Converters.cs               # 8 konwerterów WPF
├── ThemeManager.cs             # Zarządzanie motywami
├── phonebook.json              # Dane kontaktów
└── settings.json               # Ustawienia

### Główne komponenty

Contact.cs – model danych

MainViewModel.cs – logika aplikacji

ThemeManager.cs – obsługa motywów

---

## Przechowywanie danych
Pliki danych

| Plik | Opis | Format | Przykład |
| ------ | ------ | ------ | ------ |
| phonebook.json | Kontakty | JSON | [{"FirstName":"Jan",...}] |
| settings.json | Ustawienia | JSON | {"Theme":"Default"} |

### Mechanizm zapisywania

Automatyczne zapisywanie przy:
dodaniu / edycji / usunięciu kontaktu
zamknięciu aplikacji

Automatyczne wczytywanie przy:
uruchomieniu aplikacji
sprawdzaniu istnienia plików

Możliwość ręcznej edycji plików

---

## Motywy graficzne
### Dostępne motywy

|Motyw | Paleta kolorów | Przeznaczenie |
| ------ | ------ | ------ |
|Domyślny | Niebiesko-zielony, jasny | Standardowy, przyjazny|
|Ciemny | Ciemne tło, różowe akcenty | Praca wieczorem|
|Hakerski | Zielone tło, terminalowy | Dla programistów|

### Zmiana motywu

    Kliknij ComboBox w prawym górnym rogu

    Wybierz jeden z trzech motywów

    Interfejs zmienia się natychmiast

    Wybór jest zapisywany w settings.json

---

## Informacje dodatkowe

Dokumentacja zaktualizowana: 25-01-2026

Wersja aplikacji: 1.0.0

Technologie: WPF, C#, .NET 6, JSON
